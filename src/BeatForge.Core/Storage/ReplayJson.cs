using System.Text.Json;
using System.Text.Json.Serialization;

namespace BeatForge.Core;

public static class ReplayJson
{
    private static readonly JsonSerializerOptions Options = new(JsonSerializerDefaults.Web)
    {
        WriteIndented = true,
        Converters = { new JsonStringEnumConverter() }
    };
    public const long MaxBytes = 20 * 1024 * 1024;

    public static string Serialize(ReplayData data)
    {
        if (data is null) throw new ArgumentNullException(nameof(data));
        return JsonSerializer.Serialize(data, Options);
    }

    public static ReplayData Deserialize(string json)
    {
        if (string.IsNullOrWhiteSpace(json)) throw new InvalidDataException("Replay JSON is empty.");
        var data = JsonSerializer.Deserialize<ReplayData>(json, Options) ?? throw new InvalidDataException("Replay JSON is invalid.");
        if (data.FormatVersion != 1) throw new InvalidDataException($"Unsupported replay format version {data.FormatVersion}.");
        if (string.IsNullOrWhiteSpace(data.ChartId) || data.ChartId.Length > 128) throw new InvalidDataException("Replay chart ID is invalid.");
        if (data.Inputs.Count > 1_000_000 || data.Judgements.Count > 1_000_000) throw new InvalidDataException("Replay contains too many events.");
        if (data.Inputs.Any(input => input.TimeSeconds < 0 || input.Lane < 0) || data.Judgements.Any(item => item.TimeSeconds < 0 || item.ErrorMs is < -10_000 or > 10_000)) throw new InvalidDataException("Replay contains an invalid event.");
        return data;
    }
}
