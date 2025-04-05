using System;
using Godot;

public partial class AggressiveFish : RigidBody3D
{
    [Export]
    float Speed;

    [Export]
    float DamagePerHit;

    [Export]
    float AggroRange;

    float BackOffTime;
    float LeashingTime;

    Vector3 StartingPosition;

    float LeashRange;

    bool Aggroed;

    public override void _Ready()
    {
        base._Ready();

        StartingPosition = GlobalPosition;
    }

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);

        var chariot = GetTree().CurrentScene.FindChildByType<Chariot>();
        if (chariot != null)
        {
            if (chariot.MainBodyPos.DistanceTo(GlobalPosition) < AggroRange)
            {
                Aggroed = true;
            }
        }

        var targetSpeed = new Vector3(0, 0, 0);

        if (chariot != null && Aggroed && LeashingTime <= 0)
        {
            if (BackOffTime <= 0)
            {
                targetSpeed = (chariot.GlobalPosition - GlobalPosition).Normalized() * Speed;
            }
            else
            {
                targetSpeed = (GlobalPosition - chariot.GlobalPosition).Normalized() * Speed;
            }
        }

        ApplyForce((targetSpeed - LinearVelocity) * 100, ToGlobal(new Vector3(0, 0, 0)));
    }
}
