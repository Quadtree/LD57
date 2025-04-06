using System;
using Godot;

public partial class DebrisOverlay : Node3D
{
    public override void _Process(double delta)
    {
        base._Process(delta);

        var camPos = GetViewport().GetCamera3D().GlobalPosition;
        var zone = new Vector2I(
            Mathf.RoundToInt(camPos.X / 20),
            Mathf.RoundToInt(camPos.Y / 20)
        );

        GlobalPosition = new Vector3(
            zone.X * 20,
            zone.Y * 20,
            0
        );
    }
}
