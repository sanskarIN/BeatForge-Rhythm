namespace BeatForge.Core;

public interface IAudioPositionSource
{
    bool IsPlaying { get; }
    double PositionSeconds { get; }
}

public sealed class SongClock
{
    private readonly BeatClock _beatClock;
    private readonly IAudioPositionSource? _audio;
    private double _fallbackSeconds;
    private double _pausePosition;
    public double AudioOffsetMs { get; set; }
    public double InputOffsetMs { get; set; }
    public bool IsPaused { get; private set; }
    public double PositionSeconds => IsPaused ? _pausePosition : ReadPosition();
    public double CurrentBeat => _beatClock.SecondsToBeat(PositionSeconds);

    public SongClock(BeatChart chart, IAudioPositionSource? audio = null)
    {
        _beatClock = new BeatClock(chart);
        _audio = audio;
    }

    public double Tick(double deltaSeconds)
    {
        if (!IsPaused && _audio is not { IsPlaying: true }) _fallbackSeconds = Math.Max(0, _fallbackSeconds + deltaSeconds);
        return PositionSeconds;
    }

    public void Pause()
    {
        if (IsPaused) return;
        _pausePosition = PositionSeconds;
        IsPaused = true;
    }

    public void Resume()
    {
        if (!IsPaused) return;
        _fallbackSeconds = _pausePosition;
        IsPaused = false;
    }

    public double CorrectInputTimestamp(double rawInputSeconds) => rawInputSeconds + (AudioOffsetMs + InputOffsetMs) / 1000d;
    public double NoteTimeSeconds(ChartNote note) => _beatClock.NoteTimeSeconds(note);
    public double ErrorMs(ChartNote note, double rawInputSeconds) => (CorrectInputTimestamp(rawInputSeconds) - NoteTimeSeconds(note)) * 1000;

    private double ReadPosition() => _audio is { IsPlaying: true } ? Math.Max(0, _audio.PositionSeconds + AudioOffsetMs / 1000d) : _fallbackSeconds;
}
