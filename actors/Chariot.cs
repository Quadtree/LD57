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

    [Export]
    public float SlapRange = 8;

    [Export]
    public float GrabRange = 8;

    RigidBody3D CurrentlyGrabbed;

    Vector3 PrevVelocity;
    float PrevRotation;

    float RecentCollision = 0;

    [Export]
    public float SlapCooldown = 2;

    float SlapCooldownLeft = 0;

    [Export]
    public float InvulnerableTime = 2;

    [Export]
    public float StartingMainBuoyancy = 100;

    public override void _Ready()
    {
        base._Ready();

        this.FindChildByName<Node>("DriverHead").FindChildByType<Buoyancy>().Amount = Util.Clamp(StartingMainBuoyancy, MinBuoyancy, MaxBuoyancy);

        var myParts = this.FindChildrenByType<RigidBody3D>().ToArray();

        foreach (var it in myParts)
        {
            if (it.Name == "SeaHorse") continue;

            it.ContactMonitor = true;
            it.MaxContactsReported = 20;

            it.BodyEntered += (body) =>
            {
                if (!myParts.Contains(body) && !(body is AggressiveFish))
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
            //GD.Print($"mbb.Amount={mbb.Amount}");
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
            if (distToHead > GrabRange)
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
        if (InvulnerableTime <= 0 && changeInVelocity > 0.2f && RecentCollision > 0)
        {
            var damage = changeInVelocity * (5f / .3f);

            GD.Print($"changeInVelocity={changeInVelocity} damage={damage}");

            Health -= damage;
        }

        PrevVelocity = mb.LinearVelocity;
        PrevRotation = mb.Rotation.Z;
        RecentCollision -= (float)delta;
        SlapCooldownLeft -= (float)delta;
        InvulnerableTime -= (float)delta;
    }

    public override void _Process(double delta)
    {
        base._Process(delta);


    }

    public override void _Input(InputEvent @event)
    {
        base._Input(@event);

        if (@event is InputEventKey)
        {
            if (@event.IsActionPressed("exit_game")) GetTree().Quit();
        }

        if (@event is InputEventMouseButton)
        {
            if (@event.IsActionPressed("grab"))
            {
                var picked = Picking.PickObjectAtCursor(this);
                GD.Print($"picked={picked}");
                if (picked is Grabbable)
                {
                    GD.Print("Grabbed something");
                    CurrentlyGrabbed = (Grabbable)picked;
                }
                else if (SlapCooldownLeft <= 0)
                {
                    // maybe slap a shark?
                    var cursorPosition = Picking.PickPlaneAtCursor(this, new Plane(
                        new Vector3(0, 0, 0),
                        new Vector3(1, 0, 0),
                        new Vector3(0, 1, 0)
                    ));

                    if (cursorPosition != null)
                    {
                        var head = this.FindChildByName<RigidBody3D>("DriverHead");

                        var nearestShark = GetTree().CurrentScene.FindChildrenByType<AggressiveFish>().MinBy(it => it.GlobalPosition.DistanceTo(cursorPosition.Value));

                        if (nearestShark != null)
                        {
                            if (nearestShark.GlobalPosition.DistanceTo(head.GlobalPosition) <= SlapRange)
                            {
                                GD.Print($"SLAPPED {nearestShark}");
                                SlapCooldownLeft = SlapCooldown;

                                Util.StartOneShotTimer(this, 0.2f, () =>
                                {
                                    nearestShark.Slapped();
                                });
                            }
                            else
                            {
                                GD.Print("Out of SLAP range!");
                            }
                        }
                        else
                        {
                            GD.Print("No shark to slap");
                        }
                    }
                    else
                    {
                        GD.Print("Weird picking error?");
                    }
                }
            }
        }
    }

    public Vector3 MainBodyPos => this.FindChildByName<Node3D>("MainBody").GlobalPosition;
}
