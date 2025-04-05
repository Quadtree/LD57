using System;
using System.Linq;
using Godot;

public partial class Chariot : Node3D
{
    [Export]
    float BuoyancyChangeRate;

    [Export]
    float MaxBuoyancy;

    [Export]
    float MinBuoyancy;

    [Export]
    float LeftRightRatio = 3;

    [Export]
    public float Health = 100;

    RigidBody3D CurrentlyGrabbed;

    Vector3 PrevVelocity;
    float PrevRotation;

    float RecentCollision = 0;

    public override void _Ready()
    {
        base._Ready();

        var myParts = this.FindChildrenByType<RigidBody3D>().ToArray();

        foreach (var it in myParts)
        {
            it.ContactMonitor = true;
            it.MaxContactsReported = 20;

            it.BodyEntered += (body) =>
            {
                if (!myParts.Contains(body))
                {
                    RecentCollision = 0.1f;

                    // var otherSpeed = new Vector3(0, 0, 0);
                    // if (body is RigidBody3D rb3d)
                    // {
                    //     otherSpeed = rb3d.LinearVelocity;
                    // }
                    // var relSpeed = otherSpeed.DistanceTo(it.LinearVelocity);

                    // var damage = Util.Clamp((relSpeed - 0.1f) * (5f / .2f), 0, 1000);

                    // GD.Print($"{it} has hit {body}, relSpeed={relSpeed} damage={damage}");

                    // Health -= damage;
                }
            };
        }
    }

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);

        var moveVector = new Vector2(0, 0);

        var mbb = this.FindChildByName<Buoyancy>("MainBodyBuoyancy");
        var oldB = mbb.Amount;

        if (Input.IsActionPressed("move_left")) moveVector.X = -1;
        if (Input.IsActionPressed("move_right")) moveVector.X = 1;
        if (Input.IsActionPressed("move_up"))
        {
            moveVector.Y = 1 / LeftRightRatio;
            mbb.Amount = Util.Clamp(mbb.Amount + ((float)delta * BuoyancyChangeRate), MinBuoyancy, MaxBuoyancy);
        }
        if (Input.IsActionPressed("move_down"))
        {
            moveVector.Y = -1 / LeftRightRatio;
            mbb.Amount = Util.Clamp(mbb.Amount - ((float)delta * BuoyancyChangeRate), MinBuoyancy, MaxBuoyancy);
        }

        this.FindChildByName<RigidBody3D>("SeaHorse").ApplyCentralForce(new Vector3(moveVector.X, moveVector.Y, 0) * 300);

        if (Input.IsActionPressed("increase_buoyancy")) mbb.Amount = Util.Clamp(mbb.Amount + ((float)delta * BuoyancyChangeRate), MinBuoyancy, MaxBuoyancy);
        if (Input.IsActionPressed("decrease_buoyancy")) mbb.Amount = Util.Clamp(mbb.Amount - ((float)delta * BuoyancyChangeRate), MinBuoyancy, MaxBuoyancy);
        if (mbb.Amount != oldB)
        {
            GD.Print($"mbb.Amount={mbb.Amount}");
        }

        this.FindChildByName<Node3D>("DriverHeadMesh").Scale = Vector3.One * (Mathf.Pow(mbb.Amount, .333333f) / 5);

        // move camera to follow head
        var cam = this.FindChildByName<Node3D>("ChaseCameraArm");
        var head = this.FindChildByName<RigidBody3D>("DriverHead");

        var camDest = head.GlobalPosition + head.LinearVelocity * 2f;

        cam.GlobalPosition = cam.GlobalPosition * .99f + camDest * .01f;



        // grabbed

        if (!Input.IsActionPressed("grab") || !IsInstanceValid(CurrentlyGrabbed)) CurrentlyGrabbed = null;

        if (CurrentlyGrabbed != null)
        {
            var distToHead = head.GlobalPosition.DistanceTo(CurrentlyGrabbed.GlobalPosition);
            if (distToHead > 8)
            {
                GD.Print("Got too far away!");
                CurrentlyGrabbed = null;
            }
            else
            {
                var cursorPosition = Picking.PickPlaneAtCursor(this, new Plane(
                    new Vector3(0, 0, 0),
                    new Vector3(1, 0, 0),
                    new Vector3(0, 1, 0)
                ));

                GD.Print($"cursorPosition={cursorPosition}");
                if (cursorPosition != null)
                {
                    CurrentlyGrabbed.ApplyCentralForce((cursorPosition.Value - CurrentlyGrabbed.GlobalPosition).Normalized() * 40);
                }
            }
        }

        // impact detection
        var mb = this.FindChildByName<RigidBody3D>("MainBody");

        var changeInVelocity = PrevVelocity.DistanceTo(mb.LinearVelocity);
        if (changeInVelocity > 0.1f && RecentCollision > 0)
        {
            var damage = changeInVelocity * (5f / .3f);

            GD.Print($"changeInVelocity={changeInVelocity} damage={damage}");

            Health -= damage;
        }

        PrevVelocity = mb.LinearVelocity;
        PrevRotation = mb.Rotation.Z;
        RecentCollision -= (float)delta;
    }

    public override void _Process(double delta)
    {
        base._Process(delta);


    }

    public override void _UnhandledInput(InputEvent @event)
    {
        base._UnhandledInput(@event);

        if (@event is InputEventMouseButton)
        {
            if (@event.IsActionPressed("grab"))
            {
                var picked = Picking.PickObjectAtCursor(this);
                GD.Print($"picked={picked}");
                if (picked is RigidBody3D)
                {
                    CurrentlyGrabbed = (RigidBody3D)picked;
                }
            }
        }
    }

    public Vector3 MainBodyPos => this.FindChildByName<Node3D>("MainBody").GlobalPosition;
}
