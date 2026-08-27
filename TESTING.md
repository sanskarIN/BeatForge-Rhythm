# Testing

The core test project covers beat-to-time conversion, BPM changes, timing windows, combo and grade behavior, JSON round trips, malformed chart rejection, safe import limits, editor undo/redo, and replay capture. Core tests are deterministic and do not require an audio device or a rendering context.

Godot client tests should be run on desktop and an Android device. Verify touch targets, orientation, pause/resume audio synchronization, reduced-effects mode, chart import/export, and lifecycle interruptions. Performance testing should record startup time, chart load time, frame pacing, and input-to-judgement latency on representative low-end Android hardware.
