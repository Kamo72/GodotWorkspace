using Godot;
using System;


public partial class Humanoid : CharacterBody2D
{
	public readonly float moveSpeed = 500f, friction = 0.98f;

	//angleValue
	public float aimSpeed = 0.02f;
	public Vector2 aimNow = Vector2.Zero, aimTo = Vector2.Zero;
	public float direction => (Position - aimNow).AngleToPoint(Vector2.Zero);
	public float directionDegree => direction / (float)Math.PI * 180f;

	public Vector2 moveValue = Vector2.Zero;

	// AnimationPlayer animPlayer;
	// Sprite2D sprite;
	Hands hands;

    public override void _EnterTree()
    {
        base._EnterTree();
		// animPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
		// sprite = GetNode<Sprite2D>("Sprite");
		hands = FindChild("Hands") as Hands;
	}

	public override void _Process(double delta)
	{
		AimProcess(delta);
		InputProcess(this, delta);
		PhysicsProcess(delta);
	}

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);
		MoveAndSlide();
	}


    //Get User Input
    public Action<Humanoid, double> InputProcess = (thisObj, delta) =>
	{
		//Get Input
		thisObj.moveValue = Vector2.Zero;

		if (Input.IsKeyPressed(Key.Left))
			thisObj.moveValue += new Vector2(-1f, +0f);
		if (Input.IsKeyPressed(Key.Right))
			thisObj.moveValue += new Vector2(+1f, +0f);
		if (Input.IsKeyPressed(Key.Up))
			thisObj.moveValue += new Vector2(+0f, -1f);
		if (Input.IsKeyPressed(Key.Down))
			thisObj.moveValue += new Vector2(+0f, +1f);
	};
	//Apply Accel and Friction
	void PhysicsProcess(double delta)
	{
		//Accelation
		if(moveValue.Length() > 0.01f)
			Velocity += moveValue.Normalized() * moveSpeed * (float)delta;
		//Friction
		Velocity *= friction;
	}

	//Process of Aim
	void AimProcess(double delta)
	{
        Vector2 globalMousePos = GetGlobalMousePosition();
        Camera2D camera = GetViewport().GetCamera2D();
		
		if (camera != null) aimTo = globalMousePos; 
		
		aimNow  = (aimNow + aimTo * aimSpeed) / (1f + aimSpeed);
		hands.Position = Vector2.FromAngle(direction) * 10f;

		hands.direction = direction;
	}

}