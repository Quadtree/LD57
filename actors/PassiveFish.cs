using Godot;
using System;

public partial class PassiveFish : RigidBody3D
{
    [Export]
    public float Speed;

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);

        
    }
}
