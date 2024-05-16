using Godot;
using System;

public partial class CameraManager : Camera2D
{
	Node2D target;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		target = GetTree().Root.FindByName("Player") as Node2D;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		// GD.Print("MAX FPS : " + Godot.Engine.MaxFps);
		// GD.Print("NOW FPS : " + Godot.Engine.GetFramesPerSecond());
	}

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);

		if(target != null)
			Position = target.Position;
		//else GD.Print("CameraManager : target == null!");
    }

	
	void ShakeProcess(float delta)
	{


	}
	public void GetShake(float power)
	{


	}




}
