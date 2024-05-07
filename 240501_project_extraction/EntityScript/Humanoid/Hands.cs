using Godot;
using System;

public partial class Hands : Node2D
{
	public float direction = 0f;
	public float directionDegree 
	{
		get => direction / (float)Math.PI * 180f;
		set => direction = value / 180f * (float)Math.PI;
	};

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
