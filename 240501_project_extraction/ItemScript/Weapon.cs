using Godot;
using System;
using System.Collections.Generic;

public partial class Weapon : Node2D
{
	public Sprite2D sprite;
	public float barrelLength;
	public PackedScene bullet;

	public Status weaponStatus = new Status();

	public Node2D muzzleNode => this.FindByName("Muzzle") as Node2D;
	public Node2D magNode => this.FindByName("Mag") as Node2D;
	public Node2D magInsertNode => this.FindByName("MagInsert") as Node2D;
	public Node2D chamber => this.FindByName("Chamber") as Node2D;

	public enum ActionType
	{
		IDLE,
		RELOADING,
	}
	public ActionType actType;
	public float actValue = 0f, actMax = 0f;

	public Dictionary<string, Func<bool>> inputMap; 
	public bool isEquiped => GetParent() is Hands;

	public override void _Ready()
	{
        sprite = FindChild("Sprite2D") as Sprite2D;
		barrelLength = GetMeta("BarrelLength").As<float>();
		magNow = weaponStatus.mag;
		magMax = weaponStatus.mag;
    }

	public override void _Process(double delta)
	{
		LogicProcess(delta);
		base._Process(delta);
    }

    float cooldown = 0;
    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);
    }

	void LogicProcess(double delta)
	{
		if(!isEquiped) return; 

		switch(actType){
			case ActionType.IDLE : {

				if(inputMap["Fire"]())
					if (cooldown < 0 && magNow > 0)
						Fire();

				
				if(inputMap["Reload"]())
					if (magNow != magMax)
						Reload();
			
			} break;
			case ActionType.RELOADING : {
				
				if(actValue > actMax){
					actType = ActionType.IDLE;
					magNow = magMax;
				}

			} break;
		}
		
		actValue += (float)delta;
		cooldown -= (float)delta;
		if(cooldown < 0) cooldown = -0.01f;
	}


	public int magMax, magNow;
	void Fire()
    {
		cooldown += (float)60 / weaponStatus.rpm;
		magNow--;

        Projectile proj = ResourceLoader.Load<PackedScene>("res://Prefab/projectile.tscn").Instantiate() as Projectile;
		proj.GlobalPosition = muzzleNode.GlobalPosition;
		proj.GlobalRotation = GlobalRotation;
		
		proj.speed = weaponStatus.muzzleSpeed;
		proj.damage = weaponStatus.damage;

        GetTree().Root.AddChild(proj);
    }

	void Reload()
	{
		actValue = 0;
		actMax = weaponStatus.reloadTime;
		actType = ActionType.RELOADING;
	}

}
