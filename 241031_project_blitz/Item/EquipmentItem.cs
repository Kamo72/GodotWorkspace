using Godot;
using System;
using System.Collections.Generic;
public class Equipable : Item
{
    public Humanoid equipedBy { get; set; }

    //장착 및 장착 해제
    public void BeEquip(Humanoid entity)
    {
        onStorage?.RemoveItem(this);
        
        equipedBy = entity;
    }
    public Item UnEquip()
    {
        //GD.PushWarning("UnEquip Called!");
        if (equipedBy == null) throw new Exception("Equipable - UnEquip - ERROR : 장착하지 않은 아이템을 장착 해제하려고 합니다!");
        if (equipedBy is Humanoid human)
        {
            equipedBy = null;
            Item result = null;
            switch (status.category)
            {
                case Category.HEADGEAR:
                    result = human.inventory.headgear.UnEquipItem();
                    break;
                case Category.HELMET:
                    result = human.inventory.helmet.UnEquipItem();
                    break;
                case Category.PLATE:
                    result = human.inventory.plate.UnEquipItem();
                    break;
                case Category.RIG:
                    result = human.inventory.rig.UnEquipItem();
                    break;
                case Category.BACKPACK:
                    result = human.inventory.backpack.UnEquipItem();
                    break;
                case Category.S_CONTAINER:
                    result = human.inventory.sContainer.UnEquipItem();
                    break;
                case Category.WEAPON:
                    if (human.inventory.firstWeapon.item == this)
                        result = human.inventory.firstWeapon.UnEquipItem();
                    else if (human.inventory.secondWeapon.item == this)
                        result = human.inventory.secondWeapon.UnEquipItem();
                    else
                        result = human.inventory.subWeapon.UnEquipItem();
                    break;
            }

            return result;
        }
        return null;
    }
}


public abstract class Headgear : Equipable
{
    public Headgear()
    {
        status.category = Category.HEADGEAR;
    }
}

public abstract class Helmet : Equipable
{
    public Helmet()
    {
        status.category = Category.HELMET;
    }
}

public abstract class Plate : Equipable
{
    public Plate()
    {
        status.category = Category.PLATE;
    }
}

public abstract class Rig : Equipable
{
    Storage storageValue;
    public Storage storage { get => storageValue; set => storageValue = value; }
    public Rig()
    {
        status.category = Category.RIG;
    }

}

public abstract class Backpack : Equipable, HasStorage
{
    Storage storageValue;
    public Storage storage { get => storageValue; set => storageValue = value; }
    public Backpack()
    {
        status.category = Category.BACKPACK;
    }
}

public abstract class SecContainer : Equipable, HasStorage
{
    Storage storageValue;
    public Storage storage { get => storageValue; set => storageValue = value; }
    public SecContainer()
    {
        status.category = Category.S_CONTAINER;
    }
}

public abstract class WeaponItem : Equipable
{
    public WeaponItem() { }

    protected virtual void Initialize()
    {
        GD.PushWarning($"{weaponCode} Initialize");
    }

    protected string weaponCode;
    public virtual Weapon GetWeapon()
    {
        Weapon weapon = new Weapon(this, weaponCode);
        return weapon;
    }

    public WeaponStatus weaponStatus => WeaponLibrary.Get(weaponCode);
    public bool AbleSub() => status.size.X <= 3 && status.size.Y <= 2;
    public bool AbleMain() => true;

    public Magazine magazine = null;

    public Ammo chamber = null;
    public int ammoInMagazine => magazine == null ? 0 : magazine.ammoCount;

    public AmmoStatus? DetonateAmmo()
    {
        try
        {
            if (chamber == null) throw new Exception("DetonateAmmo - empty chamber!");
            if (chamber.ammoStatus.caliber != weaponStatus.typeDt.caliberType) throw new Exception("DetonateAmmo - ammo has invalid caliber!");
        }
        catch (Exception ex)
        {
            GD.PushWarning(ex);
            return null;
        }
        var res = chamber;
        chamber = null;
        return res.ammoStatus;

    }
    public bool FeedAmmo()
    {
        try
        {
            if (magazine == null) throw new Exception("FeedAmmo - no magazine!");
            //if (chamber != null) throw new Exception("FeedAmmo - already feeded!");
            if (magazine.AmmoPeek() == null) throw new Exception("FeedAmmo - empty magazine!");
        }
        catch (Exception ex) 
        {
            GD.PushWarning(ex);
            return false;
        }

        if (chamber != null)
        {
            //Eject처리
            chamber = null;
        }

        chamber = magazine.AmmoPop();
        return true;
    }

    public Magazine DetachMagazine()
    {
        if (magazine != null)
        {
            var result = magazine;
            magazine = null;
            return result;
        }
        else return null;
    }
    public bool AttachMagazine(Magazine magazine)
    {
        if (this.magazine == null)
        {
            this.magazine = magazine;
            return true;
        }
        else return false;

    }

    public (Storage storage, Storage.StorageNode node)? FindMagazine(Humanoid.Inventory inventory) 
    {
        List<Storage> storages = new();
        List<(Storage storage, Storage.StorageNode node)> foundList = new();

        storages.Add(inventory.pocket);

        if (inventory.sContainer.item != null)
            if (inventory.sContainer.item is HasStorage hasStorage)
                storages.Add(hasStorage.storage);

        if (inventory.backpack.item != null)
            if (inventory.backpack.item is HasStorage hasStoragee)
                storages.Add(hasStoragee.storage);

        if (inventory.rig.item != null)
            if (inventory.rig.item is HasStorage hasStorageee)
                storages.Add(hasStorageee.storage);

        //GD.PushError("storages.Count : " + storages.Count);

        foreach (var storage in storages)
            foreach (var node in storage.itemList)
                if (node.item is Magazine magazine)
                    if (weaponStatus.detailDt.magazineWhiteList.Contains(magazine.magazineCode))
                        foundList.Add((storage, node));
            
        //GD.PushError("foundList.Count : " + foundList.Count);

        int highestMag = 0;
        (Storage storage, Storage.StorageNode node)? tNode = null;

        foreach (var pair in foundList)
            if (pair.node.item is Magazine magazine)
                if (magazine.ammoCount > highestMag)
                {
                    highestMag = magazine.ammoCount;
                    tNode = pair;
                }

        //GD.PushError("tNode : " + tNode!=null + " - " + highestMag);
        return tNode.HasValue? tNode.Value : null;
    }

    public (Storage storage, Storage.StorageNode node)? FindAmmo(Humanoid.Inventory inventory) 
    {
        List<Storage> storages = new();
        List<(Storage storage, Storage.StorageNode node)> foundList = new();

        storages.Add(inventory.pocket);

        if (inventory.sContainer.item != null)
            if (inventory.sContainer.item is HasStorage hasStorage)
                storages.Add(hasStorage.storage);

        if (inventory.backpack.item != null)
            if (inventory.backpack.item is HasStorage hasStoragee)
                storages.Add(hasStoragee.storage);

        if (inventory.rig.item != null)
            if (inventory.rig.item is HasStorage hasStorageee)
                storages.Add(hasStorageee.storage);

        //GD.PushError("storages.Count : " + storages.Count);

        foreach (var storage in storages)
            foreach (var node in storage.itemList)
                if (node.item is Ammo ammo)
                    if (weaponStatus.typeDt.caliberType == ammo.ammoStatus.caliber)
                        //foundList.Add((storage, node));
                        return (storage, node);
        return null;

        //GD.PushError("foundList.Count : " + foundList.Count);

        //int highestMag = 0;
        //(Storage storage, Storage.StorageNode node)? tNode = null;

        //foreach (var pair in foundList)
        //    if (pair.node.item is Magazine magazine)
        //        if (magazine.ammoCount > highestMag)
        //        {
        //            highestMag = magazine.ammoCount;
        //            tNode = pair;
        //        }

        //GD.PushError("tNode : " + tNode!=null + " - " + highestMag);
        //return tNode.HasValue ? tNode.Value : null;

    }
}


public interface HasStorage
{
    public Storage storage { get; set; }
}