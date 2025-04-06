using System;
using Godot;

public partial class LevelWinOverlay : Control
{
    public override void _Ready()
    {
        base._Ready();

        this.FindChildByName<Button>("ReplayLevelButton").Pressed += () => { GetTree().Paused = false; GetTree().ReloadCurrentScene(); };
        this.FindChildByName<Button>("ContinueButton").Pressed += () => { GetTree().Paused = false; GetTree().ChangeSceneToFile(GetTree().CurrentScene.FindChildByType<LevelBounds>().NextLevel); };

        if (GetTree().CurrentScene.FindChildByType<LevelBounds>().NextLevel == "res://maps/TitleScreen.tscn") this.FindChildByName<Button>("ContinueButton").Text = "Finish Game";

        var level = GetTree().Root.FindChildByType<Default>();
        var chariot = GetTree().Root.FindChildByType<Chariot>();

        if (level.KelpPar > 0)
            this.FindChildByName<Label>("KelpLabel").Text = $"Kelp Collected: {chariot?.KelpCollectedThisLevel}/{level.KelpPar}";
        else
            this.FindChildByName<Label>("KelpLabel").Visible = false;

        if ((chariot?.KelpCollectedThisLevel ?? 0) < level.KelpPar) this.FindChildByName<Label>("KelpLabel").Modulate = Colors.Red;

        if (level.SuperGemsPar > 0)
            this.FindChildByName<Label>("GemLabel").Text += $"Gems: {chariot?.SuperGemsCollectedThisLevel}/{level.SuperGemsPar}";
        else
            this.FindChildByName<Label>("GemLabel").Visible = false;

        if ((chariot?.SuperGemsCollectedThisLevel ?? 0) < level.SuperGemsPar) this.FindChildByName<Label>("GemLabel").Modulate = Colors.Red;

        this.FindChildByName<Label>("TimeLabel").Text = $"{InGameUi.FormatSeconds((int)level.TimeElapsed)}/{InGameUi.FormatSeconds(level.TimeParSeconds)}";

        if (level.TimeElapsed > level.TimeParSeconds) this.FindChildByName<Label>("TimeLabel").Modulate = Colors.Red;


    }
}
