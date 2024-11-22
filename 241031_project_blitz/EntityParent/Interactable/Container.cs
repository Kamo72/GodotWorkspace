using Godot;
using Godot.NativeInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static UiMain;

public partial class Container : Interactable
{
    public Storage storage;

    public override void _Ready()
    {
        interactableText = "열기";
        Initiate(new(5, 3), LevelManager.Theme.ARMORY, 1f);
    }

    public void Initiate(Vector2I size, LevelManager.Theme theme, float valueRatio)
    {
        storage = new Storage(size);
        var list = LevelManager.inst.GetSpawnedItems(theme, storage.size, valueRatio);

        if (list == null) throw new Exception("Container - Initiate - LevelManager.inst.GetSpawnedItems() == null!!!");

        foreach (var item in list)
        {
            var nullableNode = storage.GetPosInsert(item);

            if(nullableNode.HasValue == false)
                throw new Exception("Container - Initiate - storage.GetPosInsert(item).HasValue == false!!!");

            storage.Insert(nullableNode.Value);
        }
    }

    public override void Interacted(Humanoid humanoid)
    {
        if (humanoid is Player player) 
        {
            player.isInventory = true;
            UiMain.instance.page = UiMain.instance.SetPage(PageType.INVENTORY);
            InventoryPage.instance.SetOtherPanel(storage);
            InventoryPage.instance.UpdateAllUI();
        }
    }

}