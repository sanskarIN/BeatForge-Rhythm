namespace BeatForge.Core;

public sealed record AccessibilitySettings
{
    public bool ReducedFlashing { get; set; }
    public bool ReducedMotion { get; set; }
    public bool HighContrastLanes { get; set; }
    public bool AlternativeVisualCues { get; set; }
    public bool VibrationEnabled { get; set; } = true;
    public bool LargeButtonMode { get; set; }
    public double NoteScale { get; set; } = 1.0;
    public double TextScale { get; set; } = 1.0;
}

public sealed record PlayerSettings
{
    public string Theme { get; set; } = "dark";
    public bool MetronomeEnabled { get; set; }
    public bool NoteSoundEnabled { get; set; } = true;
    public double PracticeSpeed { get; set; } = 1.0;
    public double AudioOffsetMs { get; set; }
    public double InputOffsetMs { get; set; }
    public AccessibilitySettings Accessibility { get; set; } = new();
}

public sealed record CalibrationSample(DateTimeOffset RecordedAt, double AudioOffsetMs, double InputOffsetMs);

public sealed class CalibrationProfile
{
    public double AudioOffsetMs { get; set; }
    public double InputOffsetMs { get; set; }
    public List<CalibrationSample> History { get; set; } = [];

    public void ApplyManual(double audioOffsetMs, double inputOffsetMs)
    {
        AudioOffsetMs = Clamp(audioOffsetMs);
        InputOffsetMs = Clamp(inputOffsetMs);
        History.Add(new CalibrationSample(DateTimeOffset.UtcNow, AudioOffsetMs, InputOffsetMs));
        TrimHistory();
    }

    public (double AudioOffsetMs, double InputOffsetMs) ApplyAutomaticSamples(IEnumerable<double> audioErrorsMs, IEnumerable<double> inputErrorsMs)
    {
        var audio = Median(audioErrorsMs.Select(Clamp).ToArray());
        var input = Median(inputErrorsMs.Select(Clamp).ToArray());
        ApplyManual(audio, input);
        return (AudioOffsetMs, InputOffsetMs);
    }

    public void Reset()
    {
        AudioOffsetMs = 0;
        InputOffsetMs = 0;
        History.Clear();
    }

    private void TrimHistory() { if (History.Count > 20) History.RemoveRange(0, History.Count - 20); }
    private static double Clamp(double value) => Math.Clamp(value, -500, 500);
    private static double Median(double[] values)
    {
        if (values.Length == 0) return 0;
        Array.Sort(values);
        var middle = values.Length / 2;
        return values.Length % 2 == 1 ? values[middle] : (values[middle - 1] + values[middle]) / 2;
    }
}

public sealed record PlayRecord
{
    public DateTimeOffset PlayedAt { get; init; } = DateTimeOffset.UtcNow;
    public string ChartId { get; init; } = "";
    public Difficulty Difficulty { get; init; }
    public int Score { get; init; }
    public int MaxCombo { get; init; }
    public double Accuracy { get; init; }
    public string Grade { get; init; } = "D";
    public double DurationSeconds { get; init; }
}

public sealed class PlayerStatistics
{
    public int TotalPlays { get; set; }
    public int SongsCompleted { get; set; }
    public int HighestCombo { get; set; }
    public double AverageAccuracy { get; set; }
    public int PerfectNotes { get; set; }
    public int Misses { get; set; }
    public double PlayTimeSeconds { get; set; }
    public int EditorChartsCreated { get; set; }
    public int DailyChallengeStreak { get; set; }
    public List<PlayRecord> RecentPlays { get; set; } = [];

    public void RecordPlay(PlayRecord record, bool completed)
    {
        TotalPlays++;
        if (completed) SongsCompleted++;
        HighestCombo = Math.Max(HighestCombo, record.MaxCombo);
        AverageAccuracy = ((AverageAccuracy * (TotalPlays - 1)) + record.Accuracy) / TotalPlays;
        PlayTimeSeconds += Math.Max(0, record.DurationSeconds);
        RecentPlays.Add(record);
        if (RecentPlays.Count > 100) RecentPlays.RemoveAt(0);
    }

    public void RecordJudgementCounts(int perfectNotes, int misses) { PerfectNotes += Math.Max(0, perfectNotes); Misses += Math.Max(0, misses); }
    public void RecordEditorChart() => EditorChartsCreated++;
    public void SetDailyChallengeStreak(int streak) => DailyChallengeStreak = Math.Max(0, streak);
}
