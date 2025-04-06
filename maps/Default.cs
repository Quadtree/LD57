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

    public static readonly bool EnableCheats = OS.HasFeature("editor");

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

            if (CurrentDepth <= 15)
            {
                var chariot = this.FindChildByType<Chariot>();

                if (this.FindChildByPredicate<Node3D>(it => it.HasMeta("light_warning")) == null)
                {
                    var thoughBubble = GD.Load<PackedScene>("res://actors/ThoughtBubble.tscn").Instantiate<Node3D>();
                    thoughBubble.SetMeta("light_warning", "1");
                    thoughBubble.Scale = new Vector3(.4f, .4f, .4f);
                    GetTree().CurrentScene.AddChild(thoughBubble);
                    thoughBubble.GlobalPosition = (chariot?.MainBodyPos ?? Vector3.Zero) + new Vector3(0, 5, 0);
                    thoughBubble.FindChildByType<Label3D>().Text = "The daylight burns!\nI've got to get back to the depths!";
                }

                lightEnergy += (25 - (CurrentDepth ?? 0)) * 2;

                if (chariot != null)
                {
                    chariot.Health -= 10 * (float)delta;
                }

                if (CurrentDepth <= 5)
                {
                    chariot.Health -= 1000 * (float)delta;
                }
            }

            this.FindChildByType<DirectionalLight3D>().LightEnergy = lightEnergy;
            this.FindChildByType<WorldEnvironment>().Environment.AmbientLightEnergy = 0.4f * lightEnergy;
            //GD.Print($"Light energy {lightEnergy}");
        }
        else
        {
            GD.PushWarning("Cant find camera?");
        }
    }
}
