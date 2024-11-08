using Godot;

public partial class ItemModel : Control
{
    public Item item;
    //private Sprite2D icon;
    public TextureRect textureRect;

    public Vector2I itemSize => item.status.size;
    public Vector2I storagePos = new Vector2I(-1, -1);

    public ItemModel(Item item, Vector2I pos, Vector2 size)
    {
        this.item = item;
        //GD.PushError("Size : " + size.ToString());

        CustomMinimumSize = size;
        //textureRect.ClipContents = true;
        Size = size;

        textureRect = new TextureRect
        {
            Texture = (Texture2D)ResourceLoader.Load(item.status.textureRoot),

            ExpandMode = TextureRect.ExpandModeEnum.FitWidth,
            StretchMode = TextureRect.StretchModeEnum.KeepAspectCentered,
        };
        AddChild(textureRect);
        textureRect.Size = size;
        storagePos = pos;
    }

    public void SetDragging(bool isDragging) 
    {
        textureRect.Modulate = new Color(1, 1, 1, isDragging ? 0.4f : 1);
    }
}