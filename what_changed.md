# What Changed

## Scope delivered in this phase

BeatForge: Rhythm Lab has been moved from an empty repository shell to a real Godot 4 C# project foundation with a standalone .NET core. The implementation follows the supplied master prompt, but it is intentionally staged: the repository now contains a working vertical slice and the architecture needed to continue toward the full Android release without mixing timing logic into UI code.

The repository has a `develop` branch with a repository-local Git identity of `Sanskar <sanskarin@outlook.in>`. The target remote is `https://github.com/sanskarIN/BeatForge-Rhythm.git`. Commits are small and meaningful; no empty or fabricated commits were used.

## Product and repository foundation

The project is configured as `BeatForge: Rhythm Lab`, with Android-first viewport settings, a dark visual baseline, an original SVG icon, and a Godot main scene. The root includes the Apache 2.0 license, NOTICE, README, architecture guide, build guide, testing strategy, roadmap, changelog, contributing guide, code of conduct, security policy, privacy policy, and support policy. The README and About screen prominently include the optional Buy Me a Coffee link without gating gameplay.

A GitHub Actions workflow runs the standalone .NET core tests on pushes and pull requests to `main` and `develop`. The repository ignores Godot editor state, build output, exported packages, and signing credentials.

## Deterministic core

`BeatChart`, `ChartNote`, difficulty levels, note types, BPM changes, and judgement categories are represented as strongly typed C# models. `BeatClock` converts beats to seconds and seconds to beats across BPM changes and chart offsets. `TimingWindows` provides configurable Perfect+, Perfect, Great, Good, and Miss decisions. `NoteScheduler` provides frame-independent look-ahead scheduling.

`ScoreState` tracks score, combo, maximum combo, accuracy, judgement counts, full combo, all-perfect state, and grades from S+ through D. `ReplayRecorder` stores chart version, input timestamps, and judgement results for deterministic replay work. `EditorDocument` provides add, delete, move, snapping, dirty state, undo, and redo.

## Chart safety and portability

The chart format is JSON schema version 1 and is documented in `docs/CHART_FORMAT.md`. Serialization validates before writing. Deserialization migrates supported legacy version zero data to version one, rejects unsupported schema versions, and rejects malformed metadata, BPM, lane, note, and file-size data. `SafeImport` limits chart files to 10 MB, accepts only `.json` and `.beatforge`, blocks traversal segments and unsafe filenames, validates before writing, and uses an atomic temporary file replacement.

The sample chart `charts/pulse-garden.json` contains original metadata and no copyrighted audio. The project does not bundle commercial music.

## Godot client vertical slice

The main menu provides navigation to Play, Song Library, Practice Mode, Chart Editor, Settings and Accessibility, and About and Support. The library displays the sample chart and its difficulty metadata. The gameplay screen provides four-lane tap/hold rendering, touch-sized lane buttons, score, combo, judgement feedback, pause, practice speed controls, frame-independent beat positioning, miss resolution, and replay capture. The editor provides a timeline-like note list, add tap, delete, snap resolution, undo, redo, save, metadata summary, and dirty-state feedback. Settings expose reduced effects, note-size, large-button, vibration, and calibration placeholders in the UI boundary while deeper calibration persistence is planned for the next slice.

## Tests and verification

The core test runner covers constant BPM conversion, BPM changes, timing-window boundaries, score/combo/grade behavior, JSON round trips, malformed chart rejection, editor undo/redo correctness, and replay recording. The standalone core project compiled successfully with .NET 8 in the development environment. Godot itself was not installed in the sandbox, so the Godot client was reviewed statically and should be opened with the Godot 4 .NET editor for scene/runtime verification.

## Remaining planned phases

The full master prompt is larger than one safe implementation slice. The next meaningful phases are audio playback and calibration with an actual Godot audio clock; persistent SQLite-backed song library, statistics, achievements, and settings; full note interaction for holds/swipes/chains/slides; campaign and daily/weekly challenge content; replay viewer; mobile file-picker integration; accessibility refinement; waveform generation; Android export and device testing; performance profiling; and production release packaging. These are recorded in `ROADMAP.md` rather than represented as fake controls or empty placeholder systems.

## Commits in this phase

| Commit | Message |
| --- | --- |
| `b00862b` | `chore: initialize BeatForge foundation` |
| `ed415f7` | `feat: add deterministic rhythm core` |
| `7f54db1` | `feat: add Godot playable vertical slice` |
| `8015f11` | `test: cover core timing and chart behavior` |
| `984b4a8` | `fix: align Godot project build paths` |
| current | `docs: record phase changes` |
