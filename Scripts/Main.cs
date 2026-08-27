using BeatForge.Core;
using Godot;
using System.Text.Json;

namespace BeatForge.Game;

public partial class Main : Control
{
    private readonly Color _background = new("0b1024");
    private readonly Color _panel = new("151d3d");
    private readonly Color _accent = new("23d5ab");
    private readonly Color _purple = new("7c5cff");
    private VBoxContainer _content = null!;
    private Label _status = null!;
    private BeatChart _chart = null!;
    private bool _reducedEffects;

    public override void _Ready()
    {
        LoadChart();
        BuildMenu();
    }

    private void LoadChart()
    {
        var path = ProjectSettings.GlobalizePath("res://charts/pulse-garden.json");
        _chart = ChartJson.Deserialize(FileAccess.GetFileAsString(path));
    }

    private void BuildMenu()
    {
        AddColorRect(_background, new Vector2(0, 0), new Vector2(1, 1));
        var margin = new MarginContainer { Name = "SafeArea" };
        margin.SetAnchorsAndOffsetsPreset(Control.LayoutPreset.FullRect);
        margin.AddThemeConstantOverride("margin_left", 48);
        margin.AddThemeConstantOverride("margin_right", 48);
        margin.AddThemeConstantOverride("margin_top", 54);
        margin.AddThemeConstantOverride("margin_bottom", 42);
        AddChild(margin);
        var scroll = new ScrollContainer { HorizontalScrollMode = ScrollContainer.ScrollMode.Disabled };
        scroll.SetAnchorsAndOffsetsPreset(Control.LayoutPreset.FullRect);
        margin.AddChild(scroll);
        _content = new VBoxContainer { Name = "MenuContent" };
        _content.AddThemeConstantOverride("separation", 18);
        _content.SizeFlagsHorizontal = Control.SizeFlags.ExpandFill;
        scroll.AddChild(_content);

        var brand = new Label { Text = "BEATFORGE", HorizontalAlignment = HorizontalAlignment.Center };
        brand.AddThemeFontSizeOverride("font_size", 42);
        brand.AddThemeColorOverride("font_color", _accent);
        _content.AddChild(brand);
        var subtitle = new Label { Text = "RHYTHM LAB  •  OFFLINE FIRST", HorizontalAlignment = HorizontalAlignment.Center };
        subtitle.AddThemeFontSizeOverride("font_size", 15);
        subtitle.AddThemeColorOverride("font_color", new Color("a9b4df"));
        _content.AddChild(subtitle);

        _content.AddChild(Section("PLAY", _purple));
        AddButton("PLAY PULSE GARDEN", () => ShowGameplay());
        AddButton("SONG LIBRARY", () => ShowLibrary());
        AddButton("PRACTICE MODE", () => ShowGameplay(true));
        _content.AddChild(Section("CREATE", _accent));
        AddButton("CHART EDITOR", () => ShowEditor());
        AddButton("IMPORT CHART", () => SetStatus("Import is ready for .json and .beatforge files in the next Android file-picker slice."));
        _content.AddChild(Section("PERSONALIZE", new Color("f4b942")));
        AddButton("SETTINGS & ACCESSIBILITY", () => ShowSettings());
        AddButton("ABOUT & SUPPORT", () => ShowAbout());
        _status = new Label { Text = "Ready. No account required.", AutowrapMode = TextServer.AutowrapMode.WordSmart };
        _status.AddThemeColorOverride("font_color", new Color("a9b4df"));
        _content.AddChild(_status);
        var footer = new Label { Text = "Made by the Sanskar  •  Support: buymeacoffee.com/sanskarIN", HorizontalAlignment = HorizontalAlignment.Center };
        footer.AddThemeFontSizeOverride("font_size", 12);
        footer.AddThemeColorOverride("font_color", new Color("7682ad"));
        _content.AddChild(footer);
    }

    private void ShowGameplay(bool practice = false)
    {
        ClearScreen();
        var game = new RhythmGame(_chart, practice, NavigateHome);
        AddChild(game);
    }

    private void ShowEditor()
    {
        ClearScreen();
        AddChild(new ChartEditorView(_chart, NavigateHome));
    }

    private void ShowLibrary()
    {
        ClearScreen();
        var root = CreateScreen("SONG LIBRARY", "Search, filter, and play your offline charts.");
        var search = new LineEdit { PlaceholderText = "Search title, artist, or tag…" };
        root.AddChild(search);
        var card = new PanelContainer();
        card.AddThemeStyleboxOverride("panel", CardStyle(_panel, 18));
        var info = new VBoxContainer();
        info.AddThemeConstantOverride("separation", 6);
        card.AddChild(info);
        info.AddChild(new Label { Text = _chart.Metadata.Title });
        info.AddChild(new Label { Text = $"{_chart.Metadata.Artist}  •  {_chart.Metadata.Difficulty} { _chart.Metadata.DifficultyRating }" });
        info.AddChild(new Label { Text = $"{_chart.Notes.Count} notes  •  { _chart.InitialBpm:0 } BPM  •  Original / Offline" });
        var play = AddButton("PLAY", () => ShowGameplay(), info);
        play.SizeFlagsHorizontal = Control.SizeFlags.ExpandFill;
        root.AddChild(card);
        root.AddChild(AddButton("BACK", NavigateHome));
    }

    private void ShowSettings()
    {
        ClearScreen();
        var root = CreateScreen("SETTINGS", "Make the playfield feel right for you.");
        var reduced = new CheckButton { Text = "Reduced visual effects", ButtonPressed = _reducedEffects };
        reduced.Toggled += value => { _reducedEffects = value; SetStatus(value ? "Reduced effects enabled." : "Full effects enabled."); };
        root.AddChild(reduced);
        root.AddChild(new HSlider { MinValue = 0.5, MaxValue = 1.5, Step = 0.05, Value = 1.0, TooltipText = "Note size" });
        root.AddChild(new Label { Text = "Note size   100%" });
        root.AddChild(new CheckButton { Text = "Large-button mode" });
        root.AddChild(new CheckButton { Text = "Vibration feedback" });
        root.AddChild(new Label { Text = "Audio offset: 0 ms\nInput offset: 0 ms\nCalibration history: none", AutowrapMode = TextServer.AutowrapMode.WordSmart });
        root.AddChild(AddButton("BACK", NavigateHome));
    }

    private void ShowAbout()
    {
        ClearScreen();
        var root = CreateScreen("ABOUT BEATFORGE", "An original, open rhythm lab for players and creators.");
        root.AddChild(new Label { Text = "BeatForge is free and open source under Apache-2.0.\n\nCore gameplay, local charts, and the editor are never locked behind donations.\n\nSupport development:\nbuymeacoffee.com/sanskarIN\n\nMade by the Sanskar", AutowrapMode = TextServer.AutowrapMode.WordSmart });
        root.AddChild(AddButton("BACK", NavigateHome));
    }

    private VBoxContainer CreateScreen(string title, string description)
    {
        AddColorRect(_background, new Vector2(0, 0), new Vector2(1, 1));
        var margin = new MarginContainer();
        margin.SetAnchorsAndOffsetsPreset(Control.LayoutPreset.FullRect);
        margin.AddThemeConstantOverride("margin_left", 48); margin.AddThemeConstantOverride("margin_right", 48);
        margin.AddThemeConstantOverride("margin_top", 54); margin.AddThemeConstantOverride("margin_bottom", 42);
        AddChild(margin);
        var root = new VBoxContainer(); root.AddThemeConstantOverride("separation", 18); margin.AddChild(root);
        var titleLabel = new Label { Text = title }; titleLabel.AddThemeFontSizeOverride("font_size", 34); titleLabel.AddThemeColorOverride("font_color", _accent); root.AddChild(titleLabel);
        root.AddChild(new Label { Text = description, AutowrapMode = TextServer.AutowrapMode.WordSmart });
        return root;
    }

    private Button AddButton(string text, Action action, Node? parent = null)
    {
        var button = new Button { Text = text, CustomMinimumSize = new Vector2(0, 60), FocusMode = Control.FocusModeEnum.All };
        button.AddThemeFontSizeOverride("font_size", 18); button.Pressed += action;
        (parent ?? _content).AddChild(button); return button;
    }

    private Label Section(string text, Color color) { var label = new Label { Text = text }; label.AddThemeColorOverride("font_color", color); label.AddThemeFontSizeOverride("font_size", 14); return label; }
    private void SetStatus(string text) { if (IsInstanceValid(_status)) _status.Text = text; }
    private void ClearScreen() { foreach (var child in GetChildren()) child.QueueFree(); }
    private void NavigateHome() { ClearScreen(); CallDeferred(nameof(BuildMenu)); }
    private void AddColorRect(Color color, Vector2 anchor, Vector2 size) { var rect = new ColorRect { Color = color }; rect.SetAnchorsAndOffsetsPreset(Control.LayoutPreset.FullRect); AddChild(rect); MoveChild(rect, 0); }
    private static StyleBoxFlat CardStyle(Color color, int radius) { var style = new StyleBoxFlat { BgColor = color, CornerRadiusTopLeft = radius, CornerRadiusTopRight = radius, CornerRadiusBottomLeft = radius, CornerRadiusBottomRight = radius }; style.ContentMarginLeft = 18; style.ContentMarginRight = 18; style.ContentMarginTop = 18; style.ContentMarginBottom = 18; return style; }
}
