using Godot;
using System;

public partial class WaterJet : StaticBody3D
{
    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);

        foreach (var it in GetTree().CurrentScene.FindChildrenByType<RigidBody3D>())
        {
            var localPos = ToLocal(it.GlobalPosition);

            
        }
    }
}
