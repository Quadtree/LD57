using System;
using Godot;

public partial class TitleScreen : Node3D
{
    public override void _Ready()
    {
        base._Ready();

        GetTree().Paused = true;
    }
}
