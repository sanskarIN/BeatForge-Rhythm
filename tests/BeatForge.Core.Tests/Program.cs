using BeatForge.Core;

static class Program
{
    private static int _passed;
    private static int _failed;

    public static int Main()
    {
        Run("beat clock converts beats at a constant BPM", TestConstantBpm);
        Run("beat clock handles BPM changes", TestBpmChanges);
        Run("timing windows classify absolute error", TestTimingWindows);
        Run("scoring tracks combo grade and accuracy", TestScoring);
        Run("chart JSON round trips", TestJsonRoundTrip);
        Run("validator rejects malformed charts", TestValidation);
        Run("editor undo and redo restore edits", TestEditorUndoRedo);
        Run("replay records inputs and judgements", TestReplay);
        Run("replay JSON round trips safely", TestReplayJson);
        Run("calibration calculates and clamps offsets", TestCalibration);
        Run("local player data saves and loads atomically", TestLocalData);
        Run("song library searches and records results", TestLibrary);
        Run("achievement catalog is extensible", TestAchievements);
        Run("campaign progression unlocks stages", TestCampaign);
        Console.WriteLine($"{_passed} passed, {_failed} failed");
        return _failed == 0 ? 0 : 1;
    }

    private static BeatChart Chart() => new()
    {
        Metadata = new ChartMetadata { Id = "test", Title = "Test Chart", Difficulty = Difficulty.Normal },
        InitialBpm = 120,
        LaneCount = 4,
        Notes = [new ChartNote { Id = "n1", Beat = 4, Lane = 1 }]
    };

    private static void TestConstantBpm()
    {
        var clock = new BeatClock(Chart());
        AssertNear(0, clock.BeatToSeconds(0));
        AssertNear(2, clock.BeatToSeconds(4));
        AssertNear(4, clock.SecondsToBeat(2));
    }

    private static void TestBpmChanges()
    {
        var chart = Chart(); chart.BpmChanges.Add(new BpmChange { Beat = 4, Bpm = 60 });
        var clock = new BeatClock(chart);
        AssertNear(2, clock.BeatToSeconds(4));
        AssertNear(3, clock.BeatToSeconds(5));
        AssertNear(5, clock.SecondsToBeat(3));
    }

    private static void TestTimingWindows()
    {
        var windows = new TimingWindows();
        AssertEqual(Judgement.PerfectPlus, windows.Judge(0));
        AssertEqual(Judgement.Perfect, windows.Judge(30));
        AssertEqual(Judgement.Great, windows.Judge(-70));
        AssertEqual(Judgement.Good, windows.Judge(140));
        AssertEqual(Judgement.Miss, windows.Judge(221));
    }

    private static void TestScoring()
    {
        var score = new ScoreState();
        score.Apply(Judgement.PerfectPlus, 2); score.Apply(Judgement.Great, -50); score.Apply(Judgement.Miss, 300);
        AssertEqual(3, score.TotalNotes); AssertEqual(2, score.MaxCombo); AssertEqual(0, score.Combo); AssertEqual(1, score.Misses); AssertEqual("D", score.Grade);
    }

    private static void TestJsonRoundTrip()
    {
        var source = Chart(); source.Notes.Add(new ChartNote { Id = "hold", Beat = 8, Lane = 2, Type = NoteType.Hold, DurationBeats = 2 });
        var json = ChartJson.Serialize(source); var copy = ChartJson.Deserialize(json);
        AssertEqual(source.Metadata.Id, copy.Metadata.Id); AssertEqual(2, copy.Notes.Count); AssertEqual(NoteType.Hold, copy.Notes[1].Type);
    }

    private static void TestValidation()
    {
        var chart = Chart(); chart.LaneCount = 2; chart.Notes[0] = chart.Notes[0] with { Lane = 3 };
        var issues = new ChartValidator().Validate(chart);
        AssertTrue(issues.Any(issue => issue.Code == "note.lane"));
        AssertThrows<InvalidDataException>(() => ChartJson.Deserialize("{\"schemaVersion\":99}"));
    }

    private static void TestEditorUndoRedo()
    {
        var chart = Chart(); var editor = new EditorDocument(chart); editor.AddNote(new ChartNote { Id = "new", Beat = 1.13, Lane = 0 });
        AssertEqual(2, chart.Notes.Count); editor.Undo(); AssertEqual(1, chart.Notes.Count); editor.Redo(); AssertEqual(2, chart.Notes.Count); AssertNear(1.13, chart.Notes[1].Beat);
    }

    private static void TestReplay()
    {
        var replay = new ReplayRecorder("chart", 1); replay.RecordInput(1.2, 2, "hit"); replay.RecordJudgement(1.2, "n1", Judgement.Perfect, 4);
        AssertEqual("chart", replay.Data.ChartId); AssertEqual(1, replay.Data.Inputs.Count); AssertEqual(Judgement.Perfect, replay.Data.Judgements[0].Judgement);
    }

    private static void TestReplayJson()
    {
        var recorder = new ReplayRecorder("chart", 1); recorder.RecordInput(1, 0, "hit"); recorder.RecordJudgement(1, "n1", Judgement.Great, -20);
        var copy = ReplayJson.Deserialize(ReplayJson.Serialize(recorder.Data)); AssertEqual("chart", copy.ChartId); AssertEqual(Judgement.Great, copy.Judgements[0].Judgement);
        AssertThrows<InvalidDataException>(() => ReplayJson.Deserialize("{\"formatVersion\":99,\"chartId\":\"x\"}"));
    }

    private static void TestCalibration()
    {
        var profile = new CalibrationProfile(); var engine = new CalibrationEngine(profile);
        var corrected = engine.ApplyAutomaticInputCalibration([new(1, 1.03), new(2, 2.04), new(3, 3.02)]);
        AssertNear(-30, corrected); AssertEqual(1, profile.History.Count);
        profile.ApplyManual(900, -900); AssertNear(500, profile.AudioOffsetMs); AssertNear(-500, profile.InputOffsetMs);
    }

    private static void TestLocalData()
    {
        var path = Path.Combine(Path.GetTempPath(), $"beatforge-test-{Guid.NewGuid():N}.json");
        try
        {
            var data = new LocalPlayerData(); data.Settings.Theme = "amoled"; data.Statistics.SetDailyChallengeStreak(3); data.UnlockedAchievements.Add("first-beat");
            var store = new LocalDataStore(path); store.Save(data); var loaded = store.Load();
            AssertEqual("amoled", loaded.Settings.Theme); AssertEqual(3, loaded.Statistics.DailyChallengeStreak); AssertTrue(loaded.UnlockedAchievements.Contains("first-beat"));
        }
        finally { if (File.Exists(path)) File.Delete(path); }
    }

    private static void TestLibrary()
    {
        var library = new SongLibrary(); var chart = Chart(); chart.Metadata = chart.Metadata with { Title = "Moon Pulse", Tags = ["night"] }; library.AddOrReplace(new SongEntry { Chart = chart });
        AssertEqual(1, library.Query(new LibraryQuery { Search = "moon" }).Count); AssertTrue(library.ToggleFavorite("test")); AssertEqual(1, library.Query(new LibraryQuery { FavoritesOnly = true }).Count);
        library.RecordResult("test", new PlayRecord { ChartId = "test", Score = 100, Accuracy = 99, Grade = "S" }, true); AssertTrue(library.Entries[0].IsCompleted); AssertEqual("S", library.Entries[0].BestGrade);
    }

    private static void TestAchievements()
    {
        var catalog = new AchievementCatalog(); AssertTrue(catalog.All.Count >= 50); var service = new AchievementService(catalog, []); AssertTrue(service.Unlock("first-beat")); AssertTrue(!service.Unlock("missing"));
    }

    private static void TestCampaign()
    {
        var worlds = CampaignCatalog.CreateDefault(); var progress = new CampaignProgress(); progress.RecordStage("world-1-stage-1", 3, worlds); AssertTrue(progress.IsUnlocked(worlds[0].Stages[1])); AssertEqual(3, progress.TotalStars);
    }

    private static void Run(string name, Action test)
    {
        try { test(); Console.WriteLine($"PASS {name}"); _passed++; }
        catch (Exception exception) { Console.WriteLine($"FAIL {name}: {exception.Message}"); _failed++; }
    }
    private static void AssertTrue(bool condition) { if (!condition) throw new InvalidOperationException("Expected true."); }
    private static void AssertEqual<T>(T expected, T actual) { if (!EqualityComparer<T>.Default.Equals(expected, actual)) throw new InvalidOperationException($"Expected {expected}, got {actual}."); }
    private static void AssertNear(double expected, double actual) { if (Math.Abs(expected - actual) > 0.0001) throw new InvalidOperationException($"Expected {expected}, got {actual}."); }
    private static void AssertThrows<T>(Action action) where T : Exception { try { action(); throw new InvalidOperationException("Expected exception."); } catch (T) { } }
}
