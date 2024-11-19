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
        Initiate(new Humanoid.Inventory(null));
    }

    public void Initiate(Humanoid.Inventory inventory)
    {
        this.inventory = inventory;
    }

    public override void Interacted(Humanoid humanoid)
    {
        if (humanoid is Player player) 
        {
            //string str = "";
            //foreach (var a in LevelManager.inst.GetSpawnedItems(LevelManager.Theme.ARMORY, new(3, 3), 1f))
            //    str += a.status.name + " - ";
            //GD.PushWarning(str);
            player.isInventory = true;
            UiMain.instance.page = UiMain.instance.SetPage(PageType.INVENTORY);
            InventoryPage.instance.SetOtherPanel(inventory);
            InventoryPage.instance.UpdateAllUI();
        }
    }

}