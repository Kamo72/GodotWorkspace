using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#region [Weapon]

public class MP_133 : WeaponItem
{
    static MP_133()
    {
        WeaponLibrary.Set("MP_133", new(
        new(
            MechanismType.CLOSED_BOLT,
            MagazineType.TUBE,
            BoltLockerType.ACTIVATE,
            new() { SelectorType.SEMI },
            CaliberType.g12
            ),
        new()
        {
            moa = 8.5f,
            recovery = 1f,
            stance = 1f,
            strength = 1f,
            traggingSpeed = 1f,
        }, new()
        {
            adsTime = 0.80f,
            sprintTime = 0.60f,
            swapTime = 0.55f,

            inspectTime = (0.42f, 0.25f),
            reloadTime = (0.20f, 1.45f, 0.65f),
            boltTime = (0.30f, 0.60f, 0.75f),

        }, new()
        {
            speed = 0.84f,
            speedAdjust = (0.77f, 0.66f, 0.77f),
        }, new()
        {
            roundPerMinute = 100,
            chamberSize = 1,
            effectiveRange = 350f,
            loudness = 4400f,
            magazineWhiteList = new(){
                "MP_133_8"
            },
            muzzleVelocity = 4400f,
            muzzleDistance = 60f,
        }, new()
        {

        }
        ));
    }
    public MP_133() : base()
    {
        weaponCode = "MP_133";
        status = new Status()
        {
            name = "MP-133 산탄총",
            shortName = "MP-133",
            description = "",
            textureRoot = "res://Asset/guns/MP-155.png",
            size = new Vector2I(4, 1),

            rarerity = Rarerity.COMMON,
            category = Category.WEAPON,
            value = 35000,
            mass = 3.40f,
        };
        Initialize();
        magazine = new MP_133_TubeMag_8();
    }
}

public class MP_155 : WeaponItem
{
    static MP_155()
    {
        WeaponLibrary.Set("MP_155", new(
        new(
            MechanismType.CLOSED_BOLT,
            MagazineType.TUBE,
            BoltLockerType.ACTIVATE,
            new() { SelectorType.SEMI },
            CaliberType.g12
            ),
        new()
        {
            moa = 8.50f,
            recovery = 0.045f,
            stance = 70f,
            strength = 200f,
            traggingSpeed = 0.12f,
        }, new()
        {
            adsTime = 0.80f,
            sprintTime = 0.60f,
            swapTime = 0.55f,

            inspectTime = (0.42f, 0.25f),
            reloadTime = (0.20f, 1.45f, 0.65f),
            boltTime = (0.30f, 0.60f, 0.75f),

        }, new()
        {
            speed = 0.84f,
            speedAdjust = (0.77f, 0.66f, 0.77f),
        }, new()
        {
            roundPerMinute = 450,
            chamberSize = 1,
            effectiveRange = 350f,
            loudness = 4400f,
            magazineWhiteList = new(){
                "MP_133_8"
            },
            muzzleVelocity = 4400f,
            muzzleDistance = 60f,
        }, new()
        {

        }
        ));
    }
    public MP_155() : base()
    {
        weaponCode = "MP_155";
        status = new Status()
        {
            name = "MP-155 산탄총",
            shortName = "MP-155",
            description = "",
            textureRoot = "res://Asset/guns/MP-155.png",
            size = new Vector2I(4, 1),

            rarerity = Rarerity.COMMON,
            category = Category.WEAPON,
            value = 35000,
            mass = 3.40f,
        };
        Initialize();
        magazine = new MP_133_TubeMag_8();
    }
}


#endregion

#region [Magazine]

public class MP_133_TubeMag_8 : Magazine
{
    static MP_133_TubeMag_8()
    {
        MagazineLibrary.Set("MP_133_8", new()
        {
            adjusts = new() { },
            whiteList = new()
            {
                CaliberType.g12,
            },
            ammoSize = 8,
        });
    }

    public MP_133_TubeMag_8() : this(null) { }
    public MP_133_TubeMag_8(Type ammo) : base("MP_133_8", ammo)
    {
        status = new()
        {
            name = "MP155 8발 들이 관형탄창",
            shortName = "MP155 8",
            description = "",
            textureRoot = "res://Asset/guns/AR15_STANAG30.png",
            size = new Vector2I(2, 1),

            rarerity = Rarerity.COMMON,
            category = Category.MAGAZINE,
            value = 2200,
            mass = 0.13f,
        };
    }
}


#endregion

#region [Ammo]

public class G12_BuckShot_7p5 : Ammo
{
    static G12_BuckShot_7p5()
    {
        AmmoLibrary.Set("G12_BUCK_7p5", new()
        {
            adjustment = new()
            {
                accuracyRatio = 3.00f,
                recoilRatio = 1.10f,
                speedRatio = 0.45f,
            },
            caliber = CaliberType.g12,
            lethality = new()
            {
                damage = 12,
                bleeding = 9,
                suppress = 45f,
                pellitCount = 11,
                pierceLevel = 1.5f,
            },
            tracer = new()
            {
                isTraced = false,
                color = Colors.Orange,
                radius = 100,
            }
        });
    }
    public G12_BuckShot_7p5() : base("G12_BUCK_7p5")
    {
        stackMax = 40;
        status = new()
        {
            name = "12게이지 7.5mm 산탄쉘 ",
            shortName = "Buck7.5",
            description = "",
            textureRoot = "res://Asset/guns/ImageAR.png",
            size = new Vector2I(1, 1),

            rarerity = Rarerity.COMMON,
            category = Category.AMMUNITION,
            value = 60,
            mass = 0.018f,
        };
    }
}
public class G12_Grizzly : Ammo
{
    static G12_Grizzly()
    {
        AmmoLibrary.Set("G12_GRIZZLY", new()
        {
            adjustment = new()
            {
                accuracyRatio = 1.00f,
                recoilRatio = 1.22f,
                speedRatio = 1.0f,
            },
            caliber = CaliberType.g12,
            lethality = new()
            {
                damage = 120,
                bleeding = 56,
                suppress = 800f,
                pellitCount = 1,
                pierceLevel = 2.6f,
            },
            tracer = new()
            {
                isTraced = false,
                color = Colors.Orange,
                radius = 100,
            }
        });
    }
    public G12_Grizzly() : base("G12_GRIZZLY")
    {
        stackMax = 40;
        status = new()
        {
            name = "12게이지 그리즐리 슬러그쉘 ",
            shortName = "Grizzly",
            description = "",
            textureRoot = "res://Asset/guns/ImageAR.png",
            size = new Vector2I(1, 1),

            rarerity = Rarerity.COMMON,
            category = Category.AMMUNITION,
            value = 120,
            mass = 0.022f,
        };
    }
}


#endregion