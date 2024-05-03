using Godot;
using System;

public partial class CameraManager : Camera2D
{

	Node2D target;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		target = GetTree().Root.GetNode("Player") as Node2D;
	}	

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);

		target = GetTree().Root.GetChild<Node2D>(0).FindChild("Player") as Node2D;
		Position = target.Position;
    }
}
