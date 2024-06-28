using Godot;
using System;

public partial class EquipSlot : Control
{

    public Label slotName => this.FindByName("SlotTypeText") as Label;

	public override void _Ready()
	{
	}

    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventMouseButton mouseEvent)
        {
            if (mouseEvent.Pressed)
            {
                GD.PrintErr("Mouse button pressed");
            }
            else
            {
                GD.PrintErr("Mouse button released");
            }
        }
        
        if (@event is InputEventMouseMotion mouseMotionEvent)
        {
            GD.PrintErr("Mouse moved");
        }
		
        if (GetRect().HasPoint(GetLocalMousePosition()))
        {
            GD.PrintErr("Mouse is over the control");
        }
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
