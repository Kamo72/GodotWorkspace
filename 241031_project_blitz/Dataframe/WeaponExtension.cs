using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public partial class Weapon : Node2D
{
    public enum Code {
        K2,
    }

    static Dictionary<Code, WeaponStatus> weaponLibrary = new Dictionary<Code, WeaponStatus>
    {
        { Code.K2, new WeaponStatus(1100, 15, 40000, 20, 2f) },
    };

    public static WeaponStatus GetStatByCode(Code code)
    {
        WeaponStatus weaponStatus;

        if (weaponLibrary.TryGetValue(code, out weaponStatus)) 
            return weaponStatus;

        return weaponLibrary[Code.K2];
    }

}

