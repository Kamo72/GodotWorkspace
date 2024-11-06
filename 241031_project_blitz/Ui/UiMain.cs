using Godot;
using Godot.Collections;
using System;

public partial class UiMain : Control
{
    public Page page {
        get{
            Array<Node> childrens = this.FindByName("Pcon").GetChildren();

            if(childrens.Count == 0) return null;

            return childrens[0] as Page;
        }
        set{
            Node pCon = this.FindByName("Pcon");
            
            if(page != null) pCon.RemoveChild(page);
            if(value == null) return;

            pCon.AddChild(value);
        }
    }

    public Button profileB => this.FindByName("ButtonProfile") as Button;
    public Button inventoryB => this.FindByName("ButtonInventory") as Button;
    public Button statusB => this.FindByName("ButtonStatus") as Button;
    public Button mapB => this.FindByName("ButtonMap") as Button;
    public Button questB => this.FindByName("ButtonQuest") as Button;
    public Button optionB => this.FindByName("ButtonOption") as Button;

    public override void _EnterTree()
    {
        profileB.Pressed += ()=>
            page = null;
        inventoryB.Pressed += ()=>
            page = ResourceLoader.Load<PackedScene>("res://Prefab/UI/InventoryPage.tscn").Instantiate() as Page;
        statusB.Pressed += ()=>
            page = null;
        mapB.Pressed += ()=>
            page = null;
        questB.Pressed += ()=>
            page = null;
        optionB.Pressed += ()=>
            page = null;
    }

} 