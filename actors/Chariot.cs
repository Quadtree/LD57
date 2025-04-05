using System;
using Godot;

public partial class Chariot : Node3D
{
    [Export]
    float BuoyancyChangeRate;

    [Export]
    float MaxBuoyancy;

    [Export]
    float MinBuoyancy;

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);

        var moveVector = new Vector2(0, 0);

        if (Input.IsActionPressed("move_left")) moveVector.X = -1;
        if (Input.IsActionPressed("move_right")) moveVector.X = 1;
        if (Input.IsActionPressed("move_up")) moveVector.Y = 1;
        if (Input.IsActionPressed("move_down")) moveVector.Y = -1;

        this.FindChildByName<RigidBody3D>("SeaHorse").ApplyCentralForce(new Vector3(moveVector.X, moveVector.Y, 0) * 300);

        var mbb = this.FindChildByName<Buoyancy>("MainBodyBuoyancy");
        var oldB = mbb.Amount;
        if (Input.IsActionPressed("increase_buoyancy")) mbb.Amount = Util.Clamp(mbb.Amount + ((float)delta * BuoyancyChangeRate), MinBuoyancy, MaxBuoyancy);
        if (Input.IsActionPressed("decrease_buoyancy")) mbb.Amount = Util.Clamp(mbb.Amount - ((float)delta * BuoyancyChangeRate), MinBuoyancy, MaxBuoyancy);
        if (mbb.Amount != oldB)
        {
            GD.Print($"mbb.Amount={mbb.Amount}");
        }

        // move camera to follow head
        var cam = this.FindChildByName<Node3D>("ChaseCameraArm");
        var head = this.FindChildByName<Node3D>("DriverHead");

        cam.GlobalPosition = cam.GlobalPosition * .99f + head.GlobalPosition * .01f;
    }

    public override void _Process(double delta)
    {
        base._Process(delta);


    }
}
