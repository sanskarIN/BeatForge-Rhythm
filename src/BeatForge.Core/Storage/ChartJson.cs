using System.Text.Json;
using System.Text.Json.Serialization;

namespace BeatForge.Core;

public static class ChartJson
{
    private static readonly JsonSerializerOptions Options = new(JsonSerializerDefaults.Web)
    {
        WriteIndented = true,
        AllowTrailingCommas = false,
        ReadCommentHandling = JsonCommentHandling.Skip,
        PropertyNameCaseInsensitive = true,
        Converters = { new JsonStringEnumConverter() }
    };

    public static string Serialize(BeatChart chart)
    {
        new ChartValidator().ThrowIfInvalid(chart);
        return JsonSerializer.Serialize(chart, Options);
    }

    public static BeatChart Deserialize(string json)
    {
        if (string.IsNullOrWhiteSpace(json)) throw new InvalidDataException("Chart JSON is empty.");
        var chart = JsonSerializer.Deserialize<BeatChart>(json, Options) ?? throw new InvalidDataException("Chart JSON is invalid.");
        Migrate(chart);
        new ChartValidator().ThrowIfInvalid(chart);
        return chart;
    }

    public static void Migrate(BeatChart chart)
    {
        if (chart.SchemaVersion == 0) chart.SchemaVersion = 1;
        chart.BpmChanges ??= [];
        chart.Notes ??= [];
        chart.Metadata ??= new();
    }
}
