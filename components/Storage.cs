using System;
using Godot;

public partial class Storage : RigidBody3D
{
    public void EnterStorage(Grabbable g)
    {
        Mass += g.Mass;
        // @TODO: Change buoyancy

        Chariot.KelpCollected += g.ValueInKelp;
        Chariot.SuperGemsCollected += g.ValueInSuperGems;

        g.QueueFree();
    }
}
