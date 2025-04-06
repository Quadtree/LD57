using System;
using Godot;

public partial class InGameUi : Control
{
    public override void _Process(double delta)
    {
        base._Process(delta);

        var chariot = GetTree().CurrentScene.FindChildByName<Chariot>("Chariot");

        if (chariot != null)
        {
            this.FindChildByName<TextureProgressBar>("HealthBar").Value = GetTree().CurrentScene.FindChildByName<Chariot>("Chariot").Health;
        }
        else
        {
            this.FindChildByName<TextureProgressBar>("HealthBar").Visible = false;
        }

        var level = GetTree().Root.FindChildByType<Default>();

        if (level != null && chariot != null)
        {
            var statusText = "";

            statusText += $"Depth: {(int)level.CurrentDepth}m";

            if (level.KelpPar > 0) statusText += $"    Kelp: {chariot.KelpCollectedThisLevel}/{level.KelpPar}";
            if (level.SuperGemsPar > 0) statusText += $"    Gems: {chariot.SuperGemsCollectedThisLevel}/{level.SuperGemsPar}";

            statusText += $"    {FormatSeconds((int)level.TimeElapsed)}/{FormatSeconds(level.TimeParSeconds)}";

            this.FindChildByName<Label>("StatusLabel").Text = statusText;
        }

        this.FindChildByName<Label>("StatusLabel").Visible = Default.EnableCheats;
    }

    public static string FormatSeconds(int seconds)
    {
        return $"{seconds / 60}:{seconds % 60:00}";
    }
}
