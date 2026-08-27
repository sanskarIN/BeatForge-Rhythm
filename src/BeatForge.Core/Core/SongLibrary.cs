namespace BeatForge.Core;

public sealed record SongEntry
{
    public BeatChart Chart { get; init; } = new();
    public bool IsFavorite { get; set; }
    public bool IsCompleted { get; set; }
    public int BestScore { get; set; }
    public double BestAccuracy { get; set; }
    public string BestGrade { get; set; } = "D";
    public DateTimeOffset? LastPlayedAt { get; set; }
}

public sealed record LibraryQuery
{
    public string Search { get; init; } = "";
    public Difficulty? Difficulty { get; init; }
    public bool FavoritesOnly { get; init; }
    public bool CompletedOnly { get; init; }
}

public sealed class SongLibrary
{
    private readonly List<SongEntry> _entries = [];
    public IReadOnlyList<SongEntry> Entries => _entries;
    public void AddOrReplace(SongEntry entry)
    {
        var index = _entries.FindIndex(item => item.Chart.Metadata.Id == entry.Chart.Metadata.Id);
        if (index < 0) _entries.Add(entry); else _entries[index] = entry;
    }

    public IReadOnlyList<SongEntry> Query(LibraryQuery query)
    {
        var search = query.Search.Trim();
        return _entries.Where(entry =>
            (!query.Difficulty.HasValue || entry.Chart.Metadata.Difficulty == query.Difficulty.Value) &&
            (!query.FavoritesOnly || entry.IsFavorite) &&
            (!query.CompletedOnly || entry.IsCompleted) &&
            (search.Length == 0 || Matches(entry, search)))
            .OrderBy(entry => entry.Chart.Metadata.Title, StringComparer.OrdinalIgnoreCase)
            .ToArray();
    }

    public bool ToggleFavorite(string chartId)
    {
        var entry = _entries.FirstOrDefault(item => item.Chart.Metadata.Id == chartId);
        if (entry is null) return false;
        entry.IsFavorite = !entry.IsFavorite;
        return entry.IsFavorite;
    }

    public void RecordResult(string chartId, PlayRecord result, bool completed)
    {
        var entry = _entries.FirstOrDefault(item => item.Chart.Metadata.Id == chartId);
        if (entry is null) return;
        entry.IsCompleted |= completed;
        entry.BestScore = Math.Max(entry.BestScore, result.Score);
        entry.BestAccuracy = Math.Max(entry.BestAccuracy, result.Accuracy);
        if (GradeValue(result.Grade) > GradeValue(entry.BestGrade)) entry.BestGrade = result.Grade;
        entry.LastPlayedAt = result.PlayedAt;
    }

    private static bool Matches(SongEntry entry, string search)
    {
        var comparison = StringComparison.OrdinalIgnoreCase;
        return entry.Chart.Metadata.Title.Contains(search, comparison) || entry.Chart.Metadata.Artist.Contains(search, comparison) || entry.Chart.Metadata.Author.Contains(search, comparison) || entry.Chart.Metadata.Tags.Any(tag => tag.Contains(search, comparison));
    }

    private static int GradeValue(string grade) => grade switch { "S+" => 6, "S" => 5, "A" => 4, "B" => 3, "C" => 2, "D" => 1, _ => 0 };
}
