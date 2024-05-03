using Godot;
using System;

public partial class playerAddon : CharacterBody2D
{
	float speed = 500, jumpSpeed = 500;

	AnimationPlayer animPlayer;
	Sprite2D sprite;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		animPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
		sprite = GetNode<Sprite2D>("Sprite");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		//Position += new Vector2((float)delta * 10f, 0f);

		float moveValue = 0f;

		if (Input.IsKeyPressed(Key.Left))
			moveValue += -1.0f;
		if (Input.IsKeyPressed(Key.Right))
			moveValue += +1.0f;
		
		Velocity = new Vector2(moveValue * speed, Velocity.Y);

		string animName = "";
		if(IsOnFloor())
			if(Mathf.IsZeroApprox(moveValue))
				animName = "idle";
			else
				animName = "run";
		else
			animName = "air";
		
		if(animName != "") animPlayer.Play(animName);


		if (Mathf.IsZeroApprox(moveValue) == false)
			if(moveValue * sprite.Scale.X < 0.0)
				sprite.Scale *= new Vector2(-1.0f, 1.0f);


	}

    public override void _PhysicsProcess(double delta)
    {
		if(IsOnFloor())
		{
			Velocity = new Vector2(Velocity.X, Input.IsKeyPressed(Key.Space)? -jumpSpeed : 0f);
		}
		else
		{
			Velocity += new Vector2( 0f, 980f * (float)delta);
		}

        base._PhysicsProcess(delta);
		MoveAndSlide();
    }
}