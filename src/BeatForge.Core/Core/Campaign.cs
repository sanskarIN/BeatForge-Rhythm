namespace BeatForge.Core;

public sealed record CampaignObjective(string Id, string Description, int RequiredValue);
public sealed record CampaignStage(string Id, string Title, string ChartId, int UnlockStars, bool IsBoss, CampaignObjective[] Objectives);
public sealed record CampaignWorld(string Id, string Title, string Theme, CampaignStage[] Stages);

public sealed class CampaignProgress
{
    public Dictionary<string, int> StarsByStage { get; set; } = [];
    public HashSet<string> UnlockedStages { get; set; } = ["world-1-stage-1"];
    public int TotalStars => StarsByStage.Values.Sum();
    public bool IsUnlocked(CampaignStage stage) => UnlockedStages.Contains(stage.Id);

    public void RecordStage(string stageId, int stars, CampaignWorld[] worlds)
    {
        StarsByStage[stageId] = Math.Clamp(Math.Max(StarsByStage.GetValueOrDefault(stageId), stars), 0, 3);
        foreach (var stage in worlds.SelectMany(world => world.Stages)) if (TotalStars >= stage.UnlockStars) UnlockedStages.Add(stage.Id);
    }
}

public static class CampaignCatalog
{
    public static CampaignWorld[] CreateDefault() =>
    [
        new("world-1", "First Pulse", "A neon garden of simple rhythms.",
        [
            new("world-1-stage-1", "Sprout", "pulse-garden", 0, false, [new("combo-10", "Reach a 10-note combo.", 10), new("accuracy-80", "Finish with 80% accuracy.", 80)]),
            new("world-1-stage-2", "Canopy", "pulse-garden", 2, false, [new("great-5", "Hit five Great or better notes.", 5)]),
            new("world-1-boss", "The Heartbeat", "pulse-garden", 5, true, [new("no-miss", "Finish without a miss.", 1), new("grade-a", "Earn an A grade.", 1)])
        ])
    ];
}
