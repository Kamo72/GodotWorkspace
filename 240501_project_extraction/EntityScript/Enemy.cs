using Godot;
using System;
using System.Collections.Generic;

public partial class Enemy : Humanoid
{
	public override void _Ready()
	{
        base._Ready();

        MovementInputProcess = (thisObj, delta) =>
        {
            //Get Input
            thisObj.moveValue = Vector2.Zero;
            //thisObj.moveValue += new Vector2(-1f, +0f);
            // if (Input.IsKeyPressed(Key.A))
            //     
            // if (Input.IsKeyPressed(Key.D))
            //     thisObj.moveValue += new Vector2(+1f, +0f);
            // if (Input.IsKeyPressed(Key.W))
            //     thisObj.moveValue += new Vector2(+0f, -1f);
            // if (Input.IsKeyPressed(Key.S))
            //     thisObj.moveValue += new Vector2(+0f, +1f);
        };

        inputMap = new Dictionary<string, Func<bool>>{
            {"Fire", ()=> true },
            {"Aim", ()=> false },
            {"Reload", ()=> false },
            {"Interact", ()=> false },
            {"Inventory", ()=> false },
        };

        Weapon weapon = LevelDesign.CreateWeapon("weapon");
        GD.Print("name : " + weapon.Name);
        hands.InitEquipWeapon(weapon);

    }

    public override void _Process(double delta)
    {
        base._Process(delta);

        // if(Input.IsActionJustPressed("Inventory")){
        //     Control mainUI = GetTree().Root.FindByName("MainUi") as Control;
        //     mainUI.Visible = !mainUI.Visible;
        // }

    }

    static Enemy()
    {
    }

}
