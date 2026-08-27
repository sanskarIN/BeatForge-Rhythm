namespace BeatForge.Core;

public sealed record ValidationIssue(string Code, string Message, bool IsError = true);

public sealed class ChartValidator
{
    public const int MaxNotes = 100_000;
    public const int MaxLanes = 16;
    public const int MaxTags = 32;

    public IReadOnlyList<ValidationIssue> Validate(BeatChart? chart)
    {
        var issues = new List<ValidationIssue>();
        if (chart is null) return [new("chart.null", "Chart data is missing.")];
        if (chart.SchemaVersion != 1) issues.Add(new("schema.unsupported", $"Unsupported chart schema version {chart.SchemaVersion}."));
        if (string.IsNullOrWhiteSpace(chart.Metadata.Id) || chart.Metadata.Id.Length > 128) issues.Add(new("metadata.id", "Chart ID must be 1–128 characters."));
        if (string.IsNullOrWhiteSpace(chart.Metadata.Title) || chart.Metadata.Title.Length > 200) issues.Add(new("metadata.title", "Chart title must be 1–200 characters."));
        if (chart.InitialBpm is < 20 or > 400) issues.Add(new("timing.bpm", "Initial BPM must be between 20 and 400."));
        if (chart.LaneCount is < 1 or > MaxLanes) issues.Add(new("lanes.count", $"Lane count must be between 1 and {MaxLanes}."));
        if (chart.Notes.Count > MaxNotes) issues.Add(new("notes.count", $"Charts may contain at most {MaxNotes} notes."));
        if (chart.Metadata.Tags.Length > MaxTags) issues.Add(new("metadata.tags", $"Charts may contain at most {MaxTags} tags."));
        var ids = new HashSet<string>(StringComparer.Ordinal);
        foreach (var note in chart.Notes)
        {
            if (string.IsNullOrWhiteSpace(note.Id) || !ids.Add(note.Id)) issues.Add(new("note.id", $"Note IDs must be unique: '{note.Id}'."));
            if (note.Beat < 0 || double.IsNaN(note.Beat) || double.IsInfinity(note.Beat)) issues.Add(new("note.beat", $"Invalid beat for note '{note.Id}'."));
            if (note.Lane < 0 || note.Lane >= chart.LaneCount) issues.Add(new("note.lane", $"Lane {note.Lane} is outside the chart's lane range."));
            if (note.DurationBeats < 0 || double.IsNaN(note.DurationBeats)) issues.Add(new("note.duration", $"Invalid duration for note '{note.Id}'."));
        }
        for (var i = 0; i < chart.BpmChanges.Count; i++)
        {
            var change = chart.BpmChanges[i];
            if (change.Beat < 0 || change.Bpm is < 20 or > 400) issues.Add(new("timing.change", $"Invalid BPM change at index {i}."));
            if (i > 0 && change.Beat <= chart.BpmChanges[i - 1].Beat) issues.Add(new("timing.order", "BPM changes must be strictly ordered by beat."));
        }
        return issues;
    }

    public void ThrowIfInvalid(BeatChart chart)
    {
        var errors = Validate(chart).Where(issue => issue.IsError).ToArray();
        if (errors.Length > 0) throw new InvalidDataException(string.Join(" ", errors.Select(error => error.Message)));
    }
}
