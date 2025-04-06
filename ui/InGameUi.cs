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
    }
}
