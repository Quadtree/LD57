using System;
using Godot;

public partial class Default : Node3D
{
    [Export]
    float StartingDepth;

    [Export]
    public int KelpPar;

    [Export]
    public int SuperGemsPar;

    [Export]
    public int TimeParSeconds;

    public float TimeElapsed;

    float YAtStartingDepth;

    public float? CurrentDepth
    {
        get
        {
            var chariot = this.FindChildByType<Chariot>();
            if (chariot != null)
            {
                return -chariot.MainBodyPos.Y + YAtStartingDepth + StartingDepth;
            }
            else
            {
                return null;
            }
        }
    }

    public override void _Ready()
    {
        base._Ready();

        YAtStartingDepth = this.FindChildByType<Chariot>().MainBodyPos.Y;
    }

    public override void _Process(double delta)
    {
        base._Process(delta);

        TimeElapsed += (float)delta;
    }
}
