using Godot;
using System;

public partial class Ship : RigidBody2D
{

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		GravityScale = 0f;
		Mass = 100f;
		posTarget = Position + new Vector2(300f, 200f);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

    public override void _PhysicsProcess(double delta)
    {
		MovementProcess(delta);
		MoveAndCollide(LinearVelocity);
        base._PhysicsProcess(delta);
    }


	//Movement
	public (float thrustPower, float angularPower) movementStat = (10f, 5f);
	Vector2 posTarget;
	float linearFriction = 0.9f, angularFriction = 2f;
	void MovementProcess(double delta)
	{
		//Far from target
		Vector2 differVec = posTarget - Position;

		if(differVec.Length() > 100f)
		{
			LinearVelocity += differVec.Normalized() *  (float)delta * movementStat.thrustPower;
		}
		LinearVelocity *= 1f - linearFriction * (float)delta;

		float toAngle = Extension.FindShortestAngle(
			Mathf.DegToRad(Rotation), 
			Mathf.DegToRad(differVec.Angle())
			);


		if(differVec.Length() > 100f && Mathf.Abs(toAngle) > 0.005f)
		{
			float side = Mathf.Sign(toAngle);
			AngularVelocity += side * (float)delta * movementStat.angularPower;
		}
		AngularVelocity *= 1f - angularFriction * (float)delta;
		 

	}




	


}
