using Godot;
using System;
using System.Collections.Generic;
using static Humanoid;

public partial class InventoryPage : Page, InventorySlotContainer
{
    public static InventoryPage instance = null;

    Panel myStatus => this.FindByName("MyStatus") as Panel;

    EquipSlot helmetSlot => this.FindByName("HelmetSlot") as EquipSlot;
    EquipSlot headgearSlot => this.FindByName("HeadgearSlot") as EquipSlot;
    EquipSlot plateSlot => this.FindByName("PlateSlot") as EquipSlot;
    EquipSlot firstWeaponSlot => this.FindByName("FirstWeaponSlot") as EquipSlot;
    EquipSlot secondWeaponSlot => this.FindByName("SecondWeaponSlot") as EquipSlot;
    EquipSlot subWeaponSlot => this.FindByName("SubWeaponSlot") as EquipSlot;

    VBoxContainer myInventory => this.FindByName("MyInventory") as VBoxContainer;

    StorageSlot rigSlot => this.FindByName("RigSlot") as StorageSlot;
    StorageSlot backpackSlot => this.FindByName("BackpackSlot") as StorageSlot;
    PocketSlot pocketSlot => this.FindByName("PocketSlot") as PocketSlot;
    StorageSlot containerSlot => this.FindByName("ContainerSlot") as StorageSlot;


    VBoxContainer otherInventory => this.FindByName("OtherInventory") as VBoxContainer;

    List<InventorySlot> mySlotList;
    List<InventorySlot> otherSlotList;

    List<Window> windowList;

    public Control cursor => this.FindByName("Cursor") as Control;

    public bool isRotated {get; set;}
    public bool toRotate { get; set; }

    public override void _EnterTree()
    {
        base._EnterTree();
        instance = this;

        cursor.TextureFilter = TextureFilterEnum.Nearest;

        helmetSlot.slotName.Text = "헬멧";
        helmetSlot.SetSocket(Player.player.inventory.helmet);
        headgearSlot.slotName.Text = "헤드기어";
        headgearSlot.SetSocket(Player.player.inventory.headgear);
        plateSlot.slotName.Text = "방탄판";
        plateSlot.SetSocket(Player.player.inventory.plate);
        firstWeaponSlot.slotName.Text = "주무장";
        firstWeaponSlot.SetSocket(Player.player.inventory.firstWeapon);
        secondWeaponSlot.slotName.Text = "부무장";
        secondWeaponSlot.SetSocket(Player.player.inventory.secondWeapon);
        subWeaponSlot.slotName.Text = "보조무장";
        subWeaponSlot.SetSocket(Player.player.inventory.subWeapon);

        pocketSlot.SetStorage(Player.player.inventory.pocket);

        rigSlot.slotName.Text = "조끼";
        rigSlot.SetSocket(Player.player.inventory.rig);
        backpackSlot.slotName.Text = "가방";
        backpackSlot.SetSocket(Player.player.inventory.backpack);
        containerSlot.slotName.Text = "컨테이너";
        containerSlot.SetSocket(Player.player.inventory.sContainer);

        mySlotList = new() {
            helmetSlot,
            headgearSlot,
            plateSlot,
            firstWeaponSlot,
            secondWeaponSlot,
            subWeaponSlot,
            pocketSlot,
            rigSlot,
            backpackSlot,
            containerSlot,
        };
        otherSlotList = new() { };
        windowList = new() { };

        //OpenStorageWindow(((Backpack)Player.player.inventory.backpack.item).storage);
    }

    public override void _Process(double delta)
    {
        UiMain uiMain = GetParent().GetParent().GetParent() as UiMain;
        if (!uiMain.Visible) return;

        base._Process(delta);
        cursor.GlobalPosition = GetGlobalMousePosition();

        foreach (var item in mySlotList)
            item.RestructureStorage();
        foreach (var item in otherSlotList)
            item.RestructureStorage();
    }

    //입력 받기
    public override void _Input(InputEvent @event)
    {
        UiMain uiMain = GetParent().GetParent().GetParent() as UiMain;
        if (!uiMain.Visible) return;

        foreach (var item in mySlotList)
            item.GetInput(@event);

        foreach (var item in otherSlotList)
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
        foreach (var item in mySlotList)
            item.updated = false;

        foreach (var item in otherSlotList)
            item.updated = false;

        foreach (var item in mySlotList)
            item.RestructureStorage();
        foreach (var item in otherSlotList)
            item.RestructureStorage();
    }

    //OtherInventory 조작
    public void SetOtherPanel(Storage storage)
    {
        ResetOtherPanel();
        if (storage == null) return;

        PocketSlot pocketSlot = ResourceLoader.Load<PackedScene>("res://Prefab/UI/InventoryPage/PocketSlot.tscn").Instantiate() as PocketSlot;
        pocketSlot.SetStorage(storage);


        otherInventory.AddChild(pocketSlot);
        otherSlotList.Add(pocketSlot);

        UpdateAllUI();
    }
    public void SetOtherPanel(Inventory inventory)
    {
        ResetOtherPanel();

        if (inventory == null) return;

        StorageSlot storageSlot;
        PocketSlot pocketSlot;
        EquipSlot equipSlot;

        Control control = new Control();
        otherInventory.AddChild(control);
        control.CustomMinimumSize = new(687, 687);
        control.Position = new(0, 0);
        //control.SizeFlagsHorizontal = SizeFlags.Fill;
        //control.SizeFlagsVertical = SizeFlags.ShrinkBegin;

        {
            equipSlot = GetEquipSlot();
            equipSlot.slotName.Text = "헬멧";
            equipSlot.SetSocket(inventory.helmet);
            otherSlotList.Add(equipSlot);
            control.AddChild(equipSlot);
            equipSlot.Position = new(250, 100);
        }
        {
            equipSlot = GetEquipSlot();
            equipSlot.slotName.Text = "헤드기어";
            equipSlot.SetSocket(inventory.headgear);
            otherSlotList.Add(equipSlot);
            control.AddChild(equipSlot);
            equipSlot.Position = new(110, 100);
        }
        {
            equipSlot = GetEquipSlot();
            equipSlot.slotName.Text = "방탄판";
            equipSlot.SetSocket(inventory.plate);
            otherSlotList.Add(equipSlot);
            control.AddChild(equipSlot);
            equipSlot.Position = new(250, 240);
        }

        {
            equipSlot = GetWeaponSlot();
            equipSlot.slotName.Text = "주무장";
            equipSlot.SetSocket(inventory.firstWeapon);
            otherSlotList.Add(equipSlot);
            control.AddChild(equipSlot);
            equipSlot.Position = new(110, 380);
        }
        {
            equipSlot = GetWeaponSlot();
            equipSlot.slotName.Text = "부무장";
            equipSlot.SetSocket(inventory.secondWeapon);
            otherSlotList.Add(equipSlot);
            control.AddChild(equipSlot);
            equipSlot.Position = new(110, 520);
        }
        {
            equipSlot = GetEquipSlot();
            equipSlot.slotName.Text = "보조무장";
            equipSlot.SetSocket(inventory.subWeapon);
            otherSlotList.Add(equipSlot);
            control.AddChild(equipSlot);
            equipSlot.Position = new(390, 380);
        }



        {
            storageSlot = GetStorageSlot();
            storageSlot.slotName.Text = "조끼";
            storageSlot.SetSocket(inventory.rig);
            otherInventory.AddChild(storageSlot);
            otherSlotList.Add(storageSlot);
        }
        {
            pocketSlot = GetPocketSlot();
            pocketSlot.SetStorage(inventory.pocket);
            otherInventory.AddChild(pocketSlot);
            otherSlotList.Add(pocketSlot);
        }
        {
            storageSlot = GetStorageSlot();
            storageSlot.slotName.Text = "가방";
            storageSlot.SetSocket(inventory.backpack);
            otherInventory.AddChild(storageSlot);
            otherSlotList.Add(storageSlot);
        }
        {
            storageSlot = GetStorageSlot();
            storageSlot.slotName.Text = "컨테이너";
            storageSlot.SetSocket(inventory.sContainer);
            otherInventory.AddChild(storageSlot);
            otherSlotList.Add(storageSlot);
        }


        UpdateAllUI();
    }
    public void ResetOtherPanel()
    {
        foreach (var control in otherInventory.GetChildren())
            control.QueueFree();

        foreach(var window in windowList)
            window.QueueFree();

        otherSlotList.Clear();
        windowList.Clear();
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
    public void OpenStorageWindow(Storage storage)
    {
        foreach (var window in windowList)
            if (window.FindByName("PocketSlot") is PocketSlot ps)
                if (ps.HasStorage(storage))
                    return;

        PocketSlot pocketSlot = GetPocketSlot();
        pocketSlot.SetStorage(storage);
        
        otherSlotList.Add(pocketSlot);

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
    //isRotated : 기존 아이템은 회전된 상태였는가?
    //toRotate : 드래그 중인 아이템은 놓이기 위해 어떤 상태로 되어 있는가?

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
    public (ItemModel, Vector2I)? ReleaseCursor() => (onDragging == null)? null : (onDragging, dragPos);

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