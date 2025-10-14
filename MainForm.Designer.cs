using System.Drawing;
using System.Windows.Forms;

namespace VideoMinimizer
{
    partial class MainForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            mainMenuStrip = new MenuStrip();
            fileToolStripMenuItem = new ToolStripMenuItem();
            selectFolderToolStripMenuItem = new ToolStripMenuItem();
            exitToolStripMenuItem = new ToolStripMenuItem();
            helpToolStripMenuItem = new ToolStripMenuItem();
            aboutUsToolStripMenuItem = new ToolStripMenuItem();
            mainPanel = new Panel();
            advancedSettingsGroupBox = new GroupBox();
            hardwareEncoderComboBox = new ComboBox();
            hardwareEncoderLabel = new Label();
            cpuCoresNumericUpDown = new NumericUpDown();
            cpuCoresLabel = new Label();
            videoCodecComboBox = new ComboBox();
            videoCodecLabel = new Label();
            gpuAccelerationCheckBox = new CheckBox();
            cancelCompressionButton = new Button();
            estimatedSizeLabel = new Label();
            compressionProgressBar = new ProgressBar();
            startCompressionButton = new Button();
            compressionSettingsGroupBox = new GroupBox();
            compressionSpeedComboBox = new ComboBox();
            audioBitrateComboBox = new ComboBox();
            videoBitrateNumericUpDown = new NumericUpDown();
            audioBitrateLabel = new Label();
            videoBitrateLabel = new Label();
            compressionSpeedLabel = new Label();
            browseOutputFolderButton = new Button();
            outputFolderTextBox = new TextBox();
            outputFolderLabel = new Label();
            browseFolderButton = new Button();
            selectedFolderTextBox = new TextBox();
            selectedFolderLabel = new Label();
            videosListView = new ListView();
            fileNameColumnHeader = new ColumnHeader();
            filePathColumnHeader = new ColumnHeader();
            originalSizeColumnHeader = new ColumnHeader();
            estimatedSizeColumnHeader = new ColumnHeader();
            statusColumnHeader = new ColumnHeader();
            videoListContextMenu = new ContextMenuStrip(components);
            compressSelectedToolStripMenuItem = new ToolStripMenuItem();
            fileInformationToolStripMenuItem = new ToolStripMenuItem();
            settingsToolTip = new ToolTip(components);
            notifyIcon = new NotifyIcon(components);
            folderBrowserDialog = new FolderBrowserDialog();
            mainMenuStrip.SuspendLayout();
            mainPanel.SuspendLayout();
            advancedSettingsGroupBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)cpuCoresNumericUpDown).BeginInit();
            compressionSettingsGroupBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)videoBitrateNumericUpDown).BeginInit();
            videoListContextMenu.SuspendLayout();
            SuspendLayout();
            // 
            // mainMenuStrip
            // 
            mainMenuStrip.Items.AddRange(new ToolStripItem[] { fileToolStripMenuItem, helpToolStripMenuItem });
            mainMenuStrip.Location = new Point(0, 0);
            mainMenuStrip.Name = "mainMenuStrip";
            mainMenuStrip.Size = new Size(1080, 24);
            mainMenuStrip.TabIndex = 0;
            mainMenuStrip.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            fileToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { selectFolderToolStripMenuItem, exitToolStripMenuItem });
            fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            fileToolStripMenuItem.Size = new Size(37, 20);
            fileToolStripMenuItem.Text = "&File";
            // 
            // selectFolderToolStripMenuItem
            // 
            selectFolderToolStripMenuItem.Name = "selectFolderToolStripMenuItem";
            selectFolderToolStripMenuItem.Size = new Size(141, 22);
            selectFolderToolStripMenuItem.Text = "&Select Folder";
            selectFolderToolStripMenuItem.Click += selectFolderToolStripMenuItem_Click;
            // 
            // exitToolStripMenuItem
            // 
            exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            exitToolStripMenuItem.Size = new Size(141, 22);
            exitToolStripMenuItem.Text = "E&xit";
            exitToolStripMenuItem.Click += exitToolStripMenuItem_Click;
            // 
            // helpToolStripMenuItem
            // 
            helpToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { aboutUsToolStripMenuItem });
            helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            helpToolStripMenuItem.Size = new Size(44, 20);
            helpToolStripMenuItem.Text = "&Help";
            // 
            // aboutUsToolStripMenuItem
            // 
            aboutUsToolStripMenuItem.Name = "aboutUsToolStripMenuItem";
            aboutUsToolStripMenuItem.Size = new Size(123, 22);
            aboutUsToolStripMenuItem.Text = "&About Us";
            aboutUsToolStripMenuItem.Click += aboutUsToolStripMenuItem_Click;
            // 
            // mainPanel
            // 
            mainPanel.Controls.Add(advancedSettingsGroupBox);
            mainPanel.Controls.Add(cancelCompressionButton);
            mainPanel.Controls.Add(estimatedSizeLabel);
            mainPanel.Controls.Add(compressionProgressBar);
            mainPanel.Controls.Add(startCompressionButton);
            mainPanel.Controls.Add(compressionSettingsGroupBox);
            mainPanel.Controls.Add(browseOutputFolderButton);
            mainPanel.Controls.Add(outputFolderTextBox);
            mainPanel.Controls.Add(outputFolderLabel);
            mainPanel.Controls.Add(browseFolderButton);
            mainPanel.Controls.Add(selectedFolderTextBox);
            mainPanel.Controls.Add(selectedFolderLabel);
            mainPanel.Dock = DockStyle.Top;
            mainPanel.Location = new Point(0, 24);
            mainPanel.Name = "mainPanel";
            mainPanel.Size = new Size(1080, 250);
            mainPanel.TabIndex = 1;
            // 
            // advancedSettingsGroupBox
            // 
            advancedSettingsGroupBox.Controls.Add(hardwareEncoderComboBox);
            advancedSettingsGroupBox.Controls.Add(hardwareEncoderLabel);
            advancedSettingsGroupBox.Controls.Add(cpuCoresNumericUpDown);
            advancedSettingsGroupBox.Controls.Add(cpuCoresLabel);
            advancedSettingsGroupBox.Controls.Add(videoCodecComboBox);
            advancedSettingsGroupBox.Controls.Add(videoCodecLabel);
            advancedSettingsGroupBox.Controls.Add(gpuAccelerationCheckBox);
            advancedSettingsGroupBox.Location = new Point(12, 136);
            advancedSettingsGroupBox.Name = "advancedSettingsGroupBox";
            advancedSettingsGroupBox.Size = new Size(1056, 64);
            advancedSettingsGroupBox.TabIndex = 9;
            advancedSettingsGroupBox.TabStop = false;
            advancedSettingsGroupBox.Text = "Advanced Settings";
            // 
            // hardwareEncoderComboBox
            // 
            hardwareEncoderComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            hardwareEncoderComboBox.FormattingEnabled = true;
            hardwareEncoderComboBox.Items.AddRange(new object[] { "Auto Detect", "H.264 (CPU)", "H.265 (CPU)", "NVIDIA NVENC", "AMD AMF", "Intel QuickSync" });
            hardwareEncoderComboBox.Location = new Point(653, 29);
            hardwareEncoderComboBox.Name = "hardwareEncoderComboBox";
            hardwareEncoderComboBox.Size = new Size(150, 23);
            hardwareEncoderComboBox.TabIndex = 6;
            // 
            // hardwareEncoderLabel
            // 
            hardwareEncoderLabel.AutoSize = true;
            hardwareEncoderLabel.Location = new Point(653, 11);
            hardwareEncoderLabel.Name = "hardwareEncoderLabel";
            hardwareEncoderLabel.Size = new Size(104, 15);
            hardwareEncoderLabel.TabIndex = 5;
            hardwareEncoderLabel.Text = "Hardware Encoder";
            // 
            // cpuCoresNumericUpDown
            // 
            cpuCoresNumericUpDown.Location = new Point(480, 33);
            cpuCoresNumericUpDown.Name = "cpuCoresNumericUpDown";
            cpuCoresNumericUpDown.Size = new Size(80, 23);
            cpuCoresNumericUpDown.TabIndex = 4;
            // 
            // cpuCoresLabel
            // 
            cpuCoresLabel.AutoSize = true;
            cpuCoresLabel.Location = new Point(480, 15);
            cpuCoresLabel.Name = "cpuCoresLabel";
            cpuCoresLabel.Size = new Size(63, 15);
            cpuCoresLabel.TabIndex = 3;
            cpuCoresLabel.Text = "CPU Cores";
            // 
            // videoCodecComboBox
            // 
            videoCodecComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            videoCodecComboBox.FormattingEnabled = true;
            videoCodecComboBox.Items.AddRange(new object[] { "H.264", "H.265 (HEVC)" });
            videoCodecComboBox.Location = new Point(250, 37);
            videoCodecComboBox.Name = "videoCodecComboBox";
            videoCodecComboBox.Size = new Size(120, 23);
            videoCodecComboBox.TabIndex = 2;
            // 
            // videoCodecLabel
            // 
            videoCodecLabel.AutoSize = true;
            videoCodecLabel.Location = new Point(250, 19);
            videoCodecLabel.Name = "videoCodecLabel";
            videoCodecLabel.Size = new Size(74, 15);
            videoCodecLabel.TabIndex = 1;
            videoCodecLabel.Text = "Video Codec";
            // 
            // gpuAccelerationCheckBox
            // 
            gpuAccelerationCheckBox.AutoSize = true;
            gpuAccelerationCheckBox.Checked = true;
            gpuAccelerationCheckBox.CheckState = CheckState.Checked;
            gpuAccelerationCheckBox.Location = new Point(20, 33);
            gpuAccelerationCheckBox.Name = "gpuAccelerationCheckBox";
            gpuAccelerationCheckBox.Size = new Size(118, 19);
            gpuAccelerationCheckBox.TabIndex = 0;
            gpuAccelerationCheckBox.Text = "GPU Acceleration";
            gpuAccelerationCheckBox.UseVisualStyleBackColor = true;
            // 
            // cancelCompressionButton
            // 
            cancelCompressionButton.Enabled = false;
            cancelCompressionButton.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            cancelCompressionButton.Location = new Point(1012, 222);
            cancelCompressionButton.Name = "cancelCompressionButton";
            cancelCompressionButton.Size = new Size(56, 25);
            cancelCompressionButton.TabIndex = 8;
            cancelCompressionButton.Text = "Cancel";
            cancelCompressionButton.UseVisualStyleBackColor = true;
            cancelCompressionButton.Click += cancelCompressionButton_Click;
            // 
            // estimatedSizeLabel
            // 
            estimatedSizeLabel.AutoSize = true;
            estimatedSizeLabel.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            estimatedSizeLabel.ForeColor = Color.Blue;
            estimatedSizeLabel.Location = new Point(696, 222);
            estimatedSizeLabel.Name = "estimatedSizeLabel";
            estimatedSizeLabel.Size = new Size(116, 15);
            estimatedSizeLabel.TabIndex = 7;
            estimatedSizeLabel.Text = "Estimated Size: N/A";
            // 
            // compressionProgressBar
            // 
            compressionProgressBar.Location = new Point(12, 210);
            compressionProgressBar.Name = "compressionProgressBar";
            compressionProgressBar.Size = new Size(678, 30);
            compressionProgressBar.TabIndex = 6;
            // 
            // startCompressionButton
            // 
            startCompressionButton.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            startCompressionButton.Location = new Point(1012, 199);
            startCompressionButton.Name = "startCompressionButton";
            startCompressionButton.Size = new Size(56, 23);
            startCompressionButton.TabIndex = 5;
            startCompressionButton.Text = "Start Compression";
            startCompressionButton.UseVisualStyleBackColor = true;
            startCompressionButton.Click += startCompressionButton_Click;
            // 
            // compressionSettingsGroupBox
            // 
            compressionSettingsGroupBox.Controls.Add(compressionSpeedComboBox);
            compressionSettingsGroupBox.Controls.Add(audioBitrateComboBox);
            compressionSettingsGroupBox.Controls.Add(videoBitrateNumericUpDown);
            compressionSettingsGroupBox.Controls.Add(audioBitrateLabel);
            compressionSettingsGroupBox.Controls.Add(videoBitrateLabel);
            compressionSettingsGroupBox.Controls.Add(compressionSpeedLabel);
            compressionSettingsGroupBox.Location = new Point(12, 70);
            compressionSettingsGroupBox.Name = "compressionSettingsGroupBox";
            compressionSettingsGroupBox.Size = new Size(1056, 60);
            compressionSettingsGroupBox.TabIndex = 4;
            compressionSettingsGroupBox.TabStop = false;
            compressionSettingsGroupBox.Text = "Compression Settings";
            // 
            // compressionSpeedComboBox
            // 
            compressionSpeedComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            compressionSpeedComboBox.FormattingEnabled = true;
            compressionSpeedComboBox.Items.AddRange(new object[] { "Ultra Fast (Lowest Quality)", "Very Fast", "Fast", "Medium (Balanced)", "Slow (High Quality)", "Very Slow (Best Quality)" });
            compressionSpeedComboBox.Location = new Point(715, 31);
            compressionSpeedComboBox.Name = "compressionSpeedComboBox";
            compressionSpeedComboBox.Size = new Size(180, 23);
            compressionSpeedComboBox.TabIndex = 7;
            compressionSpeedComboBox.SelectedIndexChanged += compressionSettings_ValueChanged;
            // 
            // audioBitrateComboBox
            // 
            audioBitrateComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            audioBitrateComboBox.FormattingEnabled = true;
            audioBitrateComboBox.Items.AddRange(new object[] { "320 kbps (High Quality)", "128 kbps (Standard)", "96 kbps (Medium)", "0 kbps (No Audio)" });
            audioBitrateComboBox.Location = new Point(464, 31);
            audioBitrateComboBox.Name = "audioBitrateComboBox";
            audioBitrateComboBox.Size = new Size(180, 23);
            audioBitrateComboBox.TabIndex = 5;
            audioBitrateComboBox.SelectedIndexChanged += compressionSettings_ValueChanged;
            // 
            // videoBitrateNumericUpDown
            // 
            videoBitrateNumericUpDown.Increment = new decimal(new int[] { 100, 0, 0, 0 });
            videoBitrateNumericUpDown.Location = new Point(250, 31);
            videoBitrateNumericUpDown.Maximum = new decimal(new int[] { 50000, 0, 0, 0 });
            videoBitrateNumericUpDown.Minimum = new decimal(new int[] { 500, 0, 0, 0 });
            videoBitrateNumericUpDown.Name = "videoBitrateNumericUpDown";
            videoBitrateNumericUpDown.Size = new Size(100, 23);
            videoBitrateNumericUpDown.TabIndex = 3;
            videoBitrateNumericUpDown.Value = new decimal(new int[] { 4000, 0, 0, 0 });
            videoBitrateNumericUpDown.ValueChanged += compressionSettings_ValueChanged;
            // 
            // audioBitrateLabel
            // 
            audioBitrateLabel.AutoSize = true;
            audioBitrateLabel.Location = new Point(464, 13);
            audioBitrateLabel.Name = "audioBitrateLabel";
            audioBitrateLabel.Size = new Size(112, 15);
            audioBitrateLabel.TabIndex = 2;
            audioBitrateLabel.Text = "Audio Bitrate (kbps)";
            // 
            // videoBitrateLabel
            // 
            videoBitrateLabel.AutoSize = true;
            videoBitrateLabel.Location = new Point(250, 13);
            videoBitrateLabel.Name = "videoBitrateLabel";
            videoBitrateLabel.Size = new Size(110, 15);
            videoBitrateLabel.TabIndex = 0;
            videoBitrateLabel.Text = "Video Bitrate (kbps)";
            settingsToolTip.SetToolTip(videoBitrateLabel, "Target video bitrate - Higher values = Better quality");
            // 
            // compressionSpeedLabel
            // 
            compressionSpeedLabel.AutoSize = true;
            compressionSpeedLabel.Location = new Point(715, 13);
            compressionSpeedLabel.Name = "compressionSpeedLabel";
            compressionSpeedLabel.Size = new Size(112, 15);
            compressionSpeedLabel.TabIndex = 6;
            compressionSpeedLabel.Text = "Compression Speed";
            // 
            // browseOutputFolderButton
            // 
            browseOutputFolderButton.Location = new Point(913, 36);
            browseOutputFolderButton.Name = "browseOutputFolderButton";
            browseOutputFolderButton.Size = new Size(75, 23);
            browseOutputFolderButton.TabIndex = 10;
            browseOutputFolderButton.Text = "Browse";
            browseOutputFolderButton.UseVisualStyleBackColor = true;
            browseOutputFolderButton.Click += browseOutputFolderButton_Click;
            // 
            // outputFolderTextBox
            // 
            outputFolderTextBox.Location = new Point(100, 37);
            outputFolderTextBox.Name = "outputFolderTextBox";
            outputFolderTextBox.ReadOnly = true;
            outputFolderTextBox.Size = new Size(807, 23);
            outputFolderTextBox.TabIndex = 9;
            // 
            // outputFolderLabel
            // 
            outputFolderLabel.AutoSize = true;
            outputFolderLabel.Location = new Point(12, 40);
            outputFolderLabel.Name = "outputFolderLabel";
            outputFolderLabel.Size = new Size(84, 15);
            outputFolderLabel.TabIndex = 8;
            outputFolderLabel.Text = "Output Folder:";
            // 
            // browseFolderButton
            // 
            browseFolderButton.Location = new Point(913, 10);
            browseFolderButton.Name = "browseFolderButton";
            browseFolderButton.Size = new Size(75, 23);
            browseFolderButton.TabIndex = 2;
            browseFolderButton.Text = "Browse";
            browseFolderButton.UseVisualStyleBackColor = true;
            browseFolderButton.Click += browseFolderButton_Click;
            // 
            // selectedFolderTextBox
            // 
            selectedFolderTextBox.Location = new Point(100, 10);
            selectedFolderTextBox.Name = "selectedFolderTextBox";
            selectedFolderTextBox.ReadOnly = true;
            selectedFolderTextBox.Size = new Size(807, 23);
            selectedFolderTextBox.TabIndex = 1;
            // 
            // selectedFolderLabel
            // 
            selectedFolderLabel.AutoSize = true;
            selectedFolderLabel.Location = new Point(12, 13);
            selectedFolderLabel.Name = "selectedFolderLabel";
            selectedFolderLabel.Size = new Size(77, 15);
            selectedFolderLabel.TabIndex = 0;
            selectedFolderLabel.Text = "Select Folder:";
            // 
            // videosListView
            // 
            videosListView.Columns.AddRange(new ColumnHeader[] { fileNameColumnHeader, filePathColumnHeader, originalSizeColumnHeader, estimatedSizeColumnHeader, statusColumnHeader });
            videosListView.ContextMenuStrip = videoListContextMenu;
            videosListView.Dock = DockStyle.Fill;
            videosListView.FullRowSelect = true;
            videosListView.GridLines = true;
            videosListView.Location = new Point(0, 274);
            videosListView.Name = "videosListView";
            videosListView.Size = new Size(1080, 376);
            videosListView.TabIndex = 2;
            videosListView.UseCompatibleStateImageBehavior = false;
            videosListView.View = View.Details;
            videosListView.ColumnClick += videosListView_ColumnClick;
            videosListView.KeyDown += videosListView_KeyDown;
            videosListView.MouseClick += videosListView_MouseClick;
            // 
            // fileNameColumnHeader
            // 
            fileNameColumnHeader.Text = "File Name";
            fileNameColumnHeader.Width = 200;
            // 
            // filePathColumnHeader
            // 
            filePathColumnHeader.Text = "Path";
            filePathColumnHeader.Width = 400;
            // 
            // originalSizeColumnHeader
            // 
            originalSizeColumnHeader.Text = "Original Size";
            originalSizeColumnHeader.Width = 120;
            // 
            // estimatedSizeColumnHeader
            // 
            estimatedSizeColumnHeader.Text = "Estimated Size";
            estimatedSizeColumnHeader.Width = 120;
            // 
            // statusColumnHeader
            // 
            statusColumnHeader.Text = "Status";
            statusColumnHeader.Width = 120;
            // 
            // videoListContextMenu
            // 
            videoListContextMenu.Items.AddRange(new ToolStripItem[] { compressSelectedToolStripMenuItem, fileInformationToolStripMenuItem });
            videoListContextMenu.Name = "videoListContextMenu";
            videoListContextMenu.Size = new Size(175, 48);
            // 
            // compressSelectedToolStripMenuItem
            // 
            compressSelectedToolStripMenuItem.Name = "compressSelectedToolStripMenuItem";
            compressSelectedToolStripMenuItem.Size = new Size(174, 22);
            compressSelectedToolStripMenuItem.Text = "Compress Selected";
            compressSelectedToolStripMenuItem.Click += compressSelectedToolStripMenuItem_Click;
            // 
            // fileInformationToolStripMenuItem
            // 
            fileInformationToolStripMenuItem.Name = "fileInformationToolStripMenuItem";
            fileInformationToolStripMenuItem.Size = new Size(174, 22);
            fileInformationToolStripMenuItem.Text = "Video Information";
            fileInformationToolStripMenuItem.Click += fileInformationToolStripMenuItem_Click;
            // 
            // notifyIcon
            // 
            notifyIcon.Text = "Video Minimizer";
            notifyIcon.Visible = true;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1080, 650);
            Controls.Add(videosListView);
            Controls.Add(mainPanel);
            Controls.Add(mainMenuStrip);
            KeyPreview = true;
            MainMenuStrip = mainMenuStrip;
            Name = "Form1";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Video Minimizer - Mohammad Taha Omidi";
            FormClosing += Form1_FormClosing;
            Load += Form1_Load;
            KeyDown += Form1_KeyDown;
            mainMenuStrip.ResumeLayout(false);
            mainMenuStrip.PerformLayout();
            mainPanel.ResumeLayout(false);
            mainPanel.PerformLayout();
            advancedSettingsGroupBox.ResumeLayout(false);
            advancedSettingsGroupBox.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)cpuCoresNumericUpDown).EndInit();
            compressionSettingsGroupBox.ResumeLayout(false);
            compressionSettingsGroupBox.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)videoBitrateNumericUpDown).EndInit();
            videoListContextMenu.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();

        }

        #endregion

        private MenuStrip mainMenuStrip;
        private ToolStripMenuItem fileToolStripMenuItem;
        private ToolStripMenuItem selectFolderToolStripMenuItem;
        private ToolStripMenuItem exitToolStripMenuItem;
        private Panel mainPanel;
        private Label estimatedSizeLabel;
        private ProgressBar compressionProgressBar;
        private Button startCompressionButton;
        private GroupBox compressionSettingsGroupBox;
        private ComboBox audioBitrateComboBox;
        private NumericUpDown videoBitrateNumericUpDown;
        private Label audioBitrateLabel;
        private Label videoBitrateLabel;
        private Button browseFolderButton;
        private TextBox selectedFolderTextBox;
        private Label selectedFolderLabel;
        private ListView videosListView;
        private ColumnHeader fileNameColumnHeader;
        private ColumnHeader filePathColumnHeader;
        private ColumnHeader originalSizeColumnHeader;
        private ColumnHeader estimatedSizeColumnHeader;
        private ColumnHeader statusColumnHeader;
        private ToolTip settingsToolTip;
        private Button cancelCompressionButton;
        private ComboBox compressionSpeedComboBox;
        private Label compressionSpeedLabel;
        private ContextMenuStrip videoListContextMenu;
        private ToolStripMenuItem compressSelectedToolStripMenuItem;
        private ToolStripMenuItem fileInformationToolStripMenuItem;
        private ToolStripMenuItem helpToolStripMenuItem;
        private ToolStripMenuItem aboutUsToolStripMenuItem;
        private NotifyIcon notifyIcon;
        private GroupBox advancedSettingsGroupBox;
        private ComboBox hardwareEncoderComboBox;
        private Label hardwareEncoderLabel;
        private NumericUpDown cpuCoresNumericUpDown;
        private Label cpuCoresLabel;
        private ComboBox videoCodecComboBox;
        private Label videoCodecLabel;
        private CheckBox gpuAccelerationCheckBox;
        private Button browseOutputFolderButton;
        private TextBox outputFolderTextBox;
        private Label outputFolderLabel;
        private FolderBrowserDialog folderBrowserDialog;
    }
}