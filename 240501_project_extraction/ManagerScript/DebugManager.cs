using Godot;
using System;

public partial class DebugManager : Node
{

	public override void _Ready()
    {
        // InputEventKey evt = new Godot.InputEventKey();
        // evt.Keycode = Key.A;
        // evt.AltPressed = false;
        // evt.CtrlPressed = false;
        // evt.ShiftPressed = false;
        // InputMap.AddAction("TestAction");
        // InputMap.ActionAddEvent("TestAction", evt);

    }

    public override void _Process(double delta)
    {
        // if (Input.IsActionPressed("TestAction"))
        //     GD.Print("IsActionPressed");
        // if (Input.IsActionJustPressed("TestAction"))
        //     GD.Print("IsActionJustPressed");
        // if (Input.IsActionJustReleased("TestAction"))
        //     GD.Print("IsActionJustReleased");
    }
}