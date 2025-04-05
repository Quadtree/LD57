using System;
using Godot;

public partial class PassiveFish : RigidBody3D
{
    [Export]
    public float Speed;

    [Export]
    public float Acceleration;

    float CurrentTurnRate;

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);

        var targetSpeed = (ToGlobal(new Vector3(-1, 0, 0)) - GlobalPosition) * Speed;

        var changeInSpeed = (targetSpeed - LinearVelocity).Normalized() * Acceleration;

        GD.Print($"changeInSpeed={changeInSpeed} LinearVelocity={LinearVelocity}");

        ApplyCentralForce(changeInSpeed);

        if (Util.RandChance((float)delta / 4))
        {
            CurrentTurnRate = Util.RandF(-1f, 1f) * 1000;
        }

        if (Util.RandChance((float)delta * 4))
        {
            CurrentTurnRate = 0;
        }

        ApplyTorque(new Vector3(0, 0, CurrentTurnRate));
    }
}
