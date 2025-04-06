using System;
using Godot;

public partial class WaterJet : StaticBody3D
{
    [Export]
    float Range;

    [Export]
    float MaxForce;

    [Export]
    float MinForce;

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);

        foreach (var it in this.FindChildByType<Area3D>().GetOverlappingBodies())
        {
            if (it is RigidBody3D rb3d)
            {
                var rotator = new Transform3D(Basis, Vector3.Zero);

                rb3d.ApplyCentralForce(rotator * Vector3.Up * Mathf.Lerp(MinForce, MaxForce, rb3d.GlobalPosition.DistanceTo(GlobalPosition) / Range));
            }
        }
    }
}
