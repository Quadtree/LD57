using System;
using Godot;

public partial class LevelBounds : Node3D
{
    [Export]
    float Slosh = 5;

    [Export]
    public string NextLevel;

    public override void _Ready()
    {
        base._Ready();

        Visible = false;
    }

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);

        var chariot = GetTree().CurrentScene.FindChildByType<Chariot>();

        Vector2 halfExtent = new Vector2(0.5f * Scale.X, 0.5f * Scale.Y);

        var left = GlobalPosition.X - halfExtent.X;
        var top = GlobalPosition.Y + halfExtent.Y;
        var right = GlobalPosition.X + halfExtent.X;
        var bottom = GlobalPosition.Y - halfExtent.Y;

        if (chariot != null)
        {
            StopSuperShark();

            if (chariot.MainBodyPos.Y >= top)
            {
                SicSuperShark();
            }
            else if (chariot.MainBodyPos.X >= right)
            {
                WinLevel();
            }
            else if (chariot.MainBodyPos.Y <= bottom)
            {
                WinLevel();
            }
            else if (chariot.MainBodyPos.X <= left)
            {
                SicSuperShark();
            }
            // near the edge
            else if (chariot.MainBodyPos.X <= left + Slosh)
            {
                SpawnSideDefense(new Vector3(left - 5, chariot.MainBodyPos.Y, 0));
            }
            else if (chariot.MainBodyPos.Y >= top - Slosh)
            {
                SpawnSideDefense(new Vector3(chariot.MainBodyPos.X, top + 5, 0));
            }
            else
            {
                GetTree().CurrentScene.FindChildByPredicate<AggressiveFish>(it => it.IsSideDefense)?.QueueFree();
            }
        }
    }

    public void WinLevel()
    {
        GetTree().Paused = true;
        GetTree().CurrentScene.FindChildByType<CanvasLayer>().AddChild(GD.Load<PackedScene>("res://ui/LevelWinOverlay.tscn").Instantiate<Control>());

        //GetTree().ChangeSceneToFile(NextLevel);
    }

    private void SicSuperShark()
    {
        var superShark = GetTree().CurrentScene.FindChildByPredicate<AggressiveFish>(it => it.IsSideDefense);
        if (superShark != null) superShark.Aggroed = true;
        //GD.Print("Siccing super shark!!!");
    }

    private void StopSuperShark()
    {
        var superShark = GetTree().CurrentScene.FindChildByPredicate<AggressiveFish>(it => it.IsSideDefense);
        if (superShark != null) superShark.Aggroed = false;
    }


    private void SpawnSideDefense(Vector3 pos)
    {
        if (GetTree().CurrentScene.FindChildByPredicate<AggressiveFish>(it => it.IsSideDefense) != null) return;

        var superShark = GD.Load<PackedScene>("res://actors/AFSuperShark.tscn").Instantiate<AggressiveFish>();
        superShark.Position = pos;
        GetTree().CurrentScene.AddChild(superShark);
        superShark.IsSideDefense = true;
        GD.Print("Spawning super shark!!!");
    }
}
