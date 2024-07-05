using Godot;
using System;

public partial class EquipSlot : Control
{

    PanelContainer slotContainer => this.FindByName("SlotContainer") as PanelContainer;
    public Label slotName => this.FindByName("SlotTypeText") as Label;
    public Item item = null;

	public override void _Ready()
	{
        // GD.PrintErr("("+this.Name+")SlotTypeText(this.~) : " + this.FindByName("SlotTypeText"));
        // GD.PrintErr("("+this.Name+")SlotTypeText : " + slotName);
	}

    public override void _Input(InputEvent @event)
    {
        // if (@event is InputEventMouseButton mouseEvent)
        // {
        //     if (mouseEvent.Pressed)
        //     {
        //         GD.PrintErr("Mouse button pressed");
        //     }
        //     else
        //     {
        //         GD.PrintErr("Mouse button released");
        //     }
        // }
        
        // if (@event is InputEventMouseMotion mouseMotionEvent)
        // {
        //     GD.PrintErr("Mouse moved");
        // }
		
        // if (GetRect().HasPoint(GetLocalMousePosition()))
        // {
        //     GD.PrintErr("Mouse is over the control");
        // }
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
        OnMouseProcess();
	}


    void OnMouseProcess()
    {
        Rect2 rectt = slotContainer.GetRect();
        rectt.Position = slotContainer.GlobalPosition;
        slotContainer.Modulate = rectt.HasPoint(GetGlobalMousePosition())?
            new Color(1f,0f,0f) : new Color(1f,1f,1f);
    }
}
