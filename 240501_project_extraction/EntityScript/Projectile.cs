using Godot;
using System;

public partial class Projectile : RigidBody2D
{
	public float damage = 10f;
	public float direction => Rotation; 
	public float speed {
		set{
			this.LinearVelocity = Vector2.FromAngle(GlobalRotation) * value;
		}
	}

	public override void _Ready(){}
	public override void _Process(double delta){}


    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);
		var collision = MoveAndCollide(LinearVelocity);

		if(collision == null) return;
		
		Node2D node = (Node2D)collision.GetCollider();
		
		if(node is Humanoid humanoid)
			CollisionHumanoid(humanoid);

		GetParent().RemoveChild(this);
    }

	void CollisionHumanoid(Humanoid humanoid)
	{
		humanoid.health.GetDamage(damage);
	}
}