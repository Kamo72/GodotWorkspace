using Godot;
using System;
using System.Collections.Generic;
using static Storage;

public partial class PocketSlot : Control
{
    /* UI Reference */
    GridContainer storageCon => this.FindByName("StorageContainer") as GridContainer;

    List<Panel> SlotList = new List<Panel>();


    /* Reference */
    public InventoryPage inventoryPage => ((InventoryPage)GetParent().GetParent().GetParent().GetParent());

    Storage storage = null; //소켓과 장비된 아이템 정보

    /* Variables */
    Vector2I storageSize = new Vector2I(0, 0);
    int step = 0;

    List<ItemModel> itemModels = new List<ItemModel>(); //장비된 아이템 내 Storage의 아이템들


    /* Override */
    public override void _EnterTree()
    {
        base._EnterTree();
        UpdateSize();
    }

    public override void _Process(double delta)
    {
        OnMouseProcess();

    }


    /* Initiate */
    public void DeclareStorageGrid(Vector2I size)
    {
        storageSize = size;
        updated = false;
    }

    public void SetStorage(Storage storage)
    {
        this.storage = storage;
    }



    /* Process */
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

    public static Dictionary<string, Color> highlight = new() //하이라이트 색상 정보
    {
        { "idle", new Color(1,1,1)},
        { "disable", new Color(1,0,0)},
        { "enable", new Color(0,1,0)},
        { "onMouse", new Color(0,0.5f,0.5f)},
    };

    public Vector2I? onMouse = null; //마우스 위치
    public ItemModel onMouseItem = null; // 마우스 아이템
    //OnMouse 정보를 찾는 과정 + 각 슬롯과 소켓의 UI 하이라이팅
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
            if (onMouse.Value != new Vector2I(-1, -1))
                GetNodeByPos(onMouse.Value).Modulate = highlight["idle"];

        onMouse = onMouseNow.HasValue ? onMouseNow.Value : null;

        //아이템 하이라이팅
        var result = inventoryPage.ReleaseCursor();
        if (result.HasValue && onMouse.HasValue)
        {
            ItemModel sentItem = result.Value.Item1;
            Vector2I dragPos = result.Value.Item2;
            bool isInsertable = false;

            isInsertable = storage.IsAbleToInsert(
                sentItem.item, onMouse.Value - dragPos, false);

            SetNodesModulate(
                Vector2I.Zero,
                storage.size,
                highlight["idle"]);
            

            SetNodesModulate(
                onMouse.Value - dragPos,
                onMouse.Value - dragPos + sentItem.itemSize,
                isInsertable ? highlight["enable"] : highlight["disable"]);
        }
        else
        {
            //노드 하이라이트 적용
            if (onMouse.HasValue)
                GetNodeByPos(onMouse.Value).Modulate = highlight["onMouse"];
        }


        //아이템 하이라이트 적용
        if (foundItem != null && foundItem.storagePos != new Vector2I(-1, -1))
            SetNodesModulate(
                onMouseItem.storagePos,
                onMouseItem.storagePos + onMouseItem.itemSize,
                highlight["onMouse"]);


        

    }


    /* Updater */
    void SetStorageGrid(Vector2I size)
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

        scon.Columns = size.X;
        scon.Size = new Vector2(
            nodeSize * size.X + margin * (size.X + 1),
            nodeSize * size.Y + margin * (size.Y + 1));

        UpdateSize();

    }

    //주어진 Equipable에 따라 모든 아이템 UI 초기화 (updated 변수에 의해 호출)
    public bool updated = false;
    public void RestructureStorage(Storage storage)
    {
        updated = true;
        
        ResetItemModel();
        SetStorageGrid(storage.size);

        foreach (Storage.StorageNode storageNode in storage.itemList)
            SetItemModel(storageNode);
        

    }

    //InventoryPage로부터 입력에 대한 처리를 호출
    public bool GetInput(InputEvent @event)
    {
        //GD.PushWarning($"OnMouse : {onMouse} - {(onMouseItem != null ? onMouseItem.item.status.name : "null")}");
        Rect2 rect = GetRect();
        rect.Position = GlobalPosition;

        if (rect.HasPoint(GetGlobalMousePosition()) == false)
        {
            //updated = false;
            return false;
        }

        if (@event is InputEventMouseButton mouseEvent)
        {
            //Mouse button pressed 
            if (mouseEvent.Pressed && mouseEvent.ButtonIndex == MouseButton.Left)
            {
                //해당 지점에 아이템이 있다면 커서에 저장
                GD.PushWarning("onMouseItem != null");
                if (onMouseItem != null)
                {
                    GD.PushWarning("onMouse.HasValue");
                    if (onMouse.HasValue)
                    {
                        GD.PushWarning("Mouse button pressed : " + onMouse.Value + " - " + onMouseItem.storagePos);

                        Vector2I dragPos = new Vector2I(
                            onMouse.Value.X - onMouseItem.storagePos.X,
                            onMouse.Value.Y - onMouseItem.storagePos.Y
                            );

                        inventoryPage.SetCursor(onMouseItem, dragPos);
                    }
                    else
                    {
                        GD.PushWarning("Mouse button pressed : " + onMouseItem.storagePos);
                        Vector2I itemSize = onMouseItem.item.status.size;
                        inventoryPage.SetCursor(onMouseItem, new(itemSize.X / 2, itemSize.Y / 2));

                    }
                }

                return true; //해당 코드에서 처리하기 성공
            }
            //Mouse button released
            else if (mouseEvent.ButtonIndex == MouseButton.Left)
            {
                //커서에 아이템이 있다면
                var result = inventoryPage.ReleaseCursor();
                if (result.HasValue)
                {
                    ItemModel draggingItem = result.Value.Item1;
                    Vector2I dragPos = result.Value.Item2;

                    if (onMouse.HasValue)
                    {
                        //Item을 Storage 객체에 Store
                        bool isStored = draggingItem.item.Store(
                            storage, onMouse.Value - dragPos, false);

                        //가져온 아이템이 장비 가능하고, 장비 중이라면, 장비 해제
                        if (isStored)
                            if (draggingItem.item is Equipable draggingEquipable)
                                if (draggingEquipable.equipedBy != null)
                                    draggingEquipable.UnEquip();

                        inventoryPage.SetCursor(null, new());

                        //Update
                        if (isStored)
                        {
                            updated = false;
                            return true; //해당 코드에서 처리하기 성공
                        }
                    }
                    onMouse = null;
                    onMouseItem = null;
                }
            }
        }

        if (@event is InputEventMouseMotion mouseMotionEvent)
        {
            GD.Print("Mouse moved");
        }

        if (GetRect().HasPoint(GetLocalMousePosition()))
        {
            GD.Print("Mouse is over the control");
        }

        return false; //해당 코드에서 처리하지 못함
    }


    /* Convenience Functions  */
    //위치를 통해 UI노드 반환
    Control GetNodeByPos(Vector2I pos)
    {
        return this.FindByName(pos.X + "x" + pos.Y) as Control;
    }

    //UI 하이라이팅
    void SetNodesModulate(Vector2I start, Vector2I end, Color color)
    {
        if (storage != null)
        {
            start = new Vector2I(
                Math.Max(0, start.X),
                Math.Max(0, start.Y)
                );
            end = new Vector2I(
                Math.Min(storage.size.X, end.X),
                Math.Min(storage.size.Y, end.Y)
                );

            for (int x = start.X; x < end.X; x++)
                for (int y = start.Y; y < end.Y; y++)
                    GetNodeByPos(new(x, y)).Modulate = color;
        }
    }

    //인벤토리 아이템 UI 추가
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
        itemModels.Add(iModel);
        iModel.Position = storageCon.Position + pos;

    }

    //모든 아이템 UI 삭제
    public void ResetItemModel()
    {
        foreach (ItemModel iModel in itemModels)
            iModel.QueueFree();
        itemModels.Clear();
    }

}

