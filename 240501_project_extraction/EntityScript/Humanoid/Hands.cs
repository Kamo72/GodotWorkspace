using Godot;
using System;
using System.Data.Common;

public partial class Hands : Node2D
{
	public float direction = 0f;
	public float directionDegree 
	{
		get => direction / (float)Math.PI * 180f;
		set => direction = value / 180f * (float)Math.PI;
	}

	public Humanoid master => GetParent() as Humanoid;

	public override void _Ready(){}
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
	public void EquipWeapon()
	{
		Weapon weapon = equipTarget;
		if(equiped != null) RemoveChild(equiped);

		if(weapon.GetParent() != null)
			weapon.GetParent().RemoveChild(weapon);
			
		AddChild(weapon);
		weapon.inputMap = master.inputMap;
		
		weapon.Position = new Vector2(0, 0);
		weapon.Rotation = 0f;

		return;
	}
	public void InitEquipWeapon(Weapon weapon)
	{
		equipTarget = weapon;

	}
	void WeaponProcess(double delta)
	{
		if(equiped == null) return;
		//TODO

	}
}

public partial class Hands
{
    public ActionType actType = ActionType.IDLE;
	public float actTime = 0f;
	public Weapon equipTarget = null;
	public enum ActionType
	{
		IDLE,
		RELOADING,
		SWAP_IN,
		SWAP_OUT,
	}

public void ActionProcess(double delta)
{
	switch(actType)
	{
		case ActionType.IDLE : {
			if(equiped != equipTarget)
			{
				actType = ActionType.SWAP_IN;
				actTime = equiped.weaponStatus.swapTime;
			}
		}break;
		case ActionType.SWAP_IN : {

		}break;
		case ActionType.SWAP_OUT : {

		}break;
		case ActionType.RELOADING : {

		}break;
	}

}


}