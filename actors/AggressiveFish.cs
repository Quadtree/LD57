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

    Vector3 StartingPosition;

    [Export]
    float BackOffTimeSetTo = 3;

    [Export]
    float LeashRange;

    public bool Aggroed;

    [Export]
    float SlapEffectiveness = 1;

    public bool IsSideDefense;

    public override void _Ready()
    {
        base._Ready();

        StartingPosition = GlobalPosition;

        ContactMonitor = true;
        MaxContactsReported = 20;

        BodyEntered += (body) =>
        {
            if (BackOffTime > 0) return;

            var chariot = body.FindParentByType<Chariot>();
            if (chariot != null)
            {
                chariot.Health -= DamagePerHit;
                BackOffTime = BackOffTimeSetTo;
            }
        };

        //foreach (var it in this.FindChildrenByType<Label3D>()) it.Visible
    }

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);

        var chariot = GetTree().CurrentScene.FindChildByType<Chariot>();
        if (chariot != null)
        {
            if (chariot.MainBodyPos.DistanceTo(GlobalPosition) < AggroRange)
            {
                if (!Aggroed) GD.Print($"{this} has been aggroed");
                Aggroed = true;
            }
        }

        var targetSpeed = new Vector3(0, 0, 0);

        if (chariot == null || chariot.MainBodyPos.DistanceTo(StartingPosition) > LeashRange) Aggroed = false;

        if (chariot != null && Aggroed)
        {
            if (BackOffTime <= 0)
            {
                targetSpeed = (chariot.MainBodyPos - GlobalPosition).Normalized() * Speed;
            }
            else
            {
                targetSpeed = (GlobalPosition - chariot.MainBodyPos).Normalized() * Speed;
            }
        }
        else
        {
            targetSpeed = (StartingPosition - GlobalPosition).Normalized() * Speed;
        }

        var force = (targetSpeed - LinearVelocity) * 100;

        if (force.Length() > 10_000) force = force.Normalized() * 10_000;

        ApplyForce(force, new Transform3D(Basis, new Vector3(0, 0, 0)) * new Vector3(-1, 0, 0));

        this.FindChildByType<Label3D>().Text = $"force={force} BackOffTime={BackOffTime} Aggroed={Aggroed}";

        BackOffTime -= (float)delta;
    }

    public void Slapped()
    {
        BackOffTime = 3 * SlapEffectiveness;
        SlapEffectiveness *= .66666f;
    }
}
