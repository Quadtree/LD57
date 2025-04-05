using System;
using Godot;

public partial class ThoughtZone : Node3D
{
    [Export]
    float Radius;

    [Export]
    string Text;

    bool Fired;

    public override void _Process(double delta)
    {
        base._Process(delta);

        var chariot = GetTree().CurrentScene.FindChildByType<Chariot>();

        if (!Fired && chariot != null && chariot.MainBodyPos.DistanceTo(GlobalPosition) < Radius)
        {
            var bubble = GD.Load<PackedScene>("res://actors/ThoughtBubble.tscn").Instantiate<Node3D>();
            GetTree().CurrentScene.AddChild(bubble);
            bubble.FindChildByType<Label3D>().Text = Text;
            bubble.Scale = new Vector3(0.424f, 0.424f, 0.424f);
            bubble.GlobalPosition = chariot.FindChildByName<Node3D>("DriverHead").GlobalPosition + new Vector3(0, 0, 1);
            Fired = true;
            QueueFree();
        }
    }
}
