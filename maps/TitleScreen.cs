using System;
using Godot;

public partial class TitleScreen : Node3D
{
    public override void _Ready()
    {
        base._Ready();

        GetTree().Paused = true;

        for (var i = 0; i <= 4; ++i)
        {
            var li = i;
            this.FindChildByName<Button>($"Level{i}Button").Pressed += () =>
            {
                var musicPlayer = new AudioStreamPlayer();
                GetTree().Root.AddChild(musicPlayer);
                musicPlayer.Stream = GD.Load<AudioStream>("res://midi/bgm.ogg");
                musicPlayer.VolumeDb = -5;
                musicPlayer.ProcessMode = ProcessModeEnum.Always;
                musicPlayer.Play();

                GetTree().Paused = false;
                Util.StartOneShotTimer(this, 1f, () =>
                {
                    GetTree().ChangeSceneToFile($"res://maps/Level{li}.tscn");
                });
            };
        }
    }
}
