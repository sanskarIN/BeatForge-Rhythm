using BeatForge.Core;
using Godot;
using System;
using System.Collections.Generic;

namespace BeatForge.Game;

public partial class CalibrationView : Control
{
    private readonly CalibrationProfile _profile;
    private readonly Action _save;
    private readonly Action _back;
    private readonly List<CalibrationBeat> _samples = [];
    private Label _status = null!;
    private Button _tap = null!;
    private ulong _startedAt;
    private int _sampleIndex;

    public CalibrationView(CalibrationProfile profile, Action save, Action back) { _profile = profile; _save = save; _back = back; }

    public override void _Ready()
    {
        SetAnchorsAndOffsetsPreset(LayoutPreset.FullRect);
        AddChild(new ColorRect { Color = new Color("0b1024"), MouseFilter = MouseFilterEnum.Ignore });
        var margin = new MarginContainer(); margin.SetAnchorsAndOffsetsPreset(LayoutPreset.FullRect); margin.AddThemeConstantOverride("margin_left", 42); margin.AddThemeConstantOverride("margin_right", 42); margin.AddThemeConstantOverride("margin_top", 54); margin.AddThemeConstantOverride("margin_bottom", 42); AddChild(margin);
        var root = new VBoxContainer(); root.AddThemeConstantOverride("separation", 18); margin.AddChild(root);
        var header = new HBoxContainer(); root.AddChild(header); var title = new Label { Text = "CALIBRATION" }; title.AddThemeFontSizeOverride("font_size", 32); title.SizeFlagsHorizontal = SizeFlags.ExpandFill; header.AddChild(title); var back = new Button { Text = "BACK" }; back.Pressed += _back; header.AddChild(back);
        root.AddChild(new Label { Text = "Tap the large button on each visual beat. Eight samples produce a robust median offset.", AutowrapMode = TextServer.AutowrapMode.WordSmart });
        _status = new Label { Text = "Ready for sample 1 / 8" }; _status.AddThemeFontSizeOverride("font_size", 22); root.AddChild(_status);
        _tap = new Button { Text = "START / TAP BEAT", CustomMinimumSize = new Vector2(0, 160) }; _tap.Pressed += TapBeat; root.AddChild(_tap);
        var reset = new Button { Text = "RESET HISTORY" }; reset.Pressed += () => { _profile.Reset(); _samples.Clear(); _sampleIndex = 0; _startedAt = 0; UpdateStatus(); _save(); }; root.AddChild(reset);
        root.AddChild(new Label { Text = $"Current audio offset: {_profile.AudioOffsetMs:0} ms\nCurrent input offset: {_profile.InputOffsetMs:0} ms\nStored runs: {_profile.History.Count}", AutowrapMode = TextServer.AutowrapMode.WordSmart });
    }

    private void TapBeat()
    {
        if (_startedAt == 0) _startedAt = Time.GetTicksMsec();
        var observed = (Time.GetTicksMsec() - _startedAt) / 1000d;
        var expected = (_sampleIndex + 1) * 0.5;
        _samples.Add(new CalibrationBeat(expected, observed)); _sampleIndex++;
        if (_sampleIndex >= CalibrationEngine.RecommendedSamples)
        {
            var engine = new CalibrationEngine(_profile); engine.ApplyAutomaticInputCalibration(_samples); _save(); _tap.Disabled = true; _status.Text = $"Complete • input offset {_profile.InputOffsetMs:0} ms";
        }
        else UpdateStatus();
    }

    private void UpdateStatus() => _status.Text = _startedAt == 0 ? "Ready for sample 1 / 8" : $"Sample {_sampleIndex + 1} / {CalibrationEngine.RecommendedSamples} • keep tapping on the beat";
}
