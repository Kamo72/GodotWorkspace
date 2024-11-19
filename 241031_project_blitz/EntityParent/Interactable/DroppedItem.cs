using Godot;
using System;

public partial class DroppedItem : Interactable
{
    public override void _Process(double delta)
    {
        base._Process(delta);
    }

    public Item item = null;
    public void SetItem(Item item)
    {
        this.item = item;
        Sprite2D sprite = this.FindByName("Sprite2D") as Sprite2D;
        sprite.Texture = ResourceLoader.Load<Texture2D>(item.status.textureRoot);
        sprite.TextureFilter = TextureFilterEnum.Nearest;
    }

    public override void Interacted(Humanoid humanoid)
    {
        if (item == null || humanoid == null) return;
        if (humanoid.inventory == null) return;

        bool taken = humanoid.inventory.TakeItem(item);

        if (!taken) return;

        GD.Print("DropppedItem interacted!");
        item.droppedItem = null;
        GetParent().RemoveChild(this);
    }
}