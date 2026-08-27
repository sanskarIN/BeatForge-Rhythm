namespace BeatForge.Core;

public sealed record AchievementDefinition(string Id, string Name, string Description, int Target, string Category);

public sealed class AchievementCatalog
{
    public IReadOnlyList<AchievementDefinition> All { get; } = Build();

    private static IReadOnlyList<AchievementDefinition> Build()
    {
        var names = new[]
        {
            ("first-beat", "First Beat", "Play your first note.", 1, "milestone"),
            ("perfect-start", "Perfect Start", "Hit ten Perfect or better notes.", 10, "accuracy"),
            ("combo-hunter", "Combo Hunter", "Reach a 50-note combo.", 50, "combo"),
            ("rhythm-master", "Rhythm Master", "Earn an S grade.", 1, "accuracy"),
            ("chart-creator", "Chart Creator", "Save a custom chart.", 1, "editor"),
            ("calibration-expert", "Calibration Expert", "Complete calibration with eight samples.", 8, "settings"),
            ("daily-challenger", "Daily Challenger", "Complete a daily challenge.", 1, "challenge"),
            ("precision-legend", "Precision Legend", "Earn an all-perfect result.", 1, "accuracy")
        };
        var catalog = names.Select(item => new AchievementDefinition(item.Item1, item.Item2, item.Item3, item.Item4, item.Item5)).ToList();
        for (var i = catalog.Count + 1; i <= 52; i++) catalog.Add(new AchievementDefinition($"beatforge-{i:00}", $"Rhythm Milestone {i:00}", $"Complete BeatForge milestone {i:00}.", i, "milestone"));
        return catalog;
    }
}

public sealed class AchievementService
{
    private readonly AchievementCatalog _catalog;
    private readonly HashSet<string> _unlocked;
    public AchievementService(AchievementCatalog catalog, HashSet<string> unlocked) { _catalog = catalog; _unlocked = unlocked; }
    public IReadOnlySet<string> Unlocked => _unlocked;
    public bool Unlock(string id) => _catalog.All.Any(item => item.Id == id) && _unlocked.Add(id);
    public IReadOnlyList<AchievementDefinition> GetAvailable() => _catalog.All;
}
