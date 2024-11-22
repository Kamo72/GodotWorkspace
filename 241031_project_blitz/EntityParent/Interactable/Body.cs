using Godot;
using Godot.NativeInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static UiMain;

public partial class Body : Interactable
{
    public Humanoid.Inventory inventory;

    public override void _Ready()
    {
        interactableText = "확인";
    }

    public void Initiate(Humanoid.Inventory inventory)
    {
        this.inventory = inventory;
    }

    public override void Interacted(Humanoid humanoid)
    {
        if(inventory == null)
            Initiate(new Humanoid.Inventory(null));

        if (humanoid is Player player) 
        {
            player.isInventory = true;
            UiMain.instance.page = UiMain.instance.SetPage(PageType.INVENTORY);
            InventoryPage.instance.SetOtherPanel(inventory);
            InventoryPage.instance.UpdateAllUI();
        }
    }

}