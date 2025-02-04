using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public partial class Sound : AudioStreamPlayer2D
{
    //public float decibel;   //소음 전달 정도 및 크기
    public Humanoid source = null;
    public float danger;    //위협 레벨
    public AudioEffect audioEffect;

    public static void Make(Humanoid source, Vector2 pos, float distance, float danger, string rscPath, float volume = 1f)
    {
        var sound = new Sound()
        {
            VolumeDb = volume,
            MaxDistance = distance,
            danger = danger,
            Stream = ResourceLoader.Load<AudioStream>(rscPath),
        };
        WorldManager.instance.AddChild(sound);
        sound.GlobalPosition = pos;
        sound.Play();
    }

    public static void MakeSelf(Humanoid source, Vector2 pos, float distance, float danger, string rscPath, float volume = 1f)
    {
        var sound = new Sound()
        {
            VolumeDb = volume,
            MaxDistance = distance,
            danger = danger,
            Stream = ResourceLoader.Load<AudioStream>(rscPath),
        };
        source.AddChild(sound);
        sound.GlobalPosition = pos;
        sound.Play();
    }

    public override void _Process(double delta)
    {
        if (Stream != null && !Playing && danger != 0f)
        {
            QueueFree();
        }
    }

    public override void _EnterTree()
    {
        WorldManager.sounds.Add(this);
    }
    public override void _ExitTree()
    {
        WorldManager.sounds.Remove(this);
    }

}