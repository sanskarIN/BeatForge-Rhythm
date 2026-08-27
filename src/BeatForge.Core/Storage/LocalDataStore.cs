using System.Text.Json;
using System.Text.Json.Serialization;

namespace BeatForge.Core;

public sealed class LocalPlayerData
{
    public int SchemaVersion { get; set; } = 1;
    public PlayerSettings Settings { get; set; } = new();
    public CalibrationProfile Calibration { get; set; } = new();
    public PlayerStatistics Statistics { get; set; } = new();
    public HashSet<string> UnlockedAchievements { get; set; } = [];
    public HashSet<string> FavoriteChartIds { get; set; } = [];
    public Dictionary<string, int> BestScores { get; set; } = [];
    public Dictionary<string, double> BestAccuracy { get; set; } = [];
}

public sealed class LocalDataStore
{
    private static readonly JsonSerializerOptions Options = new(JsonSerializerDefaults.Web)
    {
        WriteIndented = true,
        Converters = { new JsonStringEnumConverter() }
    };
    private const long MaxBytes = 5 * 1024 * 1024;
    private readonly string _path;

    public LocalDataStore(string path) => _path = Path.GetFullPath(path);

    public LocalPlayerData Load()
    {
        if (!File.Exists(_path)) return new LocalPlayerData();
        var info = new FileInfo(_path);
        if (info.Length > MaxBytes) throw new InvalidDataException("Local player data exceeds the 5 MB safety limit.");
        var data = JsonSerializer.Deserialize<LocalPlayerData>(File.ReadAllText(_path), Options) ?? new LocalPlayerData();
        Migrate(data);
        return data;
    }

    public void Save(LocalPlayerData data)
    {
        if (data is null) throw new ArgumentNullException(nameof(data));
        Migrate(data);
        Directory.CreateDirectory(Path.GetDirectoryName(_path)!);
        var temporaryPath = _path + ".tmp";
        File.WriteAllText(temporaryPath, JsonSerializer.Serialize(data, Options));
        File.Move(temporaryPath, _path, true);
    }

    private static void Migrate(LocalPlayerData data)
    {
        if (data.SchemaVersion == 0) data.SchemaVersion = 1;
        data.Settings ??= new();
        data.Settings.Accessibility ??= new();
        data.Calibration ??= new();
        data.Calibration.History ??= [];
        data.Statistics ??= new();
        data.Statistics.RecentPlays ??= [];
        data.UnlockedAchievements ??= [];
        data.FavoriteChartIds ??= [];
        data.BestScores ??= [];
        data.BestAccuracy ??= [];
    }
}
