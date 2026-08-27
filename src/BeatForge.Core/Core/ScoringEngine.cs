namespace BeatForge.Core;

public sealed record ScoreEvent(Judgement Judgement, double ErrorMs, int Combo, int Points);

public sealed class ScoreState
{
    private readonly List<ScoreEvent> _events = [];
    public int Score { get; private set; }
    public int Combo { get; private set; }
    public int MaxCombo { get; private set; }
    public int TotalNotes { get; private set; }
    public int PerfectPlus { get; private set; }
    public int Perfect { get; private set; }
    public int Great { get; private set; }
    public int Good { get; private set; }
    public int Misses { get; private set; }
    public IReadOnlyList<ScoreEvent> Events => _events;
    public double Accuracy => TotalNotes == 0 ? 0 : (PerfectPlus * 1d + Perfect * 1d + Great * 0.8d + Good * 0.5d) / TotalNotes * 100;
    public bool FullCombo => TotalNotes > 0 && Misses == 0;
    public bool AllPerfect => TotalNotes > 0 && Misses == 0 && Great == 0 && Good == 0;

    public ScoreEvent Apply(Judgement judgement, double errorMs)
    {
        TotalNotes++;
        var points = judgement switch
        {
            Judgement.PerfectPlus => 1000,
            Judgement.Perfect => 950,
            Judgement.Great => 800,
            Judgement.Good => 500,
            _ => 0
        };
        if (judgement == Judgement.Miss) Combo = 0;
        else
        {
            Combo++;
            MaxCombo = Math.Max(MaxCombo, Combo);
        }
        switch (judgement)
        {
            case Judgement.PerfectPlus: PerfectPlus++; break;
            case Judgement.Perfect: Perfect++; break;
            case Judgement.Great: Great++; break;
            case Judgement.Good: Good++; break;
            case Judgement.Miss: Misses++; break;
        }
        var comboBonus = Math.Min(Math.Max(Combo - 1, 0) * 10, 1000);
        var result = new ScoreEvent(judgement, errorMs, Combo, points + comboBonus);
        Score += result.Points;
        _events.Add(result);
        return result;
    }

    public string Grade => Accuracy switch
    {
        >= 99.5 when AllPerfect => "S+",
        >= 95 => "S",
        >= 90 => "A",
        >= 80 => "B",
        >= 70 => "C",
        _ => "D"
    };
}
