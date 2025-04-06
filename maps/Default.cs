using System;
using Godot;

public partial class Default : Node3D
{
    [Export]
    float StartingDepth;

    [Export]
    int KelpPar;

    [Export]
    int SuperGemsPar;

    [Export]
    int TimeParSeconds;

    float YAtStartingDepth;

    public float? CurrentDepth
    {
        get
        {
            var chariot = this.FindChildByType<Chariot>();
            if (chariot != null)
            {
                return chariot.MainBodyPos.Y - YAtStartingDepth + StartingDepth;
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
}
