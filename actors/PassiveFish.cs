using System;
using Godot;

public partial class PassiveFish : RigidBody3D
{
    [Export]
    public float Speed;

    [Export]
    public float Acceleration;

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);

        var targetSpeed = (ToGlobal(Vector3.Forward) - GlobalPosition) * Speed;

        var changeInSpeed = (targetSpeed - LinearVelocity).Normalized() * Acceleration;

        GD.Print($"changeInSpeed={changeInSpeed}");

        ApplyCentralForce(changeInSpeed);
    }
}
