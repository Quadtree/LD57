using System;
using Godot;

public partial class Grabbable : RigidBody3D
{
    [Export]
    public float Buoyancy;

    [Export]
    public bool InitiallyFrozen;

    [Export]
    public int ValueInSuperGems = 0;

    [Export]
    public int ValueInKelp = 0;

    public override void _Ready()
    {
        base._Ready();

        AxisLockLinearX = InitiallyFrozen;
        AxisLockLinearY = InitiallyFrozen;
        AxisLockAngularZ = InitiallyFrozen;

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
