using BeatForge.Core;
using Godot;
using System;
using System.Linq;

namespace BeatForge.Game;

public partial class ChartEditorView : Control
{
    private readonly EditorDocument _document;
    private readonly Action _back;
    private Label _info = null!;
    private VBoxContainer _notes = null!;

    public ChartEditorView(BeatChart chart, Action back) { _document = new EditorDocument(chart); _back = back; }

    public override void _Ready()
    {
        SetAnchorsAndOffsetsPreset(LayoutPreset.FullRect);
        AddChild(new ColorRect { Color = new Color("0b1024"), MouseFilter = MouseFilterEnum.Ignore });
        var margin = new MarginContainer(); margin.SetAnchorsAndOffsetsPreset(LayoutPreset.FullRect); margin.AddThemeConstantOverride("margin_left", 34); margin.AddThemeConstantOverride("margin_right", 34); margin.AddThemeConstantOverride("margin_top", 34); margin.AddThemeConstantOverride("margin_bottom", 34); AddChild(margin);
        var root = new VBoxContainer(); root.AddThemeConstantOverride("separation", 12); margin.AddChild(root);
        var header = new HBoxContainer(); root.AddChild(header); var title = new Label { Text = "CHART EDITOR" }; title.AddThemeFontSizeOverride("font_size", 30); title.SizeFlagsHorizontal = SizeFlags.ExpandFill; header.AddChild(title); var back = new Button { Text = "BACK" }; back.Pressed += _back; header.AddChild(back);
        _info = new Label { Text = "" }; root.AddChild(_info);
        var toolbar = new HBoxContainer(); root.AddChild(toolbar); AddTool(toolbar, "ADD TAP", AddTap); AddTool(toolbar, "UNDO", Undo); AddTool(toolbar, "REDO", Redo); AddTool(toolbar, "SAVE", Save);
        var snap = new OptionButton(); snap.AddItem("Snap 1/4", 0); snap.AddItem("Snap 1/8", 1); snap.AddItem("Snap 1/16", 2); snap.ItemSelected += index => _document.SnapBeats = index switch { 0 => 0.25, 1 => 0.125, _ => 0.0625 }; toolbar.AddChild(snap);
        var scroll = new ScrollContainer { SizeFlagsVertical = SizeFlags.ExpandFill }; root.AddChild(scroll); _notes = new VBoxContainer(); scroll.AddChild(_notes); Refresh();
    }

    private void Refresh()
    {
        foreach (var child in _notes.GetChildren()) child.QueueFree();
        foreach (var note in _document.Chart.Notes.OrderBy(note => note.Beat))
        {
            var row = new HBoxContainer(); var label = new Label { Text = $"{note.Id}   beat {note.Beat:0.###}   lane {note.Lane}   {note.Type}" }; label.SizeFlagsHorizontal = SizeFlags.ExpandFill; row.AddChild(label); var delete = new Button { Text = "DELETE" }; var id = note.Id; delete.Pressed += () => { _document.DeleteNote(id); Refresh(); }; row.AddChild(delete); _notes.AddChild(row);
        }
        _info.Text = $"{_document.Chart.Metadata.Title}  •  {_document.Chart.Notes.Count} notes  •  {_document.Chart.InitialBpm:0} BPM  •  {(_document.IsDirty ? "UNSAVED" : "SAVED")}";
    }

    private void AddTap() { var next = _document.Chart.Notes.Count == 0 ? 1 : _document.Chart.LastBeat + 1; _document.AddNote(new ChartNote { Id = $"editor-{DateTime.UtcNow:HHmmssfff}", Beat = _document.Snap(next), Lane = _document.Chart.Notes.Count % _document.Chart.LaneCount, Type = NoteType.Tap }); Refresh(); }
    private void Undo() { _document.Undo(); Refresh(); }
    private void Redo() { _document.Redo(); Refresh(); }
    private void Save() { new SafeImport().SaveChart(ProjectSettings.GlobalizePath("user://pulse-garden.beatforge"), _document.Chart); _document.MarkSaved(); Refresh(); }
    private static void AddTool(HBoxContainer parent, string text, Action action) { var button = new Button { Text = text }; button.Pressed += action; parent.AddChild(button); }
}
