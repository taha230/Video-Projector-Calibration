using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Text;
using System.Linq;
using System.Threading;
using System.Runtime.InteropServices;
using EDSDKLib;

namespace CanonSDK
{
    public class SDKHandler : IDisposable
    {
        #region Variables
              
        /// <summary>
        /// The used camera
        /// </summary>
        public Camera MainCamera { get; private set; }
        /// <summary>
        /// States if a session with the MainCamera is opened
        /// </summary>
        public bool CameraSessionOpen { get; private set; }
        /// <summary>
        /// States if the LiveView is on or not
        /// </summary>
        public bool IsLiveViewOn { get; private set; }
        /// <summary>
        /// States if LiveView is recorded or not
        /// </summary>
        public bool IsEvfFilming { get; private set; }
        /// <summary>
        /// States if camera is recorded or not
        /// </summary>
        public bool IsFilming { get; private set; }
        /// <summary>
        /// Directory to where photos will be saved
        /// </summary>
        public string ImageSaveDirectory { get; set; }
        /// <summary>
        /// Handles errors that happen with the SDK
        /// </summary>
        /// 
        public string ImageName { get; set; }

        public uint Error
        {
            get { return EDSDK.EDS_ERR_OK; }
           // if (value == EDSDK.EDS_ERR_TAKE_PICTURE_AF_NG) { /*Handle focus error here*/ }
            set { if (value != EDSDK.EDS_ERR_OK) throw new Exception("SDK Error: " + value); }
        }
        
        public void SetUserPictureStyle(int User, EDSDK.EdsPictureStyleDesc Values)
        {
            if (MainCamera.Ref != IntPtr.Zero)
            {
                //Get the id for the user
                uint user;
                switch (User)
                {

                    case 1:
                        user = EDSDK.PictureStyle_User1;
                        break;
                    case 2:
                        user = EDSDK.PictureStyle_User2;
                        break;
                    case 3:
                        user = EDSDK.PictureStyle_User3;
                        break;

                    default: return;
                }

                //Set picture style to user define
                SetSetting(EDSDK.PropID_PictureStyle, user);
                //Set the values to the correct user. Note the "inParam" value is (int)user. this is the extra parameter it needs to work
                lock (LVlock) Error = EDSDK.EdsSetPropertyData(MainCamera.Ref, EDSDK.PropID_PictureStyleDesc, (int)user, Marshal.SizeOf(typeof(EDSDK.EdsPictureStyleDesc)), Values);
            }
            else { throw new ArgumentNullException("Camera or camera reference is null/zero"); }
        }

        // set picture style
        public void SetPictureStyle(uint Style, EDSDK.EdsPictureStyleDesc Values)
        {
            SetSetting(EDSDK.PropID_PictureStyle, Style);
            SetStructSetting<EDSDK.EdsPictureStyleDesc>(EDSDK.PropID_PictureStyleDesc, Values);
        }

        public uint GetPictureStyle(out EDSDK.EdsPictureStyleDesc Values)
        {
            Values = GetStructSetting<EDSDK.EdsPictureStyleDesc>(EDSDK.PropID_PictureStyleDesc);
            return GetSetting(EDSDK.PropID_PictureStyle);
        }

        /// <summary>
        /// Frame buffer for LiveView recording
        /// </summary>
        private Queue<byte[]> FrameBuffer = new Queue<byte[]>(1000);
        /// <summary>
        /// LiveView has to be paused while communicating with camera
        /// </summary>
        private readonly object LVlock = new object();
        /// <summary>
        /// States if a finished video should be downloaded from the camera
        /// </summary>
        private bool DownloadVideo;
        /// <summary>
        /// For video recording, SaveTo has to be Camera (this is to save previous setting)
        /// </summary>
        private uint PrevSaveTo;
        /// <summary>
        /// The LiveView thread
        /// </summary>
        private Thread LVThread;

        #endregion

        #region Events

        #region SDK Events

        public event EDSDK.EdsCameraAddedHandler SDKCameraAddedEvent;
        public event EDSDK.EdsObjectEventHandler SDKObjectEvent;
        public event EDSDK.EdsProgressCallback SDKProgressCallbackEvent;
        public event EDSDK.EdsPropertyEventHandler SDKPropertyEvent;
        public event EDSDK.EdsStateEventHandler SDKStateEvent;

        #endregion

        #region Custom Events

        public delegate void CameraAddedHandler();
        public delegate void ProgressHandler(int Progress);
        public delegate void ImageUpdate(Image img);
        public delegate void FloatUpdate(float Value);

        /// <summary>
        /// Fires if a camera is added
        /// </summary>
        public event CameraAddedHandler CameraAdded;
        /// <summary>
        /// Fires if any process reports progress
        /// </summary>
        public event ProgressHandler ProgressChanged;
        /// <summary>
        /// Fires if the LiveView image is updated
        /// </summary>
        public event ImageUpdate LiveViewUpdated;
        /// <summary>
        /// Fires if a new framerate is calculated
        /// </summary>
        public event FloatUpdate FrameRateUpdated;
        /// <summary>
        /// If the camera is disconnected or shuts down, this event is fired
        /// </summary>
        public event EventHandler CameraHasShutdown;

        #endregion

        #endregion


        #region Basic SDK and Session handling

        /// <summary>
        /// Initialises the SDK and adds events
        /// </summary>
        public SDKHandler()
        {
            //initialize SDK
            Error = EDSDK.EdsInitializeSDK();
            //subscribe to camera added event
            SDKCameraAddedEvent += new EDSDK.EdsCameraAddedHandler(SDKHandler_CameraAddedEvent);
            EDSDK.EdsSetCameraAddedHandler(SDKCameraAddedEvent, IntPtr.Zero);

            //subscribe to the camera events
            SDKStateEvent += new EDSDK.EdsStateEventHandler(Camera_SDKStateEvent);
            SDKPropertyEvent += new EDSDK.EdsPropertyEventHandler(Camera_SDKPropertyEvent);
            SDKProgressCallbackEvent += new EDSDK.EdsProgressCallback(Camera_SDKProgressCallbackEvent);
            SDKObjectEvent += new EDSDK.EdsObjectEventHandler(Camera_SDKObjectEvent);
            ImageName = string.Empty;
        }


        /// <summary>
        /// Get a list of all connected cameras
        /// </summary>
        /// <returns>The camera list</returns>
        public List<Camera> GetCameraList()
        {
            IntPtr camlist;
            //Get Cameralist
            Error = EDSDK.EdsGetCameraList(out camlist);

            //Get each camera from camlist
            int c;
            Error = EDSDK.EdsGetChildCount(camlist, out c);
            List<Camera> OutCamList = new List<Camera>();
            for (int i = 0; i < c; i++)
            {
                IntPtr cptr;
                Error = EDSDK.EdsGetChildAtIndex(camlist, i, out cptr);
                OutCamList.Add(new Camera(cptr));
            }
            return OutCamList;
        }

        /// <summary>
        /// Opens a session with given camera
        /// </summary>
        /// <param name="NewCamera">The camera which will be used</param>
        public void OpenSession(Camera NewCamera)
        {
            if (CameraSessionOpen) CloseSession();
            if (NewCamera != null)
            {
                MainCamera = NewCamera;
                //open a session
                Error = EDSDK.EdsOpenSession(MainCamera.Ref);
                //subscribe to the camera events (this time, in-Camera)
                EDSDK.EdsSetCameraStateEventHandler(MainCamera.Ref, EDSDK.StateEvent_All, SDKStateEvent, IntPtr.Zero);
                EDSDK.EdsSetObjectEventHandler(MainCamera.Ref, EDSDK.ObjectEvent_All, SDKObjectEvent, IntPtr.Zero);
                EDSDK.EdsSetPropertyEventHandler(MainCamera.Ref, EDSDK.PropertyEvent_All, SDKPropertyEvent, IntPtr.Zero);
                CameraSessionOpen = true;
            }
        }

        /// <summary>
        /// Closes the session with the current camera
        /// </summary>
        public void CloseSession()
        {
            if (CameraSessionOpen)
            {
                //if LiveView is still on, stop it and wait till the thread has stopped
                if (IsLiveViewOn)
                {
                    StopLiveView();
                    LVThread.Join(1000);
                }

                lock (LVlock)
                {
                    //close session and release camera
                    Error = EDSDK.EdsCloseSession(MainCamera.Ref);
                    Error = EDSDK.EdsRelease(MainCamera.Ref);
                    CameraSessionOpen = false;
                }
            }
        }

        /// <summary>
        /// Closes open session and terminates the SDK
        /// </summary>
        public void Dispose()
        {
            //close session
            CloseSession();
            //terminate SDK
            Error = EDSDK.EdsTerminateSDK();
        }

        #endregion

        #region Eventhandling

        /// <summary>
        /// A new camera was plugged into the computer
        /// </summary>
        /// <param name="inContext">The pointer to the added camera</param>
        /// <returns>An EDSDK errorcode</returns>
        private uint SDKHandler_CameraAddedEvent(IntPtr inContext)
        {
            //Handle new camera here
            if (CameraAdded != null) CameraAdded();
            return EDSDK.EDS_ERR_OK;
        }

        /// <summary>
        /// An Objectevent fired
        /// </summary>
        /// <param name="inEvent">The ObjectEvent id</param>
        /// <param name="inRef">Pointer to the object</param>
        /// <param name="inContext"></param>
        /// <returns>An EDSDK errorcode</returns>
        private uint Camera_SDKObjectEvent(uint inEvent, IntPtr inRef, IntPtr inContext)
        {
            //handle object event here
            switch (inEvent)
            {
                case EDSDK.ObjectEvent_All:
                    break;
                case EDSDK.ObjectEvent_DirItemCancelTransferDT:
                    break;
                case EDSDK.ObjectEvent_DirItemContentChanged:
                    break;
                case EDSDK.ObjectEvent_DirItemCreated:
                    if (DownloadVideo) { DownloadImage(inRef, ImageSaveDirectory); DownloadVideo = false; }
                    break;
                case EDSDK.ObjectEvent_DirItemInfoChanged:
                    break;
                case EDSDK.ObjectEvent_DirItemRemoved:
                    break;
                case EDSDK.ObjectEvent_DirItemRequestTransfer:
                    DownloadImage(inRef, ImageSaveDirectory);
                    break;
                case EDSDK.ObjectEvent_DirItemRequestTransferDT:
                    break;
                case EDSDK.ObjectEvent_FolderUpdateItems:
                    break;
                case EDSDK.ObjectEvent_VolumeAdded:
                    break;
                case EDSDK.ObjectEvent_VolumeInfoChanged:
                    break;
                case EDSDK.ObjectEvent_VolumeRemoved:
                    break;
                case EDSDK.ObjectEvent_VolumeUpdateItems:
                    break;
            }

            return EDSDK.EDS_ERR_OK;
        }

        /// <summary>
        /// A progress was made
        /// </summary>
        /// <param name="inPercent">Percent of progress</param>
        /// <param name="inContext">...</param>
        /// <param name="outCancel">Set true to cancel event</param>
        /// <returns>An EDSDK errorcode</returns>
        private uint Camera_SDKProgressCallbackEvent(uint inPercent, IntPtr inContext, ref bool outCancel)
        {
            //Handle progress here
            if (ProgressChanged != null) ProgressChanged((int)inPercent);
            return EDSDK.EDS_ERR_OK;
        }

        /// <summary>
        /// A property changed
        /// </summary>
        /// <param name="inEvent">The PropetyEvent ID</param>
        /// <param name="inPropertyID">The Property ID</param>
        /// <param name="inParameter">Event Parameter</param>
        /// <param name="inContext">...</param>
        /// <returns>An EDSDK errorcode</returns>
        private uint Camera_SDKPropertyEvent(uint inEvent, uint inPropertyID, uint inParameter, IntPtr inContext)
        {
            //Handle property event here
            switch (inEvent)
            {
                case EDSDK.PropertyEvent_All:
                    break;
                case EDSDK.PropertyEvent_PropertyChanged:
                    break;
                case EDSDK.PropertyEvent_PropertyDescChanged:
                    break;
            }

            switch (inPropertyID)
            {
                case EDSDK.PropID_AEBracket:
                    break;
                case EDSDK.PropID_AEMode:
                    break;
                case EDSDK.PropID_AEModeSelect:
                    break;
                case EDSDK.PropID_AFMode:
                    break;
                case EDSDK.PropID_Artist:
                    break;
                case EDSDK.PropID_AtCapture_Flag:
                    break;
                case EDSDK.PropID_Av:
                    break;
                case EDSDK.PropID_AvailableShots:
                    break;
                case EDSDK.PropID_BatteryLevel:
                    break;
                case EDSDK.PropID_BatteryQuality:
                    break;
                case EDSDK.PropID_BodyIDEx:
                    break;
                case EDSDK.PropID_Bracket:
                    break;
                case EDSDK.PropID_CFn:
                    break;
                case EDSDK.PropID_ClickWBPoint:
                    break;
                case EDSDK.PropID_ColorMatrix:
                    break;
                case EDSDK.PropID_ColorSaturation:
                    break;
                case EDSDK.PropID_ColorSpace:
                    break;
                case EDSDK.PropID_ColorTemperature:
                    break;
                case EDSDK.PropID_ColorTone:
                    break;
                case EDSDK.PropID_Contrast:
                    break;
                case EDSDK.PropID_Copyright:
                    break;
                case EDSDK.PropID_DateTime:
                    break;
                case EDSDK.PropID_DepthOfField:
                    break;
                case EDSDK.PropID_DigitalExposure:
                    break;
                case EDSDK.PropID_DriveMode:
                    break;
                case EDSDK.PropID_EFCompensation:
                    break;
                case EDSDK.PropID_Evf_AFMode:
                    break;
                case EDSDK.PropID_Evf_ColorTemperature:
                    break;
                case EDSDK.PropID_Evf_DepthOfFieldPreview:
                    break;
                case EDSDK.PropID_Evf_FocusAid:
                    break;
                case EDSDK.PropID_Evf_Histogram:
                    break;
                case EDSDK.PropID_Evf_HistogramStatus:
                    break;
                case EDSDK.PropID_Evf_ImagePosition:
                    break;
                case EDSDK.PropID_Evf_Mode:
                    break;
                case EDSDK.PropID_Evf_OutputDevice:
                    if (IsEvfFilming == true) DownloadEvfFilm();
                    else if (IsLiveViewOn == true) DownloadEvf();
                    break;
                case EDSDK.PropID_Evf_WhiteBalance:
                    break;
                case EDSDK.PropID_Evf_Zoom:
                    break;
                case EDSDK.PropID_Evf_ZoomPosition:
                    break;
                case EDSDK.PropID_ExposureCompensation:
                    break;
                case EDSDK.PropID_FEBracket:
                    break;
                case EDSDK.PropID_FilterEffect:
                    break;
                case EDSDK.PropID_FirmwareVersion:
                    break;
                case EDSDK.PropID_FlashCompensation:
                    break;
                case EDSDK.PropID_FlashMode:
                    break;
                case EDSDK.PropID_FlashOn:
                    break;
                case EDSDK.PropID_FocalLength:
                    break;
                case EDSDK.PropID_FocusInfo:
                    break;
                case EDSDK.PropID_GPSAltitude:
                    break;
                case EDSDK.PropID_GPSAltitudeRef:
                    break;
                case EDSDK.PropID_GPSDateStamp:
                    break;
                case EDSDK.PropID_GPSLatitude:
                    break;
                case EDSDK.PropID_GPSLatitudeRef:
                    break;
                case EDSDK.PropID_GPSLongitude:
                    break;
                case EDSDK.PropID_GPSLongitudeRef:
                    break;
                case EDSDK.PropID_GPSMapDatum:
                    break;
                case EDSDK.PropID_GPSSatellites:
                    break;
                case EDSDK.PropID_GPSStatus:
                    break;
                case EDSDK.PropID_GPSTimeStamp:
                    break;
                case EDSDK.PropID_GPSVersionID:
                    break;
                case EDSDK.PropID_HDDirectoryStructure:
                    break;
                case EDSDK.PropID_ICCProfile:
                    break;
                case EDSDK.PropID_ImageQuality:
                    break;
                case EDSDK.PropID_ISOBracket:
                    break;
                case EDSDK.PropID_ISOSpeed:
                    break;
                case EDSDK.PropID_JpegQuality:
                    break;
                case EDSDK.PropID_LensName:
                    break;
                case EDSDK.PropID_LensStatus:
                    break;
                case EDSDK.PropID_Linear:
                    break;
                case EDSDK.PropID_MakerName:
                    break;
                case EDSDK.PropID_MeteringMode:
                    break;
                case EDSDK.PropID_NoiseReduction:
                    break;
                case EDSDK.PropID_Orientation:
                    break;
                case EDSDK.PropID_OwnerName:
                    break;
                case EDSDK.PropID_ParameterSet:
                    break;
                case EDSDK.PropID_PhotoEffect:
                    break;
                case EDSDK.PropID_PictureStyle:
                    break;
                case EDSDK.PropID_PictureStyleCaption:
                    break;
                case EDSDK.PropID_PictureStyleDesc:
                    break;
                case EDSDK.PropID_ProductName:
                    break;
                case EDSDK.PropID_Record:
                    break;
                case EDSDK.PropID_RedEye:
                    break;
                case EDSDK.PropID_SaveTo:
                    break;
                case EDSDK.PropID_Sharpness:
                    break;
                case EDSDK.PropID_ToneCurve:
                    break;
                case EDSDK.PropID_ToningEffect:
                    break;
                case EDSDK.PropID_Tv:
                    break;
                case EDSDK.PropID_Unknown:
                    break;
                case EDSDK.PropID_WBCoeffs:
                    break;
                case EDSDK.PropID_WhiteBalance:
                    break;
                case EDSDK.PropID_WhiteBalanceBracket:
                    break;
                case EDSDK.PropID_WhiteBalanceShift:
                    break;
            }
            return EDSDK.EDS_ERR_OK;
        }

        /// <summary>
        /// The camera state changed
        /// </summary>
        /// <param name="inEvent">The StateEvent ID</param>
        /// <param name="inParameter">Parameter from this event</param>
        /// <param name="inContext">...</param>
        /// <returns>An EDSDK errorcode</returns>
        private uint Camera_SDKStateEvent(uint inEvent, uint inParameter, IntPtr inContext)
        {
            //Handle state event here
            switch (inEvent)
            {
                case EDSDK.StateEvent_All:
                    break;
                case EDSDK.StateEvent_AfResult:
                    break;
                case EDSDK.StateEvent_BulbExposureTime:
                    break;
                case EDSDK.StateEvent_CaptureError:
                    break;
                case EDSDK.StateEvent_InternalError:
                    break;
                case EDSDK.StateEvent_JobStatusChanged:
                    break;
                case EDSDK.StateEvent_Shutdown:
                    CameraSessionOpen = false;
                    if (CameraHasShutdown != null) CameraHasShutdown(this, new EventArgs());
                    break;
                case EDSDK.StateEvent_ShutDownTimerUpdate:
                    break;
                case EDSDK.StateEvent_WillSoonShutDown:
                    break;
            }
            return EDSDK.EDS_ERR_OK;
        }

        #endregion

        #region Camera commands
        
        /// <summary>
        /// Downloads an image to given directory
        /// </summary>
        /// <param name="Info">Pointer to the object. Get it from the SDKObjectEvent.</param>
        /// <param name="directory"></param>
        public void DownloadImage(IntPtr ObjectPointer, string directory)
        {
            
            lock (LVlock)
            {
                EDSDK.EdsDirectoryItemInfo dirInfo;
                IntPtr streamRef;
                //get information about object
                Error = EDSDK.EdsGetDirectoryItemInfo(ObjectPointer, out dirInfo);
                string CurrentPhoto;
                if (ImageName != string.Empty)
                {
                    CurrentPhoto = Path.Combine(directory, ImageName + ".jpg");
                }
                else
                {
                    CurrentPhoto = Path.Combine(directory, dirInfo.szFileName);
                }
                
                //create filestream to data
                Error = EDSDK.EdsCreateFileStream(CurrentPhoto, EDSDK.EdsFileCreateDisposition.CreateAlways, EDSDK.EdsAccess.ReadWrite, out streamRef);
                //download file
                DownloadData(ObjectPointer, streamRef);
                //release stream
                Error = EDSDK.EdsRelease(streamRef);
            }
        }
        
        /// <summary>
        /// Downloads a jpg image from the camera into a Bitmap
        /// </summary>
        /// <param name="Info">The DownloadInfo that is provided by the "DownloadReady" event</param>
        /// <returns>A Bitmap containing the jpg or null if not a jpg</returns>
        public Bitmap DownloadImage(IntPtr ObjectPointer)
        {
            lock (LVlock)
            {
                //get information about image
                EDSDK.EdsDirectoryItemInfo dirInfo;
                Error = EDSDK.EdsGetDirectoryItemInfo(ObjectPointer, out dirInfo);

                //check the extension. Raw data cannot be read by the Bitmap class
                string ext;
                if (ImageName!=string.Empty)
                {
                    ext = Path.GetExtension(ImageName + ".jpg").ToLower();
                }
                else
                {
                    ext = Path.GetExtension(dirInfo.szFileName).ToLower();
                }
                
                //if (ext == ".jpg" || ext == ".jpeg")
                if (ext == ".jpg")
                {
                    IntPtr streamRef, jpgPointer;
                    uint length;
                    Bitmap bmp;

                    //create stream to memory
                    Error = EDSDK.EdsCreateMemoryStream(dirInfo.Size, out streamRef);
                    //download the data into the memory stream
                    DownloadData(ObjectPointer, streamRef);

                    //get pointer to image data
                    Error = EDSDK.EdsGetPointer(streamRef, out jpgPointer);
                    Error = EDSDK.EdsGetLength(streamRef, out length);

                    unsafe
                    {
                        //create image from stream
                        UnmanagedMemoryStream ums = new UnmanagedMemoryStream((byte*)jpgPointer.ToPointer(), length, length, FileAccess.Read);
                        bmp = new Bitmap(ums);
                        ums.Close();
                    }

                    //release stream
                    Error = EDSDK.EdsRelease(streamRef);

                    return bmp;
                }
                else return null;
            }
        }

        /// <summary>
        /// Gets the thumbnail of an image (can be raw or jpg)
        /// </summary>
        /// <param name="filepath">The filename of the image</param>
        /// <returns>The thumbnail of the image</returns>
        public Bitmap GetFileThumb(string filepath)
        {
            lock (LVlock)
            {
                IntPtr stream;
                //create a filestream to given file
                Error = EDSDK.EdsCreateFileStream(filepath, EDSDK.EdsFileCreateDisposition.OpenExisting, EDSDK.EdsAccess.Read, out stream);
                return GetImage(stream, EDSDK.EdsImageSource.Thumbnail);
            }
        }

        /// <summary>
        /// Gets the list of possible values for the current camera to set.
        /// Only the PropertyIDs "AEModeSelect", "ISO", "Av", "Tv", "MeteringMode" and "ExposureCompensation" are allowed. driving mode and AF mode added for other modes by my self
        /// </summary>
        /// <param name="PropID">The property ID</param>
        /// <returns>A list of available values for the given property ID</returns>
        public List<int> GetSettingsList(uint PropID)
        {
            if (MainCamera.Ref != IntPtr.Zero)
            {
                //a list of settings can only be retrived for following properties
                if (PropID == EDSDK.PropID_AEModeSelect || PropID == EDSDK.PropID_ISOSpeed || PropID == EDSDK.PropID_Av 
                    || PropID == EDSDK.PropID_DriveMode || PropID == EDSDK.PropID_PictureStyleDesc
                    || PropID == EDSDK.PropID_AFMode || PropID == EDSDK.PropID_ImageQuality || PropID == EDSDK.PropID_PictureStyle
                    || PropID == EDSDK.PropID_Tv || PropID == EDSDK.PropID_MeteringMode || PropID == EDSDK.PropID_ExposureCompensation || PropID==EDSDK.PropID_WhiteBalance || PropID==EDSDK.PropID_FilterEffect||
                    PropID==EDSDK.PropID_ToningEffect||PropID==EDSDK.PropID_FlashCompensation)
                {
                    //get the list of possible values
                    EDSDK.EdsPropertyDesc des;
                    lock (LVlock) Error = EDSDK.EdsGetPropertyDesc(MainCamera.Ref, PropID, out des);
                    return des.PropDesc.Take(des.NumElements).ToList();
                }
                else throw new ArgumentException("Method cannot be used with this Property ID");
            }
            else { throw new ArgumentNullException("Camera or camera reference is null/zero"); }
        }

        /// <summary>
        /// Gets the current setting of given property ID as an uint
        /// </summary>
        /// <param name="PropID">The property ID</param>
        /// <returns>The current setting of the camera</returns>
        public uint GetSetting(uint PropID)
        {
            if (MainCamera.Ref != IntPtr.Zero)
            {
                uint property;
                lock (LVlock) Error = EDSDK.EdsGetPropertyData(MainCamera.Ref, PropID, 0, out property);
                return property;
            }
            else { throw new ArgumentNullException("Camera or camera reference is null/zero"); }
        }

        /// <summary>
        /// Gets the current setting of given property ID as a string
        /// </summary>
        /// <param name="PropID">The property ID</param>
        /// <returns>The current setting of the camera</returns>
        public string GetStringSetting(uint PropID)
        {
            if (MainCamera.Ref != IntPtr.Zero)
            {
                string data;
                lock (LVlock) EDSDK.EdsGetPropertyData(MainCamera.Ref, PropID, 0, out data);
                return data;
            }
            else { throw new ArgumentNullException("Camera or camera reference is null/zero"); }
        }
        
        /// <summary>
        /// Gets the current setting of given property ID as a struct
        /// </summary>
        /// <param name="PropID">The property ID</param>
        /// <typeparam name="T">One of the EDSDK structs</typeparam>
        /// <returns>The current setting of the camera</returns>
        public T GetStructSetting<T>(uint PropID) where T : struct
        {
            if (MainCamera.Ref != IntPtr.Zero)
            {
                //get type and size of struct
                Type structureType = typeof(T);
                int bufferSize = Marshal.SizeOf(structureType);

                //allocate memory
                IntPtr ptr = Marshal.AllocHGlobal(bufferSize);
                //retrieve value
                lock (LVlock) Error = EDSDK.EdsGetPropertyData(MainCamera.Ref, PropID, 0, bufferSize, ptr);

                try
                {
                    //convert pointer to managed structure
                    T data = (T)Marshal.PtrToStructure(ptr, structureType);
                    return data;
                }
                finally
                {
                    if (ptr != IntPtr.Zero)
                    {
                        //free the allocated memory
                        Marshal.FreeHGlobal(ptr);
                        ptr = IntPtr.Zero;
                    }
                }
            }
            else { throw new ArgumentNullException("Camera or camera reference is null/zero"); }
        }

        /// <summary>
        /// Sets an uint value for the given property ID
        /// </summary>
        /// <param name="PropID">The property ID</param>
        /// <param name="Value">The value which will be set</param>
        public void SetSetting(uint PropID, uint Value)
        {
            if (MainCamera.Ref != IntPtr.Zero)
            {
                lock (LVlock)
                {
                    int propsize;
                    EDSDK.EdsDataType proptype;
                    //get size of property
                    Error = EDSDK.EdsGetPropertySize(MainCamera.Ref, PropID, 0, out proptype, out propsize);
                    //set given property
                    Error = EDSDK.EdsSetPropertyData(MainCamera.Ref, PropID, 0, propsize, Value);
                }
            }
            else { throw new ArgumentNullException("Camera or camera reference is null/zero"); }
        }

        /// <summary>
        /// Sets a string value for the given property ID
        /// </summary>
        /// <param name="PropID">The property ID</param>
        /// <param name="Value">The value which will be set</param>
        public void SetStringSetting(uint PropID, string Value)
        {
            if (MainCamera.Ref != IntPtr.Zero)
            {
                if (Value == null) throw new ArgumentNullException("String must not be null");

                //convert string to byte array
                byte[] propertyValueBytes = System.Text.Encoding.ASCII.GetBytes(Value + '\0');
                int propertySize = propertyValueBytes.Length;

                //check size of string
                if (propertySize > 32) throw new ArgumentOutOfRangeException("Value must be smaller than 32 bytes");

                //set value
                lock (LVlock) Error = EDSDK.EdsSetPropertyData(MainCamera.Ref, PropID, 0, 32, propertyValueBytes);
            }
            else { throw new ArgumentNullException("Camera or camera reference is null/zero"); }
        }

        /// <summary>
        /// Sets a struct value for the given property ID
        /// </summary>
        /// <param name="PropID">The property ID</param>
        /// <param name="Value">The value which will be set</param>
        public void SetStructSetting<T>(uint PropID, T Value) where T : struct
        {
            if (MainCamera.Ref != IntPtr.Zero)
            {
                lock (LVlock) Error = EDSDK.EdsSetPropertyData(MainCamera.Ref, PropID, 0, Marshal.SizeOf(typeof(T)), Value);
            }
            else { throw new ArgumentNullException("Camera or camera reference is null/zero"); }
        }

        /// <summary>
        /// Starts the LiveView
        /// </summary>
        public void StartLiveView()
        {
            if (!IsLiveViewOn)
            {
                IsLiveViewOn = true;
                SetSetting(EDSDK.PropID_Evf_OutputDevice, EDSDK.EvfOutputDevice_PC);
            }
        }

        /// <summary>
        /// Stops the LiveView
        /// </summary>
        public void StopLiveView()
        {
            IsLiveViewOn = false;
        }

        /// <summary>
        /// Starts LiveView and records it
        /// </summary>
        public void StartEvfFilming()
        {
            if (!IsLiveViewOn)
            {
                SetSetting(EDSDK.PropID_Evf_OutputDevice, EDSDK.EvfOutputDevice_PC);
                IsLiveViewOn = true;
                IsEvfFilming = true;
            }
        }

        /// <summary>
        /// Stops LiveView and filming
        /// </summary>
        public void StopEvfFilming()
        {
            IsLiveViewOn = false;
            IsEvfFilming = false;
        }

        /// <summary>
        /// Starts recording a video and downloads it when finished
        /// </summary>
        /// <param name="FilePath">Directory to where the final video will be saved to</param>
        public void StartFilming(string FilePath)
        {
            if (!IsFilming)
            {
                StartFilming();
                this.DownloadVideo = true;
                ImageSaveDirectory = FilePath;
            }
        }

        /// <summary>
        /// Starts recording a video
        /// </summary>
        public void StartFilming()
        {
            if (!IsFilming)
            {
                //To restore the current setting after recording
                PrevSaveTo = GetSetting(EDSDK.PropID_SaveTo);
                //When recording videos, it has to be saved on the camera internal memory
                SetSetting(EDSDK.PropID_SaveTo, (uint)EDSDK.EdsSaveTo.Camera);
                IsFilming = true;
                this.DownloadVideo = false;
                //Start the video recording
                Error = EDSDK.EdsSetPropertyData(MainCamera.Ref, EDSDK.PropID_Record, 0, 4, 4);
            }
        }

        /// <summary>
        /// Stops recording a video
        /// </summary>
        public void StopFilming()
        {
            if (IsFilming)
            {
                //Stop video recording
                Error = EDSDK.EdsSetPropertyData(MainCamera.Ref, EDSDK.PropID_Record, 0, 4, 0);
                //Set back to previous state
                SetSetting(EDSDK.PropID_SaveTo, PrevSaveTo);
                IsFilming = false;
            }
        }

        /// <summary>
        /// Tells the camera that there is enough space on the HDD if SaveTo is set to Host
        /// This method does not use the actual free space!
        /// </summary>
        public void SetCapacity()
        {
            lock (LVlock)
            {
                //create new capacity struct
                EDSDK.EdsCapacity capacity = new EDSDK.EdsCapacity();

                //set big enough values
                capacity.Reset = 1;
                capacity.BytesPerSector = 0x1000;
                capacity.NumberOfFreeClusters = 0x7FFFFFFF;

                //set the values to camera
                Error = EDSDK.EdsSetCapacity(MainCamera.Ref, capacity);
            }
        }

        /// <summary>
        /// Tells the camera how much space is available on the host PC
        /// </summary>
        /// <param name="BytesPerSector">Bytes per sector on HD</param>
        /// <param name="NumberOfFreeClusters">Number of free clusters on HD</param>
        public void SetCapacity(int BytesPerSector, int NumberOfFreeClusters)
        {
            lock (LVlock)
            {
                //create new capacity struct
                EDSDK.EdsCapacity capacity = new EDSDK.EdsCapacity();

                //set given values
                capacity.Reset = 1;
                capacity.BytesPerSector = BytesPerSector;
                capacity.NumberOfFreeClusters = NumberOfFreeClusters;

                //set the values to camera
                Error = EDSDK.EdsSetCapacity(MainCamera.Ref, capacity);
            }
        }

        /// <summary>
        /// Moves the focus (only works while in LiveView)
        /// </summary>
        /// <param name="Speed">Speed and direction of focus movement</param>
        public void SetFocus(uint Speed)
        {
            if (IsLiveViewOn) lock (LVlock) { Error = EDSDK.EdsSendCommand(MainCamera.Ref, EDSDK.CameraCommand_DriveLensEvf, Speed); }
        }

        /// <summary>
        /// Sets the WB of the LiveView while in LiveView
        /// </summary>
        /// <param name="x">X Coordinate</param>
        /// <param name="y">Y Coordinate</param>
        public void SetManualWBEvf(ushort x, ushort y)
        {
            if (IsLiveViewOn)
            {
                lock (LVlock)
                {
                    //converts the coordinates to a form the camera accepts
                    byte[] xa = BitConverter.GetBytes(x);
                    byte[] ya = BitConverter.GetBytes(y);
                    uint coord = BitConverter.ToUInt32(new byte[] { xa[0], xa[1], ya[0], ya[1] }, 0);
                    //send command to camera
                    Error = EDSDK.EdsSendCommand(MainCamera.Ref, EDSDK.CameraCommand_DoClickWBEvf, coord);
                }
            }
        }

        /// <summary>
        /// Press the shutter button
        /// </summary>
        /// <param name="state">State of the shutter button</param>
        public void PressShutterButton(EDSDK.EdsShutterButton state)
        {
            lock (LVlock) Error = EDSDK.EdsSendCommand(MainCamera.Ref, EDSDK.CameraCommand_PressShutterButton, (uint)state);
        }
        
        /// <summary>
        /// Gets all volumes, folders and files existing on the camera
        /// </summary>
        /// <returns>A CameraFileEntry with all informations</returns>
        public CameraFileEntry GetAllEntries()
        {
            //create the main entry which contains all subentries
            CameraFileEntry MainEntry = new CameraFileEntry("Camera", true);

            //get the number of volumes currently installed in the camera
            int VolumeCount;
            Error = EDSDK.EdsGetChildCount(MainCamera.Ref, out VolumeCount);
            List<CameraFileEntry> VolumeEntries = new List<CameraFileEntry>();

            //iterate through all of them
            for (int i = 0; i < VolumeCount; i++)
            {
                //get information about volume
                IntPtr ChildPtr;
                Error = EDSDK.EdsGetChildAtIndex(MainCamera.Ref, i, out ChildPtr);
                EDSDK.EdsVolumeInfo vinfo;
                Error = EDSDK.EdsGetVolumeInfo(ChildPtr, out vinfo);

                //ignore the HDD
                if (vinfo.szVolumeLabel != "HDD")
                {
                    //add volume to the list
                    VolumeEntries.Add(new CameraFileEntry("Volume" + i + "(" + vinfo.szVolumeLabel + ")", true));
                    //get all child entries on this volume
                    VolumeEntries[i].AddSubEntries(GetChildren(ChildPtr));
                }
                //release the volume
                Error = EDSDK.EdsRelease(ChildPtr);
            }
            //add all volumes to the main entry and return it
            MainEntry.AddSubEntries(VolumeEntries.ToArray());
            return MainEntry;
        }
        
        /// <summary>
        /// Locks or unlocks the cameras UI
        /// </summary>
        /// <param name="LockState">True for locked, false to unlock</param>
        public void UILock(bool LockState)
        {
            lock (LVlock)
            {
                if (LockState == true) Error = EDSDK.EdsSendStatusCommand(MainCamera.Ref, EDSDK.CameraState_UILock, 0);
                else Error = EDSDK.EdsSendStatusCommand(MainCamera.Ref, EDSDK.CameraState_UIUnLock, 0);
            }
        }

        /// <summary>
        /// Takes a photo with the current camera settings
        /// </summary>
        public void TakePhoto()
        {
            //start thread to not block everything
            new Thread(delegate()
            {
                int BusyCount = 0;
                uint err = EDSDK.EDS_ERR_OK;
                lock (LVlock)
                {
                    //sometimes the camera is not ready immediately
                    while (BusyCount < 20)
                    {
                        //send command to camera
                        err = EDSDK.EdsSendCommand(MainCamera.Ref, EDSDK.CameraCommand_TakePicture, 0);
                        //if camera is busy, wait and try again
                        if (err == EDSDK.EDS_ERR_DEVICE_BUSY) { BusyCount++; Thread.Sleep(50); }
                        else { break; }
                    }
                }
                Error = err;
            }).Start();
        }

        /// <summary>
        /// Takes a photo in bulb mode with the current camera settings
        /// </summary>
        /// <param name="BulbTime">The time in milliseconds for how long the shutter will be open</param>
        public void TakePhoto(uint BulbTime)
        {
            //start thread to not block everything
            new Thread(delegate()
            {
                lock (LVlock)
                {
                    //bulbtime has to be at least a second
                    if (BulbTime < 1000) { throw new ArgumentException("Bulbtime has to be bigger than 1000ms"); }
                    int BusyCount = 0;
                    uint err = EDSDK.EDS_ERR_OK;
                    //sometimes the camera is not ready immediately
                    while (BusyCount < 20)
                    {
                        //open the shutter
                        err = EDSDK.EdsSendCommand(MainCamera.Ref, EDSDK.CameraCommand_BulbStart, 0);
                        //if camera is busy, wait and try again
                        if (err == EDSDK.EDS_ERR_DEVICE_BUSY) { BusyCount++; Thread.Sleep(50); }
                        else { break; }
                    }
                    Error = err;

                    //wait for the specified time
                    Thread.Sleep((int)BulbTime);
                    //close shutter
                    Error = EDSDK.EdsSendCommand(MainCamera.Ref, EDSDK.CameraCommand_BulbEnd, 0);
                }
            }).Start();
        }
        


        /// <summary>
        /// Downloads the LiveView image
        /// </summary>
        private void DownloadEvf()
        {
            LVThread = new Thread(delegate()
            {
                //To give the camera time to switch the mirror
                Thread.Sleep(1500);

                IntPtr jpgPointer;
                IntPtr stream = IntPtr.Zero;
                IntPtr EvfImageRef = IntPtr.Zero;
                UnmanagedMemoryStream ums;

                uint err;
                uint length;
                Image img;
                //create streams
                Error = EDSDK.EdsCreateMemoryStream(0, out stream);
                Error = EDSDK.EdsCreateEvfImageRef(stream, out EvfImageRef);

                Stopwatch watch = new Stopwatch();  //stopwatch for FPS calculation
                float lastfr = 24; //last actual FPS

                //Run LiveView
                while (IsLiveViewOn)
                {
                    lock (LVlock)
                    {
                        watch.Restart();
                        //download current LiveView image
                        err = EDSDK.EdsDownloadEvfImage(MainCamera.Ref, EvfImageRef);

                        //get pointer
                        Error = EDSDK.EdsGetPointer(stream, out jpgPointer);
                        Error = EDSDK.EdsGetLength(stream, out length);

                        unsafe
                        {
                            //create stream to image
                            ums = new UnmanagedMemoryStream((byte*)jpgPointer.ToPointer(), length, length, FileAccess.Read);
                            //create image from the stream
                            img = Image.FromStream(ums);
                            ums.Close();
                        }
                    }
                    //fire the LiveViewUpdated event with the LiveView image
                    if (LiveViewUpdated != null) LiveViewUpdated(img);

                    //calculate the framerate and fire the FrameRateUpdated event
                    lastfr = lastfr * 0.9f + (100f / watch.ElapsedMilliseconds);
                    if (FrameRateUpdated != null) FrameRateUpdated(lastfr);
                }

                //Release and finish
                if (stream != IntPtr.Zero) { Error = EDSDK.EdsRelease(stream); }
                if (EvfImageRef != IntPtr.Zero) { Error = EDSDK.EdsRelease(EvfImageRef); }
                //stop the LiveView
                SetSetting(EDSDK.PropID_Evf_OutputDevice, 0);
            });
            LVThread.Start();
        }

        /// <summary>
        /// Records the LiveView image
        /// </summary>
        private void DownloadEvfFilm()
        {
            LVThread = new Thread(delegate()
            {
                //To give the camera time to switch the mirror
                Thread.Sleep(1500);

                IntPtr jpgPointer;
                IntPtr stream = IntPtr.Zero;
                IntPtr EvfImageRef = IntPtr.Zero;
                UnmanagedMemoryStream ums;
                uint err;
                uint length;
                lock (LVlock)
                {
                    err = EDSDK.EdsCreateMemoryStream(0, out stream);
                    err = EDSDK.EdsCreateEvfImageRef(stream, out EvfImageRef);

                    //Download one frame to init the video size
                    err = EDSDK.EdsDownloadEvfImage(MainCamera.Ref, EvfImageRef);
                    unsafe
                    {
                        Error = EDSDK.EdsGetPointer(stream, out jpgPointer);
                        Error = EDSDK.EdsGetLength(stream, out length);
                        ums = new UnmanagedMemoryStream((byte*)jpgPointer.ToPointer(), length, length, FileAccess.Read);

                        Bitmap bmp = new Bitmap(ums);
                        StartEvfVideoWriter(bmp.Width, bmp.Height);
                        bmp.Dispose();
                        ums.Close();
                    }
                }

                Stopwatch watch = new Stopwatch();
                byte[] barr;        //bitmap byte array
                const long ft = 41; //Frametime at 24FPS (actually 41.66)
                float lastfr = 24;  //last actual FPS

                int LVUpdateBreak1 = 0;

                //Run LiveView
                while (IsEvfFilming)
                {
                    lock (LVlock)
                    {
                        watch.Restart();
                        err = EDSDK.EdsDownloadEvfImage(MainCamera.Ref, EvfImageRef);
                        unsafe
                        {
                            Error = EDSDK.EdsGetPointer(stream, out jpgPointer);
                            Error = EDSDK.EdsGetLength(stream, out length);
                            ums = new UnmanagedMemoryStream((byte*)jpgPointer.ToPointer(), length, length, FileAccess.Read);
                            barr = new byte[length];
                            ums.Read(barr, 0, (int)length);

                            //For better performance the LiveView is only updated with every 4th frame
                            if (LVUpdateBreak1 == 0 && LiveViewUpdated != null) { LiveViewUpdated(Image.FromStream(ums)); LVUpdateBreak1 = 4; }
                            LVUpdateBreak1--;
                            FrameBuffer.Enqueue(barr);

                            ums.Close();
                        }
                    }
                    //To get a steady framerate:
                    while (true) if (watch.ElapsedMilliseconds >= ft) break;
                    lastfr = lastfr * 0.9f + (100f / watch.ElapsedMilliseconds);
                    if (FrameRateUpdated != null) FrameRateUpdated(lastfr);
                }

                //Release and finish
                if (stream != IntPtr.Zero) { Error = EDSDK.EdsRelease(stream); }
                if (EvfImageRef != IntPtr.Zero) { Error = EDSDK.EdsRelease(EvfImageRef); }
                SetSetting(EDSDK.PropID_Evf_OutputDevice, 0);
            });
            LVThread.Start();
        }

        /// <summary>
        /// Writes video frames from the buffer to a file
        /// </summary>
        /// <param name="Width">Width of the video</param>
        /// <param name="Height">Height of the video</param>
        private void StartEvfVideoWriter(int Width, int Height)
        {
            new Thread(delegate()
            {
                byte[] byteArray;
                ImageConverter ic = new ImageConverter();
                Image img;

                while (IsEvfFilming)
                {
                    while (FrameBuffer.Count > 0)
                    {
                        byteArray = FrameBuffer.Dequeue();
                        img = (Image)ic.ConvertFrom(byteArray);
                        //Save video frame here. e.g. with the VideoFileWriter from the AForge library.
                    }
                    if (IsEvfFilming) Thread.Sleep(10);
                }
            }).Start();
        }

        /// <summary>
        /// Downloads data from the camera
        /// </summary>
        /// <param name="ObjectPointer">Pointer to the object</param>
        /// <param name="stream">Pointer to the stream created in advance</param>
        private void DownloadData(IntPtr ObjectPointer, IntPtr stream)
        {
            //get information about the object
            EDSDK.EdsDirectoryItemInfo dirInfo;
            Error = EDSDK.EdsGetDirectoryItemInfo(ObjectPointer, out dirInfo);

            //download the data in blocks
            uint blockSize = 1024 * 1024;
            uint remainingBytes = dirInfo.Size;
            do
            {
                if (remainingBytes < blockSize) { blockSize = (uint)(remainingBytes / 512) * 512; }
                remainingBytes -= blockSize;
                Error = EDSDK.EdsDownload(ObjectPointer, blockSize, stream);
                //report the progress
                if (ProgressChanged != null) ProgressChanged((int)(remainingBytes * 100d / dirInfo.Size));
            } while (remainingBytes > 512);

            //download the remaining data
            Error = EDSDK.EdsDownload(ObjectPointer, remainingBytes, stream);
            //report to the camera that the download is complete
            Error = EDSDK.EdsDownloadComplete(ObjectPointer);
        }

        /// <summary>
        /// Creates a Bitmap out of a stream
        /// </summary>
        /// <param name="img_stream">Image stream</param>
        /// <param name="imageSource">Type of image</param>
        /// <returns>The bitmap from the stream</returns>
        private Bitmap GetImage(IntPtr img_stream, EDSDK.EdsImageSource imageSource)
        {
            IntPtr stream = IntPtr.Zero;
            IntPtr img_ref = IntPtr.Zero;
            IntPtr streamPointer = IntPtr.Zero;
            EDSDK.EdsImageInfo imageInfo;
            EDSDK.EdsSize outputSize = new EDSDK.EdsSize();
            Byte[] buffer;
            Byte temp;

            //create reference to image
            Error = EDSDK.EdsCreateImageRef(img_stream, out img_ref);
            //get information about image
            Error = EDSDK.EdsGetImageInfo(img_ref, imageSource, out imageInfo);

            //calculate size, stride and buffersize
            outputSize.width = imageInfo.EffectiveRect.width;
            outputSize.height = imageInfo.EffectiveRect.height;
            int Stride = ((outputSize.width * 3) + 3) & ~3;
            uint bufferSize = (uint)(outputSize.height * Stride);

            //Init buffer
            buffer = new Byte[bufferSize];
            //Create memory stream to buffer
            Error = EDSDK.EdsCreateMemoryStreamFromPointer(buffer, bufferSize, out stream);
            //copy image into buffer
            Error = EDSDK.EdsGetImage(img_ref, imageSource, EDSDK.EdsTargetImageType.RGB, imageInfo.EffectiveRect, outputSize, stream);

            //makes RGB out of BGR
            if (outputSize.width % 4 == 0)
            {
                for (int t = 0; t < bufferSize; t += 3)
                {
                    temp = buffer[t];
                    buffer[t] = buffer[t + 2];
                    buffer[t + 2] = temp;
                }
            }
            else
            {
                int Padding = Stride - (outputSize.width * 3);
                for (int y = outputSize.height - 1; y > -1; y--)
                {
                    int RowStart = (outputSize.width * 3) * y;
                    int TargetStart = Stride * y;

                    Array.Copy(buffer, RowStart, buffer, TargetStart, outputSize.width * 3);

                    for (int t = TargetStart; t < TargetStart + (outputSize.width * 3); t += 3)
                    {
                        temp = buffer[t];
                        buffer[t] = buffer[t + 2];
                        buffer[t + 2] = temp;
                    }
                }
            }

            //create pointer to image data
            Error = EDSDK.EdsGetPointer(stream, out streamPointer);
            //Release all ressources
            Error = EDSDK.EdsRelease(img_stream);
            Error = EDSDK.EdsRelease(img_ref);
            Error = EDSDK.EdsRelease(stream);

            //create and return Bitmap from pointer
            return new Bitmap(outputSize.width, outputSize.height, Stride, PixelFormat.Format24bppRgb, streamPointer);
        }

        /// <summary>
        /// Gets the children of a camera folder/volume. Recursive method.
        /// </summary>
        /// <param name="ptr">Pointer to volume or folder</param>
        /// <returns></returns>
        private CameraFileEntry[] GetChildren(IntPtr ptr)
        {
            int ChildCount;
            //get children of first pointer
            Error = EDSDK.EdsGetChildCount(ptr, out ChildCount);
            if (ChildCount > 0)
            {
                //if it has children, create an array of entries
                CameraFileEntry[] MainEntry = new CameraFileEntry[ChildCount];
                for (int i = 0; i < ChildCount; i++)
                {
                    IntPtr ChildPtr;
                    //get children of children
                    Error = EDSDK.EdsGetChildAtIndex(ptr, i, out ChildPtr);
                    //get the information about this children
                    EDSDK.EdsDirectoryItemInfo ChildInfo;
                    Error = EDSDK.EdsGetDirectoryItemInfo(ChildPtr, out ChildInfo);

                    //create entry from information
                    if (ImageName != string.Empty)
                    {
                        MainEntry[i] = new CameraFileEntry(ImageName + ".jpg", GetBool(ChildInfo.isFolder));
                    }
                    else
                    {
                        MainEntry[i] = new CameraFileEntry(ChildInfo.szFileName, GetBool(ChildInfo.isFolder));
                    }
                    if (!MainEntry[i].IsFolder)
                    {
                        //if it's not a folder, create thumbnail and safe it to the entry
                        IntPtr stream;
                        Error = EDSDK.EdsCreateMemoryStream(0, out stream);
                        Error = EDSDK.EdsDownloadThumbnail(ChildPtr, stream);
                        MainEntry[i].AddThumb(GetImage(stream, EDSDK.EdsImageSource.Thumbnail));
                    }
                    else
                    {
                        //if it's a folder, check for children with recursion
                        CameraFileEntry[] retval = GetChildren(ChildPtr);
                        if (retval != null) MainEntry[i].AddSubEntries(retval);
                    }
                    //release current children
                    Error = EDSDK.EdsRelease(ChildPtr);
                }
                return MainEntry;
            }
            else return null;
        }

        /// <summary>
        /// Converts an int to a bool
        /// </summary>
        /// <param name="val">Value</param>
        /// <returns>A bool created from the value</returns>
        private bool GetBool(int val)
        {
            if (val == 0) return false;
            else return true;
        }

        #endregion
    }

    public class Camera
    {
        internal IntPtr Ref;
        public EDSDK.EdsDeviceInfo Info { get; private set; }

        public uint Error
        {
            get { return EDSDK.EDS_ERR_OK; }
            set { if (value != EDSDK.EDS_ERR_OK) throw new Exception("SDK Error: " + value); }
        }

        public Camera(IntPtr Reference)
        {
            if (Reference == IntPtr.Zero) throw new ArgumentNullException("Camera pointer is zero");
            this.Ref = Reference;
            EDSDK.EdsDeviceInfo dinfo;
            Error = EDSDK.EdsGetDeviceInfo(Reference, out dinfo);
            this.Info = dinfo;
        }
    }

    public static class CameraValues
    {
        private static CultureInfo cInfo = new CultureInfo("en-US");

        public static string FILTER_EFFECT(uint filterEffect)
        {
            switch (filterEffect)
            {
                case (uint)EDSDK.FilterEffect_None:
                    return "None";
                case (uint)EDSDK.FilterEffect_Yellow:
                    return "Yellow";
                case (uint)EDSDK.FilterEffect_Orange:
                    return "Orange";
                case (uint)EDSDK.FilterEffect_Red:
                    return "Red";
                case EDSDK.FilterEffect_Green:
                    return "Green";
                default:
                    return "N/A";
            }
        }

        public static uint FILTER_EFFECT(string filterEffect)
        {
            switch (filterEffect)
            {
                case "Yellow":
                    return (uint)EDSDK.FilterEffect_Yellow;
                case "Orange":
                    return (uint)EDSDK.FilterEffect_Orange;
                case "Red":
                    return (uint)EDSDK.FilterEffect_Red;
                case "Green":
                    return (uint)EDSDK.FilterEffect_Green;
                default:
                    return (uint)EDSDK.FilterEffect_None;
            }
        }

        public static string TONING_EFFECT(uint toningEffect)
        {
            switch (toningEffect)
            {
                case EDSDK.TonigEffect_None:
                    return "None";
                case (uint)EDSDK.TonigEffect_Sepia:
                    return "Sepia";
                case (uint)EDSDK.TonigEffect_Blue:
                    return "Blue";
                case (uint)EDSDK.TonigEffect_Purple:
                    return "Violet";
                case (uint)EDSDK.TonigEffect_Green:
                    return "Green";
                default:
                    return "N/A";
            }
        }


        public static uint TONING_EFFECT(string toningEffect)
        {
            switch (toningEffect)
            {
                case "None":
                    return (uint)EDSDK.TonigEffect_None;
                case "Sepia":
                    return (uint)EDSDK.TonigEffect_Sepia;
                case "Blue":
                    return (uint)EDSDK.TonigEffect_Blue;
                case "Violet":
                    return (uint)EDSDK.TonigEffect_Purple;
                case "Green":
                    return (uint)EDSDK.TonigEffect_Green;
                default:
                    return 0xffffffff;
            }
        }
        public static string PICTURESTYLE(uint pictureStyle)
        {
            switch (pictureStyle)
            {
                case (uint)EDSDK.PictureStyle_Standard:
                    return "Standard";
                case (uint)EDSDK.PictureStyle_Portrait:
                    return "Portrait";
                case (uint)EDSDK.PictureStyle_Landscape:
                    return "LandScape";
                case (uint)EDSDK.PictureStyle_Neutral:
                    return "Neutral";
                case (uint)EDSDK.PictureStyle_Faithful:
                    return "Faithful";
                case (uint)EDSDK.PictureStyle_Monochrome:
                    return "Monochrome";
                case (uint)EDSDK.PictureStyle_User1:
                    return "User Define 1";
                case (uint)EDSDK.PictureStyle_User2:
                    return "User Define 2";
                case (uint)EDSDK.PictureStyle_User3:
                    return "User Define 3";
                default:
                    return "N/A";
            }
        }
        public static uint PICTURESTYLE(string pictureStyle)
        {
            switch (pictureStyle)
            {
                case "Standard":
                    return (uint)EDSDK.PictureStyle_Standard;
                case "Portrait":
                    return (uint)EDSDK.PictureStyle_Portrait;
                case "LandScape":
                    return (uint)EDSDK.PictureStyle_Landscape;
                case "Neutral":
                    return (uint)EDSDK.PictureStyle_Neutral;
                case "Faithful":
                    return (uint)EDSDK.PictureStyle_Faithful;
                case "Monochrome":
                    return (uint)EDSDK.PictureStyle_Monochrome;
                case "User Define 1":
                    return (uint)EDSDK.PictureStyle_User1;
                case "User Define 2":
                    return (uint)EDSDK.PictureStyle_User2;
                case "User Define 3":
                    return (uint)EDSDK.PictureStyle_User3;
                default:
                    return 0xffffffff;
            }
        }
        public static uint WB(string wb)
        {
            switch (wb)
            {
                case "Auto":
                    return 0;
                case "Daylight":
                    return 1;
                case "Cloudy":
                    return 2;
                case "Tungsten":
                    return 3;
                case "Fluorescent":
                    return 4;
                case "Flash":
                    return 5;
                case "Manual":
                    return 6;
                case "Shade":
                    return 8;
                default:
                    return 0xffffffff;
            }
        }
        public static string WB(uint wb)
        {
            switch (wb)
            {
                case 0:
                    return "Auto";
                case 1:
                    return "Daylight";
                case 2:
                    return "Cloudy";
                case 3:
                    return "Tungsten";
                case 4:
                    return "Fluorescent";
                case 5:
                    return "Flash";
                case 6:
                    return "Manual";
                case 8:
                    return "Shade";
                default:
                    return "N/A";
            }
        }
        public static string AV(uint v)
        {
            switch (v)
            {
                case 0x08:
                    return "1";
                case 0x40:
                    return "11";
                case 0x0B:
                    return "1.1";
                case 0x43:
                    return "13 (1/3)";
                case 0x0C:
                    return "1.2";
                case 0x44:
                    return "13";
                case 0x0D:
                    return "1.2 (1/3)";
                case 0x45:
                    return "14";
                case 0x10:
                    return "1.4";
                case 0x48:
                    return "16";
                case 0x13:
                    return "1.6";
                case 0x4B:
                    return "18";
                case 0x14:
                    return "1.8";
                case 0x4C:
                    return "19";
                case 0x15:
                    return "1.8 (1/3)";
                case 0x4D:
                    return "20";
                case 0x18:
                    return "2";
                case 0x50:
                    return "22";
                case 0x1B:
                    return "2.2";
                case 0x53:
                    return "25";
                case 0x1C:
                    return "2.5";
                case 0x54:
                    return "27";
                case 0x1D:
                    return "2.5 (1/3)";
                case 0x55:
                    return "29";
                case 0x20:
                    return "2.8";
                case 0x58:
                    return "32";
                case 0x23:
                    return "3.2";
                case 0x5B:
                    return "36";
                case 0x24:
                    return "3.5";
                case 0x5C:
                    return "38";
                case 0x25:
                    return "3.5 (1/3)";
                case 0x5D:
                    return "40";
                case 0x28:
                    return "4";
                case 0x60:
                    return "45";
                case 0x2B:
                    return "4.5";
                case 0x63:
                    return "51";
                case 0x2C:
                    return "4.5 (1/3)";
                case 0x64:
                    return "54";
                case 0x2D:
                    return "5.0";
                case 0x65:
                    return "57";
                case 0x30:
                    return "5.6";
                case 0x68:
                    return "64";
                case 0x33:
                    return "6.3";
                case 0x6B:
                    return "72";
                case 0x34:
                    return "6.7";
                case 0x6C:
                    return "76";
                case 0x35:
                    return "7.1";
                case 0x6D:
                    return "80";
                case 0x38:
                    return " 8";
                case 0x70:
                    return "91";
                case 0x3B:
                    return "9";
                case 0x3C:
                    return "9.5";
                case 0x3D:
                    return "10";

                case 0xffffffff:
                default:
                    return "N/A";
            }
        }

        public static string ISO(uint v)
        {
            switch (v)
            {
                case 0x00000000:
                    return "Auto ISO";
                case 0x00000028:
                    return "ISO 6";
                case 0x00000030:
                    return "ISO 12";
                case 0x00000038:
                    return "ISO 25";
                case 0x00000040:
                    return "ISO 50";
                case 0x00000048:
                    return "ISO 100";
                case 0x0000004b:
                    return "ISO 125";
                case 0x0000004d:
                    return "ISO 160";
                case 0x00000050:
                    return "ISO 200";
                case 0x00000053:
                    return "ISO 250";
                case 0x00000055:
                    return "ISO 320";
                case 0x00000058:
                    return "ISO 400";
                case 0x0000005b:
                    return "ISO 500";
                case 0x0000005d:
                    return "ISO 640";
                case 0x00000060:
                    return "ISO 800";
                case 0x00000063:
                    return "ISO 1000";
                case 0x00000065:
                    return "ISO 1250";
                case 0x00000068:
                    return "ISO 1600";
                case 0x00000070:
                    return "ISO 3200";
                case 0x00000078:
                    return "ISO 6400";
                case 0x00000080:
                    return "ISO 12800";
                case 0x00000088:
                    return "ISO 25600";
                case 0x00000090:
                    return "ISO 51200";
                case 0x00000098:
                    return "ISO 102400";
                case 0xffffffff:
                default:
                    return "N/A";
            }
        }

        public static string TV(uint v)
        {
            switch (v)
            {
                case 0x0C:
                    return "Bulb";
                case 0x5D:
                    return "1/25";
                case 0x10:
                    return "30\"";
                case 0x60:
                    return "1/30";
                case 0x13:
                    return "25\"";
                case 0x63:
                    return "1/40";
                case 0x14:
                    return "20\"";
                case 0x64:
                    return "1/45";
                case 0x15:
                    return "20\" (1/3)";
                case 0x65:
                    return "1/50";
                case 0x18:
                    return "15\"";
                case 0x68:
                    return "1/60";
                case 0x1B:
                    return "13\"";
                case 0x6B:
                    return "1/80";
                case 0x1C:
                    return "10\"";
                case 0x6C:
                    return "1/90";
                case 0x1D:
                    return "10\" (1/3)";
                case 0x6D:
                    return "1/100";
                case 0x20:
                    return "8\"";
                case 0x70:
                    return "1/125";
                case 0x23:
                    return "6\" (1/3)";
                case 0x73:
                    return "1/160";
                case 0x24:
                    return "6\"";
                case 0x74:
                    return "1/180";
                case 0x25:
                    return "5\"";
                case 0x75:
                    return "1/200";
                case 0x28:
                    return "4\"";
                case 0x78:
                    return "1/250";
                case 0x2B:
                    return "3\"2";
                case 0x7B:
                    return "1/320";
                case 0x2C:
                    return "3\"";
                case 0x7C:
                    return "1/350";
                case 0x2D:
                    return "2\"5";
                case 0x7D:
                    return "1/400";
                case 0x30:
                    return "2\"";
                case 0x80:
                    return "1/500";
                case 0x33:
                    return "1\"6";
                case 0x83:
                    return "1/640";
                case 0x34:
                    return "1\"5";
                case 0x84:
                    return "1/750";
                case 0x35:
                    return "1\"3";
                case 0x85:
                    return "1/800";
                case 0x38:
                    return "1\"";
                case 0x88:
                    return "1/1000";
                case 0x3B:
                    return "0\"8";
                case 0x8B:
                    return "1/1250";
                case 0x3C:
                    return "0\"7";
                case 0x8C:
                    return "1/1500";
                case 0x3D:
                    return "0\"6";
                case 0x8D:
                    return "1/1600";
                case 0x40:
                    return "0\"5";
                case 0x90:
                    return "1/2000";
                case 0x43:
                    return "0\"4";
                case 0x93:
                    return "1/2500";
                case 0x44:
                    return "0\"3";
                case 0x94:
                    return "1/3000";
                case 0x45:
                    return "0\"3 (1/3)";
                case 0x95:
                    return "1/3200";
                case 0x48:
                    return "1/4";
                case 0x98:
                    return "1/4000";
                case 0x4B:
                    return "1/5";
                case 0x9B:
                    return "1/5000";
                case 0x4C:
                    return "1/6";
                case 0x9C:
                    return "1/6000";
                case 0x4D:
                    return "1/6 (1/3)";
                case 0x9D:
                    return "1/6400";
                case 0x50:
                    return "1/8";
                case 0xA0:
                    return "1/8000";
                case 0x53:
                    return "1/10 (1/3)";
                case 0x54:
                    return "1/10";
                case 0x55:
                    return "1/13";
                case 0x58:
                    return "1/15";
                case 0x5B:
                    return "1/20 (1/3)";
                case 0x5C:
                    return "1/20";

                case 0xffffffff:
                default:
                    return "N/A";
            }
        }

        public static string Drive(uint v)
        {
            switch (v)
            { 
                case 0x00000000:
                    return "1 shot";
                case 0x00000001:
                    return "++shooting";
                case 0x00000002:
                    return "video";
                case 0x00000004:
                    return "High-Speed ++shots";
                case 0x00000005:
                    return "Low-Speed ++shots";
                case 0x00000006:
                    return "silent 1 shot";
                case 0x00000007:
                    return "10(s)timer & ++shots";
                case 0x00000010:
                    return "10(s)timer";
                case 0x00000011:
                    return "2(s)timer";
                case 0xffffffff:
                default:
                    return "N/A";
            }
        }

        public static string Metering(uint v)
        {
            switch (v)
            { 
                case 1:
                    return "Spot Metering";
                case 3:
                    return "Evaluative Metering";
                case 4:
                    return "Partial Metering";
                case 5:
                    return "Center-Wighted averaging metering";
                case 0xFFFFFFFF:
                default:
                    return "N/A";
            }
        }

        public static string AFMode(uint v)
        {
            switch (v)
            { 
                case 0:
                    return "1 shot AF";
                case 1:
                    return "All servo AF";
                case 2:
                    return "AI Focus AF";
                case 3:
                    return "Manual Focus";
                case 0xffffffff:
                default:
                    return "N/A";
            }
        }

        public static string ExpoComp (uint v)
        {
            switch(v)
            {
                case 0x18:
                    return "+3";
                case 0x15:
                    return "+2 2/3";
                case 0x14:
                    return "+2 1/2";
                case 0x13:
                    return "+2 1/3";
                case 0x10:
                    return "+2";
                case 0x0D:
                    return "+1 2/3";
                case 0x0C:
                    return "+1 1/2";
                case 0x0B:
                    return "+1 1/3";
                case 0x08:
                    return "+1";
                case 0x05:
                    return "+2/3";
                case 0x04:
                    return "+1/2";
                case 0x03:
                    return "+1/3";
                case 0x00:
                    return "0";
                case 0xFD:
                    return "-1/3";
                case 0xFC:
                    return "-1/2";
                case 0xFB:
                    return "-2/3";
                case 0xF8:
                    return "-1";
                case 0xF5:
                    return "-1 1/3";
                case 0xF4:
                    return "-1 1/2";
                case 0xF3:
                    return "-1 2/3";
                case 0xF0:
                    return "-2";
                case 0xED:
                    return "-2 1/3";
                case 0xEC:
                    return "-2 1/2";
                case 0xEB:
                    return "-2 2/3";
                case 0xE8:
                    return "-3";
                case 0xffffffff:
                default:
                    return "N/A";
            }
        }

        //public static string PicStyle(uint v)
        //{
        //    switch (v)
        //    {
        //        case 0x0081:
        //            return "Standard";
        //        case 0x0082:
        //            return "Portrait";
        //        case 0x0083:
        //            return "Landscape";
        //        case 0x0084:
        //            return "Neutral";
        //        case 0x0085:
        //            return "Faithful";
        //        case 0x0086:
        //            return "Monochrome";
        //        case 0x0087:
        //            return "Auto";
        //        case 0x0041:
        //            return "Computer Sett1";
        //        case 0x0042:
        //            return "Computer Sett2";
        //        case 0x0043:
        //            return "Computer Sett3";
        //        case 0xFFFF:
        //        default:
        //            return "N/A";
        //    }
        //}

        public static string FillterEff(uint v)
        {
            switch (v)
            {
                case 0:
                    return "None";
                case 1:
                    return "Yellow";
                case 2:
                    return "Orange";
                case 3:
                    return "Red";
                case 4:
                    return "Green";
                case 0xFFFFFFFF:
                default:
                    return "Unknown";
            }
        }

        public static string MonoTone(uint v)
        {
            switch (v)
            {
                case 0:
                    return "None";
                case 1:
                    return "Sepia";
                case 2:
                    return "Blue";
                case 3:
                    return "Violet";
                case 4:
                    return "Green";
                case 0xFFFFFFFF:
                default:
                    return "Unknown";
            }
        }

        public static string QUALITY(uint q)
        {
            switch (q)
            {
                case 1310479:
                    return "Large(High Quality)";
                case 1244943:
                    return "Large(Standard)";
                case 18087695:
                    return "Medium(High Quality)";
                case 18022159:
                    return "Medium(Standard)";
                case 236191503:
                    return "Small 1 (High Quality)";
                case 236125967:
                    return "Small 1 (Standard)";
                case 252968719:
                    return "Small 2 (For Print)";
                case 269745935:
                    return "Small 3 (For Emailing)";
                case 6553619:
                    return "RAW And Large(High Quality)";
                case 6618895:
                    return "RAW";
                case 0xffffffff:
                default:
                    return "N/A";
            }
        }


        
        
        public static uint QUALITY(string q)
        {
            switch (q)
            {
                case "Large(High Quality)":
                    return 1310479;
                case "Large(Standard)":
                    return 1244943;
                case "Medium(High Quality)":
                    return 18087695;
                case "Medium(Standard)":
                    return 18022159;
                case "Small 1 (High Quality)":
                    return 236191503;
                case "Small 1 (Standard)":
                    return 236125967;
                case "Small 2 (For Print)":
                    return 252968719;
                case "Small 3 (For Emailing)":
                    return 269745935;
                case "RAW And Large(High Quality)":
                    return 6553619;
                case "RAW":
                    return 6618895;
                case "N/A":
                default:
                    return 0xffffffff;
            }
        }

        public static uint AV(string v)
        {
            switch (v)
            {
                case "1":
                    return 0x08;
                case "11":
                    return 0x40;
                case "1.1":
                    return 0x0B;
                case "13 (1/3)":
                    return 0x43;
                case "1.2":
                    return 0x0C;
                case "13":
                    return 0x44;
                case "1.2 (1/3)":
                    return 0x0D;
                case "14":
                    return 0x45;
                case "1.4":
                    return 0x10;
                case "16":
                    return 0x48;
                case "1.6":
                    return 0x13;
                case "18":
                    return 0x4B;
                case "1.8":
                    return 0x14;
                case "19":
                    return 0x4C;
                case "1.8 (1/3)":
                    return 0x15;
                case "20":
                    return 0x4D;
                case "2":
                    return 0x18;
                case "22":
                    return 0x50;
                case "2.2":
                    return 0x1B;
                case "25":
                    return 0x53;
                case "2.5":
                    return 0x1C;
                case "27":
                    return 0x54;
                case "2.5 (1/3)":
                    return 0x1D;
                case "29":
                    return 0x55;
                case "2.8":
                    return 0x20;
                case "32":
                    return 0x58;
                case "3.2":
                    return 0x23;
                case "36":
                    return 0x5B;
                case "3.5":
                    return 0x24;
                case "38":
                    return 0x5C;
                case "3.5 (1/3)":
                    return 0x25;
                case "40":
                    return 0x5D;
                case "4":
                    return 0x28;
                case "45":
                    return 0x60;
                case "4.5":
                    return 0x2B;
                case "51":
                    return 0x63;
                case "4.5 (1/3)":
                    return 0x2C;
                case "54":
                    return 0x64;
                case "5.0":
                    return 0x2D;
                case "57":
                    return 0x65;
                case "5.6":
                    return 0x30;
                case "64":
                    return 0x68;
                case "6.3":
                    return 0x33;
                case "72":
                    return 0x6B;
                case "6.7":
                    return 0x34;
                case "76":
                    return 0x6C;
                case "7.1":
                    return 0x35;
                case "80":
                    return 0x6D;
                case " 8":
                    return 0x38;
                case "91":
                    return 0x70;
                case "9":
                    return 0x3B;
                case "9.5":
                    return 0x3C;
                case "10":
                    return 0x3D;

                case "N/A":
                default:
                    return 0xffffffff;
            }
        }

        public static uint ISO(string v)
        {
            switch (v)
            {
                case "Auto ISO":
                    return 0x00000000;
                case "ISO 6":
                    return 0x00000028;
                case "ISO 12":
                    return 0x00000030;
                case "ISO 25":
                    return 0x00000038;
                case "ISO 50":
                    return 0x00000040;
                case "ISO 100":
                    return 0x00000048;
                case "ISO 125":
                    return 0x0000004b;
                case "ISO 160":
                    return 0x0000004d;
                case "ISO 200":
                    return 0x00000050;
                case "ISO 250":
                    return 0x00000053;
                case "ISO 320":
                    return 0x00000055;
                case "ISO 400":
                    return 0x00000058;
                case "ISO 500":
                    return 0x0000005b;
                case "ISO 640":
                    return 0x0000005d;
                case "ISO 800":
                    return 0x00000060;
                case "ISO 1000":
                    return 0x00000063;
                case "ISO 1250":
                    return 0x00000065;
                case "ISO 1600":
                    return 0x00000068;
                case "ISO 3200":
                    return 0x00000070;
                case "ISO 6400":
                    return 0x00000078;
                case "ISO 12800":
                    return 0x00000080;
                case "ISO 25600":
                    return 0x00000088;
                case "ISO 51200":
                    return 0x00000090;
                case "ISO 102400":
                    return 0x00000098;

                case "N/A":
                default:
                    return 0xffffffff;
            }
        }

        public static uint ShautterSpeed(string v)
        {
            switch (v)
            {
                case "Bulb":
                    return 0x0C;
                case "1/25":
                    return 0x5D;
                case "30\"":
                    return 0x10;
                case "1/30":
                    return 0x60;
                case "25\"":
                    return 0x13;
                case "1/40":
                    return 0x63;
                case "20\"":
                    return 0x14;
                case "1/45":
                    return 0x64;
                case "20\" (1/3)":
                    return 0x15;
                case "1/50":
                    return 0x65;
                case "15\"":
                    return 0x18;
                case "1/60":
                    return 0x68;
                case "13\"":
                    return 0x1B;
                case "1/80":
                    return 0x6B;
                case "10\"":
                    return 0x1C;
                case "1/90":
                    return 0x6C;
                case "10\" (1/3)":
                    return 0x1D;
                case "1/100":
                    return 0x6D;
                case "8\"":
                    return 0x20;
                case "1/125":
                    return 0x70;
                case "6\" (1/3)":
                    return 0x23;
                case "1/160":
                    return 0x73;
                case "6\"":
                    return 0x24;
                case "1/180":
                    return 0x74;
                case "5\"":
                    return 0x25;
                case "1/200":
                    return 0x75;
                case "4\"":
                    return 0x28;
                case "1/250":
                    return 0x78;
                case "3\"2":
                    return 0x2B;
                case "1/320":
                    return 0x7B;
                case "3\"":
                    return 0x2C;
                case "1/350":
                    return 0x7C;
                case "2\"5":
                    return 0x2D;
                case "1/400":
                    return 0x7D;
                case "2\"":
                    return 0x30;
                case "1/500":
                    return 0x80;
                case "1\"6":
                    return 0x33;
                case "1/640":
                    return 0x83;
                case "1\"5":
                    return 0x34;
                case "1/750":
                    return 0x84;
                case "1\"3":
                    return 0x35;
                case "1/800":
                    return 0x85;
                case "1\"":
                    return 0x38;
                case "1/1000":
                    return 0x88;
                case "0\"8":
                    return 0x3B;
                case "1/1250":
                    return 0x8B;
                case "0\"7":
                    return 0x3C;
                case "1/1500":
                    return 0x8C;
                case "0\"6":
                    return 0x3D;
                case "1/1600":
                    return 0x8D;
                case "0\"5":
                    return 0x40;
                case "1/2000":
                    return 0x90;
                case "0\"4":
                    return 0x43;
                case "1/2500":
                    return 0x93;
                case "0\"3":
                    return 0x44;
                case "1/3000":
                    return 0x94;
                case "0\"3 (1/3)":
                    return 0x45;
                case "1/3200":
                    return 0x95;
                case "1/4":
                    return 0x48;
                case "1/4000":
                    return 0x98;
                case "1/5":
                    return 0x4B;
                case "1/5000":
                    return 0x9B;
                case "1/6":
                    return 0x4C;
                case "1/6000":
                    return 0x9C;
                case "1/6 (1/3)":
                    return 0x4D;
                case "1/6400":
                    return 0x9D;
                case "1/8":
                    return 0x50;
                case "1/8000":
                    return 0xA0;
                case "1/10 (1/3)":
                    return 0x53;
                case "1/10":
                    return 0x54;
                case "1/13":
                    return 0x55;
                case "1/15":
                    return 0x58;
                case "1/20 (1/3)":
                    return 0x5B;
                case "1/20":
                    return 0x5C;

                case "N/A":
                default:
                    return 0xffffffff;
            }
        }

        public static uint Drive(string v)
        {
            switch(v)
            {
                case "1 shot":
                    return 0x00000000;
                case "++shooting":
                    return 0x00000001;
                case "video":
                    return 0x00000002;
                case "High-Speed ++shots":
                    return 0x00000004;
                case "Low-Speed ++shots":
                    return 0x00000005;
                case "silent 1 shot":
                    return 0x00000006;
                case "10(s)timer & ++shots":
                    return 0x00000007;
                case "10(s)timer":
                    return 0x00000010;
                case "2(s)timer":
                    return 0x00000011;
                case "N/A":
                default:
                    return 0xffffffff;
            }
        }

        public static uint Metering(string v)
        {
            switch (v)
            { 
                case "Spot Metering" :
                    return 1;
                case "Evaluative Metering":
                    return 3;
                case "Partial Metering":
                    return 4;
                case "Center-Wighted averaging metering":
                    return 5;
                case "N/A":
                default:
                    return 0xFFFFFFFF;
            }
        }

        public static uint AFMode(string v)
        {
            switch (v)
            {
                case "1 shot AF":
                    return 0;
                case "All servo AF":
                    return 1;
                case "AI Focus AF":
                    return 2;
                case "Manual Focus":
                    return 3;
                case "N/A":
                default:
                    return 0xffffffff;
            }
        }

        public static uint ExpoComp(string v)
        {
            switch (v)
            {
                case "+3":
                    return 0x18;
                case "+2 2/3":
                    return 0x15;
                case "+2 1/2":
                    return 0x14;
                case "+2 1/3":
                    return 0x13;
                case "+2":
                    return 0x10;
                case "+1 2/3":
                    return 0x0D;
                case "+1 1/2":
                    return 0x0C;
                case "+1 1/3":
                    return 0x0B;
                case "+1":
                    return 0x08;
                case "+2/3":
                    return 0x05;
                case "+1/2":
                    return 0x04;
                case "+1/3":
                    return 0x03;
                case "0":
                    return 0x00;
                case "-1/3":
                    return 0xFD;
                case "-1/2":
                    return 0xFC;
                case "-2/3":
                    return 0xFB;
                case "-1":
                    return 0xF8;
                case "-1 1/3":
                    return 0xF5;
                case "-1 1/2":
                    return 0xF4;
                case "-1 2/3":
                    return 0xF3;
                case "-2":
                    return 0xF0;
                case "-2 1/3":
                    return 0xED;
                case "-2 1/2":
                    return 0xEC;
                case "-2 2/3":
                    return 0xEB;
                case "-3":
                    return 0xE8;
                case "N/A":
                default:
                    return 0xffffffff;
            }
        }

        //public static uint PicStyle(string v)
        //{
        //    switch (v)
        //    {
        //        case "Standard":
        //            return 0x0081;
        //        case "Portrait":
        //            return 0x0082;
        //        case "Landscape":
        //            return 0x0083;
        //        case "Neutral":
        //            return 0x0084;
        //        case "Faithful":
        //            return 0x0085;
        //        case "Monochrome":
        //            return 0x0086;
        //        case "Auto":
        //            return 0x0087;
        //        case "Computer Sett1":
        //            return 0x0041;
        //        case "Computer Sett2":
        //            return 0x0042;
        //        case "Computer Sett3":
        //            return 0x0043;
        //        case "N/A":
        //        default:
        //            return 0xFFFF;
        //    }
        //}

        public static uint FillterEff(string v)
        {
            switch (v)
            {
                case "None":
                    return 0;
                case "Yellow":
                    return 1;
                case "Orange":
                    return 2;
                case "Red":
                    return 3;
                case "Green":
                    return 4;
                case "Unknown":
                default:
                    return 0xFFFFFFFF;
            }
        }

        public static uint MonoTone(string v)
        {
            switch (v)
            {
                case "None":
                    return 0;
                case "Sepia":
                    return 1;
                case "Blue":
                    return 2;
                case "Violet":
                    return 3;
                case "Green":
                    return 4;
                case "Unknown":
                default:
                    return 0xFFFFFFFF;
            }
        }
    }
    
    public class CameraFileEntry
    {
        public string Name { get; private set; }
        public bool IsFolder { get; private set; }
        public Bitmap Thumbnail { get; private set; }
        public CameraFileEntry[] Entries { get; private set; }

        public CameraFileEntry(string Name, bool IsFolder)
        {
            this.Name = Name;
            this.IsFolder = IsFolder;
        }

        public void AddSubEntries(CameraFileEntry[] Entries)
        {
            this.Entries = Entries;
        }

        public void AddThumb(Bitmap Thumbnail)
        {
            this.Thumbnail = Thumbnail;
        }
    }
}
