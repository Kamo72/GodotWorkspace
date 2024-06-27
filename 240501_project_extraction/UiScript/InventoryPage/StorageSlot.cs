using Godot;
using System;

public partial class StorageSlot : Control
{

    GridContainer storageCon => this.FindByName("StorageContainer") as GridContainer;
    PanelContainer slotContainer => this.FindByName("SlotContainer") as PanelContainer;
    Label slotName => this.FindByName("SlotTypeText") as Label;

    public override void _EnterTree()
    {
        base._EnterTree();
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

    public override void _Process(double delta)
    {
        base._Process(delta);
        slotName.Text = "가방";
        
        if(updated == false){
            updated = true;
            SetStorageGrid(new Vector2I(7,7));
		    UpdateSize();
        }
    }
    bool updated = false;

    private void UpdateSize()
    {
        float maxWidth = 0;
        float maxHeight = 0;

        foreach (Control child in GetChildren())
        {
            if (child is Control controlChild)
            {
                Rect2 childRect = controlChild.GetGlobalRect();
                //maxWidth = Math.Max(maxWidth, childRect.Size.X + childRect.Position.X);
                maxHeight = Math.Max(maxHeight, child.Size.Y + child.Position.Y);
                GD.PrintErr("child : " + child + "child.Size.Y : " + child.Size.Y + " / child.Position.Y : " + child.Position.Y);
            }
        }

        CustomMinimumSize = new Vector2(CustomMinimumSize.X, maxHeight + 20);

    }

    public override void _Draw()
    {
        // Rect2 rect = new Rect2(Vector2.Zero, Size);
        // DrawRect(rect, new Color(0.5f,0.5f,0.5f), false, 1f);
    }

    void SetStorageGrid(Vector2I size)
    {
        int maxColumn = 7;

        Control parent = GetParent() as Control;
        float parentWidth = parent.Size.X;

        float margin = 2f;
        float widthAvailable = parentWidth - 150;
        float nodeSize = (widthAvailable - margin * (maxColumn + 1))/ maxColumn;
        float width = nodeSize * size.X + margin * (size.X + 1);
        
        GridContainer scon = storageCon;

        foreach(Node node in scon.GetChildren())
            scon.RemoveChild(node);

        
        GD.PrintErr("scon.Size : " + scon.Size);
        GD.PrintErr("margin : " + margin + " / width : " + width + " / nodeSize : " + nodeSize);
        
        for(int x = 0; x < size.X; x++)
        for(int y = 0; y < size.Y; y++)
        {
            Vector2 tPosition = new Vector2(
                nodeSize * x + margin * (x+1),
                nodeSize * y + margin * (y+1)
            );

            Panel p = new Panel();
            p.Position = tPosition;
            p.Size = new Vector2(nodeSize, nodeSize);
            p.CustomMinimumSize = p.Size;
            scon.AddChild(p);
           //GD.PrintErr("p : " + p.Position + " / s : " + p.Size);
        }


        scon.Columns = size.X;
        scon.Size = new Vector2(
            nodeSize * size.X + margin * (size.X+1),
            nodeSize * size.Y + margin * (size.Y+1));

    } 


}

