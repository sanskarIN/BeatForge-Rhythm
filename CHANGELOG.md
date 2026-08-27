# Changelog

## Unreleased

The initial development slice establishes a Godot C# client shell, a standalone deterministic core, versioned chart JSON, safe import/export, scoring, replay capture, editor commands, a sample chart, documentation, and CI.

The next slice adds versioned local player data, atomic settings and progress saves, calibration history and median-offset calculation, searchable favorites and best-score library state, a 52-entry achievement catalog, campaign worlds and star progression, safe replay JSON portability, and a campaign browser in the Godot menu. The full core and Godot solution build cleanly, and the deterministic suite now reports 14 passing checks.

The synchronization slice adds an audio-position-aware `SongClock` with pause/resume compensation and calibrated timestamp correction, note-type-aware tap/hold/slide/swipe/directional-swipe judgement, hold release validation, Godot press/release lane input, and an interactive eight-sample calibration screen. The complete solution builds cleanly and the deterministic suite now reports 16 passing checks.
