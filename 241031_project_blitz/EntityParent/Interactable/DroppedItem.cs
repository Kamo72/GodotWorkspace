using Godot;
using System;

public partial class DroppedItem : Interactable
{
    public override void _Process(double delta)
    {
        if (isHighlighted)
            if (Player.player != null && item != null)
                interactableText = $"줍기 {item.status.shortName}" + (Player.player.inventory.TakeItemAvailable(item) ? "":"(가득 참)");

        base._Process(delta);
    }

    public Item item = null;
    public void SetItem(Item item)
    {
        this.item = item;
        Sprite2D sprite = this.FindByName("Sprite2D") as Sprite2D;
        sprite.Texture = ResourceLoader.Load<Texture2D>(item.status.textureRoot);
        sprite.Scale = Vector2.One * 2.5f;
        sprite.TextureFilter = TextureFilterEnum.Nearest;
    }

    public override void Interacted(Humanoid humanoid)
    {
        if (item == null) return;

        bool takable = humanoid.inventory.TakeItemAvailable(item);

        if (!takable) return;


        humanoid.inventory.TakeItem(item);
        item.droppedItem = null;
        QueueFree();
    }
}