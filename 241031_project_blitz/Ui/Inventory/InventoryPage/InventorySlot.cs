using Godot;
using System.Collections.Generic;

public abstract partial class InventorySlot : Control
{
    public InventoryPage inventoryPage => ((InventoryPage)GetParent().GetParent().GetParent().GetParent());

    public bool updated = false;
    public abstract void OnMouseProcess();

    public static Dictionary<string, Color> highlight = new() //하이라이트 색상 정보
    {
        { "idle", new Color(1,1,1)},
        { "disable", new Color(1,0,0)},
        { "enable", new Color(0,1,0)},
        { "onMouse", new Color(0,0.5f,0.5f)},
    };

    public abstract bool GetInput(InputEvent @event);

    public abstract void RestructureStorage();
}