namespace BeatForge.Core;

public sealed class SafeImport
{
    public const long MaxFileBytes = 10 * 1024 * 1024;
    private static readonly HashSet<string> AllowedExtensions = new(StringComparer.OrdinalIgnoreCase) { ".json", ".beatforge" };

    public BeatChart LoadChart(string path)
    {
        var fullPath = ValidatePath(path);
        var info = new FileInfo(fullPath);
        if (info.Length > MaxFileBytes) throw new InvalidDataException("The imported chart exceeds the 10 MB safety limit.");
        return ChartJson.Deserialize(File.ReadAllText(fullPath));
    }

    public void SaveChart(string path, BeatChart chart)
    {
        var fullPath = ValidatePath(path);
        new ChartValidator().ThrowIfInvalid(chart);
        Directory.CreateDirectory(Path.GetDirectoryName(fullPath)!);
        var temporary = fullPath + ".tmp";
        File.WriteAllText(temporary, ChartJson.Serialize(chart));
        File.Move(temporary, fullPath, true);
    }

    private static string ValidatePath(string path)
    {
        if (string.IsNullOrWhiteSpace(path)) throw new ArgumentException("A path is required.", nameof(path));
        if (path.Split(['/', '\\'], StringSplitOptions.RemoveEmptyEntries).Any(segment => segment == "..")) throw new InvalidDataException("Path traversal is not allowed.");
        var fullPath = Path.GetFullPath(path);
        if (!AllowedExtensions.Contains(Path.GetExtension(fullPath))) throw new InvalidDataException("Only .json and .beatforge chart files are supported.");
        if (Path.GetFileName(fullPath).Contains("..", StringComparison.Ordinal)) throw new InvalidDataException("Unsafe filename.");
        return fullPath;
    }
}
