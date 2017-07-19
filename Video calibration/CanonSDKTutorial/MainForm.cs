using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using EDSDKLib;
using MathWorks.MATLAB.NET.Arrays;
using MathWorks.MATLAB.NET;
using MathWorks.MATLAB.NET.Utility;
using System.Diagnostics;
using HDRMaker;
using VideoProjectorProject;
using MathWorks.MATLAB.NET.Arrays;
using MathWorks.MATLAB.NET.Utility;
using AForge;
using AForge.Controls;
using AForge.Vision;
using AForge.DebuggerVisualizers;
using AForge.Imaging;
using AForge.Imaging.Formats;
using AForge.MachineLearning;
using AForge.Video;
using AForge.Video.FFMPEG;
using AForge.Video.DirectShow;
using AForge.Video.VFW;
using AForge.Video.Ximea;
using AForge.Math;
using System.Text;
using System.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AForge;
using System.Drawing;
using AForge.Imaging.Filters;
using AForge.Math.Geometry;
using CSML;


namespace CanonSDK
{
    public partial class formMain : Form
    {
        
        SDKHandler CameraHandler;
        List<int> AvList;
        List<int> TvList;
        List<int> ISOList;
        List<int> DriveList;
        List<int> AFModeList;
        List<int> MeteringList;
        List<int> ExpoCompList;
        List<int> ImgQualityList;
        List<int> WBList;
        List<int> PictureStyleList;
        List<int> FilterEffectList;
        List<int> ToningEffectList;
        List<Camera> CamList;

        //// temporary variabels
        //double FirstDig;
        //double SecDig;
        //double FinalDig;
        //int num = 0;
        //// matlab variabels
        //string PicPath;
        //int numExpo;
        //int NumSamples;
        //List<double> ExpoList;

        int LiveViewWidth = 1024;
        int LiveViewHeight = 680;
        EDSDK.EdsPictureStyleDesc[] userDefine;

        int delayAmount = 3;
        int leftPrOutNum = 3;
        int rightPrOutNum = 2;
        int xRes = 1050;
        int yRes = 1400;

        public formMain()
        {
            //ExpoList = new List<double>();
            userDefine = new EDSDK.EdsPictureStyleDesc[3];
            userDefine[0] = new EDSDK.EdsPictureStyleDesc();
            userDefine[1] = new EDSDK.EdsPictureStyleDesc();
            userDefine[2] = new EDSDK.EdsPictureStyleDesc();
            userDefine[0].colorTone = userDefine[1].colorTone = userDefine[2].colorTone = 0;
            userDefine[0].contrast = userDefine[1].contrast = userDefine[2].contrast = 0;
            userDefine[0].saturation = userDefine[1].saturation = userDefine[2].saturation = 0;
            userDefine[0].sharpness = userDefine[1].sharpness = userDefine[2].sharpness = 0;
            InitializeComponent();
            CameraHandler = new SDKHandler();
            CameraHandler.CameraAdded += new SDKHandler.CameraAddedHandler(SDK_CameraAdded);
            CameraHandler.FrameRateUpdated += new SDKHandler.FloatUpdate(SDK_FrameRateUpdated);
            CameraHandler.LiveViewUpdated += new SDKHandler.ImageUpdate(SDK_LiveViewUpdated);
            CameraHandler.ProgressChanged += new SDKHandler.ProgressHandler(SDK_ProgressChanged);
            CameraHandler.CameraHasShutdown += CameraHandler_CameraHasShutdown;
            //textBoxSavePath.Text = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Photo");
            textBoxSavePath.Text = @"D:\testTaha";
            RefreshCamera();            
        }

        private void SDK_ProgressChanged(int Progress)
        {
        }

        private void SDK_LiveViewUpdated(System.Drawing.Image img)
        {
            LiveViewWidth = img.Width;
            LiveViewHeight = img.Height;
            if (CameraHandler.IsLiveViewOn) LiveViewPicBox.Image = img;
            else LiveViewPicBox.Image = null;
        }

        private void SDK_FrameRateUpdated(float Value)
        {
            this.Invoke((MethodInvoker)delegate { FrameRateLabel.Text = "FPS: " + Value.ToString("F2"); });
        }

        private void SDK_CameraAdded()
        {
            RefreshCamera();
        }

        private void CameraHandler_CameraHasShutdown(object sender, EventArgs e)
        {
            CloseSession();
        }

        private void SessionButton_Click(object sender, EventArgs e)
        {            
            if (CameraHandler.CameraSessionOpen) CloseSession();
            else OpenSession();
        }

        private void RefreshButton_Click(object sender, EventArgs e)
        {
            RefreshCamera();
        }


        private void LiveViewButton_Click(object sender, EventArgs e)
        {
            if (!CameraHandler.IsEvfFilming)
            {
                if (!CameraHandler.IsLiveViewOn) { CameraHandler.StartLiveView(); LiveViewButton.Text = "Stop LiveView"; }
                else { CameraHandler.StopLiveView(); LiveViewButton.Text = "Start LiveView"; }
            }
        }

        private void RecordEfvButton_Click(object sender, EventArgs e)
        {
            if (!CameraHandler.IsEvfFilming && !CameraHandler.IsLiveViewOn)
            {
                SettingsGroupBox.Enabled = false; CameraHandler.StartEvfFilming();
                //RecordEvfButton.Text = "Stop Recording"; 
                //PictureStyleGroupBox.Enabled = false; 
            }
            else if (CameraHandler.IsEvfFilming)
            {
                SettingsGroupBox.Enabled = true; CameraHandler.StopEvfFilming();
                //RecordEvfButton.Text = "Start Recording"; 
                //PictureStyleGroupBox.Enabled = true;
            }
        }

        private void LiveViewPicBox_MouseDown(object sender, MouseEventArgs e)
        {
            CameraHandler.SetManualWBEvf((ushort)((e.X / 270d) * LiveViewWidth), (ushort)((e.Y / 180d) * LiveViewHeight));
        }

        private void FocusNear3Button_Click(object sender, EventArgs e)
        {
            CameraHandler.SetFocus(EDSDK.EvfDriveLens_Near3);
        }

        private void FocusNear2Button_Click(object sender, EventArgs e)
        {
            CameraHandler.SetFocus(EDSDK.EvfDriveLens_Near2);
        }

        private void FocusNear1Button_Click(object sender, EventArgs e)
        {
            CameraHandler.SetFocus(EDSDK.EvfDriveLens_Near1);
        }

        private void FocusFar1Button_Click(object sender, EventArgs e)
        {
            CameraHandler.SetFocus(EDSDK.EvfDriveLens_Far1);
        }

        private void FocusFar2Button_Click(object sender, EventArgs e)
        {
            CameraHandler.SetFocus(EDSDK.EvfDriveLens_Far2);
        }

        private void FocusFar3Button_Click(object sender, EventArgs e)
        {
            CameraHandler.SetFocus(EDSDK.EvfDriveLens_Far3);
        }

        //public void atimerdelay()
        //{
        //    //lblStat.Text = "In the timer";
        //    System.Timers.Timer timedelay;
        //    timedelay = new System.Timers.Timer(5000);
        //    timedelay.Enabled = true;
        //    timedelay.Start();
        //}
        private void MyDelay(int second)
        {
            DateTime dt1 = DateTime.Now;
            int diff = 0;

            while (diff < second)
            {

                DateTime dt2 = DateTime.Now;
                TimeSpan ts = dt2.Subtract(dt1);
                diff = (int)ts.TotalSeconds;
                Application.DoEvents();

            }
        }

        //private void RecordVideoButton_Click(object sender, EventArgs e)
        //{
        //    if (!CameraHandler.IsFilming)
        //    {
        //        if (radioButtonSaveToComputer.Checked || radioButtonSaveToBoth.Checked)
        //        {
        //            if (!Directory.Exists(textBoxSavePath.Text)) 
        //                Directory.CreateDirectory(textBoxSavePath.Text);
        //            CameraHandler.StartFilming(textBoxSavePath.Text);
        //        }
        //        else CameraHandler.StartFilming();
        //        //RecordVideoButton.Text = "Stop Video";
        //    }
        //    else
        //    {
        //        CameraHandler.StopFilming();
        //        //RecordVideoButton.Text = "Record Video";
        //    }
        //}

        private void BrowseButton_Click(object sender, EventArgs e)
        {
            if (Directory.Exists(textBoxSavePath.Text))
            {
                SaveFolderBrowser.SelectedPath = textBoxSavePath.Text;
                //PicPath=(string)SavePathTextBox.Text;
            }

            if (SaveFolderBrowser.ShowDialog() == DialogResult.OK)
            {
                textBoxSavePath.Text = SaveFolderBrowser.SelectedPath;
            }
            //PicPath = SavePathTextBox.Text;
        }

        private void AvCoBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            CameraHandler.SetSetting(EDSDK.PropID_Av, CameraValues.AV((string)AvCoBox.SelectedItem));
        }

        private void TvCoBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            CameraHandler.SetSetting(EDSDK.PropID_Tv, CameraValues.ShautterSpeed((string)comboBoxTV.SelectedItem));
            string temp1 = (string)comboBoxTV.SelectedItem;
            var temparray = temp1.Split('/' , '"');
            //if (temparray[0] == "Bulb")
            //{
            //    FinalDig = (int)BulbUpDo.Value;
            //}
            //else
            //{

                //FirstDig = Convert.ToDouble(temparray[0]);
            //}
            //if (temparray[1] == "0" || temparray[1] == "" /*|| temparray[1] == "Bulb"*/)
            //{
            //    FinalDig = FirstDig;
            //}
            //else
            //{
            //    SecDig = Convert.ToDouble(temparray[1]);
            //    FinalDig = FirstDig / SecDig;
            //}
            //ExpoList.Add(FinalDig); 
            // do something here
            //if ((string)TvCoBox.SelectedItem == "Bulb") BulbUpDo.Enabled = true;
            //else BulbUpDo.Enabled = false;
        }

        private void ISOCoBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            CameraHandler.SetSetting(EDSDK.PropID_ISOSpeed, CameraValues.ISO((string)ISOCoBox.SelectedItem));
        }

        private void WBCoBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            //switch (WBCoBox.SelectedIndex)
            //{
            //    case 0: CameraHandler.SetSetting(EDSDK.PropID_WhiteBalance, EDSDK.WhiteBalance_Auto); break;
            //    case 1: CameraHandler.SetSetting(EDSDK.PropID_WhiteBalance, EDSDK.WhiteBalance_Daylight); break;
            //    case 2: CameraHandler.SetSetting(EDSDK.PropID_WhiteBalance, EDSDK.WhiteBalance_Cloudy); break;
            //    case 3: CameraHandler.SetSetting(EDSDK.PropID_WhiteBalance, EDSDK.WhiteBalance_Tangsten); break;
            //    case 4: CameraHandler.SetSetting(EDSDK.PropID_WhiteBalance, EDSDK.WhiteBalance_Fluorescent); break;
            //    case 5: CameraHandler.SetSetting(EDSDK.PropID_WhiteBalance, EDSDK.WhiteBalance_Strobe); break;
            //    case 6: CameraHandler.SetSetting(EDSDK.PropID_WhiteBalance, EDSDK.WhiteBalance_WhitePaper); break;
            //    case 7: CameraHandler.SetSetting(EDSDK.PropID_WhiteBalance, EDSDK.WhiteBalance_Shade); break;
            //    case 8: CameraHandler.SetSetting(EDSDK.PropID_WhiteBalance, EDSDK.WhiteBalance_ColorTemp); break;
            //    case 9: CameraHandler.SetSetting(EDSDK.PropID_WhiteBalance, EDSDK.WhiteBalance_PCSet1); break;
            //    case 10: CameraHandler.SetSetting(EDSDK.PropID_WhiteBalance, EDSDK.WhiteBalance_PCSet2); break;
            //    case 11: CameraHandler.SetSetting(EDSDK.PropID_WhiteBalance, EDSDK.WhiteBalance_PCSet3); break;
            //}
            CameraHandler.SetSetting(EDSDK.PropID_WhiteBalance, CameraValues.WB((string)ComboBoxWB.SelectedItem));

            //if (ComboBoxWB.SelectedIndex == 7) WBUpDo.Enabled = true;
            //else WBUpDo.Enabled = false;
        }

        private void WBUpDo_ValueChanged(object sender, EventArgs e)
        {
            //CameraHandler.SetSetting(EDSDK.PropID_ColorTemperature, (uint)WBUpDo.Value);
        }

        //private void SaveToButton_CheckedChanged(object sender, EventArgs e)
        //{
        //    if (radioButtonSaveToCamera.Checked)
        //    {
        //        CameraHandler.SetSetting(EDSDK.PropID_SaveTo, (uint)EDSDK.EdsSaveTo.Camera);
        //        buttonBrowse.Enabled = false;
        //        textBoxSavePath.Enabled = false;
        //        /*********************************************/
        //        //EDSDK.EdsDirectoryItemInfo dirInfo;
        //        //IntPtr streamRef;
        //        ////get information about object
        //        //Error = EDSDK.EdsGetDirectoryItemInfo(ObjectPointer, out dirInfo);
        //        //string CurrentPhoto = Path.Combine(directory, dirInfo.szFileName);
        //        ////create filestream to data
        //        //Error = EDSDK.EdsCreateFileStream(CurrentPhoto, EDSDK.EdsFileCreateDisposition.CreateAlways, EDSDK.EdsAccess.ReadWrite, out streamRef);
        //        ////download file
        //        //DownloadData(ObjectPointer, streamRef);
        //        ////release stream
        //        //Error = EDSDK.EdsRelease(streamRef);
        //        //PicPath = Directory.CreateDirectory();
        //    }
        //    else if (radioButtonSaveToComputer.Checked)
        //    {
        //        CameraHandler.SetSetting(EDSDK.PropID_SaveTo, (uint)EDSDK.EdsSaveTo.Host);
        //        CameraHandler.SetCapacity();
        //        buttonBrowse.Enabled = true;
        //        textBoxSavePath.Enabled = true;
        //    }
        //    else if (radioButtonSaveToBoth.Checked)
        //    {
        //        CameraHandler.SetSetting(EDSDK.PropID_SaveTo, (uint)EDSDK.EdsSaveTo.Both);
        //        CameraHandler.SetCapacity();
        //        buttonBrowse.Enabled = true;
        //        textBoxSavePath.Enabled = true;
        //    }
        //}
       
        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            CameraHandler.Dispose();
        }
        
        private void CloseSession()
        {
            //if (CameraHandler.IsLiveViewOn)
            //{
            //    CameraHandler.StopLiveView();
            //    LiveViewButton.Text = "Start LiveView";
            //}
            CameraHandler.CloseSession();
            AvCoBox.Items.Clear();
            comboBoxTV.Items.Clear();
            ISOCoBox.Items.Clear();
            DriveCoBox.Items.Clear();
            MeteringCoBox.Items.Clear();
            AFModeCoBox.Items.Clear();
            //ExpoCompCoBox.Items.Clear();
            ImgQualityCoBox.Items.Clear();
            ComboBoxFilterEffect.Items.Clear();
            ComboBoxToningEffect.Items.Clear();
            numericUpDownContrast.Enabled = true;
            numericUpDownSharpness.Enabled = true;
            numericUpDownSaturation.Enabled = true;
            numericUpDownColorTone.Enabled = true;
            //WBUpDo.Enabled = false;
            ComboBoxWB.Enabled = true;
            SettingsGroupBox.Enabled = false;
            //PictureStyleGroupBox.Enabled = false;
            LiveViewGroupBox.Enabled = false;
            SessionButton.Text = "Open Session";
            SessionLabel.Text = "No open session";
            //HDRgroupBox.Enabled = false;
            //NumSamplesTxtBox.Text = null;
        }

        private void RefreshCamera()
        {
            CloseSession();
            CameraListBox.Items.Clear();
            CamList = CameraHandler.GetCameraList();
            foreach (Camera cam in CamList) CameraListBox.Items.Add(cam.Info.szDeviceDescription);
            if (CamList.Count > 0) CameraListBox.SelectedIndex = 0;
        }

        private void OpenSession()
        {
            if (CameraListBox.SelectedIndex >= 0)
            {
                CameraHandler.OpenSession(CamList[CameraListBox.SelectedIndex]);
                SessionButton.Text = "Close Session";
                string cameraname = CameraHandler.MainCamera.Info.szDeviceDescription;
                SessionLabel.Text = cameraname;
                if (CameraHandler.GetSetting(EDSDK.PropID_AEMode) != EDSDK.AEMode_Manual) MessageBox.Show("Camera is not in manual mode. Some features might not work!");
                // Get Camera Mode
                AvList = CameraHandler.GetSettingsList((uint)EDSDK.PropID_Av);
                TvList = CameraHandler.GetSettingsList((uint)EDSDK.PropID_Tv);
                ISOList = CameraHandler.GetSettingsList((uint)EDSDK.PropID_ISOSpeed);
                DriveList = CameraHandler.GetSettingsList((uint)EDSDK.PropID_DriveMode);
                AFModeList = CameraHandler.GetSettingsList((uint)EDSDK.PropID_AFMode);
                MeteringList = CameraHandler.GetSettingsList((uint)EDSDK.PropID_MeteringMode);
                ExpoCompList = CameraHandler.GetSettingsList((uint)EDSDK.PropID_ExposureCompensation);
                ImgQualityList = CameraHandler.GetSettingsList((uint)EDSDK.PropID_ImageQuality);
                WBList = CameraHandler.GetSettingsList((uint)EDSDK.PropID_WhiteBalance);
                PictureStyleList = CameraHandler.GetSettingsList((uint)EDSDK.PropID_PictureStyle);

                //FlashExposureCompensationList = CameraHandler.GetSettingsList((uint)EDSDK.PropID_FlashCompensation);

                FilterEffectList = new List<int>();
                ToningEffectList = new List<int>();
                for (int i = 0; i < 5; i++) FilterEffectList.Add(i);
                for (int i = 0; i < 5; i++) ToningEffectList.Add(i);
                
                // complete the combo-box list
                foreach (int Av in AvList) AvCoBox.Items.Add(CameraValues.AV((uint)Av));
                foreach (int Tv in TvList) comboBoxTV.Items.Add(CameraValues.TV((uint)Tv));
                foreach (int ISO in ISOList) ISOCoBox.Items.Add(CameraValues.ISO((uint)ISO));
                foreach (int Drive in DriveList) DriveCoBox.Items.Add(CameraValues.Drive((uint)Drive));
                foreach (int AFMode in AFModeList) AFModeCoBox.Items.Add(CameraValues.AFMode((uint)AFMode));
                foreach (int Metering in MeteringList) MeteringCoBox.Items.Add(CameraValues.Metering((uint)Metering));
                //foreach (int ExpoComp in ExpoCompList) ExpoCompCoBox.Items.Add(CameraValues.ExpoComp((uint)ExpoComp));
                foreach (int QUALITY in ImgQualityList) ImgQualityCoBox.Items.Add(CameraValues.QUALITY((uint)QUALITY));
                foreach (int wb in WBList) ComboBoxWB.Items.Add(CameraValues.WB((uint)wb));
                foreach (int pictureStyle in PictureStyleList) ComboBoxPictureStyle.Items.Add(CameraValues.PICTURESTYLE((uint)pictureStyle));
                foreach (int filterEff in FilterEffectList) ComboBoxFilterEffect.Items.Add(CameraValues.FILTER_EFFECT((uint)filterEff));
                foreach (int toningEff in ToningEffectList) ComboBoxToningEffect.Items.Add(CameraValues.TONING_EFFECT((uint)toningEff));

                // Set comboBox according to camera setting
                AvCoBox.SelectedIndex = AvCoBox.Items.IndexOf(CameraValues.AV((uint)CameraHandler.GetSetting((uint)EDSDK.PropID_Av)));
                comboBoxTV.SelectedIndex = comboBoxTV.Items.IndexOf(CameraValues.TV((uint)CameraHandler.GetSetting((uint)EDSDK.PropID_Tv)));
                ISOCoBox.SelectedIndex = ISOCoBox.Items.IndexOf(CameraValues.ISO((uint)CameraHandler.GetSetting((uint)EDSDK.PropID_ISOSpeed)));
                DriveCoBox.SelectedIndex = DriveCoBox.Items.IndexOf(CameraValues.Drive((uint)CameraHandler.GetSetting((uint)EDSDK.PropID_DriveMode)));
                AFModeCoBox.SelectedIndex = AFModeCoBox.Items.IndexOf(CameraValues.AFMode((uint)CameraHandler.GetSetting((uint)EDSDK.PropID_AFMode)));
                MeteringCoBox.SelectedIndex = MeteringCoBox.Items.IndexOf(CameraValues.Metering((uint)CameraHandler.GetSetting((uint)EDSDK.PropID_MeteringMode)));
                //ExpoCompCoBox.SelectedIndex = ExpoCompCoBox.Items.IndexOf(CameraValues.ExpoComp((uint)CameraHandler.GetSetting((uint)EDSDK.PropID_ExposureCompensation)));
                ImgQualityCoBox.SelectedIndex = ImgQualityCoBox.Items.IndexOf(CameraValues.QUALITY((uint)CameraHandler.GetSetting((uint)EDSDK.PropID_ImageQuality)));
                ComboBoxWB.SelectedIndex = ComboBoxWB.Items.IndexOf(CameraValues.WB((uint)CameraHandler.GetSetting((uint)EDSDK.PropID_WhiteBalance)));
                ComboBoxPictureStyle.SelectedIndex = ComboBoxPictureStyle.Items.IndexOf(CameraValues.PICTURESTYLE((uint)CameraHandler.GetSetting((uint)EDSDK.PropID_PictureStyle)));

                //EDSDK.EdsPictureStyleDesc pictureStyleValues = new EDSDK.EdsPictureStyleDesc();
                //uint pictureStyleCurrent = CameraHandler.GetPictureStyle(out pictureStyleValues);
                //ShowPictureStyleSettingInGroupBox(pictureStyleCurrent, pictureStyleValues);

                //ComboBoxPictureStyle.SelectedIndex = ComboBoxPictureStyle.Items.IndexOf(CameraValues.PICTURESTYLE(pictureStyleCurrent));

                //SetPictureStyleGroupBox(pictureStyleCurrent, pictureStyleValues);

                //WBCoBox.Enabled = true;

                //if (ComboBoxWB.SelectedIndex == 7) WBUpDo.Enabled = true;
                //else WBUpDo.Enabled = false;

                //if (cameraname != "Canon EOS 1100D" && cameraname != "Canon EOS REBEL T3" && cameraname != "Canon EOS Kiss X50")
                //{
                //    int wbidx = (int)CameraHandler.GetSetting((uint)EDSDK.PropID_WhiteBalance);
                //    WBCoBox.SelectedIndex = (wbidx > 8) ? wbidx - 1 : wbidx;
                //    //WBUpDo.Value = CameraHandler.GetSetting((uint)EDSDK.PropID_ColorTemperature); not support
                    
                //}
                //else
                //{
                //    WBUpDo.Enabled = true;
                //    WBCoBox.Enabled = true;
                //}

                SettingsGroupBox.Enabled = true;
                //PictureStyleGroupBox.Enabled = true;
                LiveViewGroupBox.Enabled = true;
                //buttonBrowse.Enabled = false;
                //textBoxSavePath.Enabled = false;
                //HDRgroupBox.Enabled = true;
                //PicStyleGroupBox.Enabled = true;
                CameraHandler.SetSetting(EDSDK.PropID_SaveTo, (uint)EDSDK.EdsSaveTo.Host);
                CameraHandler.SetCapacity();
                //buttonBrowse.Enabled = true;
                //textBoxSavePath.Enabled = true;
            }
        }

        private void ShowPictureStyleSettingInGroupBox(uint pictureStyle, EDSDK.EdsPictureStyleDesc pictureStyleValues)
        {
            switch (pictureStyle)
            {
                case EDSDK.PictureStyle_Faithful:
                case EDSDK.PictureStyle_Landscape:
                case EDSDK.PictureStyle_Neutral:
                case EDSDK.PictureStyle_Portrait:
                case EDSDK.PictureStyle_Standard:
                    numericUpDownSharpness.Enabled = numericUpDownSaturation.Enabled = numericUpDownContrast.Enabled = numericUpDownColorTone.Enabled = false;
                    ComboBoxFilterEffect.SelectedIndex = ComboBoxToningEffect.SelectedIndex = -1;
                    ComboBoxFilterEffect.Enabled = ComboBoxToningEffect.Enabled = false;
                    numericUpDownSharpness.Value = pictureStyleValues.sharpness;
                    numericUpDownSaturation.Value = pictureStyleValues.saturation;
                    numericUpDownContrast.Value = pictureStyleValues.contrast;
                    numericUpDownColorTone.Value = pictureStyleValues.colorTone;
                    break;
                case EDSDK.PictureStyle_User1:
                case EDSDK.PictureStyle_User2:
                case EDSDK.PictureStyle_User3:
                    numericUpDownSharpness.Enabled = numericUpDownSaturation.Enabled = numericUpDownContrast.Enabled = numericUpDownColorTone.Enabled = true;
                    ComboBoxFilterEffect.SelectedIndex = ComboBoxToningEffect.SelectedIndex = -1;
                    ComboBoxFilterEffect.Enabled = ComboBoxToningEffect.Enabled = false;
                    numericUpDownSharpness.Value = pictureStyleValues.sharpness;
                    numericUpDownSaturation.Value = pictureStyleValues.saturation;
                    numericUpDownContrast.Value = pictureStyleValues.contrast;
                    numericUpDownColorTone.Value = pictureStyleValues.colorTone;
                    break;
                case (uint)EDSDK.PictureStyle_Monochrome:
                    numericUpDownSharpness.Enabled = numericUpDownContrast.Enabled = false;
                    numericUpDownSaturation.Enabled = numericUpDownColorTone.Enabled = false;
                    ComboBoxFilterEffect.Enabled = ComboBoxToningEffect.Enabled = true;
                    numericUpDownSharpness.Value = pictureStyleValues.sharpness;
                    numericUpDownContrast.Value = pictureStyleValues.contrast;
                    ComboBoxFilterEffect.SelectedIndex = ComboBoxFilterEffect.Items.IndexOf(CameraValues.FILTER_EFFECT(pictureStyleValues.filterEffect));
                    ComboBoxToningEffect.SelectedIndex = ComboBoxToningEffect.Items.IndexOf(CameraValues.TONING_EFFECT(pictureStyleValues.toningEffect));
                    break;
            }
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
        }

        //private void BulbUpDo_ValueChanged(object sender, EventArgs e)
        //{
        //}

        private void SettingsGroupBox_Enter(object sender, EventArgs e)
        {

        }

        private void MeteringCoBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            CameraHandler.SetSetting(EDSDK.PropID_MeteringMode, CameraValues.Metering((string)MeteringCoBox.SelectedItem));
        }

        private void ExpoCompCoBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            //CameraHandler.SetSetting(EDSDK.PropID_ExposureCompensation, CameraValues.ExpoComp((string)ExpoCompCoBox.SelectedItem));
        }

        private void AFModeCoBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            CameraHandler.SetSetting(EDSDK.PropID_AFMode, CameraValues.AFMode((string)AFModeCoBox.SelectedItem));
        }

        private void DriveCoBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            CameraHandler.SetSetting(EDSDK.PropID_DriveMode, CameraValues.Drive((string)DriveCoBox.SelectedItem));
        }

        private void PicStyleCoBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            uint selectedPictureStyle = CameraValues.PICTURESTYLE((string)ComboBoxPictureStyle.SelectedItem);
            EDSDK.EdsPictureStyleDesc pictureStyleValues = GetDefaultSetting(selectedPictureStyle);
            SetPictureStyle(selectedPictureStyle, pictureStyleValues);
        }

        void SetPictureStyle(uint selectedPictureStyle, EDSDK.EdsPictureStyleDesc pictureStyleValues)
        {
            switch (selectedPictureStyle)
            {
                case EDSDK.PictureStyle_User1:
                    CameraHandler.SetUserPictureStyle(1, pictureStyleValues);
                    break;
                case EDSDK.PictureStyle_User2:
                    CameraHandler.SetUserPictureStyle(2, pictureStyleValues);
                    break;
                case EDSDK.PictureStyle_User3:
                    CameraHandler.SetUserPictureStyle(3, pictureStyleValues);
                    break;
                case EDSDK.PictureStyle_Faithful:
                case EDSDK.PictureStyle_Landscape:
                case EDSDK.PictureStyle_Monochrome:
                case EDSDK.PictureStyle_Neutral:
                case EDSDK.PictureStyle_Portrait:
                case EDSDK.PictureStyle_Standard:
                    CameraHandler.SetPictureStyle(selectedPictureStyle, pictureStyleValues);
                    break;
            }
            ShowPictureStyleSettingInGroupBox(selectedPictureStyle, pictureStyleValues);
        }

        private EDSDK.EdsPictureStyleDesc GetDefaultSetting(uint selectedPictureStyle)
        {
            EDSDK.EdsPictureStyleDesc values = new EDSDK.EdsPictureStyleDesc();
            switch (selectedPictureStyle)
            {
                case (uint)EDSDK.PictureStyle_Faithful:
                    values.sharpness = 3;
                    values.colorTone = 0;
                    values.contrast = 0;
                    values.saturation = 0;
                    break;
                case (uint)EDSDK.PictureStyle_Landscape:
                    values.sharpness = 4;
                    values.colorTone = 0;
                    values.contrast = 0;
                    values.saturation = 0;
                    break;
                case (uint)EDSDK.PictureStyle_Monochrome:
                    values.sharpness = 3;
                    values.colorTone = 0;
                    values.contrast = 0;
                    values.saturation = 0;
                    values.filterEffect = 0;
                    values.toningEffect = 0;
                    break;
                case (uint)EDSDK.PictureStyle_Neutral:
                    values.sharpness = 0;
                    values.colorTone = 0;
                    values.contrast = 0;
                    values.saturation = 0;
                    break;
                case (uint)EDSDK.PictureStyle_Portrait:
                    values.sharpness = 2;
                    values.colorTone = 0;
                    values.contrast = 0;
                    values.saturation = 0;
                    break;
                case (uint)EDSDK.PictureStyle_Standard:
                    values.sharpness = 3;
                    values.colorTone = 0;
                    values.contrast = 0;
                    values.saturation = 0;
                    break;
                case (uint)EDSDK.PictureStyle_User1:
                    values = userDefine[0];
                    break;
                case (uint)EDSDK.PictureStyle_User2:
                    values = userDefine[1];
                    break;
                case (uint)EDSDK.PictureStyle_User3:
                    values = userDefine[2];
                    break;
            }
            return values;
        }
        
        private void ContrastUpDown_ValueChanged(object sender, EventArgs e)
        {
            NumericUpDownValueChangedOperation();
        }

        private void ImgQualityCoBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            CameraHandler.SetSetting(EDSDK.PropID_ImageQuality, CameraValues.QUALITY((string)ImgQualityCoBox.SelectedItem));
        }

        private void SharpnessUpDown_ValueChanged(object sender, EventArgs e)
        {
            NumericUpDownValueChangedOperation();
        }

        private void SaturationUpDown_ValueChanged(object sender, EventArgs e)
        {
            NumericUpDownValueChangedOperation();
        }

        private void ColorToneUpDown_ValueChanged(object sender, EventArgs e)
        {
            NumericUpDownValueChangedOperation();
        }

        private void NumericUpDownValueChangedOperation()
        {
            switch (CameraValues.PICTURESTYLE((string)ComboBoxPictureStyle.SelectedItem))
            {
                case EDSDK.PictureStyle_User1:
                    userDefine[0].colorTone = (int)numericUpDownColorTone.Value;
                    userDefine[0].contrast = (int)numericUpDownContrast.Value;
                    userDefine[0].saturation = (int)numericUpDownSaturation.Value;
                    userDefine[0].sharpness = (uint)numericUpDownSharpness.Value;
                    SetPictureStyle(EDSDK.PictureStyle_User1, userDefine[0]);
                    break;
                case EDSDK.PictureStyle_User2:
                    userDefine[1].colorTone = (int)numericUpDownColorTone.Value;
                    userDefine[1].contrast = (int)numericUpDownContrast.Value;
                    userDefine[1].saturation = (int)numericUpDownSaturation.Value;
                    userDefine[1].sharpness = (uint)numericUpDownSharpness.Value;
                    SetPictureStyle(EDSDK.PictureStyle_User2, userDefine[1]);
                    break;
                case EDSDK.PictureStyle_User3:
                    userDefine[2].colorTone = (int)numericUpDownColorTone.Value;
                    userDefine[2].contrast = (int)numericUpDownContrast.Value;
                    userDefine[2].saturation = (int)numericUpDownSaturation.Value;
                    userDefine[2].sharpness = (uint)numericUpDownSharpness.Value;
                    SetPictureStyle(EDSDK.PictureStyle_User3, userDefine[2]);
                    break;
            }
        }

        private void FilterEffCoBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (CameraValues.PICTURESTYLE((string)ComboBoxPictureStyle.SelectedItem)==EDSDK.PictureStyle_Monochrome)
            {
                EDSDK.EdsPictureStyleDesc pictureStyleValues = new EDSDK.EdsPictureStyleDesc();
                pictureStyleValues.sharpness = (uint)numericUpDownSharpness.Value;
                pictureStyleValues.contrast = (int)numericUpDownContrast.Value;
                pictureStyleValues.filterEffect = CameraValues.FILTER_EFFECT((string)ComboBoxFilterEffect.SelectedItem);
                pictureStyleValues.toningEffect = CameraValues.TONING_EFFECT((string)ComboBoxToningEffect.SelectedItem);
                SetPictureStyle(EDSDK.PictureStyle_Monochrome, pictureStyleValues);   
            }
         }

        private void MonoToneCoBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (CameraValues.PICTURESTYLE((string)ComboBoxPictureStyle.SelectedItem) == EDSDK.PictureStyle_Monochrome)
            {
                EDSDK.EdsPictureStyleDesc pictureStyleValues = new EDSDK.EdsPictureStyleDesc();
                pictureStyleValues.sharpness = (uint)numericUpDownSharpness.Value;
                pictureStyleValues.contrast = (int)numericUpDownContrast.Value;
                pictureStyleValues.filterEffect = CameraValues.FILTER_EFFECT((string)ComboBoxFilterEffect.SelectedItem);
                pictureStyleValues.toningEffect = CameraValues.TONING_EFFECT((string)ComboBoxToningEffect.SelectedItem);
                SetPictureStyle(EDSDK.PictureStyle_Monochrome, pictureStyleValues);
            }
        }

        private void TakePhotoButton_Click(object sender, EventArgs e)
        {
            if (!Directory.Exists(textBoxSavePath.Text))
            {
                Directory.CreateDirectory(textBoxSavePath.Text);
            }

            CameraHandler.ImageSaveDirectory = textBoxSavePath.Text;

            // object from matlab class should be created
            MatlabFunction matlabFunction = new MatlabFunction();

            int[] a = new int[8] { 1, 0, 0, 1, 1, 0, 1, 0 };
            int[] b = new int[8] { 0, 1, 0, 1, 0, 1, 1, 0 };
            int[] c = new int[8] { 0, 0, 1, 0, 1, 1, 1, 0 };

            for (int i = 0; i < 8; i++)
            {
                matlabFunction.Project_Image_Test((MWNumericArray)a[i],
                            (MWNumericArray)b[i], (MWNumericArray)c[i],(MWNumericArray)xRes,(MWNumericArray)xRes);
                MyDelay(delayAmount);
                CameraHandler.ImageName = "Im" + Convert.ToString(a[i]) + Convert.ToString(b[i]) +
                    Convert.ToString(c[i]);
                CameraHandler.TakePhoto();
                MyDelay(delayAmount);
                matlabFunction.closescreen1();
                matlabFunction.closescreen2();
            }

            while (!File.Exists(textBoxSavePath.Text + "\\newWhiteRightAfterWB.png")) ;

            matlabFunction.Project_Image((MWCharArray)"new red left after WB", (MWNumericArray)1024, (MWNumericArray)768,
                leftPrOutNum, (MWCharArray)"first projection", (MWCharArray)textBoxSavePath.Text);
            matlabFunction.Project_Image((MWCharArray)"new red right after WB", (MWNumericArray)1024, (MWNumericArray)768,
                rightPrOutNum, (MWCharArray)"second projection", (MWCharArray)textBoxSavePath.Text);
            MyDelay(delayAmount);
            CameraHandler.ImageName = "redAfterWB";
            CameraHandler.TakePhoto();
            MyDelay(delayAmount);
            matlabFunction.closescreen1();
            matlabFunction.closescreen2();

            matlabFunction.Project_Image((MWCharArray)"new green left after WB", (MWNumericArray)1024, (MWNumericArray)768,
                leftPrOutNum, (MWCharArray)"first projection", (MWCharArray)textBoxSavePath.Text);
            matlabFunction.Project_Image((MWCharArray)"new green right after WB", (MWNumericArray)1024, (MWNumericArray)768,
                rightPrOutNum, (MWCharArray)"second projection", (MWCharArray)textBoxSavePath.Text);
            MyDelay(delayAmount);
            CameraHandler.ImageName = "greenAfterWB";
            CameraHandler.TakePhoto();
            MyDelay(delayAmount);
            matlabFunction.closescreen1();
            matlabFunction.closescreen2();

            matlabFunction.Project_Image((MWCharArray)"new blue left after WB", (MWNumericArray)1024, (MWNumericArray)768,
                leftPrOutNum, (MWCharArray)"first projection", (MWCharArray)textBoxSavePath.Text);
            matlabFunction.Project_Image((MWCharArray)"new blue right after WB", (MWNumericArray)1024, (MWNumericArray)768,
                rightPrOutNum, (MWCharArray)"second projection", (MWCharArray)textBoxSavePath.Text);
            MyDelay(delayAmount);
            CameraHandler.ImageName = "blueAfterWB";
            CameraHandler.TakePhoto();
            MyDelay(delayAmount);
            matlabFunction.closescreen1();
            matlabFunction.closescreen2();

            matlabFunction.Project_Image((MWCharArray)"new white left after WB", (MWNumericArray)1024, (MWNumericArray)768,
                leftPrOutNum, (MWCharArray)"first projection", (MWCharArray)textBoxSavePath.Text);
            matlabFunction.Project_Image((MWCharArray)"new white right after WB", (MWNumericArray)1024, (MWNumericArray)768,
                rightPrOutNum, (MWCharArray)"second projection", (MWCharArray)textBoxSavePath.Text);
            MyDelay(delayAmount);
            CameraHandler.ImageName = "whiteAfterWB";
            CameraHandler.TakePhoto();
            MyDelay(delayAmount);
            matlabFunction.closescreen1();
            matlabFunction.closescreen2();
            while (!File.Exists(textBoxSavePath.Text + "\\newWhiteG.png")) ;

            //matlabFunction.Project_Image((MWCharArray)"new white GImg", (MWNumericArray)1024, (MWNumericArray)768,
            //        2, (MWCharArray)"first projection", (MWCharArray)textBoxSavePath.Text);
            //matlabFunction.Project_Image((MWCharArray)"white GImg", (MWNumericArray)1024, (MWNumericArray)768,
            //    3, (MWCharArray)"second projection", (MWCharArray)textBoxSavePath.Text);
            //MyDelay(delayAmount);
            //CameraHandler.ImageName = "whiteNewAfteWBLC";
            //CameraHandler.TakePhoto();
            //MyDelay(delayAmount);
            //matlabFunction.closescreen1();
            //matlabFunction.closescreen2();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (!Directory.Exists(textBoxSavePath.Text))
            {
                Directory.CreateDirectory(textBoxSavePath.Text);
            }

            CameraHandler.ImageSaveDirectory = textBoxSavePath.Text;

            // object from matlab class should be created
            MatlabFunction matlabFunction = new MatlabFunction();
            int[] a = new int[4] { 1, 0, 0, 1 };
            int[] b = new int[4] { 0, 1, 0, 1 };
            int[] c = new int[4] { 0, 0, 1, 1 };
            for (int i = 0; i < 4; i++)
            {
                matlabFunction.Project_Image_Test((MWNumericArray)a[i],
                            (MWNumericArray)b[i], (MWNumericArray)c[i]);
                MyDelay(delayAmount);
                CameraHandler.ImageName = "Im" + Convert.ToString(a[i]) + Convert.ToString(b[i]) +
                    Convert.ToString(c[i]);
                CameraHandler.TakePhoto();
                MyDelay(delayAmount);
                matlabFunction.closescreen1();
                matlabFunction.closescreen2();
            }

            while (!File.Exists(textBoxSavePath.Text + "\\newWhiteRightAfterLC.png")) ;

            matlabFunction.Project_Image((MWCharArray)"new red left after LC", (MWNumericArray)1024, (MWNumericArray)768,
                leftPrOutNum, (MWCharArray)"first projection", (MWCharArray)textBoxSavePath.Text);
            matlabFunction.Project_Image((MWCharArray)"new red right after LC", (MWNumericArray)1024, (MWNumericArray)768,
                rightPrOutNum, (MWCharArray)"second projection", (MWCharArray)textBoxSavePath.Text);
            MyDelay(delayAmount);
            CameraHandler.ImageName = "redAfterLC";
            CameraHandler.TakePhoto();
            MyDelay(delayAmount);
            matlabFunction.closescreen1();
            matlabFunction.closescreen2();

            matlabFunction.Project_Image((MWCharArray)"new green left after LC", (MWNumericArray)1024, (MWNumericArray)768,
                leftPrOutNum, (MWCharArray)"first projection", (MWCharArray)textBoxSavePath.Text);
            matlabFunction.Project_Image((MWCharArray)"new green right after LC", (MWNumericArray)1024, (MWNumericArray)768,
                rightPrOutNum, (MWCharArray)"second projection", (MWCharArray)textBoxSavePath.Text);
            MyDelay(delayAmount);
            CameraHandler.ImageName = "greenAfterLC";
            CameraHandler.TakePhoto();
            MyDelay(delayAmount);
            matlabFunction.closescreen1();
            matlabFunction.closescreen2();

            matlabFunction.Project_Image((MWCharArray)"new blue left after LC", (MWNumericArray)1024, (MWNumericArray)768,
                leftPrOutNum, (MWCharArray)"first projection", (MWCharArray)textBoxSavePath.Text);
            matlabFunction.Project_Image((MWCharArray)"new blue right after LC", (MWNumericArray)1024, (MWNumericArray)768,
                rightPrOutNum, (MWCharArray)"second projection", (MWCharArray)textBoxSavePath.Text);
            MyDelay(delayAmount);
            CameraHandler.ImageName = "blueAfterLC";
            CameraHandler.TakePhoto();
            MyDelay(delayAmount);
            matlabFunction.closescreen1();
            matlabFunction.closescreen2();

            matlabFunction.Project_Image((MWCharArray)"new white left after LC", (MWNumericArray)1024, (MWNumericArray)768,
                leftPrOutNum, (MWCharArray)"first projection", (MWCharArray)textBoxSavePath.Text);
            matlabFunction.Project_Image((MWCharArray)"new white right after LC", (MWNumericArray)1024, (MWNumericArray)768,
                rightPrOutNum, (MWCharArray)"second projection", (MWCharArray)textBoxSavePath.Text);
            MyDelay(delayAmount);
            CameraHandler.ImageName = "whiteAfterLC";
            CameraHandler.TakePhoto();
            MyDelay(delayAmount);
            matlabFunction.closescreen1();
            matlabFunction.closescreen2();
        }

        private void formMain_Load(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (!Directory.Exists(textBoxSavePath.Text))
            {
                Directory.CreateDirectory(textBoxSavePath.Text);
            }

            CameraHandler.ImageSaveDirectory = textBoxSavePath.Text;

            // object from matlab class should be created
            MatlabFunction matlabFunction = new MatlabFunction();
            int[] a = new int[8] { 1, 0, 0, 1, 1, 0, 1, 0 };
            int[] b = new int[8] { 0, 1, 0, 1, 0, 1, 1, 0 };
            int[] c = new int[8] { 0, 0, 1, 0, 1, 1, 1, 0 };

            for (int i = 0; i < 8; i++)
            {
                matlabFunction.Project_Image_Test((MWNumericArray)a[i],
                            (MWNumericArray)b[i], (MWNumericArray)c[i], (MWNumericArray)xRes, (MWNumericArray)yRes,
                            (MWNumericArray)leftPrOutNum, (MWNumericArray)rightPrOutNum);
                MyDelay(delayAmount);
                CameraHandler.ImageName = "Im" + Convert.ToString(a[i]) + Convert.ToString(b[i]) +
                    Convert.ToString(c[i]);
                CameraHandler.TakePhoto();
                MyDelay(delayAmount);
                matlabFunction.closescreen1();
                matlabFunction.closescreen2();
            }

            while (!File.Exists(textBoxSavePath.Text + "\\newWhiteRightAfterWB.png")) ;

            matlabFunction.Project_Image((MWCharArray)"new red left after WB", (MWNumericArray)yRes, (MWNumericArray)xRes,
                leftPrOutNum, (MWCharArray)"first projection", (MWCharArray)textBoxSavePath.Text);
            matlabFunction.Project_Image((MWCharArray)"new red right after WB", (MWNumericArray)yRes, (MWNumericArray)xRes,
                rightPrOutNum, (MWCharArray)"second projection", (MWCharArray)textBoxSavePath.Text);
            MyDelay(delayAmount);
            CameraHandler.ImageName = "redAfterWB";
            CameraHandler.TakePhoto();
            MyDelay(delayAmount);
            matlabFunction.closescreen1();
            matlabFunction.closescreen2();

            matlabFunction.Project_Image((MWCharArray)"new green left after WB", (MWNumericArray)yRes, (MWNumericArray)xRes,
                leftPrOutNum, (MWCharArray)"first projection", (MWCharArray)textBoxSavePath.Text);
            matlabFunction.Project_Image((MWCharArray)"new green right after WB", (MWNumericArray)yRes, (MWNumericArray)xRes,
                rightPrOutNum, (MWCharArray)"second projection", (MWCharArray)textBoxSavePath.Text);
            MyDelay(delayAmount);
            CameraHandler.ImageName = "greenAfterWB";
            CameraHandler.TakePhoto();
            MyDelay(delayAmount);
            matlabFunction.closescreen1();
            matlabFunction.closescreen2();

            matlabFunction.Project_Image((MWCharArray)"new blue left after WB", (MWNumericArray)yRes, (MWNumericArray)xRes,
                leftPrOutNum, (MWCharArray)"first projection", (MWCharArray)textBoxSavePath.Text);
            matlabFunction.Project_Image((MWCharArray)"new blue right after WB", (MWNumericArray)yRes, (MWNumericArray)xRes,
                rightPrOutNum, (MWCharArray)"second projection", (MWCharArray)textBoxSavePath.Text);
            MyDelay(delayAmount);
            CameraHandler.ImageName = "blueAfterWB";
            CameraHandler.TakePhoto();
            MyDelay(delayAmount);
            matlabFunction.closescreen1();
            matlabFunction.closescreen2();

            matlabFunction.Project_Image((MWCharArray)"new white left after WB", (MWNumericArray)yRes, (MWNumericArray)xRes,
                leftPrOutNum, (MWCharArray)"first projection", (MWCharArray)textBoxSavePath.Text);
            matlabFunction.Project_Image((MWCharArray)"new white right after WB", (MWNumericArray)yRes, (MWNumericArray)xRes,
                rightPrOutNum, (MWCharArray)"second projection", (MWCharArray)textBoxSavePath.Text);
            MyDelay(delayAmount);
            CameraHandler.ImageName = "whiteAfterWB";
            CameraHandler.TakePhoto();
            MyDelay(delayAmount);
            matlabFunction.closescreen1();
            matlabFunction.closescreen2();

            //while (!File.Exists(textBoxSavePath.Text + "\\newWhiteRightAfterWBLC.png")) ;

            //matlabFunction.Project_Image((MWCharArray)"new red left after WB LC", (MWNumericArray)yRes, (MWNumericArray)xRes,
            //    leftPrOutNum, (MWCharArray)"first projection", (MWCharArray)textBoxSavePath.Text);
            //matlabFunction.Project_Image((MWCharArray)"new red right after WB LC", (MWNumericArray)yRes, (MWNumericArray)xRes,
            //    rightPrOutNum, (MWCharArray)"second projection", (MWCharArray)textBoxSavePath.Text);
            //MyDelay(delayAmount);
            //CameraHandler.ImageName = "redAfterWBLC";
            //CameraHandler.TakePhoto();
            //MyDelay(delayAmount);
            //matlabFunction.closescreen1();
            //matlabFunction.closescreen2();

            //matlabFunction.Project_Image((MWCharArray)"new green left after LC", (MWNumericArray)yRes, (MWNumericArray)xRes,
            //    leftPrOutNum, (MWCharArray)"first projection", (MWCharArray)textBoxSavePath.Text);
            //matlabFunction.Project_Image((MWCharArray)"new green right after LC", (MWNumericArray)yRes, (MWNumericArray)xRes,
            //    rightPrOutNum, (MWCharArray)"second projection", (MWCharArray)textBoxSavePath.Text);
            //MyDelay(delayAmount);
            //CameraHandler.ImageName = "greenAfterLC";
            //CameraHandler.TakePhoto();
            //MyDelay(delayAmount);
            //matlabFunction.closescreen1();
            //matlabFunction.closescreen2();

            //matlabFunction.Project_Image((MWCharArray)"new blue left after LC", (MWNumericArray)yRes, (MWNumericArray)xRes,
            //    leftPrOutNum, (MWCharArray)"first projection", (MWCharArray)textBoxSavePath.Text);
            //matlabFunction.Project_Image((MWCharArray)"new blue right after LC", (MWNumericArray)yRes, (MWNumericArray)xRes,
            //    rightPrOutNum, (MWCharArray)"second projection", (MWCharArray)textBoxSavePath.Text);
            //MyDelay(delayAmount);
            //CameraHandler.ImageName = "blueAfterLC";
            //CameraHandler.TakePhoto();
            //MyDelay(delayAmount);
            //matlabFunction.closescreen1();
            //matlabFunction.closescreen2();

            //matlabFunction.Project_Image((MWCharArray)"new white left after WB LC", (MWNumericArray)yRes, (MWNumericArray)xRes,
            //    leftPrOutNum, (MWCharArray)"first projection", (MWCharArray)textBoxSavePath.Text);
            //matlabFunction.Project_Image((MWCharArray)"new white right after WB LC", (MWNumericArray)yRes, (MWNumericArray)xRes,
            //    rightPrOutNum, (MWCharArray)"second projection", (MWCharArray)textBoxSavePath.Text);
            //MyDelay(delayAmount);
            //CameraHandler.ImageName = "whiteAfterWBLC";
            //CameraHandler.TakePhoto();
            //MyDelay(delayAmount);
            //matlabFunction.closescreen1();
            //matlabFunction.closescreen2();

            while (!File.Exists(textBoxSavePath.Text + "\\newWhiteRightAfterWBLC.png")) ;
            MyDelay(delayAmount);

            matlabFunction.Project_Image((MWCharArray)"new white left after WB LC", (MWNumericArray)yRes, (MWNumericArray)xRes,
                leftPrOutNum, (MWCharArray)"first projection", (MWCharArray)textBoxSavePath.Text);
            matlabFunction.Project_Image((MWCharArray)"new white right after WB LC", (MWNumericArray)yRes, (MWNumericArray)xRes,
                rightPrOutNum, (MWCharArray)"second projection", (MWCharArray)textBoxSavePath.Text);
            MyDelay(delayAmount);
            CameraHandler.ImageName = "whiteAfterWBLC";
            CameraHandler.TakePhoto();
            MyDelay(delayAmount);
            matlabFunction.closescreen1();
            matlabFunction.closescreen2();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            int delayTime = 3;
            if (!Directory.Exists(textBoxSavePath.Text))
            {
                Directory.CreateDirectory(textBoxSavePath.Text);
            }

            CameraHandler.ImageSaveDirectory = textBoxSavePath.Text;

            // object from matlab class should be created
            MatlabFunction matlabFunction = new MatlabFunction();

            int[] a = new int[4] { 1, 0, 0, 1 };
            int[] b = new int[4] { 0, 1, 0, 1 };
            int[] c = new int[4] { 0, 0, 1, 1 };
            for (int i = 0; i < 4; i++)
            {
                matlabFunction.Project_Image_Test((MWNumericArray)a[i],
                            (MWNumericArray)b[i], (MWNumericArray)c[i]);
                MyDelay(delayTime);
                CameraHandler.ImageName = "Im" + Convert.ToString(a[i]) + Convert.ToString(b[i]) +
                    Convert.ToString(c[i]);
                CameraHandler.TakePhoto();
                MyDelay(delayTime);
                matlabFunction.closescreen1();
                matlabFunction.closescreen2();

            }

            while (!File.Exists(textBoxSavePath.Text + "\\gotoc.jpg")) ;

            MyDelay(5);
            //textBox1.Text = "amad";
        }

        private void button3_Click_1(object sender, EventArgs e)
        {
            MatlabFunction matlabFunction = new MatlabFunction();

            matlabFunction.closescreen1();
            matlabFunction.closescreen2();

            matlabFunction.Project_Image((MWCharArray)"red", (MWNumericArray)1024, (MWNumericArray)768,
                leftPrOutNum, (MWCharArray)"first projection", (MWCharArray)textBoxSavePath.Text);
            matlabFunction.Project_Image((MWCharArray)"red", (MWNumericArray)1024, (MWNumericArray)768,
                rightPrOutNum, (MWCharArray)"second projection", (MWCharArray)textBoxSavePath.Text);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            MatlabFunction matlabFunction = new MatlabFunction();

            matlabFunction.closescreen1();
            matlabFunction.closescreen2();

            matlabFunction.Project_Image((MWCharArray)"green", (MWNumericArray)1024, (MWNumericArray)768,
                leftPrOutNum, (MWCharArray)"first projection", (MWCharArray)textBoxSavePath.Text);
            matlabFunction.Project_Image((MWCharArray)"green", (MWNumericArray)1024, (MWNumericArray)768,
                rightPrOutNum, (MWCharArray)"second projection", (MWCharArray)textBoxSavePath.Text);
        }

        private void button9_Click(object sender, EventArgs e)
        {
            MatlabFunction matlabFunction = new MatlabFunction();
            matlabFunction.closescreen1();
            matlabFunction.closescreen2();

            matlabFunction.Project_Image((MWCharArray)"new red left after WB", (MWNumericArray)1024, (MWNumericArray)768,
                leftPrOutNum, (MWCharArray)"first projection", (MWCharArray)textBoxSavePath.Text);
            matlabFunction.Project_Image((MWCharArray)"new red right after WB", (MWNumericArray)1024, (MWNumericArray)768,
                rightPrOutNum, (MWCharArray)"second projection", (MWCharArray)textBoxSavePath.Text);
        }

        private void button8_Click(object sender, EventArgs e)
        {
            MatlabFunction matlabFunction = new MatlabFunction();
            
            matlabFunction.closescreen1();
            matlabFunction.closescreen2();

            matlabFunction.Project_Image((MWCharArray)"new red left after LC", (MWNumericArray)1024, (MWNumericArray)768,
                leftPrOutNum, (MWCharArray)"first projection", (MWCharArray)textBoxSavePath.Text);
            matlabFunction.Project_Image((MWCharArray)"new red right after LC", (MWNumericArray)1024, (MWNumericArray)768,
                rightPrOutNum, (MWCharArray)"second projection", (MWCharArray)textBoxSavePath.Text);
        }

        private void button7_Click(object sender, EventArgs e)
        {
            MatlabFunction matlabFunction = new MatlabFunction();
            
            matlabFunction.closescreen1();
            matlabFunction.closescreen2();

            matlabFunction.Project_Image((MWCharArray)"new red left after WB LC", (MWNumericArray)1024, (MWNumericArray)768,
                leftPrOutNum, (MWCharArray)"first projection", (MWCharArray)textBoxSavePath.Text);
            matlabFunction.Project_Image((MWCharArray)"new red right after WB LC", (MWNumericArray)1024, (MWNumericArray)768,
                rightPrOutNum, (MWCharArray)"second projection", (MWCharArray)textBoxSavePath.Text);
        }

        private void button10_Click(object sender, EventArgs e)
        {
            
        }

        private void button11_Click(object sender, EventArgs e)
        {
            MatlabFunction matlabFunction = new MatlabFunction();

            matlabFunction.closescreen1();
            matlabFunction.closescreen2();

            matlabFunction.Project_Image((MWCharArray)"blue", (MWNumericArray)1024, (MWNumericArray)768,
                leftPrOutNum, (MWCharArray)"first projection", (MWCharArray)textBoxSavePath.Text);
            matlabFunction.Project_Image((MWCharArray)"blue", (MWNumericArray)1024, (MWNumericArray)768,
                rightPrOutNum, (MWCharArray)"second projection", (MWCharArray)textBoxSavePath.Text);
        }

        private void button12_Click(object sender, EventArgs e)
        {
            MatlabFunction matlabFunction = new MatlabFunction();

            matlabFunction.closescreen1();
            matlabFunction.closescreen2();

            matlabFunction.Project_Image((MWCharArray)"white", (MWNumericArray)1024, (MWNumericArray)768,
                leftPrOutNum, (MWCharArray)"first projection", (MWCharArray)textBoxSavePath.Text);
            matlabFunction.Project_Image((MWCharArray)"white", (MWNumericArray)1024, (MWNumericArray)768,
                rightPrOutNum, (MWCharArray)"second projection", (MWCharArray)textBoxSavePath.Text);
        }

        private void button5_Click(object sender, EventArgs e)
        {

        }

        private void button13_Click(object sender, EventArgs e)
        {
            if (!Directory.Exists(textBoxSavePath.Text))
            {
                Directory.CreateDirectory(textBoxSavePath.Text);
            }

            CameraHandler.ImageSaveDirectory = textBoxSavePath.Text;

            // object from matlab class should be created
            MatlabFunction matlabFunction = new MatlabFunction();
            matlabFunction.Project_Image_Test((MWNumericArray)1,
                        (MWNumericArray)1, (MWNumericArray)1, (MWNumericArray)xRes,
                    (MWNumericArray)yRes, (MWNumericArray)leftPrOutNum, (MWNumericArray)rightPrOutNum);
            MyDelay(delayAmount);
            CameraHandler.ImageName = "Im111";
            CameraHandler.TakePhoto();
            MyDelay(delayAmount);
            matlabFunction.closescreen1();
            matlabFunction.closescreen2();
            //MatlabFunction matlabFunction = new MatlabFunction();
            MWNumericArray maxVal = (MWNumericArray)matlabFunction.saturate_test_function(textBoxSavePath.Text);

            if ((uint)maxVal < 255)
            {
                MessageBox.Show("No Saturation and maximum valus is " + maxVal.ToString(),
                    "Saturation Test With White Image", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("Saturated pixel found by value " + maxVal.ToString(),
                    "Saturation Test With White Image", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button5_Click_1(object sender, EventArgs e)
        {
            MatlabFunction matlabFunction = new MatlabFunction();
            matlabFunction.closescreen1();
            matlabFunction.closescreen2();

            matlabFunction.Project_Image((MWCharArray)"new green left after WB", (MWNumericArray)1024, (MWNumericArray)768,
                leftPrOutNum, (MWCharArray)"first projection", (MWCharArray)textBoxSavePath.Text);
            matlabFunction.Project_Image((MWCharArray)"new green right after WB", (MWNumericArray)1024, (MWNumericArray)768,
                rightPrOutNum, (MWCharArray)"second projection", (MWCharArray)textBoxSavePath.Text);
        }

        private void button10_Click_1(object sender, EventArgs e)
        {
            MatlabFunction matlabFunction = new MatlabFunction();
            matlabFunction.closescreen1();
            matlabFunction.closescreen2();

            matlabFunction.Project_Image((MWCharArray)"new blue left after WB", (MWNumericArray)1024, (MWNumericArray)768,
                leftPrOutNum, (MWCharArray)"first projection", (MWCharArray)textBoxSavePath.Text);
            matlabFunction.Project_Image((MWCharArray)"new blue right after WB", (MWNumericArray)1024, (MWNumericArray)768,
                rightPrOutNum, (MWCharArray)"second projection", (MWCharArray)textBoxSavePath.Text);
        }

        private void button14_Click(object sender, EventArgs e)
        {
            MatlabFunction matlabFunction = new MatlabFunction();
            matlabFunction.closescreen1();
            matlabFunction.closescreen2();

            matlabFunction.Project_Image((MWCharArray)"new white left after WB", (MWNumericArray)1024, (MWNumericArray)768,
                leftPrOutNum, (MWCharArray)"first projection", (MWCharArray)textBoxSavePath.Text);
            matlabFunction.Project_Image((MWCharArray)"new white right after WB", (MWNumericArray)1024, (MWNumericArray)768,
                rightPrOutNum, (MWCharArray)"second projection", (MWCharArray)textBoxSavePath.Text);
        }

        private void button17_Click(object sender, EventArgs e)
        {
            MatlabFunction matlabFunction = new MatlabFunction();

            matlabFunction.closescreen1();
            matlabFunction.closescreen2();

            matlabFunction.Project_Image((MWCharArray)"new green left after LC", (MWNumericArray)1024, (MWNumericArray)768,
                leftPrOutNum, (MWCharArray)"first projection", (MWCharArray)textBoxSavePath.Text);
            matlabFunction.Project_Image((MWCharArray)"new green right after LC", (MWNumericArray)1024, (MWNumericArray)768,
                rightPrOutNum, (MWCharArray)"second projection", (MWCharArray)textBoxSavePath.Text);
        }

        private void button16_Click(object sender, EventArgs e)
        {
            MatlabFunction matlabFunction = new MatlabFunction();

            matlabFunction.closescreen1();
            matlabFunction.closescreen2();

            matlabFunction.Project_Image((MWCharArray)"new blue left after LC", (MWNumericArray)1024, (MWNumericArray)768,
                leftPrOutNum, (MWCharArray)"first projection", (MWCharArray)textBoxSavePath.Text);
            matlabFunction.Project_Image((MWCharArray)"new blue right after LC", (MWNumericArray)1024, (MWNumericArray)768,
                rightPrOutNum, (MWCharArray)"second projection", (MWCharArray)textBoxSavePath.Text);
        }

        private void button15_Click(object sender, EventArgs e)
        {
            MatlabFunction matlabFunction = new MatlabFunction();

            matlabFunction.closescreen1();
            matlabFunction.closescreen2();

            matlabFunction.Project_Image((MWCharArray)"new white left after LC", (MWNumericArray)1024, (MWNumericArray)768,
                leftPrOutNum, (MWCharArray)"first projection", (MWCharArray)textBoxSavePath.Text);
            matlabFunction.Project_Image((MWCharArray)"new white right after LC", (MWNumericArray)1024, (MWNumericArray)768,
                rightPrOutNum, (MWCharArray)"second projection", (MWCharArray)textBoxSavePath.Text);
        }

        private void button20_Click(object sender, EventArgs e)
        {
            MatlabFunction matlabFunction = new MatlabFunction();

            matlabFunction.closescreen1();
            matlabFunction.closescreen2();

            matlabFunction.Project_Image((MWCharArray)"new green left after WB LC", (MWNumericArray)1024, (MWNumericArray)768,
                leftPrOutNum, (MWCharArray)"first projection", (MWCharArray)textBoxSavePath.Text);
            matlabFunction.Project_Image((MWCharArray)"new green right after WB LC", (MWNumericArray)1024, (MWNumericArray)768,
                rightPrOutNum, (MWCharArray)"second projection", (MWCharArray)textBoxSavePath.Text);
        }

        private void button19_Click(object sender, EventArgs e)
        {
            MatlabFunction matlabFunction = new MatlabFunction();

            matlabFunction.closescreen1();
            matlabFunction.closescreen2();

            matlabFunction.Project_Image((MWCharArray)"new blue left after WB LC", (MWNumericArray)1024, (MWNumericArray)768,
                leftPrOutNum, (MWCharArray)"first projection", (MWCharArray)textBoxSavePath.Text);
            matlabFunction.Project_Image((MWCharArray)"new blue right after WB LC", (MWNumericArray)1024, (MWNumericArray)768,
                rightPrOutNum, (MWCharArray)"second projection", (MWCharArray)textBoxSavePath.Text);
        }

        private void button18_Click(object sender, EventArgs e)
        {
            MatlabFunction matlabFunction = new MatlabFunction();

            matlabFunction.closescreen1();
            matlabFunction.closescreen2();

            matlabFunction.Project_Image((MWCharArray)"new white left after WB LC", (MWNumericArray)1024, (MWNumericArray)768,
                leftPrOutNum, (MWCharArray)"first projection", (MWCharArray)textBoxSavePath.Text);
            matlabFunction.Project_Image((MWCharArray)"new white right after WB LC", (MWNumericArray)1024, (MWNumericArray)768,
                rightPrOutNum, (MWCharArray)"second projection", (MWCharArray)textBoxSavePath.Text);
        }

        private void button21_Click(object sender, EventArgs e)
        {
            string prj = "left";
            int precision = 9;

            if (!Directory.Exists(textBoxSavePath.Text))
            {
                Directory.CreateDirectory(textBoxSavePath.Text);
            }

            CameraHandler.ImageSaveDirectory = textBoxSavePath.Text;

            // object from matlab class should be created
            MatlabFunction matlabFunction = new MatlabFunction();
            for (int i = 1; i < precision + 1; i++)
            {
                string str = "horz_" + i.ToString();
                matlabFunction.project_pattern_img((MWCharArray)str, (MWCharArray)textBoxSavePath.Text);
                MyDelay(delayAmount);
                CameraHandler.ImageName = prj + "_" + str;
                CameraHandler.TakePhoto();
                MyDelay(delayAmount);
                matlabFunction.closescreen1();
                matlabFunction.closescreen2();
            }

            for (int i = 1; i < precision + 1; i++)
            {
                string str = "vert_" + i.ToString();
                matlabFunction.project_pattern_img((MWCharArray)str, (MWCharArray)textBoxSavePath.Text);
                MyDelay(delayAmount);
                CameraHandler.ImageName = prj + "_" + str;
                CameraHandler.TakePhoto();
                MyDelay(delayAmount);
                matlabFunction.closescreen1();
                matlabFunction.closescreen2();
            }

            matlabFunction.Project_Image_Test((MWNumericArray)0, (MWNumericArray)0, (MWNumericArray)0);
            MyDelay(delayAmount);
            CameraHandler.ImageName = prj + "_Black";
            CameraHandler.TakePhoto();
            MyDelay(delayAmount);
            matlabFunction.closescreen1();
            matlabFunction.closescreen2();

            matlabFunction.Project_Image_Test((MWNumericArray)1, (MWNumericArray)1, (MWNumericArray)1);
            MyDelay(delayAmount);
            CameraHandler.ImageName = prj + "_white";
            CameraHandler.TakePhoto();
            MyDelay(delayAmount);
            matlabFunction.closescreen1();
            matlabFunction.closescreen2();

            //matlabFunction.Project_Image_Test((MWNumericArray)0, (MWNumericArray)0, (MWNumericArray)0);
            //MyDelay(delayAmount);
            //CameraHandler.ImageName = "_Black";
            //CameraHandler.TakePhoto();
            //MyDelay(delayAmount);
            //matlabFunction.closescreen1();
            //matlabFunction.closescreen2();

            //matlabFunction.Project_Image_Test((MWNumericArray)1, (MWNumericArray)1, (MWNumericArray)1);
            //MyDelay(delayAmount);
            //CameraHandler.ImageName = "right_white";
            //CameraHandler.TakePhoto();
            //MyDelay(delayAmount);
            //matlabFunction.closescreen1();
            //matlabFunction.closescreen2();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            
        }

        private void button6_Click_1(object sender, EventArgs e)
        {
            MatlabFunction matlabFunction = new MatlabFunction();

            matlabFunction.closescreen1();
            matlabFunction.closescreen2();
        }

        private void button22_Click(object sender, EventArgs e)
        {
            if (!Directory.Exists(textBoxSavePath.Text))
            {
                Directory.CreateDirectory(textBoxSavePath.Text);
            }

            CameraHandler.ImageSaveDirectory = textBoxSavePath.Text;

            // object from matlab class should be created
            MatlabFunction matlabFunction = new MatlabFunction();
            int[] a = new int[4] { 1, 0, 0, 1 };
            int[] b = new int[4] { 0, 1, 0, 1 };
            int[] c = new int[4] { 0, 0, 1, 1 };
            for (int i = 0; i < 4; i++)
            {
                matlabFunction.Project_Image_Test((MWNumericArray)a[i],
                            (MWNumericArray)b[i], (MWNumericArray)c[i]);
                MyDelay(delayAmount);
                CameraHandler.ImageName = "Im" + Convert.ToString(a[i]) + Convert.ToString(b[i]) +
                    Convert.ToString(c[i]);
                CameraHandler.TakePhoto();
                MyDelay(delayAmount);
                matlabFunction.closescreen1();
                matlabFunction.closescreen2();
            }
        }

        //private void button23_Click(object sender, EventArgs e)
        //{
        //    MatlabFunction matlabFunction = new MatlabFunction();

        //    matlabFunction.closescreen1();
        //    matlabFunction.closescreen2();

        //    matlabFunction.Project_Image((MWCharArray)"new white left after WB new coff", (MWNumericArray)1024, (MWNumericArray)768,
        //        leftPrOutNum, (MWCharArray)"first projection", (MWCharArray)textBoxSavePath.Text,
        //        (MWNumericArray)Convert.ToDouble(textBoxLCoff.Text));
        //    matlabFunction.Project_Image((MWCharArray)"new white right after WB", (MWNumericArray)1024, (MWNumericArray)768,
        //        rightPrOutNum, (MWCharArray)"second projection", (MWCharArray)textBoxSavePath.Text);
        //}

        private void button24_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            if (!Directory.Exists(textBoxSavePath.Text))
            {
                Directory.CreateDirectory(textBoxSavePath.Text);
            }

            CameraHandler.ImageSaveDirectory = textBoxSavePath.Text;

            // object from matlab class should be created
            MatlabFunction matlabFunction = new MatlabFunction();

            matlabFunction.Project_Image_Test_saturation((MWNumericArray)xRes,
                    (MWNumericArray)yRes, (MWNumericArray)leftPrOutNum, (MWNumericArray)rightPrOutNum);
          //  MyDelay(delayAmount);
            CameraHandler.ImageName = "white";
            CameraHandler.TakePhoto();
            MyDelay(delayAmount);
            matlabFunction.closescreen1();
            matlabFunction.closescreen2();

            //matlabFunction.Project_Image_Test_saturation((MWNumericArray)xRes,
            //        (MWNumericArray)yRes, (MWNumericArray)rightPrOutNum, (MWNumericArray)leftPrOutNum);
            //MyDelay(delayAmount);
            //CameraHandler.ImageName = "whiteRight";
            //CameraHandler.TakePhoto();
            //MyDelay(delayAmount);
            //matlabFunction.closescreen1();
            //matlabFunction.closescreen2();

            MWNumericArray maxVal = (MWNumericArray)matlabFunction.saturate_test_function(textBoxSavePath.Text);

            if ((uint)maxVal < 255)
            {
                MessageBox.Show("No Saturation and maximum valus is " + maxVal.ToString(),
                    "Saturation Test With White Image", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("Saturated pixel found by value " + maxVal.ToString(),
                    "Saturation Test With White Image", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SettingsGroupBox_Enter_1(object sender, EventArgs e)
        {

        }

        private void button1_Click_2(object sender, EventArgs e)
        {
            MatlabFunction matlabFunction = new MatlabFunction();
            CameraHandler.ImageName = "white";
            CameraHandler.TakePhoto();
            MyDelay(delayAmount);
            matlabFunction.closescreen1();
            matlabFunction.closescreen2();
           // MessageBox.Show("1111111111111111 " ,
           //         "2222222222222222222", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void button2_Click_1(object sender, EventArgs e)
        {

            string addresspic = "F:\\testTaha7\\Tulips.jpg";
            // Bitmap picToDisplay = (Bitmap)Bitmap.FromFile(addresspic);
            int RVid = 768, CVid = 1024, leftOutNum = 9, rightOutNum = 10;
            int RCam = 3456; int CCam = 5184;
            double[, ,] img1Project1 = new double[RVid, CVid, 3];
            double[, ,] img1Project2 = new double[RVid, CVid, 3];

            //string fileName = "F:\\kasr khedmat\\Saturation Test\\overlapTest\\horz3_left.png";
            //System.Drawing.Bitmap imageCalibration = (Bitmap)Bitmap.FromFile(fileName);
            // create filter
            AForge.Imaging.Filters.Median filter = new AForge.Imaging.Filters.Median();
            // apply filter
            //System.Drawing.Bitmap newImage = filter.Apply(imageCalibration);
            //showImage(newImage);


            double[,] msk1Array = new double[RVid, CVid];
            double[,] msk2Array = new double[RVid, CVid];

            int[] other1 = new int[] { 2 };
            int[] other2 = new int[] { 1 };

            int videoProjectorID = 1;
            string subFolder = "9X10";
            string address = "E:\\new code\\result archive\\data 2\\" + videoProjectorID + " (4.5)\\" + subFolder;
            int[] otherVideoProjectorID = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11 };
            leftOutNum = 9; rightOutNum = 10;
            int countPatternId = 1;
            //takePhotoVideoProjector(videoProjectorID, videoProjectorID, otherVideoProjectorID, leftOutNum, rightOutNum, countPatternId);
            //for (int i = 1; i <= 11; i++)
            //{
            //    if (i == videoProjectorID) continue;
            //    countPatternId++;
            //    takePhotoVideoProjector(i, videoProjectorID, otherVideoProjectorID, leftOutNum, rightOutNum, countPatternId);

            //}
            //leftOutNum = 6; rightOutNum = 7;
            //takePhotoVideoProjector(videoProjectorID, otherVideoProjectorID, leftOutNum, rightOutNum, 2);
            //leftOutNum = 9; rightOutNum = 10;
            //takePhotoVideoProjector(videoProjectorID, otherVideoProjectorID, leftOutNum, rightOutNum, 3);


            // double[, ,] map1 = geocalibmp(videoProjectorID, RVid, CVid, leftOutNum, rightOutNum, address);
            double[, ,] map1 = new double[RVid, CVid, 2];
            ////     double[, ,] map2 = new double[RVid, CVid, 2];
            msk1Array = alphamask(videoProjectorID, subFolder, otherVideoProjectorID, RVid, CVid, leftOutNum, rightOutNum, address, map1);
            //dumpArray(msk1Array, "F:\\map1.csv");
            //double [,] map11 = new double [msk1Array.GetLength(0),msk1Array.GetLength(1)];
            //getArray(map11,"F:\\map1.csv");
            // Bitmap dis11 = new Bitmap(1400,1050);
            // Bitmap dis22 = new Bitmap(1400,1050);
            //bool check = true;
            //for (int i = 0; i < RVid; i++)
            //{
            //    for (int j = 0; j < CVid; j++)
            //    {
            //        int gray = (int)(msk1Array[i, j] * 255);
            //         int gray2 = (int)(map11[i, j] * 255);

            //        dis22.SetPixel(j,i, Color.FromArgb(gray2, gray2, gray2));
            //        dis11.SetPixel(j,i, Color.FromArgb(gray, gray, gray));
            //        if (map11[i, j] != msk1Array[i, j]) check = false;
            //    }
            //}
            //showImage(dis11);
            //showImage(dis22);
            //////////// double[, ,] map2 = geocalibmp(2, RVid, CVid, leftOutNum, rightOutNum, address);
            ////////////msk2Array = alphamask(2,other2, RVid, CVid, leftOutNum, rightOutNum, address, map2);
            ////////////// SplitImage(picToDisplay, RCam, CCam, img1Project1, img1Project2, RVid, CVid, map1, map2);
            ////////// // gammaCorrection(img1Project1, img1Project2, 1, RVid, CVid);
            ////////// Bitmap img1Project1Bitmap = (Bitmap)Bitmap.FromFile("F:\\testTaha7\\img1.jpg");
            ////////// Bitmap img2Project1Bitmap = (Bitmap)Bitmap.FromFile("F:\\testTaha7\\img2.jpg");
            ////////// GC.Collect();
            ////////// Bitmap img1 = new Bitmap(CVid, RVid);
            ////////// Bitmap img2 = new Bitmap(CVid, RVid);
            ////////// double[,] img1Array = new double[RVid, CVid];
            ////////// double[,] img2Array = new double[RVid, CVid];
            ////////// // whiteArray = changeValueArray(whiteArray, RVid, CVid, 111);
            ////////// gammaCorrection(msk1Array, msk2Array, 1, RVid, CVid);


            ////////// for (int i = 0; i < RVid; i++)
            ////////// {
            //////////     for (int j = 0; j < CVid; j++)
            //////////     {
            //////////         //   img1.SetPixel(i, j, Color.FromArgb((int)img1Project1[i, j, 0], (int)img1Project1[i, j, 0], (int)img1Project1[i, j, 0]));
            //////////         //   img2.SetPixel(i, j, Color.FromArgb((int)img1Project2[i, j, 0], (int)img1Project2[i, j, 0], (int)img1Project2[i, j, 0]));
            //////////        // img1Array[i, j] = ((img1Project1[i, j, 0] + img1Project1[i, j, 1] + img1Project1[i, j, 2]) / (3 * 255) * (double)(msk1Array[i, j]));
            //////////       //  img2Array[i, j] = ((img1Project2[i, j, 0] + img1Project2[i, j, 1] + img1Project2[i, j, 2]) / (3 * 255) * (double)(msk2Array[i, j]));
            //////////         img1Array[i, j] = ((double)(img1Project1Bitmap.GetPixel(j, i).R + img1Project1Bitmap.GetPixel(j, i).G + img1Project1Bitmap.GetPixel(j, i).B) / (3 * 255) * (double)(msk1Array[i, j]));
            //////////         img2Array[i, j] = ((double)(img2Project1Bitmap.GetPixel(j, i).R + img2Project1Bitmap.GetPixel(j, i).G + img2Project1Bitmap.GetPixel(j, i).B) / (3 * 255) * (double)(msk2Array[i, j]));
            //////////         img1.SetPixel(j, i, Color.FromArgb((int)(img1Array[i, j] * 255), (int)(img1Array[i, j] * 255), (int)(img1Array[i, j] * 255)));
            //////////         img2.SetPixel(j, i, Color.FromArgb((int)(img2Array[i, j] * 255), (int)(img2Array[i, j] * 255), (int)(img2Array[i, j] * 255)));
            //////////         //  img1Array[i, j] = msk1Array[i,j];
            //////////         //  img2Array[i, j] = msk2Array[i, j];
            //////////     }
            ////////// }

            ////////// //////////////MatlabFunction mfT = new MatlabFunction();
            ////////// //////////////mfT.fullscreen1(0, (MWNumericArray)img1Array, (MWArray)3);
            ////////// //////////////mfT.fullscreen2(0, (MWNumericArray)img2Array, (MWArray)2);

            ////////// ////////////////  gammaCorrection(img1Array, img1Array, 1, RVid, CVid);

            ////////// //////////////////                showImage(img1);
            ////////// //////////////////           showImage(img2);
            ////////// showImage(img1);
            ////////// showImage(img2);

            ////////////////  mfT.fullscreen1(0, (MWNumericArray)img1Array, (MWArray)3);
            ////////////////  mfT.fullscreen2(0, (MWNumericArray)img2Array, (MWArray)2);

            //////////////MyDelay(delayAmount);
            //////////////string addressTemp = address + "\\" + "T.jpg";
            //////////////CameraHandler.ImageSaveDirectory = "D:\\testTaha7";
            //////////////CameraHandler.ImageName = "T";
            //////////////CameraHandler.TakePhoto();
            //////////////MyDelay(delayAmount);

            //////////////mfT.closescreen1();
            //////////////mfT.closescreen2();
        }
        public void takePhotoVideoProjector(int id, int videoProjectId, int[] otherId, int leftNum, int rightNum, int runId)
        {
            string address = "F:\\calibration Pattern\\";
            string subFolder = "";
            if (leftNum == 3 && rightNum == 4) subFolder = "3X4";
            if (leftNum == 4 && rightNum == 5) subFolder = "4X5";
            if (leftNum == 5 && rightNum == 6) subFolder = "5X6";
            if (leftNum == 6 && rightNum == 7) subFolder = "6X7";
            if (leftNum == 7 && rightNum == 8) subFolder = "7X8";
            if (leftNum == 8 && rightNum == 9) subFolder = "8X9";
            if (leftNum == 9 && rightNum == 10) subFolder = "9X10";
            int Bits = (int)Math.Ceiling(Math.Log(leftNum * rightNum, 2));

            for (int i = 0; i < Bits + 1; i++)
            {
                string addressTemp1 = "";
                addressTemp1 = address + subFolder + "\\" + i + ".jpg";
                if (i == Bits) addressTemp1 = address + subFolder + "\\all.jpg";
                Bitmap pic = (Bitmap)Bitmap.FromFile(addressTemp1);
                string addressTemp2 = address + subFolder + "\\black.jpg";
                Bitmap picBlack = (Bitmap)Bitmap.FromFile(addressTemp2);
                string addressTemp3 = address + "shareFolder\\" + id + ".bmp";
                pic.Save(@addressTemp3, System.Drawing.Imaging.ImageFormat.Bmp);
                for (int j = 1; j <= 11; j++)
                {
                    addressTemp3 = address + "shareFolder\\" + j + ".bmp";
                    if (j != id) picBlack.Save(@addressTemp3, System.Drawing.Imaging.ImageFormat.Bmp);
                }
                //MyDelay(5);
                //CameraHandler.ImageName = "pattern" + i;
                string path = "F:\\test\\" + videoProjectId + "\\" + subFolder + "\\" + id + "\\" + subFolder + "\\";
                //CameraHandler.ImageSaveDirectory = path;
                System.IO.DirectoryInfo di = System.IO.Directory.CreateDirectory(@path);
                di.Delete();
                //CameraHandler.TakePhoto();
                //MyDelay(1);

            }
            ////////////////////////////////////////////// white and black ////////////////////////////////
            if (runId == 1)
            {
                for (int i = 0; i < otherId.Length + 1; i++)
                {
                    string addressTemp1 = "";
                    addressTemp1 = address + subFolder + "\\white.jpg";
                    Bitmap pic = (Bitmap)Bitmap.FromFile(addressTemp1);
                    string addressTemp2 = address + subFolder + "\\black.jpg";
                    Bitmap picBlack = (Bitmap)Bitmap.FromFile(addressTemp2);
                    int selectedSaveId;
                    if (i == 0) selectedSaveId = 0;
                    //  else if (i == 1) selectedSaveId = id;
                    else selectedSaveId = otherId[i - 1];
                    string addressTemp3 = address + "shareFolder\\" + selectedSaveId + ".bmp";
                    pic.Save(@addressTemp3, System.Drawing.Imaging.ImageFormat.Bmp);

                    for (int j = 1; j <= 11; j++)
                    {
                        addressTemp3 = address + "shareFolder\\" + j + ".bmp";
                        if (j != selectedSaveId) picBlack.Save(@addressTemp3, System.Drawing.Imaging.ImageFormat.Bmp);
                    }

                    //MyDelay(5);
                    //CameraHandler.ImageName = "neighbor" + selectedSaveId;
                    string path = "F:\\test\\" + videoProjectId + "\\" + subFolder + "\\" + id + "\\" + subFolder + "\\";
                    //CameraHandler.ImageSaveDirectory = path;
                    System.IO.DirectoryInfo di = System.IO.Directory.CreateDirectory(@path);
                    di.Delete();
                    //CameraHandler.TakePhoto();
                    //MyDelay(1);
                }
            }

        }
        public void gammaCorrection(double[,] img1, double[,] img2, int mode, int RVid, int CVid)
        {
            if (mode == 1) { }
            else if (mode == 2) { }
            for (int r = 0; r < RVid - 1; r++)
            {
                for (int c = 0; c < CVid - 1; c++)
                {

                    //  img1[r, c, 0] = Math.Pow(img1[r, c, 0], (1 / 2.2));
                    // img1[r, c, 1] = Math.Pow(img1[r, c, 1], (1 / 2.2));
                    img1[r, c] = Math.Pow(img1[r, c], (1 / 2.2));
                    //  img2[r, c, 0] = Math.Pow(img2[r, c, 0], (1 / 2.2));
                    //   img2[r, c, 1] = Math.Pow(img2[r, c, 1], (1 / 2.2));
                    img2[r, c] = Math.Pow(img2[r, c], (1 / 2.2));
                }
            }
        }
        public void SplitImage(Bitmap pic, int RCam, int CCam, double[, ,] img1, double[, ,] img2, int RVid, int CVid, double[, ,] map1, double[, ,] map2)
        {
            AForge.Imaging.Filters.ResizeBilinear resize = new AForge.Imaging.Filters.ResizeBilinear(CCam, RCam);
            Bitmap resizePic = resize.Apply(pic);
            //  string addresspic = "D:\\testTaha7\\resizeT.jpg";
            //  Bitmap resizePic = (Bitmap)Bitmap.FromFile(addresspic);
            for (int r = 0; r < RVid - 1; r++)
            {
                for (int c = 0; c < CVid - 1; c++)
                {
                    int rr = (int)map1[r, c, 0]; int cc = (int)map1[r, c, 1];
                    //img1[r, c, 0] = (double)resizePic.GetPixel(rr, cc).R;
                    //img1[r, c, 1] = (double)resizePic.GetPixel(rr, cc).G;
                    //img1[r, c, 2] = (double)resizePic.GetPixel(rr, cc).B;
                    //rr = (int)map2[r, c, 0]; cc = (int)map2[r, c, 1];
                    //img2[r, c, 0] = (double)resizePic.GetPixel(rr, cc).R;
                    //img2[r, c, 1] = (double)resizePic.GetPixel(rr, cc).G;
                    //img2[r, c, 2] = (double)resizePic.GetPixel(rr, cc).B;
                    //  if (cc >= RCam) cc = RCam - 1; if (rr >= CCam) rr = CCam - 1;
                    img1[r, c, 0] = (double)resizePic.GetPixel(cc, rr).R;
                    img1[r, c, 1] = (double)resizePic.GetPixel(cc, rr).G;
                    img1[r, c, 2] = (double)resizePic.GetPixel(cc, rr).B;

                    rr = (int)map2[r, c, 0]; cc = (int)map2[r, c, 1];
                    //     if (cc >= RCam) cc = RCam - 1; if (rr >= CCam) rr = CCam - 1;
                    img2[r, c, 0] = (double)resizePic.GetPixel(cc, rr).R;
                    img2[r, c, 1] = (double)resizePic.GetPixel(cc, rr).G;
                    img2[r, c, 2] = (double)resizePic.GetPixel(cc, rr).B;
                }
            }
        }
        public double[,] alphamask(int mode, string subFolder, int[] other, int RVid, int CVid, int leftOutNum, int rightOutNum, string address, double[, ,] map1)
        {
            double[,] msk1Array = new double[RVid, CVid];
            double[,] msk2Array = new double[RVid, CVid];
            GC.Collect();
            AForge.Imaging.Filters.SobelEdgeDetector sobelFilter = new AForge.Imaging.Filters.SobelEdgeDetector();
            AForge.Imaging.Filters.Opening open = new AForge.Imaging.Filters.Opening();
            AForge.Imaging.Filters.CannyEdgeDetector cannyFilter = new AForge.Imaging.Filters.CannyEdgeDetector();
            AForge.Imaging.Filters.OtsuThreshold otsuThreshold = new AForge.Imaging.Filters.OtsuThreshold();
            AForge.Imaging.Filters.Grayscale grayScale = new AForge.Imaging.Filters.Grayscale(0.2125, 0.7154, 0.0721);
            AForge.Imaging.HoughLineTransformation hl = new AForge.Imaging.HoughLineTransformation();
            ResizeBilinear filterResizeImage = new ResizeBilinear(5184 / 2, 3456 / 2);
            // apply filter
            address = "E:\\new code\\result archive\\data 2";
            string addressTemp1 = address + "\\" + mode + " (4.5)\\" + subFolder + "\\neighbor" + mode + ".jpg";
            Bitmap pic1 = (Bitmap)Bitmap.FromFile(addressTemp1);
            pic1 = filterResizeImage.Apply(pic1);
            int RCam = pic1.Width; int CCam = pic1.Height;
            Bitmap Msk1 = otsuThreshold.Apply(grayScale.Apply(pic1));
            // Bitmap Msk1 = (Bitmap)Bitmap.FromFile("D:\\testTaha7\\pic1.jpg");
            string addressTemp2;
            Bitmap pic2;
            Bitmap Msk2 = new Bitmap(pic1.Width, pic1.Height);
            IList<int> listOtherId = new List<int>();
            List<Bitmap> listOtherMsk = new List<Bitmap>();
            double[,] IntersectTotalArray = new double[pic1.Width, pic1.Height];
            for (int i = 0; i < other.Length; i++)
            {
                if (other[i] == mode) continue;
                addressTemp2 = address + "\\" + mode + " (4.5)\\" + subFolder + "\\neighbor" + other[i] + ".jpg";
                pic2 = (Bitmap)Bitmap.FromFile(addressTemp2);
                pic2 = filterResizeImage.Apply(pic2);
                Msk2 = otsuThreshold.Apply(grayScale.Apply(pic2));
                // showImage(Msk2);
                if (!haveOverlap(Msk1, Msk2, other[i])) continue;
                listOtherMsk.Add(Msk2);

                listOtherId.Add(other[i]);

                for (int m = 0; m < Msk1.Width; m++)
                {
                    for (int n = 0; n < Msk1.Height; n++)
                    {
                        if (Msk1.GetPixel(m, n).R > 50 && Msk2.GetPixel(m, n).R > 50)
                            IntersectTotalArray[m, n]++;
                    }
                }
            } // for in others


            //////////////////////////////////////// calc distance transform /////////////////////////
            double[,] md = new double[pic1.Width, pic1.Height];
            int width = pic1.Width; int height = pic1.Height;
            double[,] ms1Array = new double[pic1.Width, pic1.Height];
            double[,] ms2Array = new double[pic1.Width, pic1.Height];
            Bitmap dis1 = new Bitmap(pic1.Width, pic1.Height);
            Bitmap dis2 = new Bitmap(pic1.Width, pic1.Height);
            int maxLength = Math.Min(width, height);

            for (int k = 0; k < listOtherMsk.Count(); k++)
            {
                Msk2 = listOtherMsk[k];


                List<double[,]> msArrayOtherList = new List<double[,]>();
                for (int i = 0; i < pic1.Width; i++)
                {
                    for (int j = 0; j < pic1.Height; j++)
                    {
                        int R1 = Msk1.GetPixel(i, j).R;
                        int R2 = Msk2.GetPixel(i, j).R;
                        //    if (R1 > 0) pic1Array[i, j] = 1; else pic1Array[i, j] = 0;
                        //     if (R2 > 0) pic2Array[i, j] = 1; else pic2Array[i, j] = 0;

                        //  if (R1 > 0 && R2 > 0) pic12Array[i, j] = 1;
                        //        if (R1 > 0 || R2 > 0) TMskArray[i, j] = 1;
                        int msk1 = Msk1.GetPixel(i, j).R;
                        int msk2 = Msk2.GetPixel(i, j).R;
                        ///////////////////////////////// ms1 //////////////////////////////
                        if (msk1 > 50 && (R1 > 50 && R2 > 50)) { ms1Array[i, j] = 1; }
                        else if (msk1 == 50 && (R1 > 50 && R2 > 50)) { ms1Array[i, j] = 1; } //changed
                        else if (msk1 > 50 && !(R1 > 50 && R2 > 50))
                        {
                            ms1Array[i, j] = 0;

                        }
                        else if (msk1 == 0 && !(R1 > 0 && R2 > 0)) { ms1Array[i, j] = 1; }
                        ///////////////////////////////// ms2 ///////////////////////////////
                        if (msk2 > 0 && (R1 > 0 && R2 > 0)) { ms2Array[i, j] = 1; }
                        else if (msk2 == 0 && (R1 > 0 && R2 > 0)) { ms2Array[i, j] = 1; } //changed
                        else if (msk2 > 0 && !(R1 > 0 && R2 > 0))
                        {
                            ms2Array[i, j] = 0;

                        }
                        else if (msk2 == 0 && !(R1 > 0 && R2 > 0)) { ms2Array[i, j] = 1; }
                        // if (pic1Edge.GetPixel(i, j).R > 0) pic1EdgeArray[i, j] = 1; else pic1EdgeArray[i, j] = 0;
                        //  if (pic2Edge.GetPixel(i, j).R > 0) pic2EdgeArray[i, j] = 1; else pic2EdgeArray[i, j] = 0;
                    }
                }



                GC.Collect();

                for (int i = 0; i < width; i++)
                {
                    for (int j = 0; j < height; j++)
                    {
                        int top = 0; int left = 0;
                        if (ms1Array[i, j] == 0) continue;
                        if (i == 0 && j == 0) { ms1Array[i, j] = 1000000; continue; }
                        else if (j == 0) { top = 1000000; left = (int)ms1Array[i - 1, j]; }
                        else if (i == 0) { left = 1000000; top = (int)ms1Array[i, j - 1]; }
                        else
                        {
                            top = (int)ms1Array[i, j - 1]; left = (int)ms1Array[i - 1, j];
                        }
                        ms1Array[i, j] = Math.Min(top + 1, left + 1);
                    }
                }
                for (int i = width - 1; i > -1; i--)
                {
                    for (int j = height - 1; j > -1; j--)
                    {
                        int down = 0; int right = 0; int self = (int)ms1Array[i, j];
                        if (ms1Array[i, j] == 0) continue;
                        if (i == width - 1 && j == height - 1) { ms1Array[i, j] = self; continue; }
                        else if (j == height - 1) { down = 1000000; right = (int)ms1Array[i + 1, j]; }
                        else if (i == width - 1) { right = 1000000; down = (int)ms1Array[i, j + 1]; }
                        else
                        {
                            down = (int)ms1Array[i, j + 1]; right = (int)ms1Array[i + 1, j];
                        }
                        ms1Array[i, j] = Math.Min(self, Math.Min(right + 1, down + 1));
                    }
                }
                for (int i = 0; i < width; i++)
                {
                    for (int j = 0; j < height; j++)
                    {
                        int top = 0; int left = 0;
                        if (ms2Array[i, j] == 0) continue;
                        if (i == 0 && j == 0) { ms2Array[i, j] = 1000000; continue; }
                        else if (j == 0) { top = 1000000; left = (int)ms2Array[i - 1, j]; }
                        else if (i == 0) { left = 1000000; top = (int)ms2Array[i, j - 1]; }
                        else
                        {
                            top = (int)ms2Array[i, j - 1]; left = (int)ms2Array[i - 1, j];
                        }
                        ms2Array[i, j] = Math.Min(top + 1, left + 1);
                    }
                }
                for (int i = width - 1; i > -1; i--)
                {
                    for (int j = height - 1; j > -1; j--)
                    {
                        int down = 0; int right = 0; int self = (int)ms2Array[i, j];
                        if (ms2Array[i, j] == 0) continue;
                        if (i == width - 1 && j == height - 1) { ms2Array[i, j] = self; continue; }
                        else if (j == height - 1) { down = 1000000; right = (int)ms2Array[i + 1, j]; }
                        else if (i == width - 1) { right = 1000000; down = (int)ms2Array[i, j + 1]; }
                        else
                        {
                            down = (int)ms2Array[i, j + 1]; right = (int)ms2Array[i + 1, j];
                        }
                        ms2Array[i, j] = Math.Min(self, Math.Min(right + 1, down + 1));
                        dis1.SetPixel(i, j, Color.FromArgb((int)(ms2Array[i, j] / maxLength * 100), (int)(ms2Array[i, j] / maxLength * 100), (int)(ms2Array[i, j] / maxLength * 100)));
                        md[i, j] = md[i, j] + ms2Array[i, j];
                    }
                }

                GC.Collect();

                //   showImage(dis1);

            }// end of for list other

            // //                                  calc md exempt md1
            //  //////////////////////////////////////////////// md1 calculation after for ////////////////////////////////////////
            //for (int i = 0; i < pic1.Width; i++)
            //{
            //    for (int j = 0; j < pic1.Height; j++)
            //    {

            //        dis1.SetPixel(i, j, Color.FromArgb((int) md[i, j], (int)md[i, j], (int)md[i, j]));


            //    }
            //}
            //showImage(dis1);

            for (int i = 0; i < pic1.Width; i++)
            {
                for (int j = 0; j < pic1.Height; j++)
                {
                    int R1 = Msk1.GetPixel(i, j).R;
                    int R2 = (int)IntersectTotalArray[i, j]; // int R2 = Msk2.GetPixel(i, j).R;
                    //if (R1 > 50 && R2 > 0) { ms1Array[i, j] = 1; ms2Array[i, j] = 1; }
                    //else if (R1 > 50) { ms1Array[i, j] = 0; ms2Array[i, j] = 1; }
                    //else if (R2 > 0) { ms1Array[i, j] = 1; ms2Array[i, j] = 0; }
                    //else { ms1Array[i, j] = 1; ms2Array[i, j] = 1; }
                    if (R1 > 50) ms1Array[i, j] = 0; else ms1Array[i, j] = 1;
                    dis1.SetPixel(i, j, Color.FromArgb((int)(ms1Array[i, j] * 250), (int)(ms1Array[i, j] * 250), (int)(ms1Array[i, j] * 250)));
                }

            }
            showImage(dis1);


            GC.Collect();

            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    int top = 0; int left = 0;
                    if (ms1Array[i, j] == 0) continue;
                    if (i == 0 && j == 0) { ms1Array[i, j] = 1000000; continue; }
                    else if (j == 0) { top = 1000000; left = (int)ms1Array[i - 1, j]; }
                    else if (i == 0) { left = 1000000; top = (int)ms1Array[i, j - 1]; }
                    else
                    {
                        top = (int)ms1Array[i, j - 1]; left = (int)ms1Array[i - 1, j];
                    }
                    ms1Array[i, j] = Math.Min(top + 1, left + 1);
                }
            }
            for (int i = width - 1; i > -1; i--)
            {
                for (int j = height - 1; j > -1; j--)
                {
                    int down = 0; int right = 0; int self = (int)ms1Array[i, j];
                    if (ms1Array[i, j] == 0) continue;
                    if (i == width - 1 && j == height - 1) { ms1Array[i, j] = self; continue; }
                    else if (j == height - 1) { down = 1000000; right = (int)ms1Array[i + 1, j]; }
                    else if (i == width - 1) { right = 1000000; down = (int)ms1Array[i, j + 1]; }
                    else
                    {
                        down = (int)ms1Array[i, j + 1]; right = (int)ms1Array[i + 1, j];
                    }
                    ms1Array[i, j] = Math.Min(self, Math.Min(right + 1, down + 1));
                }
            }
           

            GC.Collect();
            ///////////////////////////////////////////////////////////////////////
            for (int i = 0; i < pic1.Width; i++)
            {
                for (int j = 0; j < pic1.Height; j++)
                {
                    dis1.SetPixel(i, j, Color.FromArgb((int)(ms1Array[i, j] / maxLength * 250), (int)(ms1Array[i, j] / maxLength * 250), (int)(ms1Array[i, j] / maxLength * 250)));

                }
            }
            showImage(dis1);


            ////////////////////////////////////// md = md1 +md; //////////////////////////////////////////////////

            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    md[i, j] = md[i, j] + ms1Array[i, j];
                    dis1.SetPixel(i, j, Color.FromArgb((int)(ms1Array[i, j] / maxLength * 255), (int)(ms1Array[i, j] / maxLength * 255), (int)(ms1Array[i, j] / maxLength * 255)));
                }
            }
            //  showImage(dis1);
            ////////////////////////////////////////////////////////////////////////////////////////
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    /////////////////////////////////////////////////////////////
                    int R1 = Msk1.GetPixel(i, j).R;
                    //  int R2 = Msk2.GetPixel(i, j).R;
                    // if (R1 == 0 || R2 == 0) { ms1Array[i, j] = 1; ms2Array[i, j] = 1; }
                    if (ms1Array[i, j] + md[i, j] == 0) { ms1Array[i, j] = 0; md[i, j] = 0; }
                    else
                    {
                        //    ms1Array[i, j] = ((double)1 - ((double)ms1Array[i, j] / ((double)ms1Array[i, j] + (double)ms2Array[i, j])));
                        //    ms2Array[i, j] = ((double)1 - ((double)ms2Array[i, j] / ((double)ms1Array[i, j] + (double)ms2Array[i, j])));

                        //    double dd2 = (1 - ((double)md[i, j] / ((double)ms1Array[i, j] + (double)md[i, j])));


                        double dd = (1 - ((double)ms1Array[i, j] / (double)md[i, j]));
                        //     if (R1 == 0) dd = 0;
                        //    if (R2 == 0) dd2 = 0;
                        //    if (R1 <200 ) dd = 1;
                        //    else if (IntersectTotalArray[i, j] <= 0) dd = 0;
                        //    if (R2 == 1 && (R1 != 0)) dd2 = 1;
                        //   ms1Array[i, j] = dd;
                        //  ms2Array[i, j] = dd2;
                        int gray = (int)(dd * 255);
                        // int gray2 = (int)(ms2Array[i, j] * 255);

                        //  dis2.SetPixel(i, j, Color.FromArgb(gray2, gray2, gray2));
                        dis1.SetPixel(i, j, Color.FromArgb(gray, gray, gray));
                    }


                }
            }
            showImage(dis1);
            //  showImage(dis2);

            ////////////////////////////////////// Extract Mask /////////////////////////////////////////////

            dis1 = new Bitmap(CVid, RVid);
            //  dis2 = new Bitmap(CVid, RVid);

            for (int r = 0; r < RVid; r++)
            {
                for (int c = 0; c < CVid; c++)
                {
                    double rr = map1[r, c, 1]; double cc = map1[r, c, 0];
                    if (rr >= width) rr = width - 1;
                    if (cc >= height) cc = height - 1;
                    msk1Array[r, c] = ms1Array[(int)rr, (int)cc]; //msk1(r,c,:)=m1(rr,cc,:);
                    msk1Array[r, c] = pic1.GetPixel((int)rr, (int)cc).R;
                    //   dis1.SetPixel(c, r, Color.FromArgb((int)(msk1Array[r, c] * 255), (int)(msk1Array[r, c] * 255), (int)(msk1Array[r, c] * 255)));
                    //   dis1.SetPixel(c, r, Color.FromArgb((int)(msk1Array[r, c]), (int)(msk1Array[r, c]), (int)(msk1Array[r, c])));
                }
            }
            // showImage(dis1);
            //  showImage(dis2);
            return msk1Array;

        }
        public bool haveOverlap(Bitmap img1, Bitmap img2, int id)
        {
            //  showImage(img1);
            //  showImage(img2);

            Bitmap overlapImage = orImage(img1, img2);
            AForge.Imaging.BlobCounter blobCounter = new AForge.Imaging.BlobCounter();
            //  showImage(overlapImage);
            blobCounter.ObjectsOrder = AForge.Imaging.ObjectsOrder.Area;
            blobCounter.ProcessImage(overlapImage);
            AForge.Imaging.Blob[] blobs = blobCounter.GetObjectsInformation();
            int overlapCount = countBlobBasedOnArea(blobs, 100);
            if (overlapCount > 0) return true;
            else return false;
        }
        public double calcDist(int i, int j, List<List<int>> li, int[,] array)
        {
            if (array[i, j] == 0) return 0;
            double minDist = int.MaxValue;
            for (int ii = 0; ii < li.Count; ii++)
            {
                double dis = Math.Sqrt(Math.Pow(i - li[ii][0], 2) + (Math.Pow(j - li[ii][1], 2)));
                if (dis < minDist) minDist = dis;
            }
            return minDist;
        }
        public double[, ,] geocalibmp(int mode, int RVid, int CVid, int leftOutNum, int rightOutNum, string address)
        {
            //  MatlabFunction matlabFunction = new MatlabFunction();
            GC.Collect();
            int Rs, Ls;
            if (mode == 1)
            {
                Rs = 1;
                Ls = 0;
            }
            else
            {
                Rs = 0;
                Ls = 1;
            }
            ResizeBilinear filterResizeImage = new ResizeBilinear(5184 / 2, 3456 / 2);
            int CntPntR = leftOutNum, CntPntC = rightOutNum, Rad = 10;
            int ClrBrdr = 40;
            int Offset = ClrBrdr + Rad;
            Bitmap whiteImage = new Bitmap(RVid, CVid);
            // changeColor(whiteImage, System.Drawing.Color.White);
            ////////////MatlabFunction mf = new MatlabFunction();
            ////////////MWArray a = 3;
            ////////////int[,] aaa = new int[RVid, CVid];
            ////////////double[,] whiteArray = new double[RVid, CVid];
            ////////////double[,] blackArray = new double[RVid, CVid];
            ////////////whiteArray = changeValueArray(whiteArray, RVid, CVid, 255);
            ////////////if (mode == 1)
            ////////////{
            ////////////    mf.fullscreen1(0, (MWNumericArray)whiteArray, (MWArray)3);
            ////////////    mf.fullscreen2(0, (MWNumericArray)blackArray, (MWArray)2);
            ////////////}
            ////////////else
            ////////////{
            ////////////    mf.fullscreen1(0, (MWNumericArray)whiteArray, (MWArray)2);
            ////////////    mf.fullscreen2(0, (MWNumericArray)blackArray, (MWArray)3);
            ////////////}
            ////////////MyDelay(delayAmount);
            string addressTemp = address + "\\" + mode + "white.jpg";
            //////////CameraHandler.ImageName = mode + "whitetow";
            //////////CameraHandler.ImageSaveDirectory = "D:\\testTaha7";
            //////////CameraHandler.TakePhoto();
            //////////MyDelay(delayAmount);
            //////////mf.closescreen1();
            //////////mf.closescreen2();
            string address2 = "F:\\testTaha7\\";
            Bitmap CapImage = (Bitmap)Bitmap.FromFile(address + "\\neighbor" + mode + ".jpg");
            CapImage = filterResizeImage.Apply(CapImage);

            int RCam = CapImage.Width; int CCam = CapImage.Height;
            AForge.Imaging.Filters.OtsuThreshold otsuThreshold = new AForge.Imaging.Filters.OtsuThreshold();
            AForge.Imaging.Filters.Grayscale grayScale = new AForge.Imaging.Filters.Grayscale(0.2125, 0.7154, 0.0721);
            // apply filter
            Bitmap CapImageGray = grayScale.Apply(CapImage);
            Bitmap Msk = otsuThreshold.Apply(CapImageGray);
            AForge.Imaging.Filters.ApplyMask maskFilter = new AForge.Imaging.Filters.ApplyMask(Msk);

            //showImage(CapImageGray);
            //showImage(CapImageBinary);
            int DelR = (int)Math.Round((double)(RVid - 2 * Offset) / (CntPntR - 1));
            int DelC = (int)Math.Round((double)(CVid - 2 * Offset) / (CntPntC - 1));
            int Stp = Math.Min(DelC, DelR);
            if (Rad > (Stp / 4)) Rad = Stp / 4;
            double[,] CPoints = new double[CntPntR * CntPntC, 2];
            //Bitmap img = new Bitmap(CVid, RVid);
            //changeColor(img, System.Drawing.Color.White);
            //double[,] imgArray = new double[RVid, CVid];
            //imgArray = changeValueArray(imgArray, RVid, CVid, 255);
            for (int r = 0; r < CntPntR; r++)
            {
                for (int c = 0; c < CntPntC; c++)
                {
                    int rP = Offset + r * DelR;
                    int cP = Offset + c * DelC;
                    CPoints[r * CntPntC + c, 0] = rP;
                    CPoints[r * CntPntC + c, 1] = cP;
                    for (int i = 0; i < Rad * 2; i++)
                    {
                        for (int j = 0; j < Rad * 2; j++)
                        {
                            //img.SetPixel(cP + j, rP + i, Color.Black);
                            //imgArray[rP + i, cP + j] = 0;
                        }
                    }
                    //img.SetPixel(rP, cP , Color.White);
                }
            }


            ////////////if (mode == 1)
            ////////////{
            ////////////    mf.fullscreen1(0, (MWNumericArray)imgArray, (MWArray)3);
            ////////////    mf.fullscreen2(0, (MWNumericArray)blackArray, (MWArray)2);
            ////////////}
            ////////////else
            ////////////{
            ////////////    mf.fullscreen1(0, (MWNumericArray)imgArray, (MWArray)2);
            ////////////    mf.fullscreen2(0, (MWNumericArray)blackArray, (MWArray)3);
            ////////////}

            ////////////MyDelay(delayAmount);
            ////////////CameraHandler.ImageName = mode + "Camtow";
            ////////////CameraHandler.ImageSaveDirectory = "D:\\testTaha7";
            ////////////CameraHandler.TakePhoto();
            ////////////MyDelay(delayAmount);
            ////////////mf.closescreen1();
            ////////////mf.closescreen2();
            Bitmap ImgCam = (Bitmap)Bitmap.FromFile(address + "\\all.jpg");
            ImgCam = filterResizeImage.Apply(ImgCam);
            // showImage(ImgCam);
            Bitmap ImgCamGray = grayScale.Apply(ImgCam);
            Bitmap ImgCamBinary = otsuThreshold.Apply(ImgCamGray);
            //ImgCamBinary = CreateNonIndexedImage((Bitmap)ImgCamBinary);
            ImgCam = complementImage(ImgCamBinary);
            // showImage(ImgCamBinary);
            //Bitmap ImgCam2=maskFilter.Apply(ImgCam);
            //Bitmap Lc = new Bitmap(RCam, CCam);
            //Bitmap Lr = new Bitmap(RCam, CCam);
            //changeColor(Lc, System.Drawing.Color.White);
            //changeColor(Lr, System.Drawing.Color.White);
            multipleWithMask(ImgCam, Msk);
            //  showImage(ImgCam);
            AForge.Imaging.BlobCounter blobCounter = new AForge.Imaging.BlobCounter();
            blobCounter.ObjectsOrder = AForge.Imaging.ObjectsOrder.Area;
            blobCounter.ProcessImage(ImgCam);
            AForge.Imaging.Blob[] blobs = blobCounter.GetObjectsInformation();
            int BPall = countBlobBasedOnArea(blobs, 150);
            //  AForge.Point Center = new AForge.Point();
            // List<double> COG = new List<double>();
            double[,] COG = new double[CntPntR * CntPntC, 3];
            for (int i = 0; i < BPall; i++)
            {
                //  if (blobs[i].Area>100){
                COG[i, 0] = blobs[i].CenterOfGravity.Y;
                COG[i, 1] = blobs[i].CenterOfGravity.X;
                COG[i, 2] = 0;
                // }
            }
            int Bits = (int)Math.Ceiling(Math.Log(CntPntR * CntPntC, 2));

            for (int b = Bits - 1; b >= 0; b--)
            {
                //int pointCount = 0;
                //Bitmap imgW = new Bitmap(CVid, RVid); // inverse of matlab format
                //changeColor(imgW, System.Drawing.Color.White);
                //double[,] imgwArray = new double[RVid, CVid];
                //imgwArray = changeValueArray(imgwArray, RVid, CVid, 255);

                //for (int r = 0; r <= CntPntR - 1; r++)
                //{
                //    for (int c = 0; c <= CntPntC - 1; c++)
                //    {
                //        int rP = Offset + r * DelR;
                //        int cP = Offset + c * DelC;
                //        byte temp1 = (byte)(r * CntPntC + c);
                //        byte temp2 = (byte)Math.Pow(2, b);
                //        long result = temp1 & temp2;
                //        if (result > 0)
                //        {
                //            pointCount = pointCount + 1;
                //            for (int i = 0; i < Rad * 2; i++)
                //            {
                //                for (int j = 0; j < Rad * 2; j++)
                //                {
                //                    imgW.SetPixel(cP + j, rP + i, Color.Black); // inverse of matlab format
                //                    imgwArray[rP + i, cP + j] = 0;
                //                }
                //            }
                //            //  showImage(imgW);
                //        }

                //    }
                //}
                ////////////if (mode == 1)
                ////////////{
                ////////////    mf.fullscreen1(0, (MWNumericArray)imgwArray, (MWArray)3);
                ////////////    mf.fullscreen2(0, (MWNumericArray)blackArray, (MWArray)2);
                ////////////}
                ////////////else
                ////////////{
                ////////////    mf.fullscreen1(0, (MWNumericArray)imgwArray, (MWArray)2);
                ////////////    mf.fullscreen2(0, (MWNumericArray)blackArray, (MWArray)3);
                ////////////}
                ////////////MyDelay(delayAmount);
                ////////////CameraHandler.ImageName = b + mode.ToString() + "two";
                ////////////CameraHandler.ImageSaveDirectory = "D:\\testTaha7";
                ////////////CameraHandler.TakePhoto();
                ////////////MyDelay(delayAmount);
                ////////////mf.closescreen1();
                ////////////mf.closescreen2();
                GC.Collect();
                ImgCam = (Bitmap)Bitmap.FromFile(address + "\\pattern" + b + ".jpg");
                ImgCam = filterResizeImage.Apply(ImgCam);
                /////////////////////////////////////////////////////////

                Bitmap ImgCamGray2 = grayScale.Apply(ImgCam);
                Bitmap ImgCamBinary2 = otsuThreshold.Apply(ImgCamGray2);
                //ImgCam = CreateNonIndexedImage(ImgCamBinary2);
                //showImage(ImgCamBinary2);
                //ImgCam = complementImage(ImgCamBinary);
                //multipleWithMask((Bitmap)ImgCam, (Bitmap)Msk);

                for (int i = 0; i < CntPntR * CntPntC; i++)
                {
                    if (ImgCamBinary2.GetPixel((int)COG[i, 1], (int)COG[i, 0]).G < 10) //inverser matlab format
                    {
                        COG[i, 2] = COG[i, 2] + Math.Pow(2, b);
                    }
                }
                // showImage(imgW);
            } // end of bits
            double[] findIndex = new double[BPall];
            for (int k = 0; k < BPall; k++) findIndex[k] = COG[k, 2];
            double[] notFound = notfindIndex(CntPntC * CntPntR, findIndex);
            for (int k = BPall; k < CntPntC * CntPntR; k++)
            {
                COG[k, 2] = notFound[k - BPall];
                COG[k, 0] = 1;
                COG[k, 1] = 1;
            }

            double[,] COGt = COG;
            COGt = COG;
            //array.OrderBy(COGt => COGt[2]);
            double[,] COGtSort = sortArray(COGt, CntPntC * CntPntR);
            double[,] cpointNew = new double[CntPntC * CntPntR, 4];
            for (int i = 0; i < CntPntC * CntPntR; i++)
            {
                cpointNew[i, 0] = CPoints[i, 0];
                cpointNew[i, 1] = CPoints[i, 1];
                cpointNew[i, 2] = COGtSort[i, 0];
                cpointNew[i, 3] = COGtSort[i, 1];
            }
            //if (mode == 1)
            //{
            //    cpointNew[0, 0] = 50; cpointNew[0, 1] = 50; cpointNew[0, 2] = 354.0; cpointNew[0, 3] = 2143.0;
            //    cpointNew[1, 0] = 50; cpointNew[1, 1] = 483; cpointNew[1, 2] = 373.0; cpointNew[1, 3] = 2967.0;
            //    cpointNew[2, 0] = 50; cpointNew[2, 1] = 916; cpointNew[2, 2] = 395.0; cpointNew[2, 3] = 3775.0;
            //    cpointNew[3, 0] = 50; cpointNew[3, 1] = 1349; cpointNew[3, 2] = 413.0; cpointNew[3, 3] = 4563.0;
            //    cpointNew[4, 0] = 525; cpointNew[4, 1] = 50; cpointNew[4, 2] = 1293.0; cpointNew[4, 3] = 2171.0;
            //    cpointNew[5, 0] = 525; cpointNew[5, 1] = 483; cpointNew[5, 2] = 1304.0; cpointNew[5, 3] = 2980.0;
            //    cpointNew[6, 0] = 525; cpointNew[6, 1] = 916; cpointNew[6, 2] = 1315.0; cpointNew[6, 3] = 3770.0;
            //    cpointNew[7, 0] = 525; cpointNew[7, 1] = 1349; cpointNew[7, 2] = 1327.0; cpointNew[7, 3] = 4541.0;
            //    cpointNew[8, 0] = 1000; cpointNew[8, 1] = 50; cpointNew[8, 2] = 2194.0; cpointNew[8, 3] = 2196.0;
            //    cpointNew[9, 0] = 1000; cpointNew[9, 1] = 483; cpointNew[9, 2] = 2197.0; cpointNew[9, 3] = 2988.0;
            //    cpointNew[10, 0] = 1000; cpointNew[10, 1] = 916; cpointNew[10, 2] = 2198.0; cpointNew[10, 3] = 3760.0;
            //    cpointNew[11, 0] = 1000; cpointNew[11, 1] = 1349; cpointNew[11, 2] = 2200.0; cpointNew[11, 3] = 4515.0;
            //}
            //else
            //{
            //    cpointNew[0, 0] = 50; cpointNew[0, 1] = 50; cpointNew[0, 2] = 766.0; cpointNew[0, 3] = 524.0;
            //    cpointNew[1, 0] = 50; cpointNew[1, 1] = 483; cpointNew[1, 2] = 786.0; cpointNew[1, 3] = 1316.0;
            //    cpointNew[2, 0] = 50; cpointNew[2, 1] = 916; cpointNew[2, 2] = 802.0; cpointNew[2, 3] = 2104.0;
            //    cpointNew[3, 0] = 50; cpointNew[3, 1] = 1349; cpointNew[3, 2] = 817.0; cpointNew[3, 3] = 2888.0;
            //    cpointNew[4, 0] = 525; cpointNew[4, 1] = 50; cpointNew[4, 2] = 1650.0; cpointNew[4, 3] = 513.0;
            //    cpointNew[5, 0] = 525; cpointNew[5, 1] = 483; cpointNew[5, 2] = 1665.0; cpointNew[5, 3] = 1298.0;
            //    cpointNew[6, 0] = 525; cpointNew[6, 1] = 916; cpointNew[6, 2] = 1677.0; cpointNew[6, 3] = 2080.0;
            //    cpointNew[7, 0] = 525; cpointNew[7, 1] = 1349; cpointNew[7, 2] = 1687.0; cpointNew[7, 3] = 2860.0;
            //    cpointNew[8, 0] = 1000; cpointNew[8, 1] = 50; cpointNew[8, 2] = 2519.0; cpointNew[8, 3] = 508.0;
            //    cpointNew[9, 0] = 1000; cpointNew[9, 1] = 483; cpointNew[9, 2] = 2529.0; cpointNew[9, 3] = 1288.0;
            //    cpointNew[10, 0] = 1000; cpointNew[10, 1] = 916; cpointNew[10, 2] = 2536.0; cpointNew[10, 3] = 2064.0;
            //    cpointNew[11, 0] = 1000; cpointNew[11, 1] = 1349; cpointNew[11, 2] = 2541.0; cpointNew[11, 3] = 2837.0;
            //}
            double[, ,] Atotal = new double[8, 8, BPall];
            List<double[,]> aT = new List<double[,]>();
            List<double[]> bT = new List<double[]>();
            double[,] Btotal = new double[8, BPall];
            int counter = 0;
            double[,] coef = new double[(CntPntR - 1) * (CntPntC - 1), 8];

            for (int r = 0; r <= CntPntR - 2; r++)
            {
                for (int c = 0; c <= CntPntC - 2; c++)
                {

                    double r11 = cpointNew[r * CntPntC + c, 0]; double c11 = cpointNew[r * CntPntC + c, 1];
                    double r12 = cpointNew[r * CntPntC + c + 1, 0]; double c12 = cpointNew[r * CntPntC + c + 1, 1];
                    double r21 = cpointNew[(r + 1) * CntPntC + c, 0]; double c21 = cpointNew[(r + 1) * CntPntC + c, 1];
                    double r22 = cpointNew[(r + 1) * CntPntC + c + 1, 0]; double c22 = cpointNew[(r + 1) * CntPntC + c + 1, 1];
                    //////////////////////////////////////////// b ////////////////////////////////////////
                    double rr11 = cpointNew[r * CntPntC + c, 2]; double cc11 = cpointNew[r * CntPntC + c, 3];
                    double rr12 = cpointNew[r * CntPntC + c + 1, 2]; double cc12 = cpointNew[r * CntPntC + c + 1, 3];
                    double rr21 = cpointNew[(r + 1) * CntPntC + c, 2]; double cc21 = cpointNew[(r + 1) * CntPntC + c, 3];
                    double rr22 = cpointNew[(r + 1) * CntPntC + c + 1, 2]; double cc22 = cpointNew[(r + 1) * CntPntC + c + 1, 3];
                    if (rr11 == 1 || rr12 == 1 || rr21 == 1 || rr22 == 1) continue;
                    double[,] A = new double[,]{{r11,c11,r11*c11,1,0,0,0,0},{0,0,0,0,r11,c11,r11*c11,1},
                   {r12,c12,r12*c12,1,0,0,0,0},{0,0,0,0,r12,c12,r12*c12,1},
                   {r21,c21,r21*c21,1,0,0,0,0},{0,0,0,0,r21,c21,r21*c21,1},
                   {r22,c22,r22*c22,1,0,0,0,0},{0,0,0,0,r22,c22,r22*c22,1}};
                    ////////////////////////////////////////////// 
                    aT.Add(A);
                    for (int k = 0; k < 8; k++)
                        for (int kk = 0; kk < 8; kk++)
                            Atotal[k, kk, counter] = A[k, kk];
                    //////////////////////////////////////////////   
                    string AString = "";
                    for (int i = 0; i < 8; i++)
                    {
                        for (int j = 0; j < 8; j++)
                        {
                            AString += A[i, j];
                            if (j < 7) AString += ",";
                        }
                        if (i < 7) AString += ";";
                    }
                    double[] b = new double[] { rr11, cc11, rr12, cc12, rr21, cc21, rr22, cc22 };
                    //////////////////////////////////////////////
                    bT.Add(b);
                    for (int k = 0; k < 8; k++) Btotal[k, counter] = b[k];
                    //////////////////////////////////////////////
                    counter++;
                    string bString = "";
                    //  for (int i = 0; i < 8; i++)
                    //  {
                    for (int j = 0; j < 8; j++)
                    {
                        bString += b[j];
                        if (j < 7) bString += ",";
                        //      }
                        //      if (i < 7) AString += ";";
                    }

                    double[] c1 = new double[8];

                    Matrix M = new Matrix("1,1;1,2"); // init
                    Matrix A2 = new Matrix(AString);
                    Matrix A3 = new Matrix(A);
                    Matrix b3 = new Matrix(b);
                    Matrix b2 = new Matrix(bString);
                    //CSML.Complex det = M.Determinant(); // det = 1 
                    //Matrix Minv = M.Inverse(); // Minv = [2, -1; -1, 1] 
                    //CSML.Complex cc = A2.Determinant();
                    //Matrix AInverse = A2.Inverse();
                    //Matrix c1Matrix = AInverse * b2.Transpose();
                    Matrix c1Solve = Matrix.Solve(A2, b2.Transpose());
                    //Complex ad = b2[0, 0];
                    // Matrix c4 = A2.SolveCG(b2);
                    //for (int i = 0; i < 8; i++)
                    //{
                    //    double sum = 0;
                    //    for (int j = 0; j < 8; j++)
                    //    {
                    //        double inv =(double) AInverse[i, j].Re;
                    //        sum += inv * b[i];
                    //    }
                    //    c1[i] = sum;
                    //}

                    for (int j = 0; j < 8; j++)
                    {
                        CSML.Complex temp = c1Solve[j + 1, 1];
                        coef[counter, j] = temp.Re;

                    }
                    // Coef=[Coef;c1'];
                }
            }


            double[, ,] map = new double[RVid, CVid, 2];
            for (int r = 1; r <= RVid; r++)
            {
                int br = (int)Math.Floor((double)(r - Offset) / DelR);
                if (br < 0) br = 0;
                if (br > (CntPntR - 2)) br = br - 1;
                for (int c = 1; c <= CVid; c++)
                {
                    int bc = (int)Math.Floor((double)(c - Offset) / DelC);
                    if (bc < 0) bc = 0;
                    if (bc > (CntPntC - 2)) bc = bc - 1;
                    double[] c1 = new double[8];
                    ///////////////////////////////////////////
                    int indexNearest = nearestBlock(r, c, aT) + 1; // to works same as matlab
                    for (int i = 0; i < 8; i++) c1[i] = coef[indexNearest, i]; //c1[i] = coef[br * (CntPntC - 1) + bc, i];
                    ///////////////////////////////////////////
                    double rr = c1[0] * r + c1[1] * c + c1[2] * r * c + c1[3];
                    double cc = c1[4] * r + c1[5] * c + c1[6] * r * c + c1[7];
                    map[r - 1, c - 1, 0] = Math.Round(rr);
                    map[r - 1, c - 1, 1] = Math.Round(cc);
                }
            }



            // Bitmap imgWhiteCam = new Bitmap(ImgCamBinary.Width, ImgCamBinary.Height);
            // changeColor(imgWhiteCam, System.Drawing.Color.White);
            //AForge.Imaging.Image.FormatImage(ref imgWhiteCam);
            // Bitmap t1 = AForge.Imaging.Image.Clone(imgWhiteCam , System.Drawing.Imaging.PixelFormat.Format8bppIndexed);
            // Bitmap t2 = AForge.Imaging.Image.Clone(ImgCam, System.Drawing.Imaging.PixelFormat.Format8bppIndexed); 

            //ImgCam = subImage.Apply(ImgCam);
            //MessageBox.Show("" + ImgCam.GetPixel(20, 20));

            //  img = ImgCam;
            //showImage(ImgCam);
            //  showImage(Lc);
            // showImage(ImgCam);
            //  showImage(ImgCamBinary);
            //  MessageBox.Show("" + ImgCam.GetPixel(10, 10).R);
            //  MessageBox.Show("" + ImgCamBinary.GetPixel(10, 10).R);


            return map;
        }
        public int nearestBlock(int r, int c, List<double[,]> aT)
        {
            int index = 0;
            int countNouFound = aT.Count();
            double minDistTotal = double.MaxValue;
            for (int i = 0; i < countNouFound; i++)
            {
                double r11 = aT[i][0, 0];
                double c11 = aT[i][0, 1];
                double r12 = aT[i][2, 0];
                double c12 = aT[i][2, 1];
                double r21 = aT[i][4, 0];
                double c21 = aT[i][4, 1];
                double r22 = aT[i][6, 0];
                double c22 = aT[i][6, 1];
                double centerC = (c11 + c12 + c21 + c22) / 4;
                double centerR = (r11 + r12 + r21 + r22) / 4;

                // dis11 = (sqrt ( (r-r11).^2 + (c-c11).^2 ) );
                // dis12 = (sqrt ( (r-r12).^2 + (c-c12).^2 ) );   // matlab version
                // dis21 = (sqrt ( (r-r21).^2 + (c-c21).^2 ) );
                // dis22 = (sqrt ( (r-r22).^2 + (c-c22).^2 ) );

                // mindis = min(dis11,min(dis12,min(dis21,dis22)));
                double mindis = Math.Sqrt(Math.Pow((r - centerR), 2) + Math.Pow((c - centerC), 2));
                if (mindis < minDistTotal)
                {
                    index = i;
                    minDistTotal = mindis;
                } // if
            }
            return index;
        }
        public double[] notfindIndex(int totalPoint, double[] findIndex)
        {
            int rest = totalPoint - findIndex.Count();
            double[] ret = new double[rest];
            if (rest == 0) return ret;
            for (int i = 0; i < totalPoint; i++)
            {
                bool found = false;
                for (int j = 0; j < findIndex.Count(); j++)
                {
                    if (i == findIndex[j]) { found = true; break; }
                }
                if (!found) { ret[rest - 1] = i; rest--; }
            } // for totalPoint
            return ret;
        }
        public int countBlobBasedOnArea(AForge.Imaging.Blob[] blobs, int areaLimit)
        {
            int ret = 0;
            for (int i = 0; i < blobs.Count(); i++)
            {
                if (blobs[i].Area > areaLimit) ret++;
            }//for
            return ret;
        }
        public static void dumpArray(double[,] arr, string fileName)
        {
            using (System.IO.StreamWriter file = new System.IO.StreamWriter(fileName))
            {
                int width = arr.GetLength(0);
                int height = arr.GetLength(1);
                for (int i = 0; i < width; i++)
                {
                    for (int j = 0; j < height; j++)
                    {
                        file.Write(arr[i, j] + ",");
                    }
                }
            }
        }
        public void getArray(double[,] array, string fileName)
        {
            //   Bitmap dis = new Bitmap((int)array.GetLongLength(1), (int)array.GetLongLength(0));
            var reader = new System.IO.StreamReader(System.IO.File.OpenRead(@"F:\\map1.csv"));

            //  while (!reader.EndOfStream)
            //  {
            var line = reader.ReadLine();
            var values = line.Split(',');
            for (int i = 0; i < array.GetLongLength(0); i++)
            {
                for (int j = 0; j < array.GetLongLength(1); j++)
                {
                    string value = values[i * array.GetLongLength(1) + j];
                    //    dis.SetPixel(j, i, Color.FromArgb((int)(Convert.ToDouble(value) * 255), (int)(Convert.ToDouble(value) * 255), (int)(Convert.ToDouble(value) * 255)));
                    array[i, j] = Convert.ToDouble(value);
                    // if (i == array.GetLongLength(0)) i = 0; else i++;
                    //  if (j == array.GetLongLength(1)) j = 0; else j++;

                    // }
                }
            }
            //        showImage(dis);
        }
        public double[,] changeValueArray(double[,] arr, int width, int height, double value)
        {
            double[,] retArr = new double[width, height];
            for (int i = 0; i < width; i++)
                for (int j = 0; j < height; j++)
                    retArr[i, j] = value;

            return retArr;
        }
        public double[,] sortArray(double[,] arr, int length)
        {
            double[,] retArr = new double[length, 3];
            int j;
            int i;
            for (i = 0; i < length; i++)
            {
                for (j = 0; j < length; j++)
                {
                    if (arr[j, 2] == i) break;
                }
                retArr[i, 0] = arr[j, 0];
                retArr[i, 1] = arr[j, 1];
                retArr[i, 2] = arr[j, 2];
            }
            return retArr;
        }
        public void multipleWithMask(Bitmap image, Bitmap mask)
        {
            // Bitmap retImage = new Bitmap(image.Width, image.Height);
            for (int i = 0; i < image.Width; i++)
            {
                for (int j = 0; j < image.Height; j++)
                {
                    int newRMask;
                    int newGMask;
                    int newBMask;
                    if (mask.GetPixel(i, j).R > 0) newRMask = image.GetPixel(i, j).R;
                    else newRMask = 0;
                    if (mask.GetPixel(i, j).G > 0) newGMask = image.GetPixel(i, j).G;
                    else newGMask = 0;
                    if (mask.GetPixel(i, j).B > 0) newBMask = image.GetPixel(i, j).B;
                    else newBMask = 0;
                    Color newColorMsk = Color.FromArgb(newRMask, newGMask, newBMask);
                    image.SetPixel(i, j, newColorMsk);
                }
            }
            //    return retImage;
        }
        public Bitmap CreateNonIndexedImage(Bitmap src)
        {
            Bitmap newBmp = new Bitmap(src.Width, src.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            using (Graphics gfx = Graphics.FromImage(newBmp))
            {
                gfx.DrawImage(src, 0, 0);
            }

            return newBmp;
        }
        public Bitmap minusImage(Bitmap image1, Bitmap image2)
        {
            Bitmap retImage = new Bitmap(image1.Width, image1.Height);
            for (int x = 0; x < image1.Width; x++)
            {
                for (int y = 0; y < image1.Height; y++)
                {
                    Color Color1 = image1.GetPixel(x, y);
                    Color Color2 = image2.GetPixel(x, y);
                    int R = Color1.R - Color2.R;
                    int G = Color1.G - Color2.G;
                    int B = Color1.B - Color2.B;
                    Color newColor = Color.FromArgb((R + 255) / 2, (G + 255) / 2, (B + 255) / 2);
                    retImage.SetPixel(x, y, newColor);
                }
            }
            //MessageBox.Show(""+image.GetPixel(100,100).R+10);
            return retImage;

        }
        public Bitmap orImage(Bitmap image1, Bitmap image2)
        {
            Bitmap retImage = new Bitmap(image1.Width, image1.Height);
            for (int x = 0; x < image1.Width; x++)
            {
                for (int y = 0; y < image1.Height; y++)
                {
                    Color Color1 = image1.GetPixel(x, y);
                    Color Color2 = image2.GetPixel(x, y);
                    Color newColor;
                    if (Color1.G > 20 && Color2.G > 20)
                    {
                        newColor = Color.FromArgb(255, 255, 255);
                    }
                    else newColor = Color.FromArgb(0, 0, 0);
                    retImage.SetPixel(x, y, newColor);
                }
            }
            //MessageBox.Show(""+image.GetPixel(100,100).R+10);
            return retImage;

        }
        public Bitmap andImage(Bitmap image1, Bitmap image2)
        {
            Bitmap retImage = new Bitmap(image1.Width, image1.Height);
            for (int x = 0; x < image1.Width; x++)
            {
                for (int y = 0; y < image1.Height; y++)
                {
                    Color Color1 = image1.GetPixel(x, y);
                    Color Color2 = image2.GetPixel(x, y);
                    Color newColor;
                    if (Color1.G > 0 || Color2.G > 0)
                    {
                        newColor = Color.FromArgb(255, 255, 255);
                    }
                    else newColor = Color.FromArgb(0, 0, 0);
                    retImage.SetPixel(x, y, newColor);
                }
            }
            //MessageBox.Show(""+image.GetPixel(100,100).R+10);
            return retImage;
        }
        public Bitmap complementImage(Bitmap image)
        {
            Bitmap retImage = new Bitmap(image.Width, image.Height);
            for (int x = 0; x < image.Width; x++)
            {
                for (int y = 0; y < image.Height; y++)
                {
                    Color originalColor = image.GetPixel(x, y);
                    int Re = (int)255 - originalColor.R;
                    int Gr = (int)255 - originalColor.G;
                    int Bl = (int)255 - originalColor.B;
                    Color newColor = Color.FromArgb(Re, Gr, Bl);

                    retImage.SetPixel(x, y, newColor);
                }
            }
            //MessageBox.Show(""+image.GetPixel(100,100).R+10);
            return retImage;
        }
        public void changeColor(Bitmap s, System.Drawing.Color target)
        {
            for (int x = 0; x < s.Width; x++)
            {
                for (int y = 0; y < s.Height; y++)
                {
                    s.SetPixel(x, y, target);
                }
            }
        }
        public void showImage(System.Drawing.Bitmap image)
        {
            System.Windows.Forms.Form form = new System.Windows.Forms.Form();
            System.Windows.Forms.PictureBox pictureBox = new System.Windows.Forms.PictureBox();
            pictureBox.Dock = System.Windows.Forms.DockStyle.Fill;
            pictureBox.Image = image;
            pictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            form.Controls.Add(pictureBox);
            form.ShowDialog();
        }

        //private void button25_Click(object sender, EventArgs e)
        //{
        //    if (!Directory.Exists(textBoxSavePath.Text))
        //    {
        //        Directory.CreateDirectory(textBoxSavePath.Text);
        //    }

        //    CameraHandler.ImageSaveDirectory = textBoxSavePath.Text;

        //    // object from matlab class should be created
        //    MatlabFunction matlabFunction = new MatlabFunction();

        //    CameraHandler.ImageName = textBoxImageName.Text;
        //    MyDelay(5);
        //    CameraHandler.TakePhoto();

        //}

        //private void checkBoxImageName_CheckedChanged(object sender, EventArgs e)
        //{
        //    if (checkBoxImageName.Checked==true)
        //    {
        //        textBoxImageName.Enabled = true;
        //        //checkBoxImageName.Focus();
        //    }
        //    else
        //    {
        //        textBoxImageName.Enabled = false;
        //        textBoxImageName.Clear();
        //    }
        //}

        
       // private void PicnumUpDown_ValueChanged(object sender, EventArgs e)
       // {
       //     NumericUpDownValueChangedOperation();
       //     numExpo = (int)PicnumUpDown.Value;
       // }

       // private void NumSamplesButton_Click(object sender, EventArgs e)
       // {
       //     if (NumSamplesTxtBox.Text != string.Empty)
       //     {
       //         num = Convert.ToInt32(NumSamplesTxtBox.Text);
       //     }
           
       //     if (NumSamplesTxtBox.Text == "" || num == 0)
       //         {
       //             MessageBox.Show("Please Enter number of samples");
       //         }
       //     else
       //         NumSamples = num;
       // }

       // private void MakeHDRButton_Click(object sender, EventArgs e)
       // {
       //     Double Lambda = 50.0;
       //     var exarray = ExpoList.ToArray();
       //     MWArray ExpoList1 = (MWNumericArray)exarray;

       //     if (ExpoList1.IsEmpty == true)
       //         MessageBox.Show("expo not assigned");

       //     MWArray PicPath1 = (MWCharArray)PicPath;

       //     if (PicPath1.IsEmpty == true)
       //         MessageBox.Show("pic path not assigned");

       //     if (PicnumUpDown.Value == 0 || NumSamplesTxtBox.Text == "")
       //         MessageBox.Show("Not enough information!");

       //     if (ExpoList1.IsEmpty == false && PicPath1.IsEmpty == false)
       //     {
       //         HDRMaker.HDRFunc MFunc = new HDRMaker.HDRFunc();
       //         MFunc.MakeHDRfunc((MWArray)numExpo, ExpoList1, (MWArray)Lambda, (MWArray)NumSamples, PicPath1);

       //         //try
       //         //{
       //         //    MFunc.MakeHDRfunc((MWArray)numExpo, ExpoList1, (MWArray)Lambda, (MWArray)NumSamples, PicPath1);
       //         //}
       //         //catch (Exception eee) 
       //         //{
       //         //    MessageBox.Show(eee.Message);
       //         //}
       //     }
       //}

      }
}
