using Godot;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Humanoid;

public partial class Trade : Control, InventorySlotContainer
{
    public static Trade instance = null;

    //Nodes
    VBoxContainer traderInventory, myInventory,stashInventory;
    Control cursor;
    List<Window> windowList = new();

    //데이터
    Dictionary<VBoxContainer, List<InventorySlot>> slotListDic = new();
    Storage traderStorage = new Storage(new(8, 12));

    Button confirmButton, conversionButton, closeButton;

    Trader trader = null;

    public override void _EnterTree()
    {
        instance = this;
        Visible = true;

        cursor = this.FindByName("Cursor") as Control;
        cursor.MouseFilter = MouseFilterEnum.Ignore;
        cursor.TextureFilter = TextureFilterEnum.Nearest;

        traderInventory = this.FindByName("TraderInventory") as VBoxContainer;
        myInventory = this.FindByName("MyInventory") as VBoxContainer;
        stashInventory = this.FindByName("StashInventory") as VBoxContainer;

        confirmButton = this.FindByName("ConfirmButton") as Button;
        conversionButton = this.FindByName("ConversionButton") as Button;
        closeButton = this.FindByName("CloseButton") as Button;
        closeButton.Pressed += () => this.Visible = false;

        slotListDic = new() {
            {traderInventory, new List<InventorySlot>() },
            {myInventory, new List<InventorySlot>() },
            {stashInventory, new List<InventorySlot>() },
        };
    }
    public override void _Ready()
    {
        SetPanel(traderInventory, traderStorage);
        SetPanel(myInventory, Player.player.inventory);
        SetPanel(stashInventory, TraderManager.instance.stashList[0]);
    }

    public override void _Process(double delta)
    {
        if (this.Visible == false) return;

        cursor.GlobalPosition = GetGlobalMousePosition();

        //foreach (var slotListPair in slotListDic)
        //    foreach (var item in slotListPair.Value)
        //        item.RestructureStorage();
    }

    //입력 받기
    public override void _Input(InputEvent @event)
    {
        if (this.Visible == false) return;
        //base._Input(@event);

        UiIngame uiIngame = UiIngame.instance;
        if (!uiIngame.Visible) return;

        foreach (var slotListPair in slotListDic)
            foreach (var item in slotListPair.Value)
                item.GetInput(@event);

        if (@event is InputEventMouseButton mouseEvent)
            if (mouseEvent.Pressed != true)
                SetCursor(null, new());

        if (Input.IsActionJustPressed("reload"))
            RotateCursor();

        UpdateAllUI();
    }

    //UI 초기화
    public void UpdateAllUI()
    {
        foreach (var slotListPair in slotListDic)
            foreach (var item in slotListPair.Value)
                item.uiUpdated = false;

        foreach (var slotListPair in slotListDic)
            foreach (var item in slotListPair.Value)
                item.RestructureStorage();
    }

    //능동 인벤토리 조작
    public void SetPanel(VBoxContainer targetArea, Inventory inventory)
    {
        ResetPanel(targetArea);

        if (inventory == null) throw new Exception("Trade SetPanel's inventory is null");

        StorageSlot storageSlot;
        PocketSlot pocketSlot;
        EquipSlot equipSlot;

        List<InventorySlot> slotList = slotListDic[targetArea];

        //Control control = targetArea;

        Control control = new Control();
        targetArea.AddChild(control);
        control.CustomMinimumSize = new(687, 687);
        control.Position = new(0, 0);
        ////control.SizeFlagsHorizontal = SizeFlags.Fill;
        ////control.SizeFlagsVertical = SizeFlags.ShrinkBegin;

        {
            equipSlot = GetEquipSlot();
            equipSlot.slotName.Text = "헬멧";
            equipSlot.SetSocket(inventory.helmet);
            slotList.Add(equipSlot);
            control.AddChild(equipSlot);

            equipSlot.Position = new(250, 100);
        }
        {
            equipSlot = GetEquipSlot();
            equipSlot.slotName.Text = "헤드기어";
            equipSlot.SetSocket(inventory.headgear);
            slotList.Add(equipSlot);
            control.AddChild(equipSlot);

            equipSlot.Position = new(110, 100);
        }
        {
            equipSlot = GetEquipSlot();
            equipSlot.slotName.Text = "방탄판";
            equipSlot.SetSocket(inventory.plate);
            slotList.Add(equipSlot);
            control.AddChild(equipSlot);

            equipSlot.Position = new(250, 240);
        }

        {
            equipSlot = GetWeaponSlot();
            equipSlot.slotName.Text = "주무장";
            equipSlot.SetSocket(inventory.firstWeapon);
            slotList.Add(equipSlot);
            control.AddChild(equipSlot);

            equipSlot.Position = new(110, 380);
        }
        {
            equipSlot = GetWeaponSlot();
            equipSlot.slotName.Text = "부무장";
            equipSlot.SetSocket(inventory.secondWeapon);
            slotList.Add(equipSlot);
            control.AddChild(equipSlot);

            equipSlot.Position = new(110, 520);
        }
        {
            equipSlot = GetEquipSlot();
            equipSlot.slotName.Text = "보조무장";
            equipSlot.SetSocket(inventory.subWeapon);
            slotList.Add(equipSlot);
            control.AddChild(equipSlot);

            equipSlot.Position = new(390, 380);
        }

        {
            storageSlot = GetStorageSlot();
            storageSlot.slotName.Text = "조끼";
            storageSlot.SetSocket(inventory.rig);
            slotList.Add(storageSlot);
            targetArea.AddChild(storageSlot);
        }
        {
            pocketSlot = GetPocketSlot();
            pocketSlot.SetStorage(inventory.pocket);
            slotList.Add(pocketSlot);
            targetArea.AddChild(pocketSlot);
        }
        {
            storageSlot = GetStorageSlot();
            storageSlot.slotName.Text = "가방";
            storageSlot.SetSocket(inventory.backpack);
            slotList.Add(storageSlot);
            targetArea.AddChild(storageSlot);
        }
        {
            storageSlot = GetStorageSlot();
            storageSlot.slotName.Text = "컨테이너";
            storageSlot.SetSocket(inventory.sContainer);
            slotList.Add(storageSlot);
            targetArea.AddChild(storageSlot);
        }

        UpdateAllUI();
    }
    public void SetPanel(VBoxContainer targetArea, Storage storage)
    {
        ResetPanel(targetArea);
        if (storage == null) return;

        PocketSlot pocketSlot = GetPocketSlot();
        pocketSlot.SetStorage(storage);
        targetArea.AddChild(pocketSlot);

        List<InventorySlot> slotList = slotListDic[targetArea];
        slotList.Add(pocketSlot);

        UpdateAllUI();
    }
    public void ResetPanel(VBoxContainer targetArea)
    {
        foreach (var control in targetArea.GetChildren())
            control.QueueFree();

        foreach (var window in windowList)
            window.QueueFree();

        List<InventorySlot> slotList = slotListDic[targetArea];
        slotList.Clear();
        windowList.Clear();
    }

    //Trade UI 관련
    public void OpenTrade()
    {
        OpenTrade(null);
    }
    public void OpenTrade(Trader trader) 
    {
        this.Visible = true;
    }

    public void CloseTrade() 
    {
        this.Visible = false;
    }


    //슬롯 객체 생성
    StorageSlot GetStorageSlot()
    {
        return ResourceLoader.Load<PackedScene>("res://Prefab/UI/InventoryPage/StorageSlot.tscn")
                .Instantiate() as StorageSlot;
    }
    PocketSlot GetPocketSlot()
    {
        return ResourceLoader.Load<PackedScene>("res://Prefab/UI/InventoryPage/PocketSlot.tscn")
                .Instantiate() as PocketSlot;
    }
    EquipSlot GetEquipSlot()
    {
        return ResourceLoader.Load<PackedScene>("res://Prefab/UI/InventoryPage/EquipSlot.tscn")
                .Instantiate() as EquipSlot;
    }
    EquipSlot GetWeaponSlot()
    {
        return ResourceLoader.Load<PackedScene>("res://Prefab/UI/InventoryPage/WeaponSlot.tscn")
                .Instantiate() as EquipSlot;
    }

    //인벤토리 '창' 생성
    public void OpenStorageWindow(VBoxContainer targetArea, Storage storage)
    {
        foreach (var window in windowList)
            if (window.FindByName("PocketSlot") is PocketSlot ps)
                if (ps.HasStorage(storage))
                    return;

        PocketSlot pocketSlot = GetPocketSlot();
        pocketSlot.SetStorage(storage);

        List<InventorySlot> slotList = slotListDic[targetArea];
        slotList.Add(pocketSlot);

        Window control = new Window();
        control.AddChild(pocketSlot);
        AddChild(control);
        control.CloseRequested += () => control.QueueFree();
        control.Size = new(687, 687);
        control.Position = new(600, 600);
        //otherInventory.AddChild(control);
        windowList.Add(control);
    }

    //커서에 집은 아이템
    ItemModel onDragging = null;        //집은 아이템
    Vector2I dragPos = Vector2I.Zero;   //집은 위치 저장

    public bool isRotated { get; set; } = false;
    public bool toRotate { get; set; } = false;

    //커서 관련 함수
    public void SetCursor(ItemModel iModel, Vector2I dragPos)
    {
        //GD.PushWarning("SetCursor : " + iModel + " - " + dragPos);

        //image 가져오기
        TextureRect image = cursor.FindByName("ItemImage") as TextureRect;

        //onDragging 세팅
        onDragging = iModel;
        onDragging?.SetDragging(true);

        //집은 위치 구하기
        this.dragPos = dragPos;

        if (iModel == null)
        {
            //image 비우기
            image.Texture = null;
            image.CustomMinimumSize = Vector2.Zero;

            //onDragging 비우기
            //onDragging?.SetDragging(false);
            onDragging = null;
            return;
        }

        isRotated = iModel.isRotated;
        toRotate = isRotated;

        SetCursorImage();

    }
    public (ItemModel, Vector2I)? ReleaseCursor() => (onDragging == null) ? null : (onDragging, dragPos);


    public void RotateCursor()
    {
        if (onDragging == null)
            return;

        toRotate = !toRotate;
        dragPos = new Vector2I(dragPos.Y, dragPos.X);
        SetCursorImage();
    }

    void SetCursorImage()
    {
        TextureRect image = cursor.FindByName("ItemImage") as TextureRect;
        ItemModel iModel = onDragging;
        Vector2 dragRatio = new Vector2(
            (-1f - dragPos.X * 2f) / (onDragging.itemSize.X * 2f),
            (-1f - dragPos.Y * 2f) / (onDragging.itemSize.Y * 2f));

        image.Texture = (Texture2D)ResourceLoader.Load(onDragging.item.status.textureRoot);
        image.CustomMinimumSize = new(onDragging.size.X, onDragging.size.Y);
        image.Position = new Vector2(
            onDragging.size.X * dragRatio.X,
            onDragging.size.Y * dragRatio.Y
            );
        image.Rotation = toRotate ? Mathf.Pi * 1 / 2f : 0;
    }

}