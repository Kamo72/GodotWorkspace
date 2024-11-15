using Godot;
using System;
using System.Drawing;
using Color = Godot.Color;

public partial class ItemModel : Control
{
    public Item item;
    public Label label => this.FindByName("Label") as Label;
    public TextureRect textureRect => this.FindByName("TextureRect") as TextureRect;

    public Vector2I itemSize => item.status.size;
    public Vector2I storagePos = new Vector2I(-1, -1);
    public bool isEquiped => storagePos == new Vector2I(-1, -1);

    public ItemModel(Item item, Control slotContainer) 
    {
        this.item = item;

        if (slotContainer.FindByName("ItemTexture") is TextureRect tRect)
        {
            CustomMinimumSize = tRect.Size;
            Size = tRect.Size;

            var label = new Label { 
                Name="Label",
                Text = item.status.name,
            };
            AddChild(label);

            var textureRect = new TextureRect
            {
                Name = "TextureRect",
                Texture = (Texture2D)ResourceLoader.Load(item.status.textureRoot),
                TextureFilter = TextureFilterEnum.Nearest,

                ExpandMode = TextureRect.ExpandModeEnum.FitWidth,
                StretchMode = TextureRect.StretchModeEnum.KeepAspectCentered,
            };
            AddChild(textureRect);
            textureRect.Size = tRect.Size;



            if (item is WeaponItem weapon)
            {
                string text = "";
                Magazine mag = weapon.magazine;
                text += mag != null ? $"{mag.ammoCount}/{mag.magStatus.ammoSize}" : "0/0";
                text += weapon.weaponStatus.detailDt.chamberSize == 0 ? "" :  weapon.chamber == null? "+0" : "+1";

                var stackLabel = new Label
                {
                    Name = "StackLabel",
                    Text = text, 
                    Size = Size,
                    HorizontalAlignment = HorizontalAlignment.Right,
                    VerticalAlignment = VerticalAlignment.Bottom,
                };
                AddChild(stackLabel);
            }

        }
    }

    public ItemModel(Item item, Vector2I pos, Vector2 size)
    {
        this.item = item;
        //GD.PushError($"ItemModel() : {item.status.name} {pos} {size}");
        CustomMinimumSize = size;
        Size = size;

        var label = new Label
        {
            Name = "Label",
            Text = item.status.shortName,
        };
        AddChild(label);

        var textureRect = new TextureRect
        {
            Name = "TextureRect",
            Texture = (Texture2D)ResourceLoader.Load(item.status.textureRoot),
            TextureFilter = TextureFilterEnum.Nearest,

            ExpandMode = size.Y > size.X? TextureRect.ExpandModeEnum.FitHeight : TextureRect.ExpandModeEnum.FitWidth,
            StretchMode = TextureRect.StretchModeEnum.KeepAspectCentered,
        };
        AddChild(textureRect);
        textureRect.Size = size;
        storagePos = pos;


        if (item is IStackable iStackable)
        {
            var stackLabel = new Label
            {
                Name = "StackLabel",
                Text = iStackable.stackNow.ToString(),
                Size = Size,
                HorizontalAlignment = HorizontalAlignment.Right,
                VerticalAlignment = VerticalAlignment.Bottom,
            };
            AddChild(stackLabel);
        }

        if (item is Magazine magazine)
        {
            var stackLabel = new Label
            {
                Name = "StackLabel",
                Text = magazine.ammoCount + "/" + magazine.magStatus.ammoSize,
                Size = Size,
                HorizontalAlignment = HorizontalAlignment.Right,
                VerticalAlignment = VerticalAlignment.Bottom,
            };
            AddChild(stackLabel);
        }



    }

    public void SetDragging(bool isDragging)
    {
        if (textureRect != null)
            textureRect.Modulate = new Color(1, 1, 1, isDragging ? 0.4f : 1);
        try
        {
           
        }
        catch { }
    }

    public override void _Draw()
    {
        if (textureRect == null)
        {
            GD.PushError("textureRect == null!!!!");
            GD.PushError(item.status.name);
        }
        DrawRect(new Rect2(Vector2.Zero, textureRect.Size), Colors.White, false, 1);
        DrawRect(new Rect2(Vector2.Zero, textureRect.Size), new Color(0.2f, 0.2f, 0.2f,0.5f), true);
    }
}