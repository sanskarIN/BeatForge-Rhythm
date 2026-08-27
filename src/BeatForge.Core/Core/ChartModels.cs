using System.Text.Json.Serialization;

namespace BeatForge.Core;

public enum Difficulty
{
    Beginner,
    Easy,
    Normal,
    Hard,
    Expert,
    Master
}

public enum NoteType
{
    Tap,
    Hold,
    Swipe,
    DirectionalSwipe,
    Rapid,
    Simultaneous,
    Chain,
    Slide,
    Special
}

public enum Judgement
{
    PerfectPlus,
    Perfect,
    Great,
    Good,
    Miss
}

public sealed record ChartMetadata
{
    public string Id { get; init; } = "untitled-chart";
    public string Title { get; init; } = "Untitled Chart";
    public string Artist { get; init; } = "Unknown Artist";
    public string Author { get; init; } = "Unknown Creator";
    public Difficulty Difficulty { get; init; } = Difficulty.Beginner;
    public int DifficultyRating { get; init; } = 1;
    public string[] Tags { get; init; } = [];
    public string AudioFile { get; init; } = "";
}

public sealed record BpmChange
{
    public double Beat { get; init; }
    public double Bpm { get; init; }
}

public sealed record ChartNote
{
    public string Id { get; init; } = "note-0";
    public double Beat { get; init; }
    public int Lane { get; init; }
    public NoteType Type { get; init; } = NoteType.Tap;
    public double DurationBeats { get; init; }
    public int Direction { get; init; }
    public string? ChainId { get; init; }
}

public sealed class BeatChart
{
    public int SchemaVersion { get; set; } = 1;
    public ChartMetadata Metadata { get; set; } = new();
    public double InitialBpm { get; set; } = 120;
    public double BeatOffsetSeconds { get; set; }
    public int LaneCount { get; set; } = 4;
    public List<BpmChange> BpmChanges { get; set; } = [];
    public List<ChartNote> Notes { get; set; } = [];

    [JsonIgnore]
    public double LastBeat => Notes.Count == 0 ? 0 : Notes.Max(note => note.Beat + note.DurationBeats);
}
