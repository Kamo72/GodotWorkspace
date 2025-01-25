using Godot;
using System;
using System.Collections.Generic;
using System.Drawing;
using static Storage;
using Color = Godot.Color;

public partial class StorageSlot : InventorySlot
{
    /* UI Reference */
    GridContainer storageCon => this.FindByName("StorageContainer") as GridContainer;
    PanelContainer slotContainer => this.FindByName("SlotContainer") as PanelContainer;
    public Label slotName => this.FindByName("SlotTypeText") as Label;

    List<Panel> SlotList = new List<Panel>();


    /* Reference */
    Humanoid.Inventory.EquipSlot slot; //소켓과 장비된 아이템 정보
    Equipable equiped = null; //소켓과 장비된 아이템 정보


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

    public void SetSocket(Humanoid.Inventory.EquipSlot equipSlot)
    {
        slot = equipSlot;
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

    public Vector2I? onMouse = null; //마우스 위치
    public ItemModel onMouseItem = null; // 마우스 아이템
    //OnMouse 정보를 찾는 과정 + 각 슬롯과 소켓의 UI 하이라이팅
    public override void OnMouseProcess()
    {
        ItemModel foundItem = null;
        Vector2I? onMouseNow = null;

        //마우스가 있는 노드 찾기
        if (GetNodeByPos(new(0, 0)) != null && GetNodeByPos(new(1, 0)) != null)
        {
            //throw new Exception("PocketSlot - OnMouseProcess :: GetNodeByPos(new(0, 0)) == null || GetNodeByPos(new(1, 0)) == null is True!!!");

            Rect2 zeroNode = GetNodeByPos(new Vector2I(0, 0)).GetRect(), oneNode = GetNodeByPos(new Vector2I(1, 0)).GetRect();
            float nodeSep = oneNode.Position.X - zeroNode.End.X;

            Vector2 zeroPos = zeroNode.Position - (Vector2.One * nodeSep / 2f);
            float inputSep = zeroNode.Size.X + nodeSep;
            Vector2 mousePos = GetLocalMousePosition();

            Vector2 nodePosFloat = (mousePos - zeroPos) / inputSep;
            Vector2I nodePos = new(
                Mathf.RoundToInt(nodePosFloat.X),
                Mathf.RoundToInt(nodePosFloat.Y));

            if (GetNodeByPos(nodePos) != null)
                onMouseNow = nodePos;
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
        var result = GetCursor();
        if (result.HasValue && onMouse.HasValue)
        {
            ItemModel sentItem = result.Value.Item1;
            Vector2I dragPos = result.Value.Item2;
            bool isInsertable = false;
            if (equiped is HasStorage hasStorage)
            {
                isInsertable = hasStorage.storage.IsAbleToInsert(
                    sentItem.item, onMouse.Value - dragPos, false);

                SetNodesModulate(
                    Vector2I.Zero,
                    hasStorage.storage.size,
                    highlight["idle"]);
            }

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


        //장비칸 하이라이트 적용
        Rect2 rectt = slotContainer.GetRect();
        rectt.Position = slotContainer.GlobalPosition;
        if (rectt.HasPoint(GetGlobalMousePosition()))
        {
            slotContainer.Modulate = highlight["onMouse"];
            onMouse = new(-1, -1);
        }
        else
        {
            slotContainer.Modulate = highlight["idle"];
        }

    }


    /* Updater */
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

    //주어진 Equipable에 따라 모든 아이템 UI 초기화 (updated 변수에 의해 호출)
    public override void RestructureStorage()
    {
        if (updated) return;
        updated = true;

        Equipable equipable = slot.item;
        {
            equiped = equipable;
            if (equipable == null)
            {
                ResetItemModel();
                SetStorageGrid(Vector2I.Zero);
                return;
            }

            if (equipable is HasStorage hasStorage)
            {
                ResetItemModel();
                SetItemEquiped(equipable);

                var result = GetCursor();
                if (result.HasValue)
                {
                    ItemModel draggingItem = result.Value.Item1;
                    Vector2I dragPos = result.Value.Item2;
                    if (draggingItem.item == equiped)
                    {
                        SetStorageGrid(Vector2I.Zero);
                        return;
                    }
                }

                SetStorageGrid(hasStorage.storage.size);

                foreach (Storage.StorageNode storageNode in hasStorage.storage.itemList)
                    SetItemModel(storageNode);
            }
        }
    }

    //InventoryPage로부터 입력에 대한 처리를 호출
    public override bool GetInput(InputEvent @event)
    {
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
                if (onMouseItem != null)
                {
                    if (onMouse.HasValue)
                    {
                        //GD.PushWarning("Mouse button pressed : " + onMouse.Value + " - " + onMouseItem.storagePos);

                        Vector2I dragPos = new Vector2I(
                            onMouse.Value.X - onMouseItem.storagePos.X,
                            onMouse.Value.Y - onMouseItem.storagePos.Y
                            );

                        SetCursor(onMouseItem, dragPos);
                    }
                    else
                    {
                        //GD.PushWarning("Mouse button pressed : " + onMouseItem.storagePos);
                        Vector2I itemSize = onMouseItem.item.status.size;
                        SetCursor(onMouseItem, new(itemSize.X / 2, itemSize.Y / 2));

                    }
                }

                return true; //해당 코드에서 처리하기 성공
            }
            //Mouse button released
            else if (mouseEvent.ButtonIndex == MouseButton.Left)
            {
                //커서에 아이템이 있다면
                var result = GetCursor();
                if (result.HasValue)
                {
                    ItemModel draggingItem = result.Value.Item1;
                    Vector2I dragPos = result.Value.Item2;

                    if (onMouse.HasValue)
                    {
                        //소켓에 템이 장착됨
                        if (equiped is HasStorage hasStorage)
                        {
                            //아이템을 스스로 안에 저장 시도 시
                            if (equiped == draggingItem.item)
                                return false;

                            //소켓에 장착된 템에 빠른 저장
                            if (onMouse == new Vector2I(-1, -1))
                            {
                                //빠른 보관
                                bool insertable = hasStorage.storage.IsAbleToInsert(draggingItem.item);
                                if (insertable)
                                {
                                    StorageNode? sNode = hasStorage.storage.GetPosInsert(draggingItem.item);
                                    if (sNode.HasValue)
                                        return hasStorage.storage.Insert(sNode.Value);
                                    //해당 코드에서 처리하기 성공
                                }

                                return false;
                            }



                            //Item을 Storage 객체에 Store
                            bool isStored = draggingItem.item.Store(
                                hasStorage.storage, onMouse.Value - dragPos, false);

                            if (isStored)
                                //가져온 아이템이 장비 가능하고, 장비 중이라면, 장비 해제
                                if (draggingItem.item is Equipable draggingEquipable)
                                {
                                    if (draggingEquipable.isEquiping)
                                        draggingEquipable.UnEquip();
                                }

                            SetCursor(null, new());

                            //Update
                            if (isStored) updated = false;
                            return true; //해당 코드에서 처리하기 성공
                        }

                        //비어 있는 소켓에 아이템에 가져다 놓기
                        else if (equiped == null && onMouse == new Vector2I(-1, -1))
                        {
                            if (draggingItem.item is Equipable draggingEquipable)
                            {
                                bool isEpquipable = slot.AbleEquipItem(draggingEquipable);

                                if (!isEpquipable) return false;

                                if (draggingEquipable.isEquiping)
                                    draggingEquipable.UnEquip();

                                bool equipTargetResult = slot.DoEquipItem(draggingEquipable);

                                return equipTargetResult;
                            }
                        }
                    }
                    onMouse = null;
                    onMouseItem = null;
                }
            }
        }

        if (@event is InputEventMouseMotion mouseMotionEvent)
        {
            //GD.Print("Mouse moved" + onMouse);
        }

        if (GetRect().HasPoint(GetLocalMousePosition()))
        {
            //GD.Print("Mouse is over the control");
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
        if (GetCursor().HasValue)
            if (GetCursor().Value.Item1.item == equiped)
                return;

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

    //소켓 장비 UI 추가
    public void SetItemEquiped(Equipable equipable)
    {
        ItemModel iModel = new ItemModel(equipable, slotContainer);
        AddChild(iModel);
        itemModels.Add(iModel);
        iModel.Position = slotContainer.Position;

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

        //GD.Print("iModel.Size" + iModel.Size);
    }
    
    //모든 아이템 UI 삭제
    public void ResetItemModel()
    {
        foreach (ItemModel iModel in itemModels) 
            iModel.QueueFree();
        itemModels.Clear();
    }



}

