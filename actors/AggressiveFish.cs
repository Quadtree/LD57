using Godot;
using System;

public partial class AggressiveFish : RigidBody3D
{
    [Export]
    float Speed;

    [Export]
    float DamagePerHit;

    public override void _Ready()
    {
        base._Ready();
    }

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);

        
    }
}
