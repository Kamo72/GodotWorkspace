using Godot;
using System;
using System.Collections.Generic;
using static Storage;

public partial class EquipSlot : InventorySlot
{
    /* UI Reference */
    PanelContainer slotContainer => this.FindByName("SlotContainer") as PanelContainer;
    public Label slotName => this.FindByName("SlotTypeText") as Label;


    /* Reference */
    Humanoid.Inventory.EquipSlot slot; //소켓과 장비된 아이템 정보
    Equipable equiped = null; //소켓과 장비된 아이템 정보


    /* Variables */
    List<ItemModel> itemModels = new List<ItemModel>(); //장비된 아이템 내 Storage의 아이템들


    /* Override */
    public override void _Process(double delta)
    {
        OnMouseProcess();
    }


    /* Initiate */
    public void SetSocket(Humanoid.Inventory.EquipSlot equipSlot)
    {
        slot = equipSlot;
    }


    /* Process */
    public ItemModel onMouseItem = null; // 마우스 아이템
    //OnMouse 정보를 찾는 과정 + 각 슬롯과 소켓의 UI 하이라이팅
    public override void OnMouseProcess()
    {
        ItemModel foundItem = null;

        //마우스가 있는 아이템 찾기
        foreach (ItemModel iModel in itemModels)
        {
            Rect2 rect = iModel.GetRect();
            rect.Position = iModel.GlobalPosition;

            if (rect.HasPoint(GetGlobalMousePosition()))
            {
                foundItem = iModel;
                //onMouseNow = iModel.storagePos;
                break;
            }
        }

        onMouseItem = foundItem;

        //장비칸 하이라이트 적용
        Rect2 rectt = slotContainer.GetRect();
        rectt.Position = slotContainer.GlobalPosition;
        if (rectt.HasPoint(GetGlobalMousePosition()))
        {
            slotContainer.Modulate = highlight["onMouse"];
        }
        else
        {
            slotContainer.Modulate = highlight["idle"];
        }

    }


    /* Updater */
    //주어진 Equipable에 따라 모든 아이템 UI 초기화 (updated 변수에 의해 호출)
    public override void RestructureStorage()
    {
        if (updated) return;
        updated = true;

        Equipable equipable = slot.item;
        {
            equiped = equipable;
            if (equipable == null)
            {
                ResetItemModel();
                return;
            }

            ResetItemModel();
            SetItemEquiped(equipable);
        }
    }

    //_Input 호출부를 InventoryPage급으로 올리며네서 리팩토링된 코드
    public override bool GetInput(InputEvent @event)
    {
        Rect2 rect = GetRect();
        rect.Position = GlobalPosition;

        if (rect.HasPoint(GetGlobalMousePosition()) == false)
        {
            //updated = false;
            return false;
        }

        if (@event is InputEventMouseButton mouseEvent)
        {
            //Mouse button pressed 
            if (mouseEvent.Pressed && mouseEvent.ButtonIndex == MouseButton.Left)
            {
                //해당 지점에 아이템이 있다면 커서에 저장
                if (onMouseItem != null)
                {
                    GD.PushWarning("Mouse button pressed : " + onMouseItem.storagePos);
                    Vector2I itemSize = onMouseItem.item.status.size;
                    inventoryPage.SetCursor(onMouseItem, new(itemSize.X / 2, itemSize.Y / 2));
                }

                return true; //해당 코드에서 처리하기 성공
            }
            //Mouse button released
            else if (mouseEvent.ButtonIndex == MouseButton.Left)
            {
                //커서에 아이템이 있다면
                var result = inventoryPage.ReleaseCursor();
                if (result.HasValue)
                {
                    ItemModel draggingItem = result.Value.Item1;
                    Vector2I dragPos = result.Value.Item2;

                    //소켓에 템이 장착됨
                    if (equiped is HasStorage hasStorage)
                    {
                        //아이템을 스스로 안에 저장 시도 시
                        if (equiped == draggingItem.item)
                            return false;

                            
                        //빠른 보관
                        bool insertable = hasStorage.storage.IsAbleToInsert(draggingItem.item);
                        if (insertable)
                        {
                            StorageNode? sNode = hasStorage.storage.GetPosInsert(draggingItem.item);
                            if (sNode.HasValue)
                                return hasStorage.storage.Insert(sNode.Value);
                            //해당 코드에서 처리하기 성공
                        }
                        return false;
                    }

                    //비어 있는 소켓에 아이템에 가져다 놓기
                    else if (equiped == null)
                    {
                        if (draggingItem.item is Equipable draggingEquipable)
                        {
                            bool isEpquipable = slot.AbleEquipItem(draggingEquipable);

                            GD.PushWarning("isEpquipable : " + isEpquipable);
                            if (!isEpquipable) return false;

                            if (draggingEquipable.isEquiping)
                                draggingEquipable.UnEquip();

                            bool equipTargetResult = slot.DoEquipItem(draggingEquipable);

                            GD.PushWarning("equipTargetResult : " + equipTargetResult);
                            return equipTargetResult;
                        }
                    }
                    
                    onMouseItem = null;
                }
            }
        }

        if (@event is InputEventMouseMotion mouseMotionEvent)
        {
            GD.Print("Mouse moved");
        }

        if (GetRect().HasPoint(GetLocalMousePosition()))
        {
            GD.Print("Mouse is over the control");
        }

        return false; //해당 코드에서 처리하지 못함
    }


    /* Convenience Functions  */
    //소켓 장비 UI 추가
    public void SetItemEquiped(Equipable equipable)
    {
        ItemModel iModel = new ItemModel(equipable, slotContainer);
        AddChild(iModel);
        itemModels.Add(iModel);
        iModel.Position = slotContainer.Position;

    }
    //모든 아이템 UI 삭제
    public void ResetItemModel()
    {
        foreach (ItemModel iModel in itemModels)
            iModel.QueueFree();
        itemModels.Clear();
    }
}
