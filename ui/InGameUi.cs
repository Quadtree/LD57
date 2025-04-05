using System;
using Godot;

public partial class InGameUi : Control
{
    public override void _Process(double delta)
    {
        base._Process(delta);

        this.FindChildByName<ProgressBar>("HealthBar").Value = GetTree().CurrentScene.FindChildByName<Chariot>("Chariot").Health;
    }
}
