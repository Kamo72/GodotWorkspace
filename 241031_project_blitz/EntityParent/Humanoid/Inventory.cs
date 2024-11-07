
using Godot;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using StorageNode = Storage.StorageNode;


public partial class Humanoid
{
    public Inventory inventory;
    public partial class Inventory
    {
        public Inventory(Humanoid master)
        {
            this.master = master;
        }
        public Humanoid master;

        #region [제공 함수]
        //드랍된 아이템 줍기
        public bool TakeItem(Item item)
        {
            StorageNode? newPlace;

            //주머니 먼저 삽입
            newPlace = pocket.GetPosInsert(item);
            if (newPlace.HasValue)
            {
                item.onStorage?.RemoveItem(item);
                pocket.Insert(newPlace.Value);

                return true;
            }

            //가방에 삽입
            if (backpack.item is Backpack bp)
            {
                newPlace = bp.storage.GetPosInsert(item);
                if (newPlace.HasValue)
                {
                    item.onStorage?.RemoveItem(item);
                    bp.storage.Insert(newPlace.Value);

                    return true;
                }
            }

            return false;
        }

        private bool EquipToSlot(Item item, EquipSlot slot)
        {
            if(item is Equipable equipable)
            if (slot.DoEquipItem(equipable))
            {
                equipable.BeEquip(master);
                return true;
            }
            return false;
        }
        //빠른 장착
        public bool EquipItemQuick(Item item)
        {
            if (item is WeaponItem newWeapon)
            {
                if (newWeapon.AbleSub() && EquipToSlot(newWeapon, subWeapon)) return true;
                if (newWeapon.AbleMain() && (EquipToSlot(newWeapon, firstWeapon) || EquipToSlot(newWeapon, secondWeapon))) return true;
            }
            else if (item is Headgear newHeadgear && EquipToSlot(newHeadgear, headgear)) return true;
            else if (item is Backpack newBackpack && EquipToSlot(newBackpack, backpack)) return true;
            else if (item is Plate newPlate && EquipToSlot(newPlate, plate)) return true;
            else if (item is Helmet newHelmet && EquipToSlot(newHelmet, helmet)) return true;

            Console.WriteLine("EquipItem - 장착할 수 없는 아이템이거나 적절한 위치를 찾지 못했습니다.");
            return false;
        }

        //슬롯 지정 장착
        public bool EquipItemTarget(EquipSlot slot, Equipable item)
        {
            if (slot.DoEquipItem((Item)item))
            {
                item.UnEquip(master);
                return true;
            }
            return false;
        }

        //아이템 장착 해제
        public bool UnEquipItem(EquipSlot slot, bool doThrow)
        {
            //해당 슬롯에 아이템이 없음.
            if (slot.item == null) { return false; }

            //인벤토리로
            if (!doThrow)
            {
                bool isSuceed = TakeItem((Item)slot.item);
                if (!isSuceed) { return false; }
            }
            else //필드로
            {
                ThrowItem((Item)slot.item);
            }

            slot.UnEquipItem();
            slot.item.UnEquip(master);

            return true;
        }

        //해당 아이템 드랍하기
        public void ThrowItem(Item item)
        {
            item.onStorage?.RemoveItem(item);
            item.GetDroppedItem(master.Position);

            //item.droppedItem.LinearVelocity = Vector2.FromAngle(master.hands.direction) * 100f;
        }

        public void PrintInventoryStatus()
        {
            Console.WriteLine("=== 장착된 아이템 ===");
            Console.WriteLine("Helmet: " + (helmet.item?.status.name ?? "없음"));
            Console.WriteLine("Headgear: " + (headgear.item?.status.name ?? "없음"));
            Console.WriteLine("First Weapon: " + (firstWeapon.item?.status.name ?? "없음"));
            Console.WriteLine("Second Weapon: " + (secondWeapon.item?.status.name ?? "없음"));
            Console.WriteLine("Sub Weapon: " + (subWeapon.item?.status.name ?? "없음"));
            Console.WriteLine("Backpack: " + (backpack.item?.status.name ?? "없음"));

            Console.WriteLine("=== 주머니 아이템 ===");
            foreach (var item in pocket.itemList)
                Console.WriteLine("Item: " + item.item.status.name);
            
        }
        #endregion

        #region [저장 공간]
        public EquipSlot helmet = new EquipSlot(Item.Category.HELMET);
        public EquipSlot plate = new EquipSlot(Item.Category.PLATE);
        public EquipSlot headgear = new EquipSlot(Item.Category.HEADGEAR);
        public EquipSlot firstWeapon = new EquipSlot(Item.Category.WEAPON);
        public EquipSlot secondWeapon = new EquipSlot(Item.Category.WEAPON);
        public EquipSlot subWeapon = new EquipSlot(Item.Category.WEAPON, true);
        public EquipSlot rig = new EquipSlot(Item.Category.RIG);
        public Storage pocket = new Storage(new Vector2I(4, 1));
        public EquipSlot backpack = new EquipSlot(Item.Category.BACKPACK);
        public EquipSlot sContainer = new EquipSlot(Item.Category.S_CONTAINER);

        #endregion


    }

}


public partial class Humanoid
{
    public partial class Inventory
    {
        public class EquipSlot
        {


            public EquipSlot(Item.Category equipmentType, bool isShortWeapon = false)
            {
                this.equipmentType = equipmentType;
            }

            public Equipable item;
            public Item.Category equipmentType;
            bool isShortWeapon;

            public bool AbleEquipItem(Item item)
            {
                return true;
            }
            public virtual bool DoEquipItem(Item item)
            {
                switch (equipmentType)
                {
                    case Item.Category.WEAPON:
                        {
                            if (item is WeaponItem == false)
                                return false;
                            else if (((WeaponItem)item).AbleSub() && !isShortWeapon
                                  && ((WeaponItem)item).AbleMain() && isShortWeapon)
                                return false;
                        }
                        break;

                    case Item.Category.HEADGEAR: if (item is Headgear == false) return false; break;
                    case Item.Category.HELMET: if (item is Helmet == false) return false; break;
                    case Item.Category.PLATE: if (item is Plate == false) return false; break;

                    case Item.Category.BACKPACK: if (item is Backpack == false) return false; break;
                    case Item.Category.RIG: if (item is Rig == false) return false; break;
                    case Item.Category.S_CONTAINER: if (item is SecContainer == false) return false; break;
                }

                if (this.item == null)
                {
                    this.item = item as Equipable;
                    return true;
                }

                return false;
            }
            public Item UnEquipItem()
            {
                if (this.item != null)
                {
                    Item item = (Item)this.item;
                    this.item = null;
                    return item;
                }
                return null;
            }


        }

    }
}