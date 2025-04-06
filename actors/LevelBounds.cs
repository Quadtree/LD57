using System;
using Godot;

public partial class LevelBounds : Node3D
{
    [Export]
    float Slosh = 5;

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);

        var chariot = GetTree().CurrentScene.FindChildByType<Chariot>();

        Vector2 halfExtent = new Vector2(0.5f * Scale.X, 0.5f * Scale.Y);

        if (chariot != null)
        {
            if (chariot.MainBodyPos.X <= -halfExtent.X + Slosh)
            {
                SpawnSideDefense(new Vector3(-halfExtent.X - 5, chariot.MainBodyPos.Y, 0));
            }
            else if (chariot.MainBodyPos.Y >= halfExtent.Y - Slosh)
            {
                SpawnSideDefense(new Vector3(chariot.MainBodyPos.X, halfExtent.Y + 5, 0));
            }
            // truly outside
            else if (chariot.MainBodyPos.Y >= halfExtent.Y)
            {
                SicSuperShark();
            }
        }
    }

    private void SicSuperShark()
    {
        var superShark = GetTree().CurrentScene.FindChildByPredicate<AggressiveFish>(it => it.IsSideDefense);
        superShark.Aggroed = true;
    }


    private void SpawnSideDefense(Vector3 pos)
    {
        if (GetTree().CurrentScene.FindChildByPredicate<AggressiveFish>(it => it.IsSideDefense) != null) return;

        var superShark = GD.Load<PackedScene>("res://actors/AFSuperShark.tscn").Instantiate<AggressiveFish>();
        GetTree().CurrentScene.AddChild(superShark);
        superShark.IsSideDefense = true;
        superShark.GlobalPosition = pos;

    }
}
