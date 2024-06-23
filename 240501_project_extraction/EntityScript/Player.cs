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
            {"Fire", ()=> Input.IsActionPressed("Fire") },
            {"Aim", ()=> Input.IsActionPressed("Aim") },
            {"Reload", ()=> Input.IsActionJustPressed("Reload") },
            {"Interact", ()=> Input.IsActionJustPressed("Interact") },
            {"Inventory", ()=> Input.IsActionJustPressed("Inventory") },
        };

        Weapon weapon = LevelDesign.CreateWeapon("weapon");
        GD.Print("name : " + weapon.Name);
        hands.InitEquipWeapon(weapon);

    }

    static Player()
    {
        List<(string actionName, Key keycode, bool alt, bool ctrl, bool shift)> keyList
        = new List<(string actionName, Key keycode, bool alt, bool ctrl, bool shift)>
        {
            ("Reload", Key.R, false, false, false),
            ("Interact", Key.F, false, false, false),
            ("Inventory", Key.Tab, false, false, false),
        };

        List<(string actionName, MouseButton mbcode, bool alt, bool ctrl, bool shift)> mbList
        = new List<(string actionName, MouseButton mbcode, bool alt, bool ctrl, bool shift)>
        {
            ("Fire", MouseButton.Left, false, false, false),
            ("Aim", MouseButton.Right, false, false, false),
        };


        foreach(var data in keyList){
            InputEventKey evt = new Godot.InputEventKey();
            evt.Keycode = data.keycode;
            evt.AltPressed = data.alt;
            evt.CtrlPressed = data.ctrl;
            evt.ShiftPressed = data.shift;
            InputMap.AddAction(data.actionName);
            InputMap.ActionAddEvent(data.actionName, evt);
        }

        foreach(var data in mbList){
            InputEventMouseButton evt = new Godot.InputEventMouseButton();
            evt.ButtonIndex = data.mbcode;
            evt.AltPressed = data.alt;
            evt.CtrlPressed = data.ctrl;
            evt.ShiftPressed = data.shift;
            InputMap.AddAction(data.actionName);
            InputMap.ActionAddEvent(data.actionName, evt);
        }
    }

}
