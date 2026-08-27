# Contributing to BeatForge

Thank you for helping build an open rhythm platform. Keep pull requests focused, preserve the core/UI boundary, and add tests for timing, parsing, scoring, or validation changes.

## Development

Use Godot 4 .NET for the client and .NET 8 for core tests. Run `dotnet test BeatForge.sln` before opening a pull request. Do not add copyrighted audio or visual assets unless you have redistribution rights.

## Commits

Use small, meaningful Conventional Commit messages such as `feat: add hold-note judgement` or `test: cover BPM change conversion`. Do not create empty commits just to inflate history.

## Pull requests

Describe behavior changes, test coverage, and any Android-specific considerations. Screenshots or a short recording are useful for UI changes. Report security issues privately using `SECURITY.md`.
