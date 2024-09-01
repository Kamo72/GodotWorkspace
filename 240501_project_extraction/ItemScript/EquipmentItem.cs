using Godot;
using System;





public class Equipable : Item
{
    public Humanoid equipedBy { get; set; }

    //장착 및 장착 해제
    public void BeEquip(Humanoid entity)
    {
        if (onStorage != null)
        {
            onStorage.RemoveItem(this);
        }

        equipedBy = entity;
    }
    public void UnEquip(Humanoid entity)
    {
        if (equipedBy == null) throw new Exception("Equipable - UnEquip - ERROR : 장착하지 않은 아이템을 장착 해제하려고 합니다!");
        if (equipedBy is Humanoid human)
        {

        }
        equipedBy = null;
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
    public Storage storage;
    public Rig()
    {
        status.category = Category.RIG;
    }
}

public abstract class Backpack : Equipable
{
    public Storage storage;
    public Backpack()
    {
        status.category = Category.BACKPACK;
    }
}

public abstract class SecContainer : Equipable
{
    public Storage storage;
    public SecContainer()
    {
        status.category = Category.S_CONTAINER;
    }
}

public abstract class WeaponItem : Equipable
{
    public WeaponItem()
    {
        status.category = Category.WEAPON;
    }

    protected string prefabRoot = "weapon"; 
    public virtual Weapon GetWeapon() 
    {
        Weapon weapon = LevelDesign.CreateWeapon(prefabRoot);
        weapon.weaponStatus = weaponStatus; 
        Sprite2D spr2D = weapon.FindByName("SprMain") as Sprite2D;
        spr2D.Texture = ResourceLoader.Load<Texture2D>(status.textureRoot);
        return weapon;
    }

    public virtual WeaponStatus weaponStatus => new WeaponStatus();
    public bool AbleSub() => true;
    public bool AbleMain() => true;
}

