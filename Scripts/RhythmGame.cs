using BeatForge.Core;
using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BeatForge.Game;

public partial class RhythmGame : Control
{
    private readonly BeatChart _chart;
    private readonly bool _practice;
    private readonly Action _back;
    private readonly BeatClock _clock;
    private readonly ScoreState _score = new();
    private readonly ReplayRecorder _replay;
    private readonly List<ChartNote> _remaining;
    private Label _scoreLabel = null!, _comboLabel = null!, _judgementLabel = null!, _progressLabel = null!;
    private double _elapsed;
    private double _speed = 1;
    private bool _paused;

    public RhythmGame(BeatChart chart, bool practice, Action back)
    {
        _chart = chart; _practice = practice; _back = back; _clock = new BeatClock(chart); _replay = new ReplayRecorder(chart.Metadata.Id, chart.SchemaVersion); _remaining = chart.Notes.OrderBy(note => note.Beat).ToList();
    }

    public override void _Ready()
    {
        SetAnchorsAndOffsetsPreset(LayoutPreset.FullRect);
        AddChild(new ColorRect { Color = new Color("080c1d"), MouseFilter = MouseFilterEnum.Ignore });
        var top = new HBoxContainer(); top.Position = new Vector2(24, 30); top.Size = new Vector2(1032, 80); AddChild(top);
        var back = new Button { Text = "‹", CustomMinimumSize = new Vector2(70, 60) }; back.Pressed += _back; top.AddChild(back);
        var title = new Label { Text = _practice ? "PRACTICE" : "PLAY" }; title.AddThemeFontSizeOverride("font_size", 24); top.AddChild(title);
        _scoreLabel = new Label { Text = "SCORE 000000", HorizontalAlignment = HorizontalAlignment.Right }; _scoreLabel.SizeFlagsHorizontal = Control.SizeFlags.ExpandFill; top.AddChild(_scoreLabel);
        _progressLabel = new Label { Text = "0 / " + _chart.Notes.Count, Position = new Vector2(24, 112) }; AddChild(_progressLabel);
        _comboLabel = new Label { Text = "COMBO 0", Position = new Vector2(24, 160) }; _comboLabel.AddThemeFontSizeOverride("font_size", 30); _comboLabel.AddThemeColorOverride("font_color", new Color("23d5ab")); AddChild(_comboLabel);
        _judgementLabel = new Label { Text = "READY", Position = new Vector2(24, 208) }; _judgementLabel.AddThemeFontSizeOverride("font_size", 24); AddChild(_judgementLabel);
        var controls = new HBoxContainer { Position = new Vector2(24, 1760), Size = new Vector2(1032, 90) }; AddChild(controls);
        var pause = new Button { Text = "PAUSE", CustomMinimumSize = new Vector2(160, 70) }; pause.Pressed += () => _paused = !_paused; controls.AddChild(pause);
        var slower = new Button { Text = "− SPEED", CustomMinimumSize = new Vector2(160, 70) }; slower.Pressed += () => _speed = Math.Max(0.5, _speed - 0.1); controls.AddChild(slower);
        var faster = new Button { Text = "+ SPEED", CustomMinimumSize = new Vector2(160, 70) }; faster.Pressed += () => _speed = Math.Min(1.5, _speed + 0.1); controls.AddChild(faster);
        for (var lane = 0; lane < _chart.LaneCount; lane++) AddLaneButton(lane);
    }

    public override void _Process(double delta)
    {
        if (_paused || _remaining.Count == 0) return;
        _elapsed += delta * _speed;
        QueueRedraw();
        var currentBeat = _clock.SecondsToBeat(_elapsed);
        while (_remaining.Count > 0 && _remaining[0].Beat < currentBeat - 0.22 * _chart.InitialBpm / 60)
        {
            Resolve(_remaining[0], Judgement.Miss, 999); _remaining.RemoveAt(0);
        }
        _progressLabel.Text = $"{_score.TotalNotes} / {_chart.Notes.Count}    SPEED {_speed:0.0}x";
    }

    public override void _Draw()
    {
        var laneWidth = 950f / _chart.LaneCount; var top = 430f; var bottom = 1620f;
        for (var lane = 0; lane < _chart.LaneCount; lane++)
        {
            var rect = new Rect2(65 + lane * laneWidth, top, laneWidth - 8, bottom - top); DrawRect(rect, lane % 2 == 0 ? new Color("111936") : new Color("0e1530"), true); DrawLine(new Vector2(rect.Position.X, bottom), new Vector2(rect.End.X, bottom), new Color("23d5ab"), 5);
        }
        var currentBeat = _clock.SecondsToBeat(_elapsed); var pixelsPerBeat = 160f;
        foreach (var note in _chart.Notes)
        {
            var y = bottom - (float)((note.Beat - currentBeat) * pixelsPerBeat); if (y < top - 100 || y > bottom + 40) continue;
            var x = 65 + note.Lane * laneWidth + 10; var height = Math.Max(20, (float)(note.DurationBeats * pixelsPerBeat)); DrawRect(new Rect2(x, y - height + 20, laneWidth - 28, height), note.Type == NoteType.Hold ? new Color("7c5cff") : new Color("23d5ab"), true);
        }
    }

    private void AddLaneButton(int lane)
    {
        var button = new Button { Text = (lane + 1).ToString(), Position = new Vector2(65 + lane * (950f / _chart.LaneCount), 1480), Size = new Vector2(950f / _chart.LaneCount - 8, 130) };
        button.Pressed += () => HitLane(lane); AddChild(button);
    }

    private void HitLane(int lane)
    {
        if (_remaining.Count == 0) return;
        var currentSeconds = _elapsed; var note = _remaining.FirstOrDefault(item => item.Lane == lane);
        if (note is null) return;
        var errorMs = (currentSeconds - _clock.NoteTimeSeconds(note)) * 1000;
        var judgement = new TimingWindows().Judge(errorMs);
        if (judgement == Judgement.Miss) return;
        Resolve(note, judgement, errorMs); _remaining.Remove(note);
    }

    private void Resolve(ChartNote note, Judgement judgement, double errorMs)
    {
        var result = _score.Apply(judgement, errorMs); _replay.RecordInput(_elapsed, note.Lane, "hit"); _replay.RecordJudgement(_elapsed, note.Id, judgement, errorMs);
        _scoreLabel.Text = $"SCORE {_score.Score:000000}"; _comboLabel.Text = $"COMBO {_score.Combo}"; _judgementLabel.Text = judgement.ToString().ToUpperInvariant(); _judgementLabel.AddThemeColorOverride("font_color", judgement == Judgement.Miss ? new Color("f06c8f") : new Color("23d5ab"));
    }
}
