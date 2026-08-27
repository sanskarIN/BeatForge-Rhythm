# Architecture

BeatForge separates the deterministic rhythm domain from the Godot presentation layer.

```text
Godot client
  Main menu / library / gameplay / editor / settings
                   |
             application services
                   |
  BeatForge.Core: chart -> timing -> judgement -> score -> replay
                   |
      JSON import/export and local persistence boundaries
```

## Core boundaries

`BeatChart` and `ChartNote` are plain C# data models. `BeatClock` converts beats to seconds across BPM changes and applies the chart offset. `TimingWindows` turns an input error into a configurable judgement. `ScoreState` owns combo, score, grade, accuracy, full-combo, and all-perfect state. `ReplayRecorder` records inputs and results without knowing about Godot nodes.

`ChartValidator` is the trust boundary for user-created and imported content. It caps chart size, checks identifiers and metadata, checks BPM ranges and ordering, and checks note positions. `SafeImport` additionally restricts file extensions and writes atomically through a temporary file.

## Godot layer

The Godot project is intentionally thin. `Main.cs` owns screen navigation and creates mobile-friendly controls. `RhythmGame.cs` owns the frame loop, visible note presentation, input mapping, and forwarding judgements to the core. `ChartEditorView.cs` uses `EditorDocument` for undo/redo rather than mutating chart data with UI-specific rules.

## Persistence roadmap

The current slice serializes charts, replays, settings, calibration history, statistics, favorites, best scores, and achievement progress as versioned local JSON. A future SQLite adapter can implement the same storage boundary for larger song-library indexes without changing gameplay. No gameplay code should depend directly on SQLite.

## Timing contract

All gameplay timestamps are seconds from the audio clock. Rendering may use frame time, but judgement uses the audio position plus the user calibration offset. A chart beat is converted through `BeatClock`; it is never reconstructed from frame count. Pause/resume restores the audio position before resuming scheduling.
