using System;
using Godot;

public partial class Grabbable : RigidBody3D
{
    [Export]
    public float Buoyancy;

    public override void _Ready()
    {
        base._Ready();

        ContactMonitor = true;
        MaxContactsReported = 20;

        BodyEntered += (body) =>
        {
            if (GetParent() == null) return;

            if (body is Storage storage)
            {
                storage.EnterStorage(this);
            }
        };
    }
}
