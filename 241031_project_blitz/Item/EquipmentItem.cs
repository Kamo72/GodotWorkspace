using Godot;
using System;
using System.Collections.Generic;
using static Humanoid;
public class Equipable : Item
{
    public Humanoid equipedBy { get; set; }
    public Humanoid.Inventory inventory { get; set; }
    public bool isEquiping => inventory != null || equipedBy != null;
    public bool isBody => inventory != null && equipedBy == null;

    //장착 및 장착 해제
    public void BeEquip(Humanoid entity)
    {
        onStorage?.RemoveItem(this);
        
        equipedBy = entity;
        inventory = entity.inventory;
    }

    public void BeEquip(Inventory inventory)
    {
        onStorage?.RemoveItem(this);

        this.inventory = inventory;
    }

    public Item UnEquip()
    {
        //GD.PushWarning("UnEquip Called!");
        if (!isEquiping) throw new Exception("Equipable - UnEquip - ERROR : 장착하지 않은 아이템을 장착 해제하려고 합니다!");

        Item result = null;
        Inventory tInventory = inventory;
        equipedBy = null;
        inventory = null;

        switch (status.category)
        {
            case Category.HEADGEAR:
                result = tInventory.headgear.UnEquipItem();
                break;
            case Category.HELMET:
                result = tInventory.helmet.UnEquipItem();
                break;
            case Category.PLATE:
                result = tInventory.plate.UnEquipItem();
                break;
            case Category.RIG:
                result = tInventory.rig.UnEquipItem();
                break;
            case Category.BACKPACK:
                result = tInventory.backpack.UnEquipItem();
                break;
            case Category.S_CONTAINER:
                result = tInventory.sContainer.UnEquipItem();
                break;
            case Category.WEAPON:
                if (tInventory.firstWeapon.item == this)
                    result = tInventory.firstWeapon.UnEquipItem();
                else if (tInventory.secondWeapon.item == this)
                    result = tInventory.secondWeapon.UnEquipItem();
                else
                    result = tInventory.subWeapon.UnEquipItem();
                break;
        }

        return result;
    }
}


public abstract class Headgear : Equipable
{
    public Headgear()
    {
        status.category = Category.HEADGEAR;
    }
}

public abstract class Helmet : Equipable, IArmour
{
    public Helmet(float armourLv, float ergoRatio, float durable)
    {
        this.armourLv = armourLv;
        this.ergoRatio = ergoRatio;
        this.durableMax = durable;
        this.durableNow = durableMax;

        zeroToDestruct = true;
        status.category = Category.HELMET;
    }

    public float armourLv { get; set; }
    public float ergoRatio { get; set; }
    public float durableNow { get; set; }
    public float durableMax { get; set; }
    public bool zeroToDestruct { get; set; }
}

public abstract class Plate : Equipable, IArmour
{
    public Plate(float armourLv, float ergoRatio, float durable)
    {
        this.armourLv = armourLv;
        this.ergoRatio = ergoRatio;
        this.durableMax = durable;
        this.durableNow = durableMax;

        zeroToDestruct = true;
        status.category = Category.PLATE;
    }

    public float armourLv { get; set; }
    public float ergoRatio { get; set; }
    public float durableNow { get; set; }
    public float durableMax { get; set; }
    public bool zeroToDestruct { get; set; }
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

public abstract class WeaponItem : Equipable, IUsable
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

public interface IArmour : IDurable 
{
    public float armourLv {  get; set; }
    public float ergoRatio { get; set; }
}