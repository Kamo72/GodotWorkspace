using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static UiMain;

public partial class PlayerStash : Interactable
{
    public override void _Ready()
    {
        base._Ready();
        Sprite2D sprite = new Sprite2D();
        sprite.Texture = ResourceLoader.Load<Texture2D>("res://Asset/guns/MCX.png");
        AddChild(sprite);

        interactableText = "개인 창고 열기";
        
    }

    public override void Interacted(Humanoid humanoid)
    {
        humanoid.isInventory = true;

        UiMain uiMain = UiMain.instance;
        uiMain.Visible = true; 
        uiMain.page = uiMain.SetPage(PageType.INVENTORY);
        InventoryPage.instance.SetOtherStash();
    }

}

