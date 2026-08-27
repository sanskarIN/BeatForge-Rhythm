# Building BeatForge

Install Godot 4 .NET and the .NET 8 SDK. Open the repository in the .NET edition of Godot, allow the C# project to restore, and run `Scenes/Main.tscn`. For a release Android build, configure the Android SDK/JDK in Godot Editor Settings, select an Android export preset, and use a locally managed signing key. Never commit signing keys or exported private credentials.

The core can be built without Godot using `dotnet build src/BeatForge.Core/BeatForge.Core.csproj`. The test runner is dependency-free and runs with `dotnet run --project tests/BeatForge.Core.Tests`.
