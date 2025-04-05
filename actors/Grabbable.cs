using System;
using Godot;

public partial class Grabbable : RigidBody3D
{
    public override void _Ready()
    {
        base._Ready();

        BodyEntered += (body) =>
        {
            if (body is Storage storage)
            {
                storage.EnterStorage(this);
            }
        };
    }
}
