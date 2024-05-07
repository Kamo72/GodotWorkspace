using Godot;
using System;
using ItemLib;

public partial class Hands : Node2D
{
	public float direction = 0f;
	public float directionDegree 
	{
		get => direction / (float)Math.PI * 180f;
		set => direction = value / 180f * (float)Math.PI;
	}

	Node2D weapon = null;

	public override void _Ready()
	{
	}

	public override void _Process(double delta)
	{
		Rotation = direction;
		Scale = new Vector2(1f, Math.Abs(directionDegree) < 90f? 1f : -1f);
	}

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);
    }


	public bool GetHandable(Handable handable)
	{
		return false;

	}
}
