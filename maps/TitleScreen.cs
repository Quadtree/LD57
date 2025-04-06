using System;
using Godot;

public partial class TitleScreen : Node3D
{
    public override void _Ready()
    {
        base._Ready();

        GetTree().Paused = true;

        for (var i = 0; i <= 4; ++i)
        {
            var li = i;
            this.FindChildByName<Button>($"Level{i}Button").Pressed += () => { GetTree().Paused = false; GetTree().ChangeSceneToFile($"res://maps/Level{li}.tscn"); };
        }
    }
}
