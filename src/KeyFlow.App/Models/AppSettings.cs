namespace KeyFlow.App.Models;

public sealed record AppSettings
{
    public int RepeatCount { get; init; } = 1;
    public int IntervalMilliseconds { get; init; } = 2_000;
    public bool ClickOnlyRecording { get; init; } = true;
    public bool FixedClickInterval { get; init; }
    public int FixedClickIntervalMilliseconds { get; init; } = 400;
    public int PostClickDelayMilliseconds { get; init; } = 50;
    public int HoverDelayMilliseconds { get; init; } = 150;
    public bool AlwaysOnTop { get; init; }
}
