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

    Control cursor => this.FindByName("Cursor") as Control;

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

        //myInventory.
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
        cursor.GlobalPosition = GetGlobalMousePosition();


        // 조끼 및 가방 Storage UI 업데이트
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
        if (slot.item == null)
        {
            grid.SetStorageGrid(new Vector2I(0, 0));
            return;
        }


        if (slot.item is HasStorage storage)
        {
            //grid.ClearChildren();

            foreach (var storageNode in storage.storage.itemList)
            {
                //Add storageNode
            }
        }
    }



}