namespace BeatForge.Core;

public sealed record ReplayInput(double TimeSeconds, int Lane, string Action);
public sealed record ReplayJudgement(double TimeSeconds, string NoteId, Judgement Judgement, double ErrorMs);

public sealed class ReplayData
{
    public int FormatVersion { get; set; } = 1;
    public string ChartId { get; set; } = "";
    public int ChartSchemaVersion { get; set; } = 1;
    public DateTimeOffset RecordedAt { get; set; } = DateTimeOffset.UtcNow;
    public List<ReplayInput> Inputs { get; set; } = [];
    public List<ReplayJudgement> Judgements { get; set; } = [];
}

public sealed class ReplayRecorder
{
    public ReplayData Data { get; }

    public ReplayRecorder(string chartId, int chartSchemaVersion)
    {
        Data = new ReplayData { ChartId = chartId, ChartSchemaVersion = chartSchemaVersion };
    }

    public void RecordInput(double timeSeconds, int lane, string action) => Data.Inputs.Add(new(timeSeconds, lane, action));
    public void RecordJudgement(double timeSeconds, string noteId, Judgement judgement, double errorMs) => Data.Judgements.Add(new(timeSeconds, noteId, judgement, errorMs));
}
