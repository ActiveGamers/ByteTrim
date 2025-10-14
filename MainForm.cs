using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VideoMinimizer
{
    public partial class MainForm : Form
    {
        private List<VideoFile> videoFiles = new List<VideoFile>();
        private CompressionSettings currentSettings = new CompressionSettings();
        private AdvancedSettings advancedSettings = new AdvancedSettings();
        private int currentSortColumn = -1;
        private bool sortAscending = true;
        private CancellationTokenSource cancellationTokenSource;
        private StreamWriter logWriter;
        private string logFilePath;
        private Process currentFFmpegProcess;
        private Queue<VideoFile> compressionQueue = new Queue<VideoFile>();
        private bool isCompressionRunning = false;
        private VideoFile currentlyCompressingFile = null;
        private Dictionary<string, VideoFileInfo> videoInfoCache = new Dictionary<string, VideoFileInfo>();
        private string outputFolder = "";

        public MainForm()
        {
            InitializeComponent();
            InitializeLogging();
            InitializeSettings();
        }

        private void InitializeLogging()
        {
            string logDirectory = Path.Combine(Application.StartupPath, "Logs");
            if (!Directory.Exists(logDirectory))
                Directory.CreateDirectory(logDirectory);

            logFilePath = Path.Combine(logDirectory, $"VideoMinimizer_Log_{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.txt");
            logWriter = new StreamWriter(logFilePath, true);
            LogMessage("Application Started");
        }

        private void LogMessage(string message)
        {
            string logEntry = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} {message}";

            if (logWriter != null && !logWriter.BaseStream.CanWrite)
                return;

            logWriter?.WriteLine(logEntry);
            logWriter?.Flush();
        }

        private void InitializeSettings()
        {
            // Basic settings
            videoBitrateNumericUpDown.Value = 2000;
            audioBitrateComboBox.SelectedIndex = 3; // No Audio
            compressionSpeedComboBox.SelectedIndex = 0; // Ultra Fast

            // Advanced settings - Dynamic CPU Cores
            int processorCount = Environment.ProcessorCount;
            cpuCoresNumericUpDown.Minimum = 1;
            cpuCoresNumericUpDown.Maximum = processorCount;
            cpuCoresNumericUpDown.Value = Math.Max(1, processorCount / 2); // Use half of available cores by default

            videoCodecComboBox.SelectedIndex = 0; // H.264
            gpuAccelerationCheckBox.Checked = true;
            hardwareEncoderComboBox.SelectedIndex = 0; // Auto Detect

            // Initialize output folder to empty (will use input folder by default)
            outputFolderTextBox.Text = "";

            UpdateCurrentSettings();
        }

        private void UpdateCurrentSettings()
        {
            currentSettings.VideoBitrate = (int)videoBitrateNumericUpDown.Value;
            currentSettings.AudioBitrate = GetSelectedAudioBitrate();
            currentSettings.SpeedPreset = GetSpeedPreset();

            advancedSettings.VideoCodec = videoCodecComboBox.Text;
            advancedSettings.CpuCores = (int)cpuCoresNumericUpDown.Value;
            advancedSettings.UseGpuAcceleration = gpuAccelerationCheckBox.Checked;
            advancedSettings.HardwareEncoder = hardwareEncoderComboBox.Text;
        }

        private int GetSelectedAudioBitrate()
        {
            string selected = audioBitrateComboBox.Text;
            return selected switch
            {
                "320 kbps (High Quality)" => 320,
                "128 kbps (Standard)" => 128,
                "96 kbps (Medium)" => 96,
                "0 kbps (No Audio)" => 0,
                _ => 0
            };
        }

        private string GetSpeedPreset()
        {
            string selected = compressionSpeedComboBox.Text;
            return selected switch
            {
                "Ultra Fast (Lowest Quality)" => "ultrafast",
                "Very Fast" => "veryfast",
                "Fast" => "fast",
                "Medium (Balanced)" => "medium",
                "Slow (High Quality)" => "slow",
                "Very Slow (Best Quality)" => "veryslow",
                _ => "ultrafast"
            };
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            LogMessage("Main form loaded");
            LogMessage($"Detected {Environment.ProcessorCount} CPU cores, using {cpuCoresNumericUpDown.Value} cores by default");
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (isCompressionRunning)
            {
                DialogResult result = MessageBox.Show(
                    "Compression is currently running. Are you sure you want to cancel compression and exit?",
                    "Confirm Exit",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning);

                if (result == DialogResult.No)
                {
                    e.Cancel = true;
                    return;
                }
                else
                {
                    cancellationTokenSource?.Cancel();
                    currentFFmpegProcess?.Kill();
                }
            }

            LogMessage("Application closing");
            logWriter?.Close();
            logWriter?.Dispose();
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Alt && e.KeyCode == Keys.Enter)
            {
                if (videosListView.SelectedItems.Count > 0)
                {
                    var selectedItem = videosListView.SelectedItems[0];
                    var videoFile = selectedItem.Tag as VideoFile;
                    if (videoFile != null)
                    {
                        ShowVideoInformation(videoFile);
                    }
                }
                e.Handled = true;
            }
            else if (e.Alt && e.KeyCode == Keys.F4)
            {
                e.Handled = true; // Let the FormClosing event handle this
            }
        }

        private void browseFolderButton_Click(object sender, EventArgs e)
        {
            LogMessage("User clicked Browse Folder button");
            using (FolderBrowserDialog folderDialog = new FolderBrowserDialog())
            {
                if (folderDialog.ShowDialog() == DialogResult.OK)
                {
                    LogMessage($"User selected directory: {folderDialog.SelectedPath}");
                    selectedFolderTextBox.Text = folderDialog.SelectedPath;
                    FindVideoFiles(folderDialog.SelectedPath);
                }
                else
                {
                    LogMessage("User cancelled folder selection");
                }
            }
        }

        private void browseOutputFolderButton_Click(object sender, EventArgs e)
        {
            LogMessage("User clicked Browse Output Folder button");
            using (FolderBrowserDialog folderDialog = new FolderBrowserDialog())
            {
                if (folderDialog.ShowDialog() == DialogResult.OK)
                {
                    outputFolder = folderDialog.SelectedPath;
                    outputFolderTextBox.Text = outputFolder;
                    LogMessage($"User selected output directory: {outputFolder}");
                }
                else
                {
                    LogMessage("User cancelled output folder selection");
                }
            }
        }

        private void selectFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LogMessage("User selected folder from menu");
            browseFolderButton_Click(sender, e);
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void aboutUsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LogMessage("User opened About Us");
            MessageBox.Show("Video Minimizer Professional Edition\n\nAuthor: Mohammad Taha Omidi\nVersion: 1.0.0\n\nhttps://github.com/ActiveGamers/", "About Us",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void compressionSettings_ValueChanged(object sender, EventArgs e)
        {
            LogMessage($"Compression settings changed");
            UpdateCurrentSettings();
            UpdateAllEstimatedSizes();
        }

        private void startCompressionButton_Click(object sender, EventArgs e)
        {
            LogMessage("User clicked Start Compression button");
            if (videoFiles.Count == 0)
            {
                LogMessage("No video files found to compress");
                MessageBox.Show("No video files found!", "Warning",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (isCompressionRunning)
            {
                MessageBox.Show("Compression is already running. Please wait or cancel current operation.", "Compression Running",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            StartBatchCompression();
        }

        private void cancelCompressionButton_Click(object sender, EventArgs e)
        {
            LogMessage("User clicked Cancel Compression button");
            cancellationTokenSource?.Cancel();

            if (currentFFmpegProcess != null && !currentFFmpegProcess.HasExited)
            {
                try
                {
                    currentFFmpegProcess.Kill();
                    LogMessage("FFmpeg process killed");

                    if (currentlyCompressingFile != null)
                    {
                        string outputPath = GetOutputPath(currentlyCompressingFile.FilePath);
                        if (File.Exists(outputPath))
                        {
                            File.Delete(outputPath);
                            LogMessage($"Deleted incomplete file: {outputPath}");
                        }
                        UpdateFileStatus(currentlyCompressingFile, "Canceled Operation");
                        currentlyCompressingFile = null;
                    }
                }
                catch (Exception ex)
                {
                    LogMessage($"Error killing process: {ex.Message}");
                }
            }

            foreach (var videoFile in compressionQueue)
            {
                UpdateFileStatus(videoFile, "Canceled Operation");
            }

            compressionQueue.Clear();
            isCompressionRunning = false;
            SetControlsEnabled(true);
            cancelCompressionButton.Enabled = false;
            startCompressionButton.Enabled = true;
            compressSelectedToolStripMenuItem.Enabled = true;
            LogMessage("Compression cancelled by user");
        }

        private void StartBatchCompression()
        {
            compressionQueue.Clear();
            foreach (var videoFile in videoFiles)
            {
                if (videoFile.Status != "Completed" && videoFile.Status != "Compressing...")
                {
                    compressionQueue.Enqueue(videoFile);
                }
            }

            if (compressionQueue.Count == 0)
            {
                MessageBox.Show("No files to compress. All files are already completed or being processed.", "Info",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            SetControlsEnabled(false);
            cancelCompressionButton.Enabled = true;
            compressSelectedToolStripMenuItem.Enabled = false;
            compressionProgressBar.Value = 0;
            compressionProgressBar.Maximum = compressionQueue.Count;

            cancellationTokenSource = new CancellationTokenSource();
            LogMessage($"Starting batch compression of {compressionQueue.Count} files");

            _ = ProcessCompressionQueueAsync(cancellationTokenSource.Token);
        }

        private void SetControlsEnabled(bool enabled)
        {
            // Basic settings
            browseFolderButton.Enabled = enabled;
            browseOutputFolderButton.Enabled = enabled;
            videoBitrateNumericUpDown.Enabled = enabled;
            audioBitrateComboBox.Enabled = enabled;
            compressionSpeedComboBox.Enabled = enabled;
            startCompressionButton.Enabled = enabled;

            // Advanced settings
            videoCodecComboBox.Enabled = enabled;
            cpuCoresNumericUpDown.Enabled = enabled;
            gpuAccelerationCheckBox.Enabled = enabled;
            hardwareEncoderComboBox.Enabled = enabled;

            // Context menu
            compressSelectedToolStripMenuItem.Enabled = enabled;
            fileInformationToolStripMenuItem.Enabled = enabled;

            // List view selection
            videosListView.Enabled = enabled;
        }

        private async Task ProcessCompressionQueueAsync(CancellationToken cancellationToken)
        {
            isCompressionRunning = true;
            int processedCount = 0;

            while (compressionQueue.Count > 0 && !cancellationToken.IsCancellationRequested)
            {
                var videoFile = compressionQueue.Dequeue();
                currentlyCompressingFile = videoFile;

                string outputPath = GetOutputPath(videoFile.FilePath);

                try
                {
                    UpdateFileStatus(videoFile, "Starting... 0%");
                    LogMessage($"Starting compression of: {videoFile.FileName}");

                    bool success = await CompressSingleVideoAsync(videoFile, outputPath, cancellationToken);

                    if (success && !cancellationToken.IsCancellationRequested)
                    {
                        UpdateFileStatus(videoFile, "Completed 100%");
                        videoFile.CompressedPath = outputPath;
                        LogMessage($"Successfully compressed: {videoFile.FileName}");

                        if (File.Exists(outputPath))
                        {
                            FileInfo compressedInfo = new FileInfo(outputPath);
                            UpdateFileEstimatedSize(videoFile, compressedInfo.Length);
                            LogMessage($"Compressed file size: {FormatFileSize(compressedInfo.Length)}");

                            // Update estimation accuracy
                            UpdateEstimationAccuracy(videoFile, compressedInfo.Length);
                        }
                    }
                    else if (!cancellationToken.IsCancellationRequested)
                    {
                        UpdateFileStatus(videoFile, "Failed");
                        LogMessage($"Failed to compress: {videoFile.FileName}");
                    }
                }
                catch (Exception ex)
                {
                    LogMessage($"Error compressing {videoFile.FileName}: {ex.Message}");
                    UpdateFileStatus(videoFile, $"Error: {ex.Message}");
                }

                processedCount++;
                compressionProgressBar.Value = processedCount;
                currentlyCompressingFile = null;

                if (cancellationToken.IsCancellationRequested)
                    break;

                await Task.Delay(100);
            }

            isCompressionRunning = false;
            SetControlsEnabled(true);
            cancelCompressionButton.Enabled = false;
            compressSelectedToolStripMenuItem.Enabled = true;

            if (cancellationToken.IsCancellationRequested)
            {
                LogMessage("Compression process cancelled by user");
                ShowNotification("Compression Cancelled", "Compression process was cancelled.");
            }
            else
            {
                LogMessage("Compression process completed successfully");
                ShowNotification("Job Done!", "Video compression completed successfully!");
            }
        }

        private async Task<bool> CompressSingleVideoAsync(VideoFile videoFile, string outputPath, CancellationToken cancellationToken)
        {
            string ffmpegPath = FindFFmpeg();
            if (string.IsNullOrEmpty(ffmpegPath))
            {
                LogMessage("FFmpeg not found");
                MessageBox.Show("FFmpeg not found! Please install FFmpeg and add it to PATH.",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            return await Task.Run(() =>
            {
                try
                {
                    string arguments = BuildFFmpegArguments(videoFile.FilePath, outputPath, currentSettings, advancedSettings);
                    LogMessage($"FFmpeg arguments: {arguments}");

                    ProcessStartInfo startInfo = new ProcessStartInfo
                    {
                        FileName = ffmpegPath,
                        Arguments = arguments,
                        UseShellExecute = false,
                        CreateNoWindow = true,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        StandardErrorEncoding = System.Text.Encoding.UTF8
                    };

                    using (Process process = new Process())
                    {
                        currentFFmpegProcess = process;
                        process.StartInfo = startInfo;
                        process.EnableRaisingEvents = true;

                        StringBuilder errorOutput = new StringBuilder();
                        string lastProgress = "0%";

                        process.ErrorDataReceived += (sender, e) =>
                        {
                            if (!string.IsNullOrEmpty(e.Data) && !cancellationToken.IsCancellationRequested)
                            {
                                errorOutput.AppendLine(e.Data);

                                string progress = ExtractProgressFromOutput(e.Data);
                                if (!string.IsNullOrEmpty(progress) && progress != lastProgress)
                                {
                                    lastProgress = progress;
                                    UpdateFileStatus(videoFile, $"Compressing... {progress}");
                                }
                            }
                        };

                        process.Start();
                        process.BeginErrorReadLine();
                        process.BeginOutputReadLine();

                        while (!process.WaitForExit(500))
                        {
                            if (cancellationToken.IsCancellationRequested)
                            {
                                process.Kill();
                                return false;
                            }
                        }

                        bool success = process.ExitCode == 0;
                        currentFFmpegProcess = null;

                        if (!success)
                        {
                            LogMessage($"FFmpeg process failed with exit code: {process.ExitCode}");
                            LogMessage($"FFmpeg error output: {errorOutput}");
                        }

                        return success;
                    }
                }
                catch (Exception ex)
                {
                    LogMessage($"Process error for {videoFile.FileName}: {ex.Message}");
                    currentFFmpegProcess = null;
                    return false;
                }
            }, cancellationToken);
        }

        private string ExtractProgressFromOutput(string output)
        {
            try
            {
                var timeMatch = Regex.Match(output, @"time=(\d+):(\d+):(\d+\.\d+)");
                if (timeMatch.Success)
                {
                    int hours = int.Parse(timeMatch.Groups[1].Value);
                    int minutes = int.Parse(timeMatch.Groups[2].Value);
                    double seconds = double.Parse(timeMatch.Groups[3].Value);

                    double totalSeconds = hours * 3600 + minutes * 60 + seconds;

                    if (videoInfoCache.ContainsKey(currentlyCompressingFile.FilePath))
                    {
                        var videoInfo = videoInfoCache[currentlyCompressingFile.FilePath];
                        if (videoInfo.DurationSeconds > 0)
                        {
                            double estimatedTotalSeconds = videoInfo.DurationSeconds;
                            int progress = (int)Math.Min((totalSeconds / estimatedTotalSeconds) * 100, 99);
                            return $"{progress}%";
                        }
                    }

                    // Fallback estimation
                    int fallbackProgress = (int)Math.Min((totalSeconds / 300) * 100, 99);
                    return $"{fallbackProgress}%";
                }

                return null;
            }
            catch (Exception ex)
            {
                LogMessage($"Error extracting progress: {ex.Message}");
                return null;
            }
        }

        private void videosListView_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            LogMessage($"User clicked on column header: {e.Column}");
            if (e.Column == currentSortColumn)
            {
                sortAscending = !sortAscending;
            }
            else
            {
                currentSortColumn = e.Column;
                sortAscending = true;
            }

            SortListView(e.Column);
        }

        private void videosListView_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                var hitTest = videosListView.HitTest(e.Location);
                if (hitTest.Item != null)
                {
                    videosListView.SelectedItems.Clear();
                    hitTest.Item.Selected = true;
                }
            }
        }

        private void videosListView_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Alt && e.KeyCode == Keys.Enter)
            {
                if (videosListView.SelectedItems.Count > 0)
                {
                    var selectedItem = videosListView.SelectedItems[0];
                    var videoFile = selectedItem.Tag as VideoFile;
                    if (videoFile != null)
                    {
                        ShowVideoInformation(videoFile);
                    }
                }
                e.Handled = true;
            }
        }

        private void compressSelectedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (videosListView.SelectedItems.Count == 0)
            {
                MessageBox.Show("Please select a video file first.", "No Selection",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (isCompressionRunning)
            {
                var result = MessageBox.Show("A compression operation is already running. Do you want to add this file to the queue?",
                    "Compression Running", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.No)
                    return;
            }

            var selectedItem = videosListView.SelectedItems[0];
            var videoFile = selectedItem.Tag as VideoFile;

            if (videoFile != null)
            {
                if (videoFile.Status == "Completed")
                {
                    var overwriteResult = MessageBox.Show("This file has already been compressed. Do you want to compress it again?",
                        "File Already Compressed", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                    if (overwriteResult == DialogResult.No)
                        return;
                }

                compressionQueue.Enqueue(videoFile);
                LogMessage($"Added {videoFile.FileName} to compression queue");

                if (!isCompressionRunning)
                {
                    SetControlsEnabled(false);
                    cancelCompressionButton.Enabled = true;
                    compressSelectedToolStripMenuItem.Enabled = false;
                    compressionProgressBar.Value = 0;
                    compressionProgressBar.Maximum = 1;

                    cancellationTokenSource = new CancellationTokenSource();
                    _ = ProcessCompressionQueueAsync(cancellationTokenSource.Token);
                }

                MessageBox.Show("File added to compression queue.", "Success",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void fileInformationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (videosListView.SelectedItems.Count == 0)
            {
                MessageBox.Show("Please select a video file first.", "No Selection",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var selectedItem = videosListView.SelectedItems[0];
            var videoFile = selectedItem.Tag as VideoFile;

            if (videoFile != null)
            {
                ShowVideoInformation(videoFile);
            }
        }

        private void ShowVideoInformation(VideoFile videoFile)
        {
            try
            {
                VideoFileInfo videoInfo = GetVideoFileInfo(videoFile.FilePath);

                string information = $"📹 Video Information - {videoFile.FileName}\n\n" +
                                   $"⏱️ Video Length: {videoInfo.DurationFormatted}\n" +
                                   $"📐 Resolution: {videoInfo.Width} × {videoInfo.Height} pixels\n" +
                                   $"🎬 Video Bitrate: {videoInfo.VideoBitrateKbps} Kbps\n" +
                                   $"📊 Frame Rate: {videoInfo.FrameRate:F2} FPS\n" +
                                   $"🎵 Audio Bitrate: {videoInfo.AudioBitrateKbps} Kbps\n" +
                                   $"🔊 Audio Sample Rate: {videoInfo.AudioSampleRate} Hz\n" +
                                   $"🎚️ Channel(s): {videoInfo.Channels}\n" +
                                   $"📄 File Type: {videoInfo.FileType}\n" +
                                   $"💾 Original Size: {FormatFileSize(videoFile.FileSize)}";

                MessageBox.Show(information, "Video Information",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error getting video information: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private VideoFileInfo GetVideoFileInfo(string filePath)
        {
            if (videoInfoCache.ContainsKey(filePath))
            {
                return videoInfoCache[filePath];
            }

            VideoFileInfo videoInfo = new VideoFileInfo();

            try
            {
                string ffprobePath = FindFFprobe();
                if (string.IsNullOrEmpty(ffprobePath))
                {
                    FileInfo fileInfo = new FileInfo(filePath);
                    videoInfo.FileType = Path.GetExtension(filePath).ToUpper().TrimStart('.');
                    videoInfoCache[filePath] = videoInfo;
                    return videoInfo;
                }

                string arguments = $"-v quiet -print_format json -show_format -show_streams \"{filePath}\"";

                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    FileName = ffprobePath,
                    Arguments = arguments,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    StandardOutputEncoding = Encoding.UTF8
                };

                using (Process process = new Process())
                {
                    process.StartInfo = startInfo;
                    process.Start();

                    string jsonOutput = process.StandardOutput.ReadToEnd();
                    process.WaitForExit();

                    if (!string.IsNullOrEmpty(jsonOutput))
                    {
                        using JsonDocument doc = JsonDocument.Parse(jsonOutput);
                        JsonElement root = doc.RootElement;

                        if (root.TryGetProperty("format", out JsonElement format))
                        {
                            if (format.TryGetProperty("duration", out JsonElement duration) &&
                                double.TryParse(duration.GetString(), out double totalSeconds))
                            {
                                videoInfo.DurationSeconds = totalSeconds;
                                videoInfo.DurationFormatted = FormatTimeSpan(TimeSpan.FromSeconds(totalSeconds));
                            }

                            if (format.TryGetProperty("bit_rate", out JsonElement bitrate) &&
                                long.TryParse(bitrate.GetString(), out long totalBitrate))
                            {
                                videoInfo.TotalBitrate = totalBitrate;
                            }

                            videoInfo.FileType = Path.GetExtension(filePath).ToUpper().TrimStart('.');
                        }

                        if (root.TryGetProperty("streams", out JsonElement streams))
                        {
                            foreach (JsonElement stream in streams.EnumerateArray())
                            {
                                if (stream.TryGetProperty("codec_type", out JsonElement codecType))
                                {
                                    string type = codecType.GetString();

                                    if (type == "video")
                                    {
                                        if (stream.TryGetProperty("width", out JsonElement width))
                                            videoInfo.Width = width.GetInt32();

                                        if (stream.TryGetProperty("height", out JsonElement height))
                                            videoInfo.Height = height.GetInt32();

                                        if (stream.TryGetProperty("r_frame_rate", out JsonElement frameRate))
                                        {
                                            string frameRateStr = frameRate.GetString();
                                            if (!string.IsNullOrEmpty(frameRateStr))
                                            {
                                                // Parse frame rate correctly
                                                string[] parts = frameRateStr.Split('/');
                                                if (parts.Length == 2 &&
                                                    double.TryParse(parts[0], out double numerator) &&
                                                    double.TryParse(parts[1], out double denominator) &&
                                                    denominator > 0)
                                                {
                                                    videoInfo.FrameRate = Math.Round(numerator / denominator, 2);
                                                }
                                                else
                                                {
                                                    videoInfo.FrameRate = 0;
                                                }
                                            }
                                        }

                                        if (stream.TryGetProperty("bit_rate", out JsonElement videoBitrate))
                                        {
                                            if (long.TryParse(videoBitrate.GetString(), out long vBitrate))
                                            {
                                                videoInfo.VideoBitrate = vBitrate;
                                            }
                                        }
                                    }
                                    else if (type == "audio")
                                    {
                                        if (stream.TryGetProperty("bit_rate", out JsonElement audioBitrate))
                                        {
                                            if (long.TryParse(audioBitrate.GetString(), out long aBitrate))
                                            {
                                                videoInfo.AudioBitrate = aBitrate;
                                            }
                                        }

                                        if (stream.TryGetProperty("sample_rate", out JsonElement sampleRate))
                                        {
                                            videoInfo.AudioSampleRate = sampleRate.GetString();
                                        }

                                        if (stream.TryGetProperty("channels", out JsonElement channels))
                                        {
                                            videoInfo.Channels = channels.GetInt32();
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogMessage($"Error getting video info for {filePath}: {ex.Message}");
            }

            videoInfoCache[filePath] = videoInfo;
            return videoInfo;
        }

        private string FormatTimeSpan(TimeSpan timeSpan)
        {
            return $"{(int)timeSpan.TotalHours:00}:{timeSpan.Minutes:00}:{timeSpan.Seconds:00}";
        }

        private void ShowNotification(string title, string message)
        {
            notifyIcon.BalloonTipTitle = title;
            notifyIcon.BalloonTipText = message;
            notifyIcon.ShowBalloonTip(3000);
        }

        private void SortListView(int columnIndex)
        {
            if (videosListView.Items.Count == 0) return;

            List<ListViewItem> items = videosListView.Items.Cast<ListViewItem>().ToList();

            items.Sort((item1, item2) =>
            {
                string text1 = item1.SubItems[columnIndex].Text;
                string text2 = item2.SubItems[columnIndex].Text;

                if (columnIndex == 2 || columnIndex == 3)
                {
                    long size1 = ParseFileSize(text1);
                    long size2 = ParseFileSize(text2);
                    return sortAscending ? size1.CompareTo(size2) : size2.CompareTo(size1);
                }
                else
                {
                    return sortAscending ?
                        string.Compare(text1, text2, StringComparison.OrdinalIgnoreCase) :
                        string.Compare(text2, text1, StringComparison.OrdinalIgnoreCase);
                }
            });

            videosListView.Items.Clear();
            videosListView.Items.AddRange(items.ToArray());

            foreach (ColumnHeader column in videosListView.Columns)
            {
                column.Text = column.Text.Replace(" ↑", "").Replace(" ↓", "");
            }

            string sortIndicator = sortAscending ? " ↑" : " ↓";
            videosListView.Columns[columnIndex].Text += sortIndicator;
        }

        private long ParseFileSize(string sizeText)
        {
            if (string.IsNullOrEmpty(sizeText) || sizeText == "N/A") return 0;

            try
            {
                string[] parts = sizeText.Split(' ');
                if (parts.Length != 2) return 0;

                decimal size = decimal.Parse(parts[0]);
                string unit = parts[1].ToUpper();

                return unit switch
                {
                    "GB" => (long)(size * 1024 * 1024 * 1024),
                    "MB" => (long)(size * 1024 * 1024),
                    "KB" => (long)(size * 1024),
                    _ => (long)size
                };
            }
            catch (Exception ex)
            {
                LogMessage($"Error parsing file size '{sizeText}': {ex.Message}");
                return 0;
            }
        }

        private void FindVideoFiles(string folderPath)
        {
            LogMessage($"Searching for video files in: {folderPath}");
            videoFiles.Clear();
            videosListView.Items.Clear();
            videoInfoCache.Clear();

            string[] videoExtensions = {
                "*.mp4", "*.avi", "*.mkv", "*.mov", "*.wmv",
                "*.flv", "*.webm", "*.m4v", "*.3gp", "*.mpg", "*.mpeg"
            };

            try
            {
                foreach (string extension in videoExtensions)
                {
                    string[] files = Directory.GetFiles(folderPath, extension,
                        SearchOption.AllDirectories);

                    foreach (string file in files)
                    {
                        FileInfo fileInfo = new FileInfo(file);
                        VideoFile videoFile = new VideoFile
                        {
                            FilePath = file,
                            FileName = fileInfo.Name,
                            FileSize = fileInfo.Length,
                            Status = "Found"
                        };

                        videoFiles.Add(videoFile);

                        VideoFileInfo videoInfo = GetVideoFileInfo(file);
                        long estimatedSize = CalculateAccurateEstimatedSize(videoFile.FileSize, currentSettings, videoInfo);

                        ListViewItem item = new ListViewItem(videoFile.FileName);
                        item.SubItems.Add(videoFile.FilePath);
                        item.SubItems.Add(FormatFileSize(videoFile.FileSize));
                        item.SubItems.Add(FormatFileSize(estimatedSize));
                        item.SubItems.Add(videoFile.Status);
                        item.Tag = videoFile;

                        videosListView.Items.Add(item);
                    }
                }

                UpdateTotalEstimation();
                LogMessage($"Found {videoFiles.Count} video files");
                MessageBox.Show($"Found {videoFiles.Count} video files", "Info",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                LogMessage($"Error searching for files: {ex.Message}");
                MessageBox.Show($"Error searching for files: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private long CalculateAccurateEstimatedSize(long originalSize, CompressionSettings settings, VideoFileInfo videoInfo)
        {
            try
            {
                if (videoInfo != null && videoInfo.DurationSeconds > 0)
                {
                    double durationInSeconds = videoInfo.DurationSeconds;

                    // محاسبه بر اساس Bitrate
                    double videoBitsPerSecond = settings.VideoBitrate * 1000.0;
                    double audioBitsPerSecond = settings.AudioBitrate * 1000.0;
                    double totalBits = (videoBitsPerSecond + audioBitsPerSecond) * durationInSeconds;
                    double estimatedSizeBytes = totalBits / 8;

                    // اعمال فاکتورهای کیفیت
                    double qualityFactor = GetQualityFactor(settings.SpeedPreset);
                    estimatedSizeBytes *= qualityFactor;

                    long estimatedSize = (long)estimatedSizeBytes;

                    // محدوده منطقی
                    long minSize = (long)(originalSize * 0.1);
                    long maxSize = (long)(originalSize * 0.95);

                    return Math.Max(minSize, Math.Min(estimatedSize, maxSize));
                }
                else
                {
                    return CalculateBasicEstimatedSize(originalSize, settings);
                }
            }
            catch (Exception ex)
            {
                LogMessage($"Error in accurate size calculation: {ex.Message}");
                return CalculateBasicEstimatedSize(originalSize, settings);
            }
        }

        private double GetQualityFactor(string speedPreset)
        {
            return speedPreset.ToLower() switch
            {
                "ultrafast" => 1.2,
                "veryfast" => 1.1,
                "fast" => 1.0,
                "medium" => 0.9,
                "slow" => 0.8,
                "veryslow" => 0.7,
                _ => 1.0
            };
        }

        private long CalculateBasicEstimatedSize(long originalSize, CompressionSettings settings)
        {
            double baseRatio = 0.5;

            // Adjust based on video bitrate
            baseRatio *= GetBitrateRatio(settings.VideoBitrate);

            // Adjust based on audio
            if (settings.AudioBitrate == 0)
                baseRatio *= 0.9;

            long estimatedSize = (long)(originalSize * baseRatio);

            // Reasonable bounds
            estimatedSize = Math.Max(estimatedSize, originalSize / 10);
            estimatedSize = Math.Min(estimatedSize, (long)(originalSize * 0.9));

            return estimatedSize;
        }

        private double GetBitrateRatio(int bitrate)
        {
            if (bitrate <= 1000) return 0.3;
            if (bitrate <= 2000) return 0.5;
            if (bitrate <= 3000) return 0.6;
            if (bitrate <= 4000) return 0.7;
            if (bitrate <= 6000) return 0.8;
            if (bitrate <= 8000) return 0.9;
            return 1.0;
        }

        private void UpdateTotalEstimation()
        {
            if (videoFiles.Count == 0)
            {
                estimatedSizeLabel.Text = "Estimated Size: N/A";
                return;
            }

            long totalOriginal = videoFiles.Sum(f => f.FileSize);
            long totalEstimated = 0;

            foreach (var videoFile in videoFiles)
            {
                VideoFileInfo videoInfo = GetVideoFileInfo(videoFile.FilePath);
                totalEstimated += CalculateAccurateEstimatedSize(videoFile.FileSize, currentSettings, videoInfo);
            }

            double reductionPercent = ((double)(totalOriginal - totalEstimated) / totalOriginal) * 100;

            estimatedSizeLabel.Text = $"Estimated Total: {FormatFileSize(totalEstimated)} " +
                                  $"(↓{reductionPercent:F1}% from {FormatFileSize(totalOriginal)})";
        }

        private string BuildFFmpegArguments(string inputPath, string outputPath, CompressionSettings settings, AdvancedSettings advanced)
        {
            var args = new List<string>();

            // Hardware acceleration (must come BEFORE input file)
            if (advanced.UseGpuAcceleration)
            {
                args.Add("-hwaccel auto");
            }

            // Input file
            args.Add($"-i \"{inputPath}\"");

            // Video codec
            string videoCodec = GetVideoCodec(advanced);
            args.Add($"-c:v {videoCodec}");

            // Video settings - Bitrate mode only
            args.Add($"-b:v {settings.VideoBitrate}k");
            args.Add($"-maxrate {settings.VideoBitrate * 1.25}k");
            args.Add($"-bufsize {settings.VideoBitrate * 2}k");

            // Preset
            args.Add($"-preset {settings.SpeedPreset}");

            // Threads
            args.Add($"-threads {advanced.CpuCores}");

            // Audio settings
            if (settings.AudioBitrate == 0)
            {
                args.Add("-an");
            }
            else
            {
                args.Add($"-c:a aac");
                args.Add($"-b:a {settings.AudioBitrate}k");
            }

            // Additional optimizations
            args.Add("-movflags +faststart");
            args.Add("-pix_fmt yuv420p");
            args.Add("-profile:v high");
            args.Add("-level 4.0");

            // Overwrite output file
            args.Add("-y");

            // Output file
            args.Add($"\"{outputPath}\"");

            return string.Join(" ", args);
        }

        private string GetVideoCodec(AdvancedSettings advanced)
        {
            if (advanced.UseGpuAcceleration && advanced.HardwareEncoder != "Auto Detect")
            {
                return advanced.HardwareEncoder switch
                {
                    "NVIDIA NVENC" => "h264_nvenc",
                    "AMD AMF" => "h264_amf",
                    "Intel QuickSync" => "h264_qsv",
                    _ => "libx264"
                };
            }

            return advanced.VideoCodec switch
            {
                "H.265 (HEVC)" => "libx265",
                _ => "libx264"
            };
        }

        private string GetOutputPath(string inputPath)
        {
            string outputDirectory = string.IsNullOrEmpty(outputFolder) ?
                Path.GetDirectoryName(inputPath) : outputFolder;

            string fileName = Path.GetFileNameWithoutExtension(inputPath);
            string extension = Path.GetExtension(inputPath);

            // Try original filename first
            string outputPath = Path.Combine(outputDirectory, $"{fileName}{extension}");

            // If file exists, use compressed suffix
            if (File.Exists(outputPath))
            {
                outputPath = Path.Combine(outputDirectory, $"{fileName}_compressed{extension}");
            }

            return outputPath;
        }

        private string FindFFmpeg()
        {
            try
            {
                string path = Environment.GetEnvironmentVariable("PATH");
                foreach (string pathDir in path.Split(Path.PathSeparator))
                {
                    string ffmpegPath = Path.Combine(pathDir, "ffmpeg.exe");
                    if (File.Exists(ffmpegPath))
                    {
                        LogMessage($"Found FFmpeg in PATH: {ffmpegPath}");
                        return ffmpegPath;
                    }
                }

                string appDir = AppDomain.CurrentDomain.BaseDirectory;
                string localFfmpeg = Path.Combine(appDir, "ffmpeg.exe");
                if (File.Exists(localFfmpeg))
                {
                    LogMessage($"Found FFmpeg in application directory: {localFfmpeg}");
                    return localFfmpeg;
                }

                LogMessage("FFmpeg not found in PATH or application directory");
                return null;
            }
            catch (Exception ex)
            {
                LogMessage($"Error finding FFmpeg: {ex.Message}");
                return null;
            }
        }

        private string FindFFprobe()
        {
            try
            {
                string ffmpegPath = FindFFmpeg();
                if (string.IsNullOrEmpty(ffmpegPath))
                    return null;

                string ffprobePath = Path.Combine(Path.GetDirectoryName(ffmpegPath), "ffprobe.exe");
                if (File.Exists(ffprobePath))
                {
                    LogMessage($"Found FFprobe at: {ffprobePath}");
                    return ffprobePath;
                }

                LogMessage("FFprobe not found");
                return null;
            }
            catch (Exception ex)
            {
                LogMessage($"Error finding FFprobe: {ex.Message}");
                return null;
            }
        }

        private void UpdateFileStatus(VideoFile videoFile, string status)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<VideoFile, string>(UpdateFileStatus), videoFile, status);
                return;
            }

            videoFile.Status = status;
            foreach (ListViewItem item in videosListView.Items)
            {
                if (item.Tag == videoFile)
                {
                    item.SubItems[4].Text = status;
                    break;
                }
            }
            videosListView.Refresh();
        }

        private void UpdateFileEstimatedSize(VideoFile videoFile, long actualSize)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<VideoFile, long>(UpdateFileEstimatedSize), videoFile, actualSize);
                return;
            }

            foreach (ListViewItem item in videosListView.Items)
            {
                if (item.Tag == videoFile)
                {
                    item.SubItems[3].Text = FormatFileSize(actualSize);
                    break;
                }
            }
            videosListView.Refresh();
        }

        private void UpdateEstimationAccuracy(VideoFile videoFile, long actualSize)
        {
            LogMessage($"Estimation accuracy for {videoFile.FileName}: Estimated vs Actual");
        }

        private void UpdateAllEstimatedSizes()
        {
            foreach (ListViewItem item in videosListView.Items)
            {
                if (item.Tag is VideoFile videoFile)
                {
                    VideoFileInfo videoInfo = GetVideoFileInfo(videoFile.FilePath);
                    long estimatedSize = CalculateAccurateEstimatedSize(videoFile.FileSize, currentSettings, videoInfo);
                    item.SubItems[3].Text = FormatFileSize(estimatedSize);
                }
            }
            UpdateTotalEstimation();
        }

        private string FormatFileSize(long bytes)
        {
            string[] suffixes = { "B", "KB", "MB", "GB" };
            int counter = 0;
            decimal number = bytes;

            while (Math.Round(number / 1024) >= 1)
            {
                number /= 1024;
                counter++;
            }

            return $"{number:n1} {suffixes[counter]}";
        }
    }

    public class VideoFile
    {
        public string FilePath { get; set; }
        public string FileName { get; set; }
        public long FileSize { get; set; }
        public string Status { get; set; }
        public string CompressedPath { get; set; }
    }

    public class VideoFileInfo
    {
        public double DurationSeconds { get; set; }
        public string DurationFormatted { get; set; } = "N/A";
        public int Width { get; set; }
        public int Height { get; set; }
        public long VideoBitrate { get; set; }
        public long VideoBitrateKbps => VideoBitrate > 0 ? VideoBitrate / 1000 : 0;
        public double FrameRate { get; set; }
        public long AudioBitrate { get; set; }
        public long AudioBitrateKbps => AudioBitrate > 0 ? AudioBitrate / 1000 : 0;
        public string AudioSampleRate { get; set; } = "N/A";
        public int Channels { get; set; }
        public string FileType { get; set; } = "N/A";
        public long TotalBitrate { get; set; }
    }

    public class CompressionSettings
    {
        public int VideoBitrate { get; set; }
        public int AudioBitrate { get; set; }
        public string SpeedPreset { get; set; }
    }

    public class AdvancedSettings
    {
        public string VideoCodec { get; set; }
        public int CpuCores { get; set; }
        public bool UseGpuAcceleration { get; set; }
        public string HardwareEncoder { get; set; }
    }
}