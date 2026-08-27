# BeatForge: Rhythm Lab

**BeatForge: Rhythm Lab** is an Android-first, offline-first rhythm game and beat-map creation platform built with **C# and Godot 4**. It is designed around a deterministic timing core so gameplay, practice, replays, chart validation, and the editor share one source of truth.

> Made by the Sanskar

## Current release slice

This repository contains the foundation for a playable vertical slice: a responsive main menu, song library, playable tap/hold chart surface, practice controls, chart editor commands, versioned JSON charts, safe import/export, scoring, replay recording, and a standalone test suite for the timing and data layers.

The code deliberately ships no commercial music. Add only original or properly licensed audio to `audio/` when creating a distributable build.

## Stack

- Godot 4.3+ with the .NET/C# build
- .NET 8 for the standalone core and tests
- JSON chart files with schema versioning
- Local filesystem persistence; SQLite integration is kept behind storage boundaries for a future mobile persistence slice
- No account required for basic use

## Run

1. Install [Godot 4 .NET](https://godotengine.org/download/archive/) and the .NET 8 SDK.
2. Open this folder in Godot and run `Scenes/Main.tscn`.
3. Run the portable core tests with `dotnet test BeatForge.sln`.

The CI workflow runs the core test project on every push and pull request.

## Product principles

- Accurate timing is independent from UI and rendering.
- Imported content is validated before use and never executed.
- Core gameplay works offline and does not require an account.
- Copyrighted commercial music is never bundled without rights.
- Donations are optional and never interrupt or gate gameplay.

Support the project: <https://buymeacoffee.com/sanskarIN>

## Repository map

| Path | Responsibility |
| --- | --- |
| `src/BeatForge.Core/Core` | Timing, chart domain, scoring, replay, editor commands, validation |
| `src/BeatForge.Core/Storage` | JSON schema serialization and safe file import/export |
| `src/BeatForge.Game` | Godot client scripts and scene project |
| `charts` | Small original sample charts only |
| `docs` | Architecture and chart-format documentation |
| `tests` | Fast, dependency-free core tests |

## License and contact

BeatForge is released under the Apache License 2.0. See `LICENSE` and `NOTICE`.

Creator: Sanskar  
Business: sanskarin@outlook.in  
Support: supportramsandesh@gmail.com  
GitHub: <https://github.com/sanskarIN>
