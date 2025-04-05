using System;
using Godot;

public partial class Buoyancy : Node
{
    [Export]
    float Amount = 0;

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);

        this.FindParentByType<RigidBody3D>().ApplyCentralForce(Vector3.Up * Amount);
    }
}
