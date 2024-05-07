using Godot;
using System;


public partial class Humanoid : CharacterBody2D
{
	public readonly float moveSpeed = 500f, friction = 0.98f;

	//방향 값
	public float aimSpeed = 0.02f;
	public Vector2 aimNow = Vector2.Zero, aimTo = Vector2.Zero;
	public float direction => (Position - aimNow).AngleToPoint(Vector2.Zero);
	public float directionDegree => direction / (float)Math.PI * 180f;

	public Vector2 moveValue = Vector2.Zero;

	AnimationPlayer animPlayer;
	Sprite2D sprite;
	Hands hands;

	public override void _Ready()
	{
		// animPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
		// sprite = GetNode<Sprite2D>("Sprite");
		hands = GetNode<Node2D>("Hands") as Hands;
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


	//사용자 입력 받기
	Action<Humanoid, double> InputProcess = (thisObj, delta) =>
	{
		//입력 받기
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
	//속도 및 마찰 적용
	void PhysicsProcess(double delta)
	{
		//가속 처리
		if(moveValue.Length() > 0.01f)
			Velocity += moveValue.Normalized() * moveSpeed * (float)delta;
		//마찰
		Velocity *= friction;
	}

	//조준 처리
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