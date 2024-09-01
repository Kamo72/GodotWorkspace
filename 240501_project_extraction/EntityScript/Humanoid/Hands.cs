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

	public override void _Process(double delta)
	{
		Rotation = direction;
		Scale = new Vector2(1f, Math.Abs(directionDegree) < 90f? 1f : -1f);
	}
    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);
		ActionProcess(delta);
		WeaponProcess(delta);
    }


	public Weapon equiped = null;
	public void EquipWeapon()
	{
		Weapon weapon = equipTarget;
		if(equiped != null) RemoveChild(equiped);

		if(weapon == null) return;


		if(weapon.GetParent() != null)
			weapon.GetParent().RemoveChild(weapon);
			
		equiped = weapon;
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
		
		if(actType == ActionType.IDLE && master.inputMap["Reload"]())
		{
			(float, float, float) timeSet = equiped.weaponStatus.timeDt.reloadTime;
			float reloadTime = timeSet.Item1 + timeSet.Item2 + timeSet.Item3;
			ActionInit(ActionType.RELOADING, reloadTime);
		}

	}
}

public partial class Hands
{
    public ActionType actType = ActionType.IDLE;
	public float actTime = 0f, actTimeMax = 0f;
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
					ActionInit( ActionType.SWAP_IN, equiped == null? 0.4f : equiped.weaponStatus.timeDt.swapTime);

			}break;
			case ActionType.SWAP_IN : {
				
				if(equiped == equipTarget)
					ActionInit( ActionType.SWAP_OUT, actTimeMax, actTimeMax - actTime);
				

				if(actTime > actTimeMax)
				{
					EquipWeapon();
					ActionInit( ActionType.SWAP_OUT, actTimeMax);
				}

			}break;
			case ActionType.SWAP_OUT : {

				if(actTime > actTimeMax)
					ActionInit( ActionType.IDLE, 0f);
				
				if(equiped != equipTarget)
					ActionInit( ActionType.SWAP_IN, actTimeMax, actTimeMax - actTime);
				

			}break;
			case ActionType.RELOADING : {

				if(actTime > actTimeMax)
				{
					equiped.magNow = equiped.magMax;
					ActionInit( ActionType.IDLE, 0f);
				}
			}break;
		}
		actTime += (float)delta;

		GD.Print("equiped : " + equiped + " / to : " + equipTarget);
		GD.Print("ActionType : " + actType);
		GD.Print("Time : " + actTime + " / " + actTimeMax);
	}

	public void ActionInit(ActionType actType, float actTimeMax, float actTime = 0f)
	{
		this.actType = actType;
		this.actTimeMax = actTimeMax;
		this.actTime = actTime;
	}

}