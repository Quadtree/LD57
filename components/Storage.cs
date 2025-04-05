using System;
using Godot;

public partial class Storage : RigidBody3D
{
    public void EnterStorage(Grabbable g)
    {
        g.QueueFree();
    }
}
