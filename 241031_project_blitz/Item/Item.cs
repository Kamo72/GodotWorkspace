using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Godot;
using static Godot.OpenXRHand;

public abstract class Item
{
    public Item()
    {
        status = new Status()
        {
            textureRoot = "res://icon.svg",
        };
    }

    public string prefab;
    public Storage onStorage = null;
    public Status status;

    public struct Status
    {
        public string name;
        public string shortName;
        public string description;
        public float mass;
        public float value;
        public Vector2I size;
        public Rarerity rarerity;
        public Category category;
        public string textureRoot;
    }

    public bool Store(Storage storage)
    {
        if (storage.IsAbleToInsert(this))
        {
            Storage.StorageNode storenode = (Storage.StorageNode)storage.GetPosInsert(this);
            return storage.Insert(storenode);
        }
        return false;
    }

    public bool Store(Storage storage, Vector2I pos, bool isRotated)
    {
        if (storage.IsAbleToInsert(this))
        {
            Storage.StorageNode storenode = new Storage.StorageNode() { item = this, pos = pos, isRotated = isRotated };
            return storage.Insert(storenode);
        }
        return false;
    }

    public DroppedItem GetDroppedItem(Vector2 pos)
    {
        if (droppedItem != null)
        {
            droppedItem.GlobalPosition = pos;
            return droppedItem;
        }

        droppedItem = ResourceLoader.Load<PackedScene>("res://Prefab/Dynamic/droppedItem.tscn").Instantiate() as DroppedItem;
        droppedItem.GlobalPosition = pos;
        droppedItem.SetItem(this);
        WorldManager wm = WorldManager.instance;
        wm.AddChild(droppedItem);

        return droppedItem;
    }

    public DroppedItem droppedItem = null;

    public enum Rarerity
    {
        COMMON,
        UNCOMMON,
        RARE,
        UNIQUE,
        QUEST,
    }

    public enum Category
    {
        WEAPON,
        HEADGEAR,
        HELMET,
        PLATE,
        RIG,
        BACKPACK,
        S_CONTAINER,
        MAGAZINE,
        AMMUNITION,
        QUEST,
        ETC,
    }

}

public static class ItemEx
{

    public static Item Split(this Item stackable, int getCount)
    {
        if (stackable is IStackable iStackable)
        {
            if (iStackable.stackNow <= getCount) throw new Exception("wrong getCount value");

            Item newItem = Activator.CreateInstance(stackable.GetType()) as Item;

            if (newItem is IStackable newItemStackable)
            {
                newItemStackable.stackNow = getCount;
                iStackable.stackNow -= getCount;

                return newItem;
            }
        }
        return null;
    }

    public static void StackFrom(this Item stackable, Item stackableResource)
    {
        if (stackable.status.name != stackableResource.status.name) return;
        if (stackable == stackableResource) return;
        if (stackable is IStackable iStackable && stackableResource is IStackable iStackableR)
        {
            int emptyCount = iStackable.stackMax - iStackable.stackNow;
            if (emptyCount <= 0) return;
            int remainCount = iStackableR.stackNow - emptyCount;

            //남는게 없음    
            if (remainCount <= 0)
            {
                iStackable.stackNow = iStackable.stackMax + remainCount;
                GD.PushWarning($"{iStackable.stackNow} = {iStackable.stackMax} - {remainCount}");

                stackableResource.onStorage?.RemoveItem(stackableResource);
            }
            //남는게 이씀
            else
            {
                iStackable.stackNow = iStackable.stackMax;
                iStackableR.stackNow = remainCount;
            }

        }
    }

}


public interface IStackable
{
    int stackNow { get; set; }
    int stackMax { get; set; }

}

public interface IDurable
{
    float durableNow { get; set; }
    float durableMax { get; set; }
    bool zeroToDestruct { get; set; }
}

public interface IHandable
{
    float equipTime { get; }
    float equipValue { get; }

    Dictionary<string, Action<Hands, bool>> commandsReact { get; set; }
}

public interface IClickable { }

