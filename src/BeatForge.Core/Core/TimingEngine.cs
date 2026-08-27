namespace BeatForge.Core;

public sealed record TimingWindows(
    double PerfectPlusMs = 22,
    double PerfectMs = 45,
    double GreatMs = 90,
    double GoodMs = 150,
    double MissMs = 220)
{
    public Judgement Judge(double errorMs)
    {
        var absolute = Math.Abs(errorMs);
        if (absolute <= PerfectPlusMs) return Judgement.PerfectPlus;
        if (absolute <= PerfectMs) return Judgement.Perfect;
        if (absolute <= GreatMs) return Judgement.Great;
        if (absolute <= GoodMs) return Judgement.Good;
        return Judgement.Miss;
    }
}

public sealed class BeatClock
{
    private readonly BeatChart _chart;
    private readonly List<BpmChange> _changes;

    public BeatClock(BeatChart chart)
    {
        _chart = chart ?? throw new ArgumentNullException(nameof(chart));
        if (chart.InitialBpm <= 0) throw new ArgumentOutOfRangeException(nameof(chart.InitialBpm));
        _changes = chart.BpmChanges.OrderBy(change => change.Beat).ToList();
    }

    public double BeatToSeconds(double beat)
    {
        var cursorBeat = 0d;
        var cursorSeconds = _chart.BeatOffsetSeconds;
        var bpm = _chart.InitialBpm;
        foreach (var change in _changes)
        {
            if (change.Beat <= cursorBeat || change.Beat >= beat) break;
            cursorSeconds += (change.Beat - cursorBeat) * 60d / bpm;
            cursorBeat = change.Beat;
            bpm = change.Bpm;
        }
        return cursorSeconds + (beat - cursorBeat) * 60d / bpm;
    }

    public double SecondsToBeat(double seconds)
    {
        var cursorBeat = 0d;
        var cursorSeconds = _chart.BeatOffsetSeconds;
        var bpm = _chart.InitialBpm;
        foreach (var change in _changes)
        {
            var nextSeconds = cursorSeconds + (change.Beat - cursorBeat) * 60d / bpm;
            if (seconds < nextSeconds) break;
            cursorSeconds = nextSeconds;
            cursorBeat = change.Beat;
            bpm = change.Bpm;
        }
        return cursorBeat + (seconds - cursorSeconds) * bpm / 60d;
    }

    public double NoteTimeSeconds(ChartNote note) => BeatToSeconds(note.Beat);
}

public sealed class NoteScheduler
{
    private readonly IReadOnlyList<ChartNote> _notes;
    private int _nextIndex;

    public NoteScheduler(IEnumerable<ChartNote> notes) => _notes = notes.OrderBy(note => note.Beat).ToArray();

    public IReadOnlyList<ChartNote> TakeWindow(double currentBeat, double lookAheadBeats)
    {
        var result = new List<ChartNote>();
        while (_nextIndex < _notes.Count && _notes[_nextIndex].Beat <= currentBeat + lookAheadBeats)
        {
            if (_notes[_nextIndex].Beat >= currentBeat) result.Add(_notes[_nextIndex]);
            _nextIndex++;
        }
        return result;
    }

    public void Reset() => _nextIndex = 0;
}
