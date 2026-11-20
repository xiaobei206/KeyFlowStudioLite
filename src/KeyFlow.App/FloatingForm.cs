using KeyFlow.Core;

namespace KeyFlow.App;

internal sealed class FloatingForm : Form
{
    private static readonly Color WindowBorder = Color.FromArgb(211, 220, 231);
    private static readonly Color WindowSurface = Color.FromArgb(250, 252, 255);
    private static readonly Color SubtleSurface = Color.FromArgb(242, 246, 250);
    private static readonly Color PrimaryText = Color.FromArgb(31, 43, 59);
    private static readonly Color SecondaryText = Color.FromArgb(99, 113, 131);
    private static readonly Color BlueText = Color.FromArgb(37, 82, 142);

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
    private readonly CheckBox _fixedClickIntervalCheck = new() { Text = "固定间隔", AutoSize = false, Width = 82 };
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
        ClientSize = new Size(600, 190);
        MinimumSize = new Size(600, 190);
        MaximumSize = new Size(600, 190);
        BackColor = WindowBorder;
        Padding = new Padding(1);
        Font = new Font("Microsoft YaHei UI", 9F);
        BuildUi();

        var workArea = Screen.FromPoint(Cursor.Position).WorkingArea;
        Location = new Point(
            Math.Max(workArea.Left + 8, workArea.Right - Width - 24),
            Math.Max(workArea.Top + 8, workArea.Bottom - Height - 72));

        _coordinateTimer.Tick += (_, _) =>
        {
            var p = Cursor.Position;
            _coordinatesLabel.Text = $"X {p.X}   Y {p.Y}";
        };
        _coordinateTimer.Start();
    }

    private void BuildUi()
    {
        var root = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 5,
            Padding = new Padding(12, 7, 12, 7),
            BackColor = WindowSurface,
            Margin = Padding.Empty
        };
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 32));
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 38));
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 38));
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 42));
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 20));

        var header = BuildHeader();
        var actions = BuildActions();
        var settingsSurface = BuildSettings();
        var pacing = BuildPacing();
        var footer = BuildFooter();

        root.Controls.Add(header, 0, 0);
        root.Controls.Add(actions, 0, 1);
        root.Controls.Add(settingsSurface, 0, 2);
        root.Controls.Add(pacing, 0, 3);
        root.Controls.Add(footer, 0, 4);
        Controls.Add(root);

        UpdatePacingControls();
    }

    private Control BuildHeader()
    {
        var header = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 4,
            RowCount = 1,
            Margin = Padding.Empty,
            Padding = Padding.Empty,
            BackColor = WindowSurface
        };
        header.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 68));
        header.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        header.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 58));
        header.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 180));

        var brand = new Label
        {
            Text = "KeyFlow",
            Dock = DockStyle.Fill,
            Font = new Font("Microsoft YaHei UI", 10.5F, FontStyle.Bold),
            ForeColor = Color.FromArgb(25, 48, 79),
            TextAlign = ContentAlignment.MiddleLeft,
            Margin = Padding.Empty
        };

        _scriptLabel.Dock = DockStyle.Fill;
        _scriptLabel.AutoSize = false;
        _scriptLabel.AutoEllipsis = true;
        _scriptLabel.Text = "脚本：未选择";
        _scriptLabel.TextAlign = ContentAlignment.MiddleLeft;
        _scriptLabel.ForeColor = PrimaryText;
        _scriptLabel.Margin = new Padding(0, 0, 8, 0);

        _statusLabel.Dock = DockStyle.Fill;
        _statusLabel.AutoSize = false;
        _statusLabel.Text = "就绪";
        _statusLabel.TextAlign = ContentAlignment.MiddleCenter;
        _statusLabel.Font = new Font("Microsoft YaHei UI", 8.25F, FontStyle.Bold);
        _statusLabel.ForeColor = Color.FromArgb(77, 91, 108);
        _statusLabel.BackColor = Color.FromArgb(237, 242, 247);
        _statusLabel.Margin = new Padding(4, 3, 4, 3);

        var windowActions = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            WrapContents = false,
            FlowDirection = FlowDirection.LeftToRight,
            Margin = Padding.Empty,
            Padding = Padding.Empty,
            BackColor = WindowSurface
        };

        var openMain = MakeButton("完整界面", 78, 26);
        openMain.Font = new Font("Microsoft YaHei UI", 8F);
        openMain.Margin = new Padding(1, 2, 1, 2);
        openMain.Click += (_, _) => OpenMainRequested?.Invoke();

        _topmostButton.Text = "置顶";
        _topmostButton.AutoSize = false;
        _topmostButton.Width = 58;
        _topmostButton.Height = 26;
        _topmostButton.Margin = new Padding(1, 2, 1, 2);
        _topmostButton.Font = new Font("Microsoft YaHei UI", 8F);
        StyleActionButton(_topmostButton, Color.White, SecondaryText);
        _topmostButton.Click += (_, _) => AlwaysOnTopChanged?.Invoke(!TopMost);

        var hide = MakeButton("×", 28, 26);
        hide.Font = new Font("Microsoft YaHei UI", 10F);
        hide.Margin = new Padding(1, 2, 1, 2);
        hide.Click += (_, _) => HideRequested?.Invoke();

        windowActions.Controls.Add(openMain);
        windowActions.Controls.Add(_topmostButton);
        windowActions.Controls.Add(hide);

        header.Controls.Add(brand, 0, 0);
        header.Controls.Add(_scriptLabel, 1, 0);
        header.Controls.Add(_statusLabel, 2, 0);
        header.Controls.Add(windowActions, 3, 0);

        AttachDrag(header);
        AttachDrag(brand);
        AttachDrag(_scriptLabel);
        AttachDrag(_statusLabel);
        return header;
    }

    private Control BuildActions()
    {
        var layout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            RowCount = 1,
            Margin = Padding.Empty,
            Padding = Padding.Empty,
            BackColor = WindowSurface
        };
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 154));

        var buttons = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            WrapContents = false,
            FlowDirection = FlowDirection.LeftToRight,
            Margin = Padding.Empty,
            Padding = Padding.Empty,
            BackColor = WindowSurface
        };

        _recordButton.Text = "● 开始录制";
        _recordButton.Width = 98;
        _recordButton.Height = 32;
        _recordButton.Margin = new Padding(2, 3, 2, 3);
        StyleActionButton(_recordButton, Color.FromArgb(255, 241, 240), Color.FromArgb(174, 46, 38), Color.FromArgb(246, 211, 207));
        _recordButton.Click += (_, _) => RecordToggleRequested?.Invoke();
        buttons.Controls.Add(_recordButton);

        _stopRecordButton.Text = "■ 结束录制";
        _stopRecordButton.Width = 88;
        _stopRecordButton.Height = 32;
        _stopRecordButton.Margin = new Padding(2, 3, 2, 3);
        StyleActionButton(_stopRecordButton, Color.White, Color.FromArgb(73, 87, 105));
        _stopRecordButton.Enabled = false;
        _stopRecordButton.Click += (_, _) => StopRecordingRequested?.Invoke();
        buttons.Controls.Add(_stopRecordButton);

        _playButton.Text = "▶ 播放";
        _playButton.Width = 74;
        _playButton.Height = 32;
        _playButton.Margin = new Padding(2, 3, 2, 3);
        StyleActionButton(_playButton, Color.FromArgb(22, 123, 96), Color.White, Color.FromArgb(22, 123, 96));
        _playButton.Click += (_, _) => PlayRequested?.Invoke();
        buttons.Controls.Add(_playButton);

        _pauseButton.Text = "Ⅱ 暂停";
        _pauseButton.Width = 74;
        _pauseButton.Height = 32;
        _pauseButton.Margin = new Padding(2, 3, 2, 3);
        StyleActionButton(_pauseButton, Color.White, Color.FromArgb(51, 65, 85));
        _pauseButton.Enabled = false;
        _pauseButton.Click += (_, _) => PauseRequested?.Invoke();
        buttons.Controls.Add(_pauseButton);

        _stopButton.Text = "■ 停止";
        _stopButton.Width = 64;
        _stopButton.Height = 32;
        _stopButton.Margin = new Padding(2, 3, 2, 3);
        StyleActionButton(_stopButton, Color.White, Color.FromArgb(51, 65, 85));
        _stopButton.Enabled = false;
        _stopButton.Click += (_, _) => StopPlaybackRequested?.Invoke();
        buttons.Controls.Add(_stopButton);

        var shortcutHint = new Label
        {
            Dock = DockStyle.Fill,
            AutoSize = false,
            Text = "F10 播放  ·  F11 暂停  ·  F12 中止",
            TextAlign = ContentAlignment.MiddleRight,
            Font = new Font("Microsoft YaHei UI", 8F),
            ForeColor = SecondaryText,
            Margin = new Padding(0, 0, 1, 0)
        };

        layout.Controls.Add(buttons, 0, 0);
        layout.Controls.Add(shortcutHint, 1, 0);
        return layout;
    }

    private Control BuildSettings()
    {
        var surface = new Panel
        {
            Dock = DockStyle.Fill,
            Margin = new Padding(0, 1, 0, 1),
            Padding = new Padding(6, 0, 2, 0),
            BackColor = SubtleSurface
        };
        var settings = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            WrapContents = false,
            FlowDirection = FlowDirection.LeftToRight,
            Margin = Padding.Empty,
            Padding = Padding.Empty,
            BackColor = SubtleSurface
        };

        settings.Controls.Add(MakeInlineLabel("循环", 32));
        _repeatCount.Minimum = 0;
        _repeatCount.Maximum = PlaybackOptions.MaximumRepeatCount;
        _repeatCount.Value = 1;
        _repeatCount.Width = 54;
        _repeatCount.Margin = new Padding(0, 6, 4, 0);
        _repeatCount.TextAlign = HorizontalAlignment.Center;
        _repeatCount.ValueChanged += (_, _) =>
        {
            if (!_syncing) RepeatCountChanged?.Invoke((int)_repeatCount.Value);
        };
        settings.Controls.Add(_repeatCount);

        settings.Controls.Add(MakeInlineLabel("0=无限", 52, SecondaryText));
        settings.Controls.Add(MakeInlineLabel("轮间隔", 48));
        _repeatInterval.Minimum = 0;
        _repeatInterval.Maximum = PlaybackOptions.MaximumIntervalMilliseconds;
        _repeatInterval.Increment = 100;
        _repeatInterval.Value = 2_000;
        _repeatInterval.Width = 66;
        _repeatInterval.Margin = new Padding(0, 6, 2, 0);
        _repeatInterval.TextAlign = HorizontalAlignment.Center;
        _repeatInterval.ValueChanged += (_, _) =>
        {
            if (!_syncing) RepeatIntervalChanged?.Invoke((int)_repeatInterval.Value);
        };
        settings.Controls.Add(_repeatInterval);
        settings.Controls.Add(MakeInlineLabel("ms", 18, SecondaryText));

        _clickOnlyCheck.AutoSize = false;
        _clickOnlyCheck.Width = 82;
        _clickOnlyCheck.Height = 30;
        _clickOnlyCheck.Margin = new Padding(5, 3, 2, 3);
        _clickOnlyCheck.ForeColor = PrimaryText;
        settings.Controls.Add(_clickOnlyCheck);
        _clickOnlyCheck.CheckedChanged += (_, _) =>
        {
            if (!_syncing) ClickOnlyRecordingChanged?.Invoke(_clickOnlyCheck.Checked);
            UpdatePacingControls();
        };

        _coordinatesLabel.AutoSize = false;
        _coordinatesLabel.Width = 132;
        _coordinatesLabel.Height = 28;
        _coordinatesLabel.Text = "X 00000   Y 00000";
        _coordinatesLabel.TextAlign = ContentAlignment.MiddleCenter;
        _coordinatesLabel.Font = new Font("Consolas", 9F, FontStyle.Bold);
        _coordinatesLabel.ForeColor = BlueText;
        _coordinatesLabel.BackColor = Color.FromArgb(232, 241, 253);
        _coordinatesLabel.Margin = new Padding(8, 3, 0, 3);
        settings.Controls.Add(_coordinatesLabel);

        surface.Controls.Add(settings);
        return surface;
    }

    private Control BuildPacing()
    {
        var pacing = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            WrapContents = false,
            FlowDirection = FlowDirection.LeftToRight,
            Margin = Padding.Empty,
            Padding = Padding.Empty,
            BackColor = WindowSurface
        };

        _fixedClickIntervalCheck.Height = 30;
        _fixedClickIntervalCheck.Margin = new Padding(2, 5, 4, 0);
        _fixedClickIntervalCheck.ForeColor = PrimaryText;
        _fixedClickIntervalCheck.CheckedChanged += (_, _) =>
        {
            UpdatePacingControls();
            if (!_syncing) FixedClickIntervalChanged?.Invoke(_fixedClickIntervalCheck.Checked);
        };
        pacing.Controls.Add(_fixedClickIntervalCheck);

        pacing.Controls.Add(new Panel
        {
            Width = 1,
            Height = 22,
            BackColor = WindowBorder,
            Margin = new Padding(2, 9, 8, 0)
        });

        pacing.Controls.Add(MakeInlineLabel("总间隔", 44));
        ConfigureDelayInput(_fixedClickIntervalMs, 400, value =>
        {
            if (!_syncing) FixedClickIntervalMillisecondsChanged?.Invoke(value);
        });
        pacing.Controls.Add(_fixedClickIntervalMs);
        pacing.Controls.Add(MakeInlineLabel("ms", 17, SecondaryText));

        pacing.Controls.Add(MakeInlineLabel("点击后", 48));
        ConfigureDelayInput(_postClickDelayMs, 50, value =>
        {
            if (!_syncing) PostClickDelayMillisecondsChanged?.Invoke(value);
        });
        pacing.Controls.Add(_postClickDelayMs);
        pacing.Controls.Add(MakeInlineLabel("ms", 17, SecondaryText));

        pacing.Controls.Add(MakeInlineLabel("悬停", 38));
        ConfigureDelayInput(_hoverDelayMs, 150, value =>
        {
            if (!_syncing) HoverDelayMillisecondsChanged?.Invoke(value);
        });
        pacing.Controls.Add(_hoverDelayMs);
        pacing.Controls.Add(MakeInlineLabel("ms", 17, SecondaryText));

        _pacingToolTip.SetToolTip(_fixedClickIntervalCheck, "仅在“只录点击”模式生效。总间隔包含点击后等待和移动后悬停等待；必要等待超出总间隔时优先保证等待。");
        _pacingToolTip.SetToolTip(_fixedClickIntervalMs, "相邻点击的目标总间隔，单位毫秒。");
        _pacingToolTip.SetToolTip(_postClickDelayMs, "上一次点击完成后、鼠标开始移动前的等待，单位毫秒。");
        _pacingToolTip.SetToolTip(_hoverDelayMs, "鼠标移动到点击坐标后、执行点击前的等待，单位毫秒。");

        return pacing;
    }

    private Control BuildFooter()
    {
        var footer = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            RowCount = 1,
            Margin = Padding.Empty,
            Padding = Padding.Empty,
            BackColor = WindowSurface
        };
        footer.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        footer.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 190));

        footer.Controls.Add(new Label
        {
            Dock = DockStyle.Fill,
            AutoSize = false,
            Text = "固定间隔仅在“只录点击”模式下生效",
            TextAlign = ContentAlignment.MiddleLeft,
            Font = new Font("Microsoft YaHei UI", 8F),
            ForeColor = SecondaryText,
            Margin = Padding.Empty
        }, 0, 0);
        footer.Controls.Add(new Label
        {
            Dock = DockStyle.Fill,
            AutoSize = false,
            Text = "F10 播放 · F11 暂停 · F12 中止",
            TextAlign = ContentAlignment.MiddleRight,
            Font = new Font("Microsoft YaHei UI", 8F),
            ForeColor = SecondaryText,
            Margin = Padding.Empty
        }, 1, 0);
        return footer;
    }

    private static Label MakeInlineLabel(string text, int width, Color? color = null)
    {
        return new Label
        {
            Text = text,
            AutoSize = false,
            Width = width,
            Height = 30,
            TextAlign = ContentAlignment.MiddleLeft,
            ForeColor = color ?? PrimaryText,
            Margin = new Padding(1, 3, 2, 3)
        };
    }

    private static Button MakeButton(string text, int width, int height = 28)
    {
        var button = new Button
        {
            Text = text,
            AutoSize = false,
            Width = width,
            Height = height,
            Margin = new Padding(2, 1, 2, 1),
            Padding = new Padding(4, 0, 4, 0)
        };
        StyleActionButton(button, Color.White, Color.FromArgb(51, 65, 85));
        return button;
    }

    private static void StyleActionButton(Button button, Color backColor, Color foreColor, Color? borderColor = null)
    {
        button.FlatStyle = FlatStyle.Flat;
        button.UseVisualStyleBackColor = false;
        button.BackColor = backColor;
        button.ForeColor = foreColor;
        button.Font = new Font("Microsoft YaHei UI", 8.5F, FontStyle.Regular);
        button.Cursor = Cursors.Hand;
        button.FlatAppearance.BorderSize = 1;
        button.FlatAppearance.BorderColor = borderColor ?? WindowBorder;
    }

    private static void ConfigureDelayInput(NumericUpDown input, int defaultValue, Action<int> changed)
    {
        input.Minimum = 0;
        input.Maximum = 60_000;
        input.Increment = 10;
        input.Value = defaultValue;
        input.Width = 54;
        input.Margin = new Padding(0, 8, 2, 0);
        input.TextAlign = HorizontalAlignment.Center;
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

        if (recordingSessionActive)
        {
            SetStatus(recordingPaused ? "录制暂停" : recording ? "正在录制" : "录制中",
                Color.FromArgb(255, 235, 233), Color.FromArgb(170, 45, 37));
        }
        else if (playbackRunning)
        {
            SetStatus(playbackPaused ? "播放暂停" : "正在播放",
                playbackPaused ? Color.FromArgb(255, 246, 220) : Color.FromArgb(222, 248, 235),
                playbackPaused ? Color.FromArgb(139, 91, 10) : Color.FromArgb(20, 105, 76));
        }
        else
        {
            SetStatus("就绪", Color.FromArgb(237, 242, 247), Color.FromArgb(77, 91, 108));
        }
    }

    private void SetStatus(string text, Color backColor, Color foreColor)
    {
        _statusLabel.Text = text;
        _statusLabel.BackColor = backColor;
        _statusLabel.ForeColor = foreColor;
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
