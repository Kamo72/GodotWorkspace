
using System;
using System.Collections.Generic;
using Godot;

public class Item 
{   
    public string prefab;
    
    public Storage onStorage = null;

    public Status status;
    public struct Status{
        public string name;
        public string shortName;
        public string description;
        public float mass;
        public float value;
        public Vector2I size;
        public Rarerity rarerity;
        public Category category;
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

    public enum Rarerity{
        COMMON,
        UNCOMMON,
        RARE,
        UNIQUE,
        QUEST,
    }

    public enum Category{
        WEAPON,
        QUEST,
        ETC,
    }
}


    #region [아이템 인터페이스]
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

    #endregion
