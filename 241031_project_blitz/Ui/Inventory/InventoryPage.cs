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

    public Control cursor => this.FindByName("Cursor") as Control;

    public override void _EnterTree()
    {
        base._EnterTree();

        helmetSlot.slotName.Text = "헬멧";
        headgearSlot.slotName.Text = "헤드기어";
        plateSlot.slotName.Text = "방탄판";
        firstWeaponSlot.slotName.Text = "주무장";
        secondWeaponSlot.slotName.Text = "부무장";
        subWeaponSlot.slotName.Text = "보조무장";

        rigSlot.slotName.Text = "조끼";
        rigSlot.DeclareStorageGrid(new Vector2I(4,2));
        pocketSlot.DeclareStorageGrid(new Vector2I(4,1));
        backpackSlot.slotName.Text = "가방";
        backpackSlot.DeclareStorageGrid(new Vector2I(4,5));
        containerSlot.slotName.Text = "컨테이너";
        containerSlot.DeclareStorageGrid(new Vector2I(2,2));

    }

    public override void _Process(double delta)
    {
        base._Process(delta);
        cursor.GlobalPosition = GetGlobalMousePosition();

        UpdateStorageUI(rigSlot, Player.player.inventory.rig);
        UpdateStorageUI(backpackSlot, Player.player.inventory.backpack);
        UpdateStorageUI(containerSlot, Player.player.inventory.sContainer);
    }

    private void UpdateEquipUI(EquipSlot grid, Player.Inventory.EquipSlot slot)
    {
        if (slot.item == null)
        {
            return;
        }
        //TODO
    }

    private void UpdateStorageUI(StorageSlot grid, Player.Inventory.EquipSlot slot)
    {
        if (grid.updated == false)
            grid.RestructureStorage(slot.item);
        //if (slot.item == null)
        //{
        //    grid.SetStorageGrid(new Vector2I(0, 0));
        //    return;
        //}

        //if (slot.item is HasStorage storage)
        //{
        //    grid.SetStorageGrid(storage.storage.size); // Storage의 크기에 맞춰 그리드 설정

        //    foreach (var storageNode in storage.storage.itemList)
        //    {
        //        if (storageNode.item != null)
        //        {
        //            // ItemModel을 생성하고 아이템의 스프라이트를 적절한 위치에 추가
        //            ItemModel itemModel = new ItemModel(storageNode.item);

        //            // storageNode의 위치에 맞춰 ItemModel 배치
        //            Vector2 position = CalculatePosition(storageNode.pos, grid);
        //            itemModel.SetPosition(position);
        //            Vector2 size = CalculateSize(storageNode.item.status.size, grid);
        //            itemModel.SetSize(size);

        //            // 그리드에 ItemModel 추가
        //            grid.AddChild(itemModel);
        //        }
        //    }
        //}
    }


    ItemModel onDragging = null;
    Vector2I dragPos = Vector2I.Zero;
    public void SetCursor(ItemModel iModel, Vector2I dragPos)
    {
        TextureRect image = cursor.FindByName("ItemImage") as TextureRect;

        if (iModel == null) 
        {
            image.Texture = null;
            image.CustomMinimumSize = Vector2.Zero;
            onDragging?.SetDragging(false);
            onDragging = null;
            return;
        }

        this.dragPos = dragPos;
        Vector2 dragRatio = new Vector2(
            (-1f - dragPos.X * 2f) / (iModel.itemSize.X * 2f),
            (-1f - dragPos.Y * 2f) / (iModel.itemSize.Y * 2f));

        GD.PushWarning("dragRatio : " + dragRatio.ToString());
        GD.PushWarning("dragPos : " + dragPos.ToString());
        GD.PushWarning("iModel.itemSize : " + iModel.itemSize.ToString());

        image.Texture = (Texture2D)ResourceLoader.Load(iModel.item.status.textureRoot);
        image.CustomMinimumSize = new(iModel.textureRect.Size.X, iModel.textureRect.Size.Y);
        image.Position = new Vector2(
            iModel.textureRect.Size.X * dragRatio.X,
            iModel.textureRect.Size.Y * dragRatio.Y
            );

        onDragging = iModel;
        onDragging?.SetDragging(true);
        
    }

    public (ItemModel, Vector2I)? ReleaseCursor()
    {
        if (onDragging == null) return null;

        return (onDragging, dragPos);
    }
}