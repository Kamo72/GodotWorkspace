using Godot;
using System;

public partial class CameraManager : Camera2D
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		//DisplayServer.WindowSetMode(DisplayServer.WindowMode.Fullscreen);
		//GetTree().Root.Size = new Vector2I(10, 10);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		// GD.Print("MAX FPS : " + Godot.Engine.MaxFps);
		// GD.Print("NOW FPS : " + Godot.Engine.GetFramesPerSecond());
	}
}
