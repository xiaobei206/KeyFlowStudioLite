using System.Diagnostics;
using KeyFlow.App.Models;
using KeyFlow.App.Services;
using Xunit;

namespace KeyFlow.Core.Tests;

public sealed class RecorderTimingTests
{
    [Fact]
    public void FixedInterval_CombinesPostClickAndHoverWaitIntoTargetGap()
    {
        var events = ClickEvents(
            firstDownMs: 0, firstUpMs: 65,
            secondDownMs: 965, secondUpMs: 1_045);

        var script = NormalizeNewLines(GlobalRecorder.GenerateLua(
            events, clickOnly: true, fixedClickInterval: true,
            fixedClickIntervalMs: 400, postClickDelayMs: 50, hoverDelayMs: 150));

        Assert.Contains("MoveTo(100, 200)\nDelay(150)\nLeftClick(65)", script);
        Assert.Contains("LeftClick(65)\nDelay(50)\nMoveTo(300, 400)\nDelay(350)\nLeftClick(80)", script);
        Assert.DoesNotContain("Delay(900)", script);
    }

    [Fact]
    public void NecessaryWaitsMayExceedTheConfiguredTotalInterval()
    {
        var events = ClickEvents(
            firstDownMs: 0, firstUpMs: 65,
            secondDownMs: 900, secondUpMs: 980);

        var script = NormalizeNewLines(GlobalRecorder.GenerateLua(
            events, clickOnly: true, fixedClickInterval: true,
            fixedClickIntervalMs: 100, postClickDelayMs: 60, hoverDelayMs: 80));

        Assert.Contains("LeftClick(65)\nDelay(60)\nMoveTo(300, 400)\nDelay(80)\nLeftClick(80)", script);
    }

    [Fact]
    public void UncheckedFixedInterval_PreservesLegacyRecordedGap()
    {
        var events = ClickEvents(
            firstDownMs: 0, firstUpMs: 65,
            secondDownMs: 965, secondUpMs: 1_045);

        var script = NormalizeNewLines(GlobalRecorder.GenerateLua(events, clickOnly: true));

        Assert.Contains("MoveTo(300, 400)\nDelay(900)\nLeftClick(80)", script);
        Assert.Contains("LeftClick(65)", script);
    }

    private static RecordedInputEvent[] ClickEvents(int firstDownMs, int firstUpMs, int secondDownMs, int secondUpMs) =>
    [
        Event(firstDownMs, RecordedEventKind.MouseDown, 100, 200, "left"),
        Event(firstUpMs, RecordedEventKind.MouseUp, 100, 200, "left"),
        Event(secondDownMs, RecordedEventKind.MouseDown, 300, 400, "left"),
        Event(secondUpMs, RecordedEventKind.MouseUp, 300, 400, "left")
    ];

    private static RecordedInputEvent Event(int ms, RecordedEventKind kind, int x, int y, string name) =>
        new((long)Math.Round(ms * (double)Stopwatch.Frequency / 1_000), kind, x, y, name);

    private static string NormalizeNewLines(string value) => value.Replace("\r\n", "\n");
}
