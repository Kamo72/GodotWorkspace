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
    public bool isActivated = true;

    public virtual void OnMouseProcess() { }

    public static Dictionary<string, Color> highlight = new() //하이라이트 색상 정보
    {
        { "idle", new Color(1,1,1)},
        { "disable", new Color(1,0,0)},
        { "enable", new Color(0,1,0)},
        { "onMouse", new Color(0,0.5f,0.5f)},
    };

    //InventoryPage로부터 입력에 대한 처리를 호출
    protected bool droppingKey = false;   //Caps  빠른 버리기
    protected bool slicingKey = false;    //shift 나누기
    protected bool movingKey = false;     //Cntl  빠른 옮기기
    protected bool equipingKey = false;   //alt   빠른 장착
    public virtual bool GetInput(InputEvent @event) {

        if (@event is InputEventKey keyEvent)
        {
            switch (keyEvent.Keycode)
            {
                case Key.Capslock:
                    droppingKey = keyEvent.Pressed; break;
                case Key.Shift:
                    slicingKey = keyEvent.Pressed; break;
                case Key.Ctrl:
                    movingKey = keyEvent.Pressed; break;
                case Key.Alt:
                    equipingKey = keyEvent.Pressed; break;
            }
        }
        return false; 
    }

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

    public void SetActivate(bool isActivated)
    {
        this.isActivated = isActivated;

        Modulate = new Color(1,1,1, isActivated? 1f : 0.4f);
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