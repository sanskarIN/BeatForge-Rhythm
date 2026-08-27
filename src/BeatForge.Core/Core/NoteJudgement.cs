namespace BeatForge.Core;

public sealed record NoteJudgementResult(ChartNote Note, Judgement Judgement, double ErrorMs, bool Accepted, string Reason);

public sealed class NoteJudger
{
    private readonly TimingWindows _windows;
    public NoteJudger(TimingWindows? windows = null) => _windows = windows ?? new();

    public NoteJudgementResult Evaluate(ChartNote note, int lane, double errorMs, int? direction = null)
    {
        if (note.Lane != lane) return new(note, Judgement.Miss, errorMs, false, "wrong-lane");
        if (note.Type == NoteType.DirectionalSwipe && note.Direction != 0 && direction.HasValue && note.Direction != direction.Value) return new(note, Judgement.Miss, errorMs, false, "wrong-direction");
        var judgement = _windows.Judge(errorMs);
        return new(note, judgement, errorMs, judgement != Judgement.Miss, judgement == Judgement.Miss ? "outside-window" : "accepted");
    }

    public bool IsHoldComplete(ChartNote note, double heldSeconds, double releaseErrorMs)
    {
        if (note.Type != NoteType.Hold && note.Type != NoteType.Slide) return true;
        var requiredSeconds = note.DurationBeats * 60d / 120d;
        return heldSeconds + 0.05 >= requiredSeconds && Math.Abs(releaseErrorMs) <= _windows.GoodMs;
    }
}
