namespace BeatForge.Core;

public sealed class EditorDocument
{
    private sealed record Command(Action Undo, Action Redo);
    private readonly Stack<Command> _undo = [];
    private readonly Stack<Command> _redo = [];
    public BeatChart Chart { get; }
    public double CursorBeat { get; private set; }
    public double SnapBeats { get; set; } = 0.25;
    public bool IsDirty { get; private set; }
    public bool CanUndo => _undo.Count > 0;
    public bool CanRedo => _redo.Count > 0;

    public EditorDocument(BeatChart chart) => Chart = chart ?? throw new ArgumentNullException(nameof(chart));

    public void AddNote(ChartNote note)
    {
        Execute(() => Chart.Notes.Add(note), () => Chart.Notes.RemoveAll(existing => existing.Id == note.Id));
    }

    public bool DeleteNote(string id)
    {
        var note = Chart.Notes.FirstOrDefault(item => item.Id == id);
        if (note is null) return false;
        Execute(() => Chart.Notes.Remove(note), () => Chart.Notes.Add(note));
        return true;
    }

    public void MoveNote(string id, double beat, int lane)
    {
        var index = Chart.Notes.FindIndex(note => note.Id == id);
        if (index < 0) return;
        var before = Chart.Notes[index];
        var after = before with { Beat = Snap(beat), Lane = lane };
        Execute(() => Chart.Notes[index] = after, () => Chart.Notes[index] = before);
    }

    public void SetCursor(double beat) => CursorBeat = Math.Max(0, beat);
    public void Undo() { if (_undo.TryPop(out var command)) { command.Undo(); _redo.Push(command); IsDirty = true; } }
    public void Redo() { if (_redo.TryPop(out var command)) { command.Redo(); _undo.Push(command); IsDirty = true; } }
    public void MarkSaved() => IsDirty = false;
    public double Snap(double beat) => SnapBeats <= 0 ? beat : Math.Round(beat / SnapBeats) * SnapBeats;

    private void Execute(Action apply, Action inverse)
    {
        apply();
        _undo.Push(new Command(inverse, apply));
        _redo.Clear();
        IsDirty = true;
    }
}
