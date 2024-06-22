using Godot;
using System;
using System.Collections.Generic;

public partial class Player : Humanoid
{
	public override void _Ready()
	{
        base._Ready();

        Interactable.player = this;
        
        MovementInputProcess = (thisObj, delta) =>
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

        
        inputMap = new Dictionary<string, Func<bool>>{
            {"Fire", ()=> Input.IsMouseButtonPressed(MouseButton.Left) },
            {"Aim", ()=> Input.IsMouseButtonPressed(MouseButton.Right) },
            {"Reload", ()=> Input.IsKeyPressed(Key.R) },
            {"Interact", ()=> Input.IsKeyPressed(Key.F) },
        };


        Weapon weapon = LevelDesign.CreateWeapon("weapon");
        GD.Print("name : " + weapon.Name);
        hands.EquipWeapon(weapon);

    }

	public override void _Process(double delta)
	{
        base._Process(delta);
        //hands.GrabWeapon(GetTree().FindByName("Weapon") as Weapon );
	}
}
