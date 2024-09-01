using Godot;
using System;
using System.Collections.Generic;


public partial class Humanoid : CharacterBody2D
{
	public readonly float moveSpeed = 1500f, friction = 0.965f;

	//angleValue
	public float aimSpeed = 0.02f;
	public Vector2 aimNow = Vector2.Zero, aimTo = Vector2.Zero;
	public float direction  = 0f;
	public float directionDegree => direction / (float)Math.PI * 180f;

	public Vector2 moveValue = Vector2.Zero;

	// AnimationPlayer animPlayer;
	// Sprite2D sprite;
	protected Hands hands;

	// public StanceType stanceType = StanceType.WALK; 
	// public float stanceValue = 1f;

	public Dictionary<string, Func<bool>> inputMap;

    public override void _EnterTree()
    {
        base._EnterTree();
		// animPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
		// sprite = GetNode<Sprite2D>("Sprite");
		hands = FindChild("Hands") as Hands;
		health = new Health(300f, () => GetParent().RemoveChild(this));
		inventory = new Inventory(this);
	}

	public override void _Process(double delta)
	{
		base._Process(delta);
		AimProcess(delta);
		MovementInputProcess(this, delta);
		PhysicsProcess(delta);
		InteractionProcess();
	}

    public override void _PhysicsProcess(double delta)
    {
		Vector2 forePostion = GlobalPosition;

        base._PhysicsProcess(delta);
		MoveAndSlide();

		aimNow += GlobalPosition - forePostion;
	}

    public override void _Draw()
    {
        base._Draw();
        DrawCircle(aimTo, 400f, Colors.Yellow);
        DrawCircle(aimNow, 400f, Colors.Red);
    }
    //Get User Input
    public Action<Humanoid, double> MovementInputProcess = (thisObj, delta) =>
	{
		//Get Input
		thisObj.moveValue = Vector2.Zero;

		if (Input.IsKeyPressed(Key.A))
			thisObj.moveValue += new Vector2(-1f, +0f);
		if (Input.IsKeyPressed(Key.D))
			thisObj.moveValue += new Vector2(+1f, +0f);
		if (Input.IsKeyPressed(Key.W))
			thisObj.moveValue += new Vector2(+0f, -1f);
		if (Input.IsKeyPressed(Key.S))
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

	List<Interactable> interactables = new List<Interactable>();
	void InteractionProcess()
	{
		interactables = new List<Interactable>();
		Godot.Collections.Array<Node> nodes = this.GetTree().Root.GetChild(0).GetChildren();

		foreach(Node node in nodes)
			if(node is Interactable interactable)
			{
				float dist = (GlobalPosition - interactable.GlobalPosition).Length();
				if(dist < interactable.interactableRange)
					interactables.Add(interactable);
			}
	
		if(inputMap["Interact"]() && interactables.Count > 0)
			interactables[0].Interacted(this);
	}
}


public partial class Humanoid
{
	public enum StanceType
	{
		CROUNCH,
		WALK,
		SPRINT,
	}
}