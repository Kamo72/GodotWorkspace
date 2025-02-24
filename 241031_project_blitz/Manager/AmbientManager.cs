using Godot;
using System;
using System.Collections.Generic;
using System.IO;

public partial class AmbientManager : Node2D
{
    private AudioStreamPlayer _audioStreamPlayer;
    private List<string> _audioFiles = new List<string>();
    private Random _random = new Random();

    public override void _Ready()
    {
        _audioStreamPlayer = new AudioStreamPlayer();
        AddChild(_audioStreamPlayer);
        LoadAudioFiles("res://Asset/SFX-NuclearWinter/");
        PlayRandomAudio();
    }

    private void LoadAudioFiles(string directoryPath)
    {
        var dir = DirAccess.Open(directoryPath);
        if (dir == null)
        {
            GD.PrintErr($"Failed to open directory: {directoryPath}");
            return;
        }
        dir.ListDirBegin();
        string fileName;
        while ((fileName = dir.GetNext()) != "")
        {
            if (fileName.EndsWith(".mp3"))
            {
                _audioFiles.Add(directoryPath + fileName);
            }
        }
        dir.ListDirEnd();
    }

    private void PlayRandomAudio()
    {
        if (_audioFiles.Count == 0)
        {
            GD.PrintErr("No audio files found to play.");
            return;
        }

        var randomFile = _audioFiles[_random.Next(_audioFiles.Count)];
        var audioStream = GD.Load<AudioStream>(randomFile);
        if (audioStream == null)
        {
            GD.PrintErr($"Failed to load audio file: {randomFile}");
            return;
        }
        _audioStreamPlayer.Stream = audioStream;
        _audioStreamPlayer.VolumeDb = -15f;
        _audioStreamPlayer.Play();

        _audioStreamPlayer.Finished += PlayRandomAudio;
    }
}
