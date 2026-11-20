using KeyFlow.Core;

namespace KeyFlow.App;

internal sealed class FloatingForm : Form
{
    private readonly Label _scriptLabel = new();
    private readonly Label _statusLabel = new();
    private readonly Label _coordinatesLabel = new();
    private readonly Button _recordButton = new();
    private readonly Button _stopRecordButton = new();
    private readonly Button _playButton = new();
    private readonly Button _pauseButton = new();
    private readonly Button _stopButton = new();
    private readonly Button _topmostButton = new();
    private readonly NumericUpDown _repeatCount = new();
    private readonly NumericUpDown _repeatInterval = new();
    private readonly CheckBox _clickOnlyCheck = new() { Text = "只录点击", AutoSize = true, Checked = true };
    private readonly CheckBox _fixedClickIntervalCheck = new() { Text = "固定间隔", AutoSize = true };
    private readonly NumericUpDown _fixedClickIntervalMs = new();
    private readonly NumericUpDown _postClickDelayMs = new();
    private readonly NumericUpDown _hoverDelayMs = new();
    private readonly ToolTip _pacingToolTip = new();
    private readonly System.Windows.Forms.Timer _coordinateTimer = new() { Interval = 120 };
    private bool _recordingSessionActive;
    private bool _syncing;
    private bool _dragging;
    private Point _dragCursor;
    private Point _dragForm;
    private bool _allowClose;

    public event Action? RecordToggleRequested;
    public event Action? StopRecordingRequested;
    public event Action? PlayRequested;
    public event Action? PauseRequested;
    public event Action? StopPlaybackRequested;
    public event Action? OpenMainRequested;
    public event Action? HideRequested;
    public event Action<int>? RepeatCountChanged;
    public event Action<int>? RepeatIntervalChanged;
    public event Action<bool>? ClickOnlyRecordingChanged;
    public event Action<bool>? FixedClickIntervalChanged;
    public event Action<int>? FixedClickIntervalMillisecondsChanged;
    public event Action<int>? PostClickDelayMillisecondsChanged;
    public event Action<int>? HoverDelayMillisecondsChanged;
    public event Action<bool>? AlwaysOnTopChanged;

    public FloatingForm(Icon icon)
    {
        Text = "KeyFlow Studio Lite";
        Icon = icon;
        FormBorderStyle = FormBorderStyle.None;
        AutoScaleMode = AutoScaleMode.Dpi;
        AutoScaleDimensions = new SizeF(96F, 96F);
        ShowInTaskbar = false;
        StartPosition = FormStartPosition.Manual;
        ClientSize = new Size(520, 140);
        MinimumSize = new Size(520, 140);
        MaximumSize = new Size(520, 140);
        BackColor = Color.FromArgb(249, 250, 252);
        Font = new Font("Microsoft YaHei UI", 9F);
        BuildUi();

        var workArea = Screen.FromPoint(Cursor.Position).WorkingArea;
        Location = new Point(
            Math.Max(workArea.Left + 8, workArea.Right - Width - 24),
            Math.Max(workArea.Top + 8, workArea.Bottom - Height - 72));

        _coordinateTimer.Tick += (_, _) =>
        {
            var p = Cursor.Position;
            _coordinatesLabel.Text = $"X {p.X}  Y {p.Y}";
        };
        _coordinateTimer.Start();
    }

    private void BuildUi()
    {
        var root = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 4,
            Padding = new Padding(8, 5, 8, 5),
            BackColor = BackColor
        };
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 28));
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 35));
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 32));
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 32));

        var header = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            WrapContents = false,
            FlowDirection = FlowDirection.LeftToRight,
            Margin = Padding.Empty,
            Padding = Padding.Empty
        };
        var brand = new Label
        {
            Text = "KeyFlow",
            AutoSize = true,
            Font = new Font("Microsoft YaHei UI", 10F, FontStyle.Bold),
            ForeColor = Color.FromArgb(28, 50, 82),
            Margin = new Padding(1, 4, 8, 0)
        };
        _scriptLabel.AutoSize = false;
        _scriptLabel.Width = 108;
        _scriptLabel.Height = 26;
        _scriptLabel.AutoEllipsis = true;
        _scriptLabel.Text = "脚本：未选择";
        _scriptLabel.TextAlign = ContentAlignment.MiddleLeft;
        _scriptLabel.ForeColor = Color.FromArgb(70, 83, 101);
        _scriptLabel.Margin = new Padding(0, 1, 4, 0);
        _statusLabel.AutoSize = true;
        _statusLabel.Text = "就绪";
        _statusLabel.ForeColor = Color.FromArgb(20, 128, 104);
        _statusLabel.Font = new Font(Font, FontStyle.Bold);
        _statusLabel.Margin = new Padding(0, 5, 4, 0);

        var openMain = MakeButton("完整界面 ↗", 78);
        openMain.Click += (_, _) => OpenMainRequested?.Invoke();
        _topmostButton.Text = "置顶";
        _topmostButton.AutoSize = false;
        _topmostButton.Width = 48;
        _topmostButton.Height = 27;
        _topmostButton.Margin = new Padding(2, 0, 2, 0);
        _topmostButton.Click += (_, _) => AlwaysOnTopChanged?.Invoke(!TopMost);
        var hide = MakeButton("×", 28);
        hide.Click += (_, _) => HideRequested?.Invoke();

        header.Controls.Add(brand);
        header.Controls.Add(_scriptLabel);
        header.Controls.Add(_statusLabel);
        header.Controls.Add(openMain);
        header.Controls.Add(_topmostButton);
        header.Controls.Add(hide);
        AttachDrag(header);
        AttachDrag(brand);
        AttachDrag(_scriptLabel);
        AttachDrag(_statusLabel);

        var actions = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            WrapContents = false,
            FlowDirection = FlowDirection.LeftToRight,
            Margin = Padding.Empty,
            Padding = Padding.Empty
        };
        _recordButton.Text = "● 开始录制";
        _recordButton.Width = 88;
        _recordButton.Height = 29;
        StyleActionButton(_recordButton, Color.FromArgb(254, 242, 242), Color.FromArgb(153, 27, 27));
        _recordButton.Click += (_, _) => RecordToggleRequested?.Invoke();
        actions.Controls.Add(_recordButton);

        _stopRecordButton.Text = "■ 结束录制";
        _stopRecordButton.Width = 86;
        _stopRecordButton.Height = 29;
        StyleActionButton(_stopRecordButton, Color.White, Color.FromArgb(71, 85, 105));
        _stopRecordButton.Enabled = false;
        _stopRecordButton.Click += (_, _) => StopRecordingRequested?.Invoke();
        actions.Controls.Add(_stopRecordButton);

        _playButton.Text = "▶ 播放";
        _playButton.Width = 66;
        _playButton.Height = 29;
        StyleActionButton(_playButton, Color.FromArgb(236, 253, 245), Color.FromArgb(6, 95, 70));
        _playButton.Click += (_, _) => PlayRequested?.Invoke();
        actions.Controls.Add(_playButton);

        _pauseButton.Text = "Ⅱ 暂停";
        _pauseButton.Width = 66;
        _pauseButton.Height = 29;
        StyleActionButton(_pauseButton, Color.White, Color.FromArgb(51, 65, 85));
        _pauseButton.Enabled = false;
        _pauseButton.Click += (_, _) => PauseRequested?.Invoke();
        actions.Controls.Add(_pauseButton);

        _stopButton.Text = "■ 停止";
        _stopButton.Width = 66;
        _stopButton.Height = 29;
        StyleActionButton(_stopButton, Color.White, Color.FromArgb(51, 65, 85));
        _stopButton.Enabled = false;
        _stopButton.Click += (_, _) => StopPlaybackRequested?.Invoke();
        actions.Controls.Add(_stopButton);

        var settings = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            WrapContents = false,
            FlowDirection = FlowDirection.LeftToRight,
            Margin = Padding.Empty,
            Padding = Padding.Empty
        };
        settings.Controls.Add(new Label { Text = "循环", AutoSize = true, Margin = new Padding(2, 7, 2, 0) });
        _repeatCount.Minimum = 0;
        _repeatCount.Maximum = PlaybackOptions.MaximumRepeatCount;
        _repeatCount.Value = 1;
        _repeatCount.Width = 54;
        _repeatCount.Margin = new Padding(0, 2, 2, 0);
        _repeatCount.ValueChanged += (_, _) =>
        {
            if (!_syncing) RepeatCountChanged?.Invoke((int)_repeatCount.Value);
        };
        settings.Controls.Add(_repeatCount);
        settings.Controls.Add(new Label { Text = "0=∞", AutoSize = true, ForeColor = Color.FromArgb(95, 108, 125), Margin = new Padding(2, 7, 6, 0) });

        settings.Controls.Add(new Label { Text = "间隔", AutoSize = true, Margin = new Padding(0, 7, 3, 0) });
        _repeatInterval.Minimum = 0;
        _repeatInterval.Maximum = PlaybackOptions.MaximumIntervalMilliseconds;
        _repeatInterval.Increment = 100;
        _repeatInterval.Value = 2_000;
        _repeatInterval.Width = 66;
        _repeatInterval.Margin = new Padding(0, 2, 4, 0);
        _repeatInterval.ValueChanged += (_, _) =>
        {
            if (!_syncing) RepeatIntervalChanged?.Invoke((int)_repeatInterval.Value);
        };
        settings.Controls.Add(_repeatInterval);

        _clickOnlyCheck.Margin = new Padding(2, 5, 0, 0);
        _clickOnlyCheck.CheckedChanged += (_, _) =>
        {
            if (!_syncing) ClickOnlyRecordingChanged?.Invoke(_clickOnlyCheck.Checked);
            UpdatePacingControls();
        };
        settings.Controls.Add(_clickOnlyCheck);

        _coordinatesLabel.AutoSize = true;
        _coordinatesLabel.Font = new Font("Consolas", 9F, FontStyle.Bold);
        _coordinatesLabel.ForeColor = Color.FromArgb(44, 82, 130);
        _coordinatesLabel.Margin = new Padding(6, 7, 0, 0);
        settings.Controls.Add(_coordinatesLabel);

        var pacing = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            WrapContents = false,
            FlowDirection = FlowDirection.LeftToRight,
            Margin = Padding.Empty,
            Padding = Padding.Empty
        };
        _fixedClickIntervalCheck.Margin = new Padding(2, 5, 6, 0);
        _fixedClickIntervalCheck.CheckedChanged += (_, _) =>
        {
            UpdatePacingControls();
            if (!_syncing) FixedClickIntervalChanged?.Invoke(_fixedClickIntervalCheck.Checked);
        };
        pacing.Controls.Add(_fixedClickIntervalCheck);

        pacing.Controls.Add(new Label { Text = "总间隔", AutoSize = true, Margin = new Padding(0, 7, 3, 0) });
        ConfigureDelayInput(_fixedClickIntervalMs, 400, value =>
        {
            if (!_syncing) FixedClickIntervalMillisecondsChanged?.Invoke(value);
        });
        pacing.Controls.Add(_fixedClickIntervalMs);

        pacing.Controls.Add(new Label { Text = "点击后", AutoSize = true, Margin = new Padding(4, 7, 3, 0) });
        ConfigureDelayInput(_postClickDelayMs, 50, value =>
        {
            if (!_syncing) PostClickDelayMillisecondsChanged?.Invoke(value);
        });
        pacing.Controls.Add(_postClickDelayMs);

        pacing.Controls.Add(new Label { Text = "悬停", AutoSize = true, Margin = new Padding(4, 7, 3, 0) });
        ConfigureDelayInput(_hoverDelayMs, 150, value =>
        {
            if (!_syncing) HoverDelayMillisecondsChanged?.Invoke(value);
        });
        pacing.Controls.Add(_hoverDelayMs);
        pacing.Controls.Add(new Label { Text = "ms", AutoSize = true, ForeColor = Color.FromArgb(100, 116, 139), Margin = new Padding(2, 7, 0, 0) });

        _pacingToolTip.SetToolTip(_fixedClickIntervalCheck, "仅在“只录点击”模式生效。总间隔包含点击后等待和移动后悬停等待；必要等待超出总间隔时优先保证等待。");
        _pacingToolTip.SetToolTip(_fixedClickIntervalMs, "相邻点击的目标总间隔，单位毫秒。");
        _pacingToolTip.SetToolTip(_postClickDelayMs, "上一次点击完成后、鼠标开始移动前的等待，单位毫秒。");
        _pacingToolTip.SetToolTip(_hoverDelayMs, "鼠标移动到点击坐标后、执行点击前的等待，单位毫秒。");

        root.Controls.Add(header, 0, 0);
        root.Controls.Add(actions, 0, 1);
        root.Controls.Add(settings, 0, 2);
        root.Controls.Add(pacing, 0, 3);
        Controls.Add(root);
        UpdatePacingControls();
    }

    private static Button MakeButton(string text, int width)
    {
        var button = new Button
        {
            Text = text,
            AutoSize = false,
            Width = width,
            Height = 28,
            Margin = new Padding(2, 1, 2, 1),
            Padding = new Padding(4, 0, 4, 0)
        };
        StyleActionButton(button, Color.White, Color.FromArgb(51, 65, 85));
        return button;
    }

    private static void StyleActionButton(Button button, Color backColor, Color foreColor)
    {
        button.FlatStyle = FlatStyle.Flat;
        button.UseVisualStyleBackColor = false;
        button.BackColor = backColor;
        button.ForeColor = foreColor;
        button.Font = new Font("Microsoft YaHei UI", 8.5F, FontStyle.Regular);
        button.Cursor = Cursors.Hand;
        button.FlatAppearance.BorderSize = 1;
        button.FlatAppearance.BorderColor = Color.FromArgb(226, 232, 240);
    }

    private static void ConfigureDelayInput(NumericUpDown input, int defaultValue, Action<int> changed)
    {
        input.Minimum = 0;
        input.Maximum = 60_000;
        input.Increment = 10;
        input.Value = defaultValue;
        input.Width = 60;
        input.Margin = new Padding(0, 2, 2, 0);
        input.TextAlign = HorizontalAlignment.Right;
        input.ValueChanged += (_, _) => changed((int)input.Value);
    }

    private void UpdatePacingControls()
    {
        _fixedClickIntervalCheck.Enabled = !_recordingSessionActive && _clickOnlyCheck.Checked;
        var enabled = !_recordingSessionActive && _clickOnlyCheck.Checked && _fixedClickIntervalCheck.Checked;
        _fixedClickIntervalMs.Enabled = enabled;
        _postClickDelayMs.Enabled = enabled;
        _hoverDelayMs.Enabled = enabled;
    }

    private void AttachDrag(Control control)
    {
        control.MouseDown += (_, e) =>
        {
            if (e.Button != MouseButtons.Left) return;
            _dragging = true;
            _dragCursor = Cursor.Position;
            _dragForm = Location;
        };
        control.MouseMove += (_, _) =>
        {
            if (_dragging)
                Location = new Point(_dragForm.X + Cursor.Position.X - _dragCursor.X,
                    _dragForm.Y + Cursor.Position.Y - _dragCursor.Y);
        };
        control.MouseUp += (_, _) => _dragging = false;
    }

    public void SetSettings(int repeatCount, int repeatInterval, bool clickOnly,
        bool fixedClickInterval, int fixedClickIntervalMs, int postClickDelayMs, int hoverDelayMs,
        bool alwaysOnTop)
    {
        _syncing = true;
        _repeatCount.Value = Math.Clamp(repeatCount, (int)_repeatCount.Minimum, (int)_repeatCount.Maximum);
        _repeatInterval.Value = Math.Clamp(repeatInterval, (int)_repeatInterval.Minimum, (int)_repeatInterval.Maximum);
        _clickOnlyCheck.Checked = clickOnly;
        _fixedClickIntervalCheck.Checked = fixedClickInterval;
        _fixedClickIntervalMs.Value = Math.Clamp(fixedClickIntervalMs, (int)_fixedClickIntervalMs.Minimum, (int)_fixedClickIntervalMs.Maximum);
        _postClickDelayMs.Value = Math.Clamp(postClickDelayMs, (int)_postClickDelayMs.Minimum, (int)_postClickDelayMs.Maximum);
        _hoverDelayMs.Value = Math.Clamp(hoverDelayMs, (int)_hoverDelayMs.Minimum, (int)_hoverDelayMs.Maximum);
        _syncing = false;
        UpdatePacingControls();
        SetAlwaysOnTop(alwaysOnTop);
    }

    public void SetRepeatCount(int value)
    {
        _syncing = true;
        _repeatCount.Value = Math.Clamp(value, (int)_repeatCount.Minimum, (int)_repeatCount.Maximum);
        _syncing = false;
    }

    public void SetRepeatInterval(int value)
    {
        _syncing = true;
        _repeatInterval.Value = Math.Clamp(value, (int)_repeatInterval.Minimum, (int)_repeatInterval.Maximum);
        _syncing = false;
    }

    public void SetClickOnlyRecording(bool value)
    {
        _syncing = true;
        _clickOnlyCheck.Checked = value;
        _syncing = false;
        UpdatePacingControls();
    }

    public void SetAlwaysOnTop(bool value)
    {
        TopMost = value;
        _topmostButton.Text = value ? "置顶 ✓" : "置顶";
    }

    public void UpdateState(string scriptName, bool recordingSessionActive, bool recording,
        bool recordingPaused, bool playbackRunning, bool playbackPaused)
    {
        _scriptLabel.Text = $"脚本：{scriptName}";
        _recordingSessionActive = recordingSessionActive;
        _clickOnlyCheck.Enabled = !recordingSessionActive;
        UpdatePacingControls();
        _recordButton.Text = !recordingSessionActive ? "● 开始录制" : recordingPaused ? "▶ 继续录制" : "Ⅱ 暂停录制";
        _recordButton.Enabled = !playbackRunning;
        _stopRecordButton.Enabled = recordingSessionActive;
        _playButton.Enabled = !recordingSessionActive && !playbackRunning;
        _pauseButton.Enabled = playbackRunning;
        _pauseButton.Text = playbackPaused ? "▶ 继续" : "Ⅱ 暂停";
        _stopButton.Enabled = playbackRunning;

        _statusLabel.Text = recordingSessionActive
            ? recordingPaused ? "录制已暂停" : recording ? "正在录制" : "录制中"
            : playbackRunning ? playbackPaused ? "播放已暂停" : "正在播放"
            : "就绪";
    }

    public void CloseForExit()
    {
        _allowClose = true;
        _coordinateTimer.Stop();
        Close();
    }

    protected override void OnFormClosing(FormClosingEventArgs e)
    {
        if (!_allowClose)
        {
            e.Cancel = true;
            HideRequested?.Invoke();
            return;
        }
        _coordinateTimer.Stop();
        _pacingToolTip.Dispose();
        base.OnFormClosing(e);
    }
}
