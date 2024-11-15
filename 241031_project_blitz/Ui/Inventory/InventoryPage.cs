using Godot;
using System;
using static Humanoid;

public partial class InventoryPage : Page
{


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

    //TODO

    public Control cursor => this.FindByName("Cursor") as Control;

    public override void _EnterTree()
    {
        base._EnterTree();
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

    }

    public override void _Process(double delta)
    {
        base._Process(delta);
        cursor.GlobalPosition = GetGlobalMousePosition();

        UpdateEquipUI(helmetSlot, Player.player.inventory.helmet);
        UpdateEquipUI(headgearSlot, Player.player.inventory.headgear);
        UpdateEquipUI(plateSlot, Player.player.inventory.plate);
        UpdateEquipUI(firstWeaponSlot, Player.player.inventory.firstWeapon);
        UpdateEquipUI(secondWeaponSlot, Player.player.inventory.secondWeapon);
        UpdateEquipUI(subWeaponSlot, Player.player.inventory.subWeapon);

        UpdatePocketUI(pocketSlot, Player.player.inventory.pocket);

        UpdateStorageUI(rigSlot, Player.player.inventory.rig);
        UpdateStorageUI(backpackSlot, Player.player.inventory.backpack);
        UpdateStorageUI(containerSlot, Player.player.inventory.sContainer);
    }

    public override void _Input(InputEvent @event)
    {
        UiMain uiMain = GetParent().GetParent().GetParent() as UiMain;
        if (!uiMain.Visible) return;


        if (helmetSlot.GetInput(@event)) { }
        if (headgearSlot.GetInput(@event)) { }
        if (plateSlot.GetInput(@event)) { }
        if (firstWeaponSlot.GetInput(@event)) { }
        if (secondWeaponSlot.GetInput(@event)) { }
        if (subWeaponSlot.GetInput(@event)) { }

        if (pocketSlot.GetInput(@event)) { }

        if (rigSlot.GetInput(@event)) { }
        if (backpackSlot.GetInput(@event)) { }
        if (containerSlot.GetInput(@event)) { }
        if (@event is InputEventMouseButton mouseEvent)
            if(mouseEvent.Pressed != true)
                SetCursor(null, new());

        UpdateAllUI();

    }

    public void UpdateAllUI()
    {
        helmetSlot.updated = false;
        headgearSlot.updated = false;
        plateSlot.updated = false;
        firstWeaponSlot.updated = false;
        secondWeaponSlot.updated = false;
        subWeaponSlot.updated = false;

        pocketSlot.updated = false;

        rigSlot.updated = false;
        backpackSlot.updated = false;
        containerSlot.updated = false;
    }

    private void UpdateEquipUI(EquipSlot grid, Player.Inventory.EquipSlot slot)
    {
        if (grid.updated == false)
            grid.RestructureStorage(slot.item);
    }

    private void UpdateStorageUI(StorageSlot grid, Player.Inventory.EquipSlot slot)
    {
        if (grid.updated == false)
            grid.RestructureStorage(slot.item);
    }

    private void UpdatePocketUI(PocketSlot grid, Storage storage)
    {
        if (grid.updated == false)
            grid.RestructureStorage(storage);
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