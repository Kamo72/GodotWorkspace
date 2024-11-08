using Godot;
using System;
using System.Collections.Generic;
using System.Drawing;
using Color = Godot.Color;

public partial class StorageSlot : Control
{

    GridContainer storageCon => this.FindByName("StorageContainer") as GridContainer;
    PanelContainer slotContainer => this.FindByName("SlotContainer") as PanelContainer;
    public Label slotName => this.FindByName("SlotTypeText") as Label;

    List<Panel> SlotList = new List<Panel>();

    public InventoryPage inventoryPage => ((InventoryPage)GetParent().GetParent().GetParent().GetParent());

    public override void _EnterTree()
    {
        base._EnterTree();
        UpdateSize();
    }

    int step = 0;
    public override void _Input(InputEvent @event)
    {
        Rect2 rect = GetRect();
        rect.Position = GlobalPosition;

        if (rect.HasPoint(GetGlobalMousePosition()) == false)
        {
            updated = false;
            return;
        }

        if (@event is InputEventMouseButton mouseEvent)
        {
            if (mouseEvent.Pressed)
            {
                GD.Print("Mouse button pressed + " + onMouseItem);

                Vector2I dragPos = new Vector2I(
                    onMouse.Value.X - onMouseItem.storagePos.X,
                    onMouse.Value.Y - onMouseItem.storagePos.Y
                    );

                GD.PushError("dragPos + " + dragPos + " - " + onMouse.Value + " - " + onMouseItem.storagePos);

                if (onMouseItem != null)
                    inventoryPage.SetCursor(onMouseItem, dragPos);
            }
            else
            {
                GD.Print("Mouse button released + " + onMouseItem);
                var result = inventoryPage.ReleaseCursor();
                if (result.HasValue)
                {
                    ItemModel sentItem = result.Value.Item1;
                    Vector2I dragPos= result.Value.Item2;

                    inventoryPage.SetCursor(null, new());

                    if (equiped is HasStorage hasStorage) 
                    {
                        bool isStored = sentItem.item.Store(
                            hasStorage.storage, onMouse.Value - dragPos, false);

                        if (isStored)
                            updated = false;
                    }

                }
            }
        }

        if (@event is InputEventMouseMotion mouseMotionEvent)
        {
            GD.Print("Mouse moved" + onMouse);
        }


    }

    public override void _Process(double delta)
    {
        base._Process(delta);
        OnMouseProcess();

        //GD.PrintErr("onMouse(" + step++ + ") : " + (onMouse.HasValue ? onMouse : null).ToString());

        //if (updated == false) {
        //    updated = true;
        //    SetStorageGrid(storageSize);
        //}
    }

    public bool updated = false;

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

    public void DeclareStorageGrid(Vector2I size) {
        storageSize = size;
        updated = false;
    }

    Vector2I storageSize = new Vector2I(0, 0);
    public void SetStorageGrid(Vector2I size)
    {
        int maxColumn = 7;
        SlotList = new List<Panel>();

        Control parent = GetParent() as Control;
        float parentWidth = parent.Size.X;

        float margin = 3f;
        float widthAvailable = parentWidth - 150;
        float nodeSize = (widthAvailable - margin * (maxColumn + 1)) / maxColumn;
        float width = nodeSize * size.X + margin * (size.X + 1);

        GridContainer scon = storageCon;

        foreach (Node node in scon.GetChildren())
            scon.RemoveChild(node);


        // GD.PrintErr("scon.Size : " + scon.Size);
        // GD.PrintErr("margin : " + margin + " / width : " + width + " / nodeSize : " + nodeSize);

        for (int y = 0; y < size.Y; y++)
            for (int x = 0; x < size.X; x++)
            {
                Vector2 tPosition = new Vector2(
                    nodeSize * x + margin * (x + 1),
                    nodeSize * y + margin * (y + 1)
                );

                Panel p = new Panel();
                p.Position = tPosition;
                p.Size = new Vector2(nodeSize, nodeSize);
                p.CustomMinimumSize = p.Size;
                p.Name = x + "x" + y;
                scon.AddChild(p);
                SlotList.Add(p);
                //GD.PrintErr("p : " + p.Position + " / s : " + p.Size);
            }
        //GD.PushError($"SetStorageGrid - size : {size.X} {size.Y}");
        scon.Columns = size.X > 1 ? size.X : 1;
        scon.Size = new Vector2(
            nodeSize * size.X + margin * (size.X + 1),
            nodeSize * size.Y + margin * (size.Y + 1));

        UpdateSize();

    }


    public static Dictionary<string, Color> highlight = new()
    {
        { "idle", new Color(1,1,1)},
        { "disable", new Color(1,0,0)},
        { "enable", new Color(0,1,0)},
        { "onMouse", new Color(0,0.5f,0.5f)},
    };

    public Vector2I? onMouse = null;
    public ItemModel onMouseItem = null;
    void OnMouseProcess()
    {
        ItemModel foundItem = null;
        Vector2I? onMouseNow = null;

        //마우스가 있는 노드 찾기
        foreach (Control node in SlotList)
        {
            Rect2 rect = node.GetRect();
            rect.Position = node.GlobalPosition;

            if (rect.HasPoint(GetGlobalMousePosition()))
            {
                string nodeName = node.Name;
                string[] ss = nodeName.Split('x');
                onMouseNow = new Vector2I(int.Parse(ss[0]), int.Parse(ss[1]));
                break;
            }
        }

        //마우스가 있는 아이템 찾기
        foreach (ItemModel iModel in itemModels)
        {
            Rect2 rect = iModel.GetRect();
            rect.Position = iModel.GlobalPosition;

            if (rect.HasPoint(GetGlobalMousePosition()))
            {
                foundItem = iModel;
                //onMouseNow = iModel.storagePos;
                break;
            }
        }

        //마우스가 있는 아이템이 바뀌면, 기존 아이템 하이라이트 원상복구
        if (foundItem != onMouseItem && onMouseItem != null)
            SetNodesModulate(
                onMouseItem.storagePos,
                onMouseItem.storagePos + onMouseItem.itemSize,
                highlight["idle"]);


        onMouseItem = foundItem;

        //마우스가 있는 노드가 바뀌면, 기존 노드 하이라이트 원상복구
        if (onMouse.HasValue && (!onMouseNow.HasValue || onMouse.Value != onMouseNow.Value))
            GetNodeByPos(onMouse.Value).Modulate = highlight["idle"];

        onMouse = onMouseNow.HasValue ? onMouseNow.Value : null;

        //아이템 하이라이팅
        var result = inventoryPage.ReleaseCursor();
        if (result.HasValue && onMouse.HasValue)
        {
            ItemModel sentItem = result.Value.Item1;
            Vector2I dragPos = result.Value.Item2;
            bool isInsertable = false;
            if (equiped is HasStorage hasStorage) 
            {
                isInsertable = hasStorage.storage.IsAbleToInsert(
                    sentItem.item,onMouse.Value - dragPos, false);

                SetNodesModulate(
                    Vector2I.Zero,
                    hasStorage.storage.size,
                    highlight["idle"]);
            }

            SetNodesModulate(
                onMouse.Value - dragPos,
                onMouse.Value - dragPos + sentItem.itemSize,
                isInsertable? highlight["enable"] : highlight["disable"]);

        }
        else {

            //노드 하이라이트 적용
            if (onMouse.HasValue)
                GetNodeByPos(onMouse.Value).Modulate = highlight["onMouse"];
        }


        //아이템 하이라이트 적용
        if (foundItem != null)
            SetNodesModulate(
                onMouseItem.storagePos,
                onMouseItem.storagePos + onMouseItem.itemSize,
                highlight["onMouse"]);


        //장비칸 하이라이트 적용
        Rect2 rectt = slotContainer.GetRect();
        rectt.Position = slotContainer.GlobalPosition;
        slotContainer.Modulate = rectt.HasPoint(GetGlobalMousePosition()) ?
             highlight["onMouse"] : highlight["idle"];
    }

    Control GetNodeByPos(Vector2I pos)
    {
        return this.FindByName(pos.X + "x" + pos.Y) as Control;
    }

    void SetNodesModulate(Vector2I start, Vector2I end, Color color)
    {
        if (equiped is HasStorage hasStorage)
        {
            start = new Vector2I(
                Math.Max(0, start.X),
                Math.Max(0, start.Y)
                );
            end = new Vector2I(
                Math.Min(hasStorage.storage.size.X, end.X),
                Math.Min(hasStorage.storage.size.Y, end.Y)
                );

            for (int x = start.X; x < end.X; x++)
                for (int y = start.Y; y < end.Y; y++)
                    GetNodeByPos(new(x, y)).Modulate = color;
        }
    }

    Equipable equiped = null;
    List<ItemModel> itemModels = new List<ItemModel>();
    public void RestructureStorage(Equipable equipable)
    {
        equiped = equipable;
        updated = true;

        if (equipable == null)
        {
            SetStorageGrid(Vector2I.Zero);
            return;
        }

        if (equipable is HasStorage hasStorage) 
        {
            ResetItemModel();
            SetStorageGrid(hasStorage.storage.size);

            foreach (Storage.StorageNode storageNode in hasStorage.storage.itemList)
                SetItemModel(storageNode);
        }

    }

    public void SetItemModel(Storage.StorageNode storageNode)
    {
        int maxColumn = 7;
        float margin = 3f;

        Control parent = GetParent() as Control;
        float parentWidth = parent.Size.X;
        float widthAvailable = parentWidth - 150;
        float nodeSize = (widthAvailable - margin * (maxColumn + 1)) / maxColumn;

        Vector2 pos = GetNodeByPos(storageNode.pos).Position;
        Vector2I itemSize = storageNode.item.status.size;
        Vector2 size = new Vector2(
            nodeSize * itemSize.X + margin * (itemSize.X - 1),
            nodeSize * itemSize.Y + margin * (itemSize.Y - 1)
            );
        ItemModel iModel = new ItemModel(storageNode.item, storageNode.pos, size);
        AddChild(iModel);
        iModel.Position = storageCon.Position + pos;
        itemModels.Add(iModel);

        //GetNodeByPos(storageNode.pos).Size = size;

        //for (int x = storageNode.pos.X; x < storageNode.pos.X + itemSize.X; x++)
        //for (int y = storageNode.pos.Y; y < storageNode.pos.Y + itemSize.Y; y++)
        //{
        //    //if (new Vector2I(x, y) == storageNode.pos)
        //    //    continue;
            
        //    //GetNodeByPos(new(x,y)).Visible = false;
        //}

    }
    public void GetItemModel(int x, int y) => GetItemModel(new Vector2I(x, y));
    public void GetItemModel(Vector2I pos)
    {

    }
    public void ResetItemModel()
    {
        foreach (ItemModel iModel in itemModels) 
            iModel.QueueFree();
        itemModels.Clear();
    }



}

