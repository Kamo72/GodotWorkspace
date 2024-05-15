using Godot;
using System;

public partial class DebugManager : Node
{

    bool ENABLE_DEBUG = true;

	public override void _Ready()
    {
        // InputEventKey evt = new Godot.InputEventKey();
        // evt.Keycode = Key.A;
        // evt.AltPressed = false;
        // evt.CtrlPressed = false;
        // evt.ShiftPressed = false;
        // InputMap.AddAction("TestAction");
        // InputMap.ActionAddEvent("TestAction", evt);
        if(ENABLE_DEBUG == false) return;

        InputEventKey evt = new Godot.InputEventKey();
        evt.Keycode = Key.Escape;
        evt.AltPressed = false;
        evt.CtrlPressed = false;
        evt.ShiftPressed = false;
        InputMap.AddAction("KillProcess");
        InputMap.ActionAddEvent("KillProcess", evt);

    }

    public override void _Process(double delta)
    {
        if (Input.IsActionJustPressed("KillProcess"))
            GetTree().Quit(0);
        // if (Input.IsActionJustPressed("TestAction"))
        //     GD.Print("IsActionJustPressed");
        // if (Input.IsActionJustReleased("TestAction"))
        //     GD.Print("IsActionJustReleased");

        
    }
}