using Godot;
using System.Collections.Generic;

public partial class InventorySlot : Control
{
    public InventoryPage inventoryPage => InventoryPage.instance;

    public static InventorySlotContainer inventoryContainer
    { get {

            if(InventoryPage.instance != null && InventoryPage.instance.Visible == true)
                return InventoryPage.instance as InventorySlotContainer;

            else if (Trade.instance != null && Trade.instance.Visible == true)
                return Trade.instance as InventorySlotContainer;

            else return null;
        } }

    public bool storageUpdated = false;
    public bool uiUpdated = false;
    public virtual void OnMouseProcess() { }

    public static Dictionary<string, Color> highlight = new() //하이라이트 색상 정보
    {
        { "idle", new Color(1,1,1)},
        { "disable", new Color(1,0,0)},
        { "enable", new Color(0,1,0)},
        { "onMouse", new Color(0,0.5f,0.5f)},
    };

    public virtual bool GetInput(InputEvent @event) { return false; }

    public virtual void RestructureStorage() { }

    public (ItemModel, Vector2I)? GetCursor()
    {
        if (InventoryPage.instance != null && InventoryPage.instance.Visible)
            return InventoryPage.instance.ReleaseCursor();
        else
            return  Trade.instance.ReleaseCursor();
    }

    public void SetCursor(ItemModel iModel, Vector2I dragPos)
    {
        if (InventoryPage.instance != null && InventoryPage.instance.Visible)
            InventoryPage.instance.SetCursor(iModel, dragPos);
        else
            Trade.instance.SetCursor(iModel, dragPos);
    }
}


public interface InventorySlotContainer
{
    public void SetCursor(ItemModel iModel, Vector2I dragPos);
    public (ItemModel, Vector2I)? ReleaseCursor();
    public void RotateCursor();


    public bool isRotated { get; set; }
    public bool toRotate{get; set;}
}