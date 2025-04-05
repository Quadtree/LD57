using System;
using Godot;

public partial class Chariot : Node3D
{
    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);

        var moveVector = new Vector2(0, 0);

        if (Input.IsActionPressed("move_left")) moveVector.X = -1;
        if (Input.IsActionPressed("move_right")) moveVector.X = 1;
        if (Input.IsActionPressed("move_up")) moveVector.Y = 1;
        if (Input.IsActionPressed("move_down")) moveVector.Y = -1;

        this.FindChildByName<RigidBody3D>("SeaHorse").ApplyCentralForce(new Vector3(moveVector.X, moveVector.Y, 0) * 300);
    }
}
