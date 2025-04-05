using Godot;
using System;

public partial class AggressiveFish : RigidBody3D
{
    [Export]
    float Speed;

    [Export]
    float DamagePerHit;

    float BackOffTime;

    public override void _Ready()
    {
        base._Ready();
    }

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);

        var chariot = GetTree().CurrentScene.FindChildByType<Chariot>();
        if (chariot != null)
        {
            if (chariot.Find)
        }
    }
}
