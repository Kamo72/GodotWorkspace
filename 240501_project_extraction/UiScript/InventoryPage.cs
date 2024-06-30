using Godot;
using System;

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
    VBoxContainer otherInventory => this.FindByName("OtherInventory") as VBoxContainer;


    public override void _Ready()
    {
        base._Ready();

        helmetSlot.slotName.Text = "헬멧";
        headgearSlot.slotName.Text = "헤드기어";
        plateSlot.slotName.Text = "방탄판";
        firstWeaponSlot.slotName.Text = "주무장";
        secondWeaponSlot.slotName.Text = "부무장";
        subWeaponSlot.slotName.Text = "보조무장";
    }




}