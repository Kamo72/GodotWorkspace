using Godot;
using System;
using System.Collections.Generic;
using static Humanoid;

public partial class InventoryPage : Page
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


    public Control cursor => this.FindByName("Cursor") as Control;

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
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
        cursor.GlobalPosition = GetGlobalMousePosition();

        foreach (var item in mySlotList)
            item.RestructureStorage();
        foreach (var item in otherSlotList)
            item.RestructureStorage();
    }

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

        UpdateAllUI();

    }

    public void UpdateAllUI()
    {
        foreach (var item in mySlotList)
            item.updated = false;

        foreach (var item in otherSlotList)
            item.updated = false;
    }

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

        {
            equipSlot = GetEquipSlot();
            equipSlot.slotName.Text = "헬멧";
            equipSlot.SetSocket(inventory.helmet);
            otherInventory.AddChild(equipSlot);
            otherSlotList.Add(equipSlot);
        }
        {
            equipSlot = GetEquipSlot();
            equipSlot.slotName.Text = "헤드기어";
            equipSlot.SetSocket(inventory.headgear);
            otherInventory.AddChild(equipSlot);
            otherSlotList.Add(equipSlot);
        }
        {
            equipSlot = GetEquipSlot();
            equipSlot.slotName.Text = "방탄판";
            equipSlot.SetSocket(inventory.plate);
            otherInventory.AddChild(equipSlot);
            otherSlotList.Add(equipSlot);
        }

        {
            equipSlot = GetWeaponSlot();
            equipSlot.slotName.Text = "주무장";
            equipSlot.SetSocket(inventory.firstWeapon);
            otherInventory.AddChild(equipSlot);
            otherSlotList.Add(equipSlot);
        }
        {
            equipSlot = GetWeaponSlot();
            equipSlot.slotName.Text = "부무장";
            equipSlot.SetSocket(inventory.secondWeapon);
            otherInventory.AddChild(equipSlot);
            otherSlotList.Add(equipSlot);
        }
        {
            equipSlot = GetEquipSlot();
            equipSlot.slotName.Text = "보조무장";
            equipSlot.SetSocket(inventory.subWeapon);
            otherInventory.AddChild(equipSlot);
            otherSlotList.Add(equipSlot);
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



    public void ResetOtherPanel()
    {
        foreach (var slot in otherSlotList)
            slot.QueueFree();
        otherSlotList.Clear();
    }


    ItemModel onDragging = null;        //집은 아이템
    Vector2I dragPos = Vector2I.Zero;   //집은 위치 저장

    //커서 클릭 시 반응
    public void SetCursor(ItemModel iModel, Vector2I dragPos)
    {
        //GD.PushWarning("SetCursor : " + iModel + " - " + dragPos);

        //image 가져오기
        TextureRect image = cursor.FindByName("ItemImage") as TextureRect;

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

        //집은 위치 구하기
        this.dragPos = dragPos;
        Vector2 dragRatio = new Vector2(
            (-1f - dragPos.X * 2f) / (iModel.itemSize.X * 2f),
            (-1f - dragPos.Y * 2f) / (iModel.itemSize.Y * 2f));

        //GD.PushWarning("dragRatio : " + dragRatio.ToString());
        //GD.PushWarning("dragPos : " + dragPos.ToString());
        //GD.PushWarning("iModel.itemSize : " + iModel.itemSize.ToString());

        //image 세팅
        image.Texture = (Texture2D)ResourceLoader.Load(iModel.item.status.textureRoot);
        image.CustomMinimumSize = new(iModel.textureRect.Size.X, iModel.textureRect.Size.Y);
        image.Position = new Vector2(
            iModel.textureRect.Size.X * dragRatio.X,
            iModel.textureRect.Size.Y * dragRatio.Y
            );

        //onDragging 세팅
        onDragging = iModel;
        onDragging?.SetDragging(true);
        
    }

    //집고 있는 아이템 구하기
    public (ItemModel, Vector2I)? ReleaseCursor()
    {
        if (onDragging == null) return null;

        return (onDragging, dragPos);
    }


}