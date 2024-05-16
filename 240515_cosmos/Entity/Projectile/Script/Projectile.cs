using Godot;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

public partial class Projectile : RigidBody2D
{
	public Damage damage = new Damage()
	{
		type = Damage.Type.Mass,
		value = 10f,
		pene = 0.9f,
	};

	public (bool ship, bool fighter, bool missile, bool bullet) isCollidable = (true, false, false, false);

	public override void _Ready()
	{
		ContactMonitor = true;
		MaxContactsReported = 1;
	}

    public override void _EnterTree()
    {
		LinearVelocity = Vector2.FromAngle(Rotation) * 1f;

		SetCollisionLayerValue(2, isCollidable.ship);
		SetCollisionLayerValue(3, isCollidable.fighter);
		SetCollisionLayerValue(4, isCollidable.missile);
		SetCollisionLayerValue(5, isCollidable.bullet);

		if(isCollidable.ship)
			BodyEntered += (node) => { if(node is Module ship) CollideShip(ship); };
		if(isCollidable.fighter)
			BodyEntered += (node) => { if(node is Ship fighter) CollideFighter(fighter); };
		if(isCollidable.missile)
			BodyEntered += (node) => { if(node is Projectile missile) CollideMissile(missile); };
		if(isCollidable.bullet)
			BodyEntered += (node) => { if(node is Projectile bullet) CollideBullet(bullet); };

        base._EnterTree();
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
	{
		base._Process(delta);
	}	

    public override void _PhysicsProcess(double delta)
    {
		MoveAndCollide(LinearVelocity);
        base._PhysicsProcess(delta);
    }

	protected void CollideShip(Module module){ GD.Print("collided! : " + module.Name + " / " + module.Position); module.GetDamage(damage); Dispose(); }
	protected void CollideFighter(Ship fighter){}
	protected void CollideMissile(Projectile missile){}
	protected void CollideBullet(Projectile bullet){}


	public struct Damage
	{
		public enum Type 
		{
			Mass, //질량
			Explosion, //폭발
			Ray, //전자기파 - 가시광선
			Electric, //전자기파 - 전파
		} 

		public float value;
		public float pene;
		
		public Type type;
	}


	//Memory
	public Ship owner;
	public List<Node2D> disableList = new List<Node2D>();
	
	public bool IsHittable(Node2D newOne) 
	{
		foreach(Node2D node in disableList)
			if(node == newOne) return false;
		return true;
	}

	public void SetNonHittable(Node2D newOne)
	{
		if(disableList.Contains(newOne)) return;
		
		disableList.Add(newOne);
	}

}
