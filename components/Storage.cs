using System;
using Godot;

public partial class Storage : RigidBody3D
{
    public void EnterStorage(Grabbable g)
    {
        Mass += g.Mass;

        Chariot.KelpCollectedTotal += g.ValueInKelp;
        Chariot.SuperGemsCollectedTotal += g.ValueInSuperGems;

        this.FindParentByType<Chariot>().KelpCollectedThisLevel += g.ValueInKelp;
        this.FindParentByType<Chariot>().SuperGemsCollectedThisLevel += g.ValueInSuperGems;

        g.QueueFree();
    }
}
