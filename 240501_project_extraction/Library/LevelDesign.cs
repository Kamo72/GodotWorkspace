

using System;
using System.Collections.Generic;
using Godot;

public class LevelDesign {

    static LevelDesign()
    {
        WeaponLibrary = new Dictionary<string, string>
        {
            { "weapon", "res://Prefab/weapon.tscn" }
        };

    }

    public static Dictionary<string, string> WeaponLibrary;


    public static Weapon CreateWeapon(string name)
    {
        if(WeaponLibrary.ContainsKey(name) == false) return null;

        string root = WeaponLibrary[name];
        GD.Print("name : " + name);
        GD.Print("root : " + root);
        Weapon node = ResourceLoader.Load<PackedScene>(root).Instantiate() as Weapon;
        return node;
    }
    
}