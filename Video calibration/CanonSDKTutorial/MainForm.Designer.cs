namespace CanonSDK
{
    partial class formMain
    {
        /// <summary>
        /// Erforderliche Designervariable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Verwendete Ressourcen bereinigen.
        /// </summary>
        /// <param name="disposing">True, wenn verwaltete Ressourcen gelöscht werden sollen; andernfalls False.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Vom Windows Form-Designer generierter Code

        /// <summary>
        /// Erforderliche Methode für die Designerunterstützung.
        /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(formMain));
            this.LiveViewPicBox = new System.Windows.Forms.PictureBox();
            this.LiveViewButton = new System.Windows.Forms.Button();
            this.FrameRateLabel = new System.Windows.Forms.Label();
            this.CameraListBox = new System.Windows.Forms.ListBox();
            this.SessionButton = new System.Windows.Forms.Button();
            this.SessionLabel = new System.Windows.Forms.Label();
            this.InitGroupBox = new System.Windows.Forms.GroupBox();
            this.button2 = new System.Windows.Forms.Button();
            this.RefreshButton = new System.Windows.Forms.Button();
            this.LiveViewGroupBox = new System.Windows.Forms.GroupBox();
            this.FocusFar3Button = new System.Windows.Forms.Button();
            this.FocusFar2Button = new System.Windows.Forms.Button();
            this.FocusNear3Button = new System.Windows.Forms.Button();
            this.FocusFar1Button = new System.Windows.Forms.Button();
            this.FocusNear2Button = new System.Windows.Forms.Button();
            this.FocusNear1Button = new System.Windows.Forms.Button();
            this.SettingsGroupBox = new System.Windows.Forms.GroupBox();
            this.button1 = new System.Windows.Forms.Button();
            this.buttonSaturationTest = new System.Windows.Forms.Button();
            this.label13 = new System.Windows.Forms.Label();
            this.buttonBrowse = new System.Windows.Forms.Button();
            this.label17 = new System.Windows.Forms.Label();
            this.textBoxSavePath = new System.Windows.Forms.TextBox();
            this.Lable12 = new System.Windows.Forms.Label();
            this.ComboBoxPictureStyle = new System.Windows.Forms.ComboBox();
            this.ComboBoxToningEffect = new System.Windows.Forms.ComboBox();
            this.ImgQualityCoBox = new System.Windows.Forms.ComboBox();
            this.numericUpDownSharpness = new System.Windows.Forms.NumericUpDown();
            this.ComboBoxWB = new System.Windows.Forms.ComboBox();
            this.label12 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label16 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.numericUpDownContrast = new System.Windows.Forms.NumericUpDown();
            this.label7 = new System.Windows.Forms.Label();
            this.ComboBoxFilterEffect = new System.Windows.Forms.ComboBox();
            this.label8 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.numericUpDownColorTone = new System.Windows.Forms.NumericUpDown();
            this.ISOCoBox = new System.Windows.Forms.ComboBox();
            this.label14 = new System.Windows.Forms.Label();
            this.comboBoxTV = new System.Windows.Forms.ComboBox();
            this.numericUpDownSaturation = new System.Windows.Forms.NumericUpDown();
            this.AvCoBox = new System.Windows.Forms.ComboBox();
            this.MeteringCoBox = new System.Windows.Forms.ComboBox();
            this.DriveCoBox = new System.Windows.Forms.ComboBox();
            this.AFModeCoBox = new System.Windows.Forms.ComboBox();
            this.WBSHCoBox = new System.Windows.Forms.ComboBox();
            this.SaveFolderBrowser = new System.Windows.Forms.FolderBrowserDialog();
            this.textBox1 = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.LiveViewPicBox)).BeginInit();
            this.InitGroupBox.SuspendLayout();
            this.LiveViewGroupBox.SuspendLayout();
            this.SettingsGroupBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownSharpness)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownContrast)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownColorTone)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownSaturation)).BeginInit();
            this.SuspendLayout();
            // 
            // LiveViewPicBox
            // 
            this.LiveViewPicBox.Location = new System.Drawing.Point(10, 19);
            this.LiveViewPicBox.Name = "LiveViewPicBox";
            this.LiveViewPicBox.Size = new System.Drawing.Size(859, 392);
            this.LiveViewPicBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.LiveViewPicBox.TabIndex = 1;
            this.LiveViewPicBox.TabStop = false;
            this.LiveViewPicBox.MouseDown += new System.Windows.Forms.MouseEventHandler(this.LiveViewPicBox_MouseDown);
            // 
            // LiveViewButton
            // 
            this.LiveViewButton.Location = new System.Drawing.Point(882, 67);
            this.LiveViewButton.Name = "LiveViewButton";
            this.LiveViewButton.Size = new System.Drawing.Size(40, 94);
            this.LiveViewButton.TabIndex = 2;
            this.LiveViewButton.Text = "Start LiveView";
            this.LiveViewButton.UseVisualStyleBackColor = true;
            this.LiveViewButton.Click += new System.EventHandler(this.LiveViewButton_Click);
            // 
            // FrameRateLabel
            // 
            this.FrameRateLabel.AutoSize = true;
            this.FrameRateLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FrameRateLabel.Location = new System.Drawing.Point(879, 164);
            this.FrameRateLabel.Name = "FrameRateLabel";
            this.FrameRateLabel.Size = new System.Drawing.Size(50, 15);
            this.FrameRateLabel.TabIndex = 5;
            this.FrameRateLabel.Text = "FPS: 24";
            // 
            // CameraListBox
            // 
            this.CameraListBox.FormattingEnabled = true;
            this.CameraListBox.Location = new System.Drawing.Point(6, 45);
            this.CameraListBox.Name = "CameraListBox";
            this.CameraListBox.Size = new System.Drawing.Size(121, 56);
            this.CameraListBox.TabIndex = 6;
            // 
            // SessionButton
            // 
            this.SessionButton.Location = new System.Drawing.Point(6, 107);
            this.SessionButton.Name = "SessionButton";
            this.SessionButton.Size = new System.Drawing.Size(84, 23);
            this.SessionButton.TabIndex = 7;
            this.SessionButton.Text = "Open Session";
            this.SessionButton.UseVisualStyleBackColor = true;
            this.SessionButton.Click += new System.EventHandler(this.SessionButton_Click);
            // 
            // SessionLabel
            // 
            this.SessionLabel.AutoSize = true;
            this.SessionLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.SessionLabel.Location = new System.Drawing.Point(6, 16);
            this.SessionLabel.Name = "SessionLabel";
            this.SessionLabel.Size = new System.Drawing.Size(110, 16);
            this.SessionLabel.TabIndex = 8;
            this.SessionLabel.Text = "No open session";
            // 
            // InitGroupBox
            // 
            this.InitGroupBox.Controls.Add(this.textBox1);
            this.InitGroupBox.Controls.Add(this.button2);
            this.InitGroupBox.Controls.Add(this.RefreshButton);
            this.InitGroupBox.Controls.Add(this.CameraListBox);
            this.InitGroupBox.Controls.Add(this.SessionLabel);
            this.InitGroupBox.Controls.Add(this.SessionButton);
            this.InitGroupBox.Location = new System.Drawing.Point(12, 12);
            this.InitGroupBox.Name = "InitGroupBox";
            this.InitGroupBox.Size = new System.Drawing.Size(135, 161);
            this.InitGroupBox.TabIndex = 9;
            this.InitGroupBox.TabStop = false;
            this.InitGroupBox.Text = "Init";
            // 
            // button2
            // 
            this.button2.BackColor = System.Drawing.SystemColors.Highlight;
            this.button2.Location = new System.Drawing.Point(6, 136);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(84, 23);
            this.button2.TabIndex = 10;
            this.button2.Text = "calibration";
            this.button2.UseVisualStyleBackColor = false;
            this.button2.Click += new System.EventHandler(this.button2_Click_1);
            // 
            // RefreshButton
            // 
            this.RefreshButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.RefreshButton.Location = new System.Drawing.Point(95, 107);
            this.RefreshButton.Name = "RefreshButton";
            this.RefreshButton.Size = new System.Drawing.Size(31, 23);
            this.RefreshButton.TabIndex = 9;
            this.RefreshButton.Text = "↻";
            this.RefreshButton.UseVisualStyleBackColor = true;
            this.RefreshButton.Click += new System.EventHandler(this.RefreshButton_Click);
            // 
            // LiveViewGroupBox
            // 
            this.LiveViewGroupBox.Controls.Add(this.FocusFar3Button);
            this.LiveViewGroupBox.Controls.Add(this.LiveViewPicBox);
            this.LiveViewGroupBox.Controls.Add(this.LiveViewButton);
            this.LiveViewGroupBox.Controls.Add(this.FocusFar2Button);
            this.LiveViewGroupBox.Controls.Add(this.FocusNear3Button);
            this.LiveViewGroupBox.Controls.Add(this.FrameRateLabel);
            this.LiveViewGroupBox.Controls.Add(this.FocusFar1Button);
            this.LiveViewGroupBox.Controls.Add(this.FocusNear2Button);
            this.LiveViewGroupBox.Controls.Add(this.FocusNear1Button);
            this.LiveViewGroupBox.Location = new System.Drawing.Point(12, 180);
            this.LiveViewGroupBox.Name = "LiveViewGroupBox";
            this.LiveViewGroupBox.Size = new System.Drawing.Size(958, 427);
            this.LiveViewGroupBox.TabIndex = 10;
            this.LiveViewGroupBox.TabStop = false;
            this.LiveViewGroupBox.Text = "LiveView";
            // 
            // FocusFar3Button
            // 
            this.FocusFar3Button.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FocusFar3Button.Location = new System.Drawing.Point(886, 203);
            this.FocusFar3Button.Name = "FocusFar3Button";
            this.FocusFar3Button.Size = new System.Drawing.Size(36, 23);
            this.FocusFar3Button.TabIndex = 6;
            this.FocusFar3Button.Text = ">>>";
            this.FocusFar3Button.UseVisualStyleBackColor = true;
            this.FocusFar3Button.Click += new System.EventHandler(this.FocusFar3Button_Click);
            // 
            // FocusFar2Button
            // 
            this.FocusFar2Button.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FocusFar2Button.Location = new System.Drawing.Point(886, 232);
            this.FocusFar2Button.Name = "FocusFar2Button";
            this.FocusFar2Button.Size = new System.Drawing.Size(36, 23);
            this.FocusFar2Button.TabIndex = 6;
            this.FocusFar2Button.Text = ">>";
            this.FocusFar2Button.UseVisualStyleBackColor = true;
            this.FocusFar2Button.Click += new System.EventHandler(this.FocusFar2Button_Click);
            // 
            // FocusNear3Button
            // 
            this.FocusNear3Button.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FocusNear3Button.Location = new System.Drawing.Point(886, 356);
            this.FocusNear3Button.Name = "FocusNear3Button";
            this.FocusNear3Button.Size = new System.Drawing.Size(36, 23);
            this.FocusNear3Button.TabIndex = 6;
            this.FocusNear3Button.Text = "<<<";
            this.FocusNear3Button.UseVisualStyleBackColor = true;
            this.FocusNear3Button.Click += new System.EventHandler(this.FocusNear3Button_Click);
            // 
            // FocusFar1Button
            // 
            this.FocusFar1Button.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FocusFar1Button.Location = new System.Drawing.Point(886, 262);
            this.FocusFar1Button.Name = "FocusFar1Button";
            this.FocusFar1Button.Size = new System.Drawing.Size(36, 23);
            this.FocusFar1Button.TabIndex = 6;
            this.FocusFar1Button.Text = ">";
            this.FocusFar1Button.UseVisualStyleBackColor = true;
            this.FocusFar1Button.Click += new System.EventHandler(this.FocusFar1Button_Click);
            // 
            // FocusNear2Button
            // 
            this.FocusNear2Button.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FocusNear2Button.Location = new System.Drawing.Point(886, 327);
            this.FocusNear2Button.Name = "FocusNear2Button";
            this.FocusNear2Button.Size = new System.Drawing.Size(36, 23);
            this.FocusNear2Button.TabIndex = 6;
            this.FocusNear2Button.Text = "<<";
            this.FocusNear2Button.UseVisualStyleBackColor = true;
            this.FocusNear2Button.Click += new System.EventHandler(this.FocusNear2Button_Click);
            // 
            // FocusNear1Button
            // 
            this.FocusNear1Button.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FocusNear1Button.Location = new System.Drawing.Point(886, 297);
            this.FocusNear1Button.Name = "FocusNear1Button";
            this.FocusNear1Button.Size = new System.Drawing.Size(36, 23);
            this.FocusNear1Button.TabIndex = 6;
            this.FocusNear1Button.Text = "<";
            this.FocusNear1Button.UseVisualStyleBackColor = true;
            this.FocusNear1Button.Click += new System.EventHandler(this.FocusNear1Button_Click);
            // 
            // SettingsGroupBox
            // 
            this.SettingsGroupBox.Controls.Add(this.button1);
            this.SettingsGroupBox.Controls.Add(this.buttonSaturationTest);
            this.SettingsGroupBox.Controls.Add(this.label13);
            this.SettingsGroupBox.Controls.Add(this.buttonBrowse);
            this.SettingsGroupBox.Controls.Add(this.label17);
            this.SettingsGroupBox.Controls.Add(this.textBoxSavePath);
            this.SettingsGroupBox.Controls.Add(this.Lable12);
            this.SettingsGroupBox.Controls.Add(this.ComboBoxPictureStyle);
            this.SettingsGroupBox.Controls.Add(this.ComboBoxToningEffect);
            this.SettingsGroupBox.Controls.Add(this.ImgQualityCoBox);
            this.SettingsGroupBox.Controls.Add(this.numericUpDownSharpness);
            this.SettingsGroupBox.Controls.Add(this.ComboBoxWB);
            this.SettingsGroupBox.Controls.Add(this.label12);
            this.SettingsGroupBox.Controls.Add(this.label3);
            this.SettingsGroupBox.Controls.Add(this.label2);
            this.SettingsGroupBox.Controls.Add(this.label16);
            this.SettingsGroupBox.Controls.Add(this.label5);
            this.SettingsGroupBox.Controls.Add(this.numericUpDownContrast);
            this.SettingsGroupBox.Controls.Add(this.label7);
            this.SettingsGroupBox.Controls.Add(this.ComboBoxFilterEffect);
            this.SettingsGroupBox.Controls.Add(this.label8);
            this.SettingsGroupBox.Controls.Add(this.label11);
            this.SettingsGroupBox.Controls.Add(this.label9);
            this.SettingsGroupBox.Controls.Add(this.label15);
            this.SettingsGroupBox.Controls.Add(this.label1);
            this.SettingsGroupBox.Controls.Add(this.numericUpDownColorTone);
            this.SettingsGroupBox.Controls.Add(this.ISOCoBox);
            this.SettingsGroupBox.Controls.Add(this.label14);
            this.SettingsGroupBox.Controls.Add(this.comboBoxTV);
            this.SettingsGroupBox.Controls.Add(this.numericUpDownSaturation);
            this.SettingsGroupBox.Controls.Add(this.AvCoBox);
            this.SettingsGroupBox.Controls.Add(this.MeteringCoBox);
            this.SettingsGroupBox.Controls.Add(this.DriveCoBox);
            this.SettingsGroupBox.Controls.Add(this.AFModeCoBox);
            this.SettingsGroupBox.Location = new System.Drawing.Point(152, 15);
            this.SettingsGroupBox.Name = "SettingsGroupBox";
            this.SettingsGroupBox.Size = new System.Drawing.Size(819, 159);
            this.SettingsGroupBox.TabIndex = 11;
            this.SettingsGroupBox.TabStop = false;
            this.SettingsGroupBox.Text = "Settings";
            this.SettingsGroupBox.Enter += new System.EventHandler(this.SettingsGroupBox_Enter_1);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(728, 77);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(66, 50);
            this.button1.TabIndex = 30;
            this.button1.Text = "pic show";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click_2);
            // 
            // buttonSaturationTest
            // 
            this.buttonSaturationTest.Location = new System.Drawing.Point(611, 77);
            this.buttonSaturationTest.Name = "buttonSaturationTest";
            this.buttonSaturationTest.Size = new System.Drawing.Size(110, 50);
            this.buttonSaturationTest.TabIndex = 12;
            this.buttonSaturationTest.Text = "Saturation Test";
            this.buttonSaturationTest.UseVisualStyleBackColor = true;
            this.buttonSaturationTest.Click += new System.EventHandler(this.button1_Click_1);
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label13.Location = new System.Drawing.Point(552, 19);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(57, 16);
            this.label13.TabIndex = 17;
            this.label13.Text = "PicStyle";
            // 
            // buttonBrowse
            // 
            this.buttonBrowse.Location = new System.Drawing.Point(331, 128);
            this.buttonBrowse.Name = "buttonBrowse";
            this.buttonBrowse.Size = new System.Drawing.Size(99, 23);
            this.buttonBrowse.TabIndex = 5;
            this.buttonBrowse.Text = "Browse";
            this.buttonBrowse.UseVisualStyleBackColor = true;
            this.buttonBrowse.Click += new System.EventHandler(this.BrowseButton_Click);
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Location = new System.Drawing.Point(695, 50);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(56, 13);
            this.label17.TabIndex = 29;
            this.label17.Text = "Toning Eff";
            // 
            // textBoxSavePath
            // 
            this.textBoxSavePath.Location = new System.Drawing.Point(9, 128);
            this.textBoxSavePath.Name = "textBoxSavePath";
            this.textBoxSavePath.Size = new System.Drawing.Size(316, 20);
            this.textBoxSavePath.TabIndex = 6;
            // 
            // Lable12
            // 
            this.Lable12.AutoSize = true;
            this.Lable12.Cursor = System.Windows.Forms.Cursors.Default;
            this.Lable12.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Lable12.Location = new System.Drawing.Point(331, 100);
            this.Lable12.Name = "Lable12";
            this.Lable12.Size = new System.Drawing.Size(71, 16);
            this.Lable12.TabIndex = 15;
            this.Lable12.Text = "ImgQuality";
            // 
            // ComboBoxPictureStyle
            // 
            this.ComboBoxPictureStyle.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ComboBoxPictureStyle.FormattingEnabled = true;
            this.ComboBoxPictureStyle.Location = new System.Drawing.Point(449, 18);
            this.ComboBoxPictureStyle.Name = "ComboBoxPictureStyle";
            this.ComboBoxPictureStyle.Size = new System.Drawing.Size(99, 21);
            this.ComboBoxPictureStyle.TabIndex = 16;
            this.ComboBoxPictureStyle.SelectedIndexChanged += new System.EventHandler(this.PicStyleCoBox_SelectedIndexChanged);
            // 
            // ComboBoxToningEffect
            // 
            this.ComboBoxToningEffect.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ComboBoxToningEffect.FormattingEnabled = true;
            this.ComboBoxToningEffect.Location = new System.Drawing.Point(611, 47);
            this.ComboBoxToningEffect.Name = "ComboBoxToningEffect";
            this.ComboBoxToningEffect.Size = new System.Drawing.Size(78, 21);
            this.ComboBoxToningEffect.TabIndex = 28;
            this.ComboBoxToningEffect.SelectedIndexChanged += new System.EventHandler(this.MonoToneCoBox_SelectedIndexChanged);
            // 
            // ImgQualityCoBox
            // 
            this.ImgQualityCoBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ImgQualityCoBox.FormattingEnabled = true;
            this.ImgQualityCoBox.Location = new System.Drawing.Point(231, 99);
            this.ImgQualityCoBox.Name = "ImgQualityCoBox";
            this.ImgQualityCoBox.Size = new System.Drawing.Size(94, 21);
            this.ImgQualityCoBox.TabIndex = 14;
            this.ImgQualityCoBox.SelectedIndexChanged += new System.EventHandler(this.ImgQualityCoBox_SelectedIndexChanged);
            // 
            // numericUpDownSharpness
            // 
            this.numericUpDownSharpness.BackColor = System.Drawing.SystemColors.Window;
            this.numericUpDownSharpness.Location = new System.Drawing.Point(449, 45);
            this.numericUpDownSharpness.Maximum = new decimal(new int[] {
            7,
            0,
            0,
            0});
            this.numericUpDownSharpness.Name = "numericUpDownSharpness";
            this.numericUpDownSharpness.ReadOnly = true;
            this.numericUpDownSharpness.Size = new System.Drawing.Size(44, 20);
            this.numericUpDownSharpness.TabIndex = 20;
            this.numericUpDownSharpness.ValueChanged += new System.EventHandler(this.SharpnessUpDown_ValueChanged);
            // 
            // ComboBoxWB
            // 
            this.ComboBoxWB.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ComboBoxWB.FormattingEnabled = true;
            this.ComboBoxWB.Location = new System.Drawing.Point(9, 100);
            this.ComboBoxWB.Name = "ComboBoxWB";
            this.ComboBoxWB.Size = new System.Drawing.Size(94, 21);
            this.ComboBoxWB.TabIndex = 7;
            this.ComboBoxWB.SelectedIndexChanged += new System.EventHandler(this.WBCoBox_SelectedIndexChanged);
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(500, 46);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(57, 13);
            this.label12.TabIndex = 21;
            this.label12.Text = "Sharpness";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(109, 74);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(74, 16);
            this.label3.TabIndex = 3;
            this.label3.Text = "ISO Speed";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(109, 47);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(122, 16);
            this.label2.TabIndex = 3;
            this.label2.Text = "Shutter Speed (TV)";
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(695, 24);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(42, 13);
            this.label16.TabIndex = 27;
            this.label16.Text = "FilterEff";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(109, 101);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(30, 16);
            this.label5.TabIndex = 3;
            this.label5.Text = "WB";
            // 
            // numericUpDownContrast
            // 
            this.numericUpDownContrast.BackColor = System.Drawing.SystemColors.Window;
            this.numericUpDownContrast.Location = new System.Drawing.Point(449, 72);
            this.numericUpDownContrast.Maximum = new decimal(new int[] {
            4,
            0,
            0,
            0});
            this.numericUpDownContrast.Minimum = new decimal(new int[] {
            4,
            0,
            0,
            -2147483648});
            this.numericUpDownContrast.Name = "numericUpDownContrast";
            this.numericUpDownContrast.ReadOnly = true;
            this.numericUpDownContrast.Size = new System.Drawing.Size(44, 20);
            this.numericUpDownContrast.TabIndex = 18;
            this.numericUpDownContrast.ValueChanged += new System.EventHandler(this.ContrastUpDown_ValueChanged);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.Location = new System.Drawing.Point(331, 47);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(50, 16);
            this.label7.TabIndex = 3;
            this.label7.Text = "Driving";
            // 
            // ComboBoxFilterEffect
            // 
            this.ComboBoxFilterEffect.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ComboBoxFilterEffect.FormattingEnabled = true;
            this.ComboBoxFilterEffect.Location = new System.Drawing.Point(611, 20);
            this.ComboBoxFilterEffect.Name = "ComboBoxFilterEffect";
            this.ComboBoxFilterEffect.Size = new System.Drawing.Size(78, 21);
            this.ComboBoxFilterEffect.TabIndex = 26;
            this.ComboBoxFilterEffect.SelectedIndexChanged += new System.EventHandler(this.FilterEffCoBox_SelectedIndexChanged);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.Location = new System.Drawing.Point(331, 19);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(60, 16);
            this.label8.TabIndex = 3;
            this.label8.Text = "Metering";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(500, 73);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(46, 13);
            this.label11.TabIndex = 19;
            this.label11.Text = "Contrast";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label9.Location = new System.Drawing.Point(331, 73);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(60, 16);
            this.label9.TabIndex = 3;
            this.label9.Text = "AFMode";
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(500, 126);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(56, 13);
            this.label15.TabIndex = 25;
            this.label15.Text = "ColorTone";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(109, 20);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(88, 16);
            this.label1.TabIndex = 3;
            this.label1.Text = "Aperture (AV)";
            // 
            // numericUpDownColorTone
            // 
            this.numericUpDownColorTone.BackColor = System.Drawing.SystemColors.Window;
            this.numericUpDownColorTone.Location = new System.Drawing.Point(449, 125);
            this.numericUpDownColorTone.Maximum = new decimal(new int[] {
            4,
            0,
            0,
            0});
            this.numericUpDownColorTone.Minimum = new decimal(new int[] {
            4,
            0,
            0,
            -2147483648});
            this.numericUpDownColorTone.Name = "numericUpDownColorTone";
            this.numericUpDownColorTone.ReadOnly = true;
            this.numericUpDownColorTone.Size = new System.Drawing.Size(44, 20);
            this.numericUpDownColorTone.TabIndex = 24;
            this.numericUpDownColorTone.ValueChanged += new System.EventHandler(this.ColorToneUpDown_ValueChanged);
            // 
            // ISOCoBox
            // 
            this.ISOCoBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ISOCoBox.FormattingEnabled = true;
            this.ISOCoBox.Location = new System.Drawing.Point(9, 73);
            this.ISOCoBox.Name = "ISOCoBox";
            this.ISOCoBox.Size = new System.Drawing.Size(94, 21);
            this.ISOCoBox.TabIndex = 0;
            this.ISOCoBox.SelectedIndexChanged += new System.EventHandler(this.ISOCoBox_SelectedIndexChanged);
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(500, 100);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(55, 13);
            this.label14.TabIndex = 23;
            this.label14.Text = "Saturation";
            // 
            // comboBoxTV
            // 
            this.comboBoxTV.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxTV.FormattingEnabled = true;
            this.comboBoxTV.Location = new System.Drawing.Point(9, 46);
            this.comboBoxTV.Name = "comboBoxTV";
            this.comboBoxTV.Size = new System.Drawing.Size(94, 21);
            this.comboBoxTV.TabIndex = 0;
            this.comboBoxTV.SelectedIndexChanged += new System.EventHandler(this.TvCoBox_SelectedIndexChanged);
            // 
            // numericUpDownSaturation
            // 
            this.numericUpDownSaturation.BackColor = System.Drawing.SystemColors.Window;
            this.numericUpDownSaturation.Location = new System.Drawing.Point(449, 99);
            this.numericUpDownSaturation.Maximum = new decimal(new int[] {
            4,
            0,
            0,
            0});
            this.numericUpDownSaturation.Minimum = new decimal(new int[] {
            4,
            0,
            0,
            -2147483648});
            this.numericUpDownSaturation.Name = "numericUpDownSaturation";
            this.numericUpDownSaturation.ReadOnly = true;
            this.numericUpDownSaturation.Size = new System.Drawing.Size(44, 20);
            this.numericUpDownSaturation.TabIndex = 22;
            this.numericUpDownSaturation.ValueChanged += new System.EventHandler(this.SaturationUpDown_ValueChanged);
            // 
            // AvCoBox
            // 
            this.AvCoBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.AvCoBox.FormattingEnabled = true;
            this.AvCoBox.Location = new System.Drawing.Point(9, 19);
            this.AvCoBox.Name = "AvCoBox";
            this.AvCoBox.Size = new System.Drawing.Size(94, 21);
            this.AvCoBox.TabIndex = 0;
            this.AvCoBox.SelectedIndexChanged += new System.EventHandler(this.AvCoBox_SelectedIndexChanged);
            // 
            // MeteringCoBox
            // 
            this.MeteringCoBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.MeteringCoBox.FormattingEnabled = true;
            this.MeteringCoBox.Location = new System.Drawing.Point(231, 18);
            this.MeteringCoBox.Name = "MeteringCoBox";
            this.MeteringCoBox.Size = new System.Drawing.Size(94, 21);
            this.MeteringCoBox.TabIndex = 0;
            this.MeteringCoBox.SelectedIndexChanged += new System.EventHandler(this.MeteringCoBox_SelectedIndexChanged);
            // 
            // DriveCoBox
            // 
            this.DriveCoBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.DriveCoBox.FormattingEnabled = true;
            this.DriveCoBox.Location = new System.Drawing.Point(231, 45);
            this.DriveCoBox.Name = "DriveCoBox";
            this.DriveCoBox.Size = new System.Drawing.Size(94, 21);
            this.DriveCoBox.TabIndex = 9;
            this.DriveCoBox.SelectedIndexChanged += new System.EventHandler(this.DriveCoBox_SelectedIndexChanged);
            // 
            // AFModeCoBox
            // 
            this.AFModeCoBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.AFModeCoBox.FormattingEnabled = true;
            this.AFModeCoBox.Location = new System.Drawing.Point(231, 73);
            this.AFModeCoBox.Name = "AFModeCoBox";
            this.AFModeCoBox.Size = new System.Drawing.Size(94, 21);
            this.AFModeCoBox.TabIndex = 10;
            this.AFModeCoBox.SelectedIndexChanged += new System.EventHandler(this.AFModeCoBox_SelectedIndexChanged);
            // 
            // WBSHCoBox
            // 
            this.WBSHCoBox.Location = new System.Drawing.Point(0, 0);
            this.WBSHCoBox.Name = "WBSHCoBox";
            this.WBSHCoBox.Size = new System.Drawing.Size(121, 21);
            this.WBSHCoBox.TabIndex = 0;
            // 
            // SaveFolderBrowser
            // 
            this.SaveFolderBrowser.Description = "Save Images To...";
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(96, 139);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(31, 20);
            this.textBox1.TabIndex = 12;
            // 
            // formMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(756, 592);
            this.Controls.Add(this.SettingsGroupBox);
            this.Controls.Add(this.LiveViewGroupBox);
            this.Controls.Add(this.InitGroupBox);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "formMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Color Calibration Software";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.Load += new System.EventHandler(this.formMain_Load);
            ((System.ComponentModel.ISupportInitialize)(this.LiveViewPicBox)).EndInit();
            this.InitGroupBox.ResumeLayout(false);
            this.InitGroupBox.PerformLayout();
            this.LiveViewGroupBox.ResumeLayout(false);
            this.LiveViewGroupBox.PerformLayout();
            this.SettingsGroupBox.ResumeLayout(false);
            this.SettingsGroupBox.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownSharpness)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownContrast)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownColorTone)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownSaturation)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox LiveViewPicBox;
        private System.Windows.Forms.Button LiveViewButton;
        private System.Windows.Forms.Label FrameRateLabel;
        private System.Windows.Forms.ListBox CameraListBox;
        private System.Windows.Forms.Button SessionButton;
        private System.Windows.Forms.Label SessionLabel;
        private System.Windows.Forms.GroupBox InitGroupBox;
        private System.Windows.Forms.GroupBox LiveViewGroupBox;
        private System.Windows.Forms.GroupBox SettingsGroupBox;
        //private System.Windows.Forms.NumericUpDown BulbUpDo;
        private System.Windows.Forms.ComboBox ISOCoBox;
        private System.Windows.Forms.ComboBox comboBoxTV;
        private System.Windows.Forms.ComboBox AvCoBox;
        private System.Windows.Forms.ComboBox DriveCoBox;
        private System.Windows.Forms.ComboBox MeteringCoBox;
        private System.Windows.Forms.ComboBox AFModeCoBox;
        private System.Windows.Forms.ComboBox WBSHCoBox;
        private System.Windows.Forms.ComboBox ImgQualityCoBox;
        private System.Windows.Forms.TextBox textBoxSavePath;
        private System.Windows.Forms.Button buttonBrowse;
        private System.Windows.Forms.FolderBrowserDialog SaveFolderBrowser;
        private System.Windows.Forms.Button RefreshButton;
        private System.Windows.Forms.ComboBox ComboBoxWB;
        //private System.Windows.Forms.NumericUpDown WBUpDo;

        //private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label Lable12;
        private System.Windows.Forms.Button FocusFar3Button;
        private System.Windows.Forms.Button FocusFar2Button;
        private System.Windows.Forms.Button FocusFar1Button;
        private System.Windows.Forms.Button FocusNear1Button;
        private System.Windows.Forms.Button FocusNear2Button;
        private System.Windows.Forms.Button FocusNear3Button;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.ComboBox ComboBoxPictureStyle;
        private System.Windows.Forms.NumericUpDown numericUpDownContrast;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.NumericUpDown numericUpDownColorTone;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.NumericUpDown numericUpDownSaturation;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.NumericUpDown numericUpDownSharpness;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.ComboBox ComboBoxToningEffect;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.ComboBox ComboBoxFilterEffect;
        private System.Windows.Forms.Button buttonSaturationTest;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.TextBox textBox1;
        //private System.Windows.Forms.GroupBox HDRgroupBox;
        //private System.Windows.Forms.Label picnumlabel;
        //private System.Windows.Forms.NumericUpDown PicnumUpDown;
        //private System.Windows.Forms.TextBox NumSamplesTxtBox;
        //private System.Windows.Forms.Button NumSamplesButton;
        //private System.Windows.Forms.Button MakeHDRButton;
        //private System.Windows.Forms.PictureBox pictureBox1;


    }
}

