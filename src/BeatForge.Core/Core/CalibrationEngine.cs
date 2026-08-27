namespace BeatForge.Core;

public sealed record CalibrationBeat(double ExpectedSeconds, double ObservedSeconds);

public sealed class CalibrationEngine
{
    public const int RecommendedSamples = 8;
    public CalibrationProfile Profile { get; }
    public CalibrationEngine(CalibrationProfile profile) => Profile = profile ?? throw new ArgumentNullException(nameof(profile));

    public double CalculateOffset(IEnumerable<CalibrationBeat> beats)
    {
        var values = beats.Select(beat => (beat.ObservedSeconds - beat.ExpectedSeconds) * 1000).ToArray();
        if (values.Length == 0) return 0;
        Array.Sort(values);
        var middle = values.Length / 2;
        return values.Length % 2 == 1 ? values[middle] : (values[middle - 1] + values[middle]) / 2;
    }

    public double ApplyAutomaticInputCalibration(IEnumerable<CalibrationBeat> beats)
    {
        var offset = CalculateOffset(beats);
        Profile.ApplyManual(Profile.AudioOffsetMs, -offset);
        return Profile.InputOffsetMs;
    }

    public double CorrectedInputErrorMs(double rawErrorMs) => rawErrorMs + Profile.InputOffsetMs;
}
