using System;
using Godot;

public partial class Default : Node3D
{
    [Export]
    float StartingDepth;

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
