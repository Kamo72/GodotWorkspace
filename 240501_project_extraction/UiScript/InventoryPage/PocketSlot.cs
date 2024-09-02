using Godot;
using System;
using System.Collections.Generic;

public partial class PocketSlot : Control
{

    GridContainer storageCon => this.FindByName("StorageContainer") as GridContainer;
    List<Panel> SlotList = new List<Panel>();
    Storage storage => null;

    public override void _EnterTree()
    {
        base._EnterTree();
        UpdateSize();
    }


    int step = 0;
    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventMouseButton mouseEvent)
        {
            if (mouseEvent.Pressed)
            {
                //GD.PrintErr("Mouse button pressed");
            }
            else
            {
                //GD.PrintErr("Mouse button released");
            }
        }
        
        if (@event is InputEventMouseMotion mouseMotionEvent)
        {
            //GD.PrintErr("Mouse moved");
        }
		
        
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
        OnMouseProcess();

        GD.PrintErr("onMouse("+step+++") : " + (onMouse.HasValue? onMouse : null).ToString());

        if(updated == false){
            updated = true;
            SetStorageGrid(storageSize);
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
                //GD.PrintErr("child : " + child + "child.Size.Y : " + child.Size.Y + " / child.Position.Y : " + child.Position.Y);
            }
        }

        CustomMinimumSize = new Vector2(CustomMinimumSize.X, maxHeight + 20);

    }

    public override void _Draw()
    {
        // Rect2 rect = new Rect2(Vector2.Zero, Size);
        // DrawRect(rect, new Color(0.5f,0.5f,0.5f), false, 1f);
    }

    public void DeclareStorageGrid(Vector2I size){
        storageSize = size;
        updated = false;
    }

    Vector2I storageSize = new Vector2I(0,0);
    void SetStorageGrid(Vector2I size)
    {
        int maxColumn = 7;
        SlotList = new List<Panel>();

        Control parent = GetParent() as Control;
        float parentWidth = parent.Size.X;

        float margin = 3f;
        float widthAvailable = parentWidth - 150;
        float nodeSize = (widthAvailable - margin * (maxColumn + 1))/ maxColumn;
        float width = nodeSize * size.X + margin * (size.X + 1);
        
        GridContainer scon = storageCon;

        foreach(Node node in scon.GetChildren())
            scon.RemoveChild(node);

        
        // GD.PrintErr("scon.Size : " + scon.Size);
        // GD.PrintErr("margin : " + margin + " / width : " + width + " / nodeSize : " + nodeSize);
        
        for(int y = 0; y < size.Y; y++)
        for(int x = 0; x < size.X; x++)
        {
            Vector2 tPosition = new Vector2(
                nodeSize * x + margin * (x+1),
                nodeSize * y + margin * (y+1)
            );

            Panel p = new Panel();
            p.Position = tPosition;
            p.Size = new Vector2(nodeSize, nodeSize);
            p.CustomMinimumSize = p.Size;
            p.Name = x+"x"+y;
            scon.AddChild(p);
            SlotList.Add(p);
           //GD.PrintErr("p : " + p.Position + " / s : " + p.Size);
        }

        scon.Columns = size.X;
        scon.Size = new Vector2(
            nodeSize * size.X + margin * (size.X+1),
            nodeSize * size.Y + margin * (size.Y+1));
            
        UpdateSize();

    } 

    public Vector2I? onMouse = null;
    void OnMouseProcess()
    {
        Vector2I? onMouseNow = null;
        
        foreach(Control node in SlotList)
        {
            Rect2 rect = node.GetRect();
            rect.Position = node.GlobalPosition;

            if(rect.HasPoint(GetGlobalMousePosition()))
            {
                string nodeName = node.Name;
                string[] ss = nodeName.Split('x');
                onMouseNow = new Vector2I( int.Parse(ss[0]), int.Parse(ss[1]));
                break;
            }
        }
        
        if(onMouse.HasValue && (!onMouseNow.HasValue || onMouse.Value != onMouseNow.Value))
            GetNodeByPos(onMouse.Value).Modulate = new Color(1f,1f,1f);

        onMouse = onMouseNow.HasValue? onMouseNow.Value : null;
        
        if(onMouse.HasValue)
            GetNodeByPos(onMouse.Value).Modulate = new Color(1f,0f,0f);
    }

    Control GetNodeByPos(Vector2I pos)
    {
        return this.FindByName(pos.X + "x" + pos.Y) as Control;
    }

}

