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

        var cam = GetViewport().GetCamera3D();
        if (cam != null)
        {
            var camDepth = -cam.GlobalPosition.Y + YAtStartingDepth + StartingDepth;

            var lightEnergy = Util.Clamp(1.2f - (camDepth / 100), 0.05f, 1);

            this.FindChildByType<DirectionalLight3D>().LightEnergy = lightEnergy;
            this.FindChildByType<WorldEnvironment>().Environment.AmbientLightEnergy = 0.4f * lightEnergy;
            GD.Print($"Light energy {lightEnergy}");
        }
        else
        {
            GD.PushWarning("Cant find camera?");
        }
    }
}
