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

            {"FirstWeapon", ()=> Input.IsActionJustPressed("FirstWeapon") },
            {"SecondWeapon", ()=> Input.IsActionJustPressed("SecondWeapon") },
            {"SubWeapon", ()=> Input.IsActionJustPressed("SubWeapon") },
        };
    }
    
	public override void _EnterTree()
	{
        base._EnterTree();
        
        bool res;
        
        res = inventory.firstWeapon.DoEquipItem(new TestSG());
        GD.PrintErr("firstWeapon.DoEquipItem : " + res);
        res = inventory.secondWeapon.DoEquipItem(new TestSMG());
        GD.PrintErr("firstWeapon.DoEquipItem : " + res);
        res = inventory.subWeapon.DoEquipItem(new TestHG());
        GD.PrintErr("firstWeapon.DoEquipItem : " + res);
        
        hands.InitEquipWeapon((inventory.firstWeapon.item as WeaponItem).GetWeapon());
    }


    public override void _Process(double delta)
    {
        direction = (GlobalPosition - aimNow).AngleToPoint(Vector2.Zero);
        base._Process(delta);

        if(inputMap["Inventory"]()){
            Control mainUI = GetTree().Root.FindByName("MainUi") as Control;
            mainUI.Visible = !mainUI.Visible;
        }

        
        if(inputMap["FirstWeapon"]())
        if(inventory.firstWeapon.item != null){
            WeaponItem wpItem = inventory.firstWeapon.item as WeaponItem;
            if(hands.equiped == null)
            {
                Weapon wp = wpItem.GetWeapon();
                hands.InitEquipWeapon(wp);
            }
            else if(hands.equiped.weaponStatus.Equals(wpItem.weaponStatus) == false)
            {
                Weapon wp = wpItem.GetWeapon();
                hands.InitEquipWeapon(wp);
            }
        }

        if(inputMap["SecondWeapon"]())
        if(inventory.secondWeapon.item != null){
            WeaponItem wpItem = inventory.secondWeapon.item as WeaponItem;
            if(hands.equiped == null)
            {
                Weapon wp = wpItem.GetWeapon();
                hands.InitEquipWeapon(wp);
            }
            else if(hands.equiped.weaponStatus.Equals(wpItem.weaponStatus) == false)
            {
                Weapon wp = wpItem.GetWeapon();
                hands.InitEquipWeapon(wp);
            }
        }
        
        if(inputMap["SubWeapon"]())
        if(inventory.subWeapon.item != null){
            WeaponItem wpItem = inventory.subWeapon.item as WeaponItem;
            if(hands.equiped == null)
            {
                Weapon wp = wpItem.GetWeapon();
                hands.InitEquipWeapon(wp);
            }
            else if(hands.equiped.weaponStatus.Equals(wpItem.weaponStatus) == false)
            {
                Weapon wp = wpItem.GetWeapon();
                hands.InitEquipWeapon(wp);
            }
        }

    }

    static Player()
    {
        List<(string actionName, Key keycode, bool alt, bool ctrl, bool shift)> keyList
        = new List<(string actionName, Key keycode, bool alt, bool ctrl, bool shift)>
        {
            ("Reload", Key.R, false, false, false),
            ("Interact", Key.F, false, false, false),
            ("Inventory", Key.Tab, false, false, false),
            ("FirstWeapon", Key.Key1, false, false, false),
            ("SecondWeapon", Key.Key2, false, false, false),
            ("SubWeapon", Key.Key3, false, false, false),
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
