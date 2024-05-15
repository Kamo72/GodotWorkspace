using Godot;
using System;

public partial class Hands : Node2D
{
	public float direction = 0f;
	public float directionDegree 
	{
		get => direction / (float)Math.PI * 180f;
		set => direction = value / 180f * (float)Math.PI;
	}


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


	public Weapon equiped 
	{ 
		get{
			foreach (var item in GetChildren(false))
			{
				if(item.Name != "Right" || item.Name != "Left") continue;
				if(item is Weapon Weapon) return Weapon;
			}
			return null;
		}
	}

	public bool GrabWeapon(Weapon weapon)
	{
		if(equiped != null) {GD.Print("이미 무기 이썽"); return false;}

		weapon.GetParent().RemoveChild(weapon);
		AddChild(weapon);
		
		weapon.Position = new Vector2(0, 0);
		weapon.Rotation = 0f;

		return true;
	}

}
