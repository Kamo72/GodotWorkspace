
using Godot;
using System;


#region [Weapon]

public class K2 : WeaponItem
{
    static K2()
    {
        WeaponLibrary.Set("K2", new(
            new(
                MechanismType.CLOSED_BOLT,
                MagazineType.MAGAZINE,
                BoltLockerType.ACTIVATE,
                new() { SelectorType.AUTO, SelectorType.BURST3, SelectorType.SEMI },
                CaliberType.mm5p56x45
                ),
            new()
            {
                moa = 2.5f,
                recovery = 1f,
                stance = 1f,
                strength = 1f,
                traggingSpeed = 1f,
            }, new()
            {
                adsTime = 0.65f,
                sprintTime = 0.45f,
                swapTime = 0.45f,

                inspectTime = (0.25f, 0.18f),
                reloadTime = (0.55f, 0.55f, 0.55f),
                boltTime = (0.25f, 0.45f, 0.70f),

            }, new()
            {
                speed = 0.85f,
                speedAdjust = (0.85f, 0.57f, 0.94f),
            }, new()
            {
                roundPerMinute = 750,
                chamberSize = 1,
                effectiveRange = 500f,
                loudness = 3000,
                magazineWhiteList = new(){
                    "STANAG_AR15_30"
                },
                muzzleVelocity = 10000f,
                muzzleDistance = 50f,
            }, new()
            {

            }
            ));
    }
    public K2() : base()
    {
        weaponCode = "K2";
        status = new Status()
        {
            name = "대우정밀 K2 돌격소총",
            shortName = "K2",
            description = "대우 정밀에서 개발 및 생산된 대한민국의 제식 돌격소총입니다. AK47에 사용되던 롱 스트로크 가스 피스톤 방식을 사용해 높은 신뢰성을 보장하고, 구조가 간단해 누구든 쉽게 다룰 수 있도록 설계되어 있습니다.",
            textureRoot = "res://Asset/guns/AKS-74U.png",
            size = new Vector2I(4, 2),

            rarerity = Rarerity.COMMON,
            category = Category.WEAPON,
            value = 48500,
            mass = 3.37f,
        };
        Initialize();
    }
}

public class K2C1 : WeaponItem
{
    static K2C1()
    {
        WeaponLibrary.Set("K2C1", new(
        new(
            MechanismType.CLOSED_BOLT,
            MagazineType.MAGAZINE,
            BoltLockerType.ACTIVATE,
            new() { SelectorType.AUTO, SelectorType.BURST3, SelectorType.SEMI },
            CaliberType.mm5p56x45
            ),
        new()
        {
            moa = 2.5f,
            recovery = 1f,
            stance = 1f,
            strength = 1f,
            traggingSpeed = 1f,
        }, new()
        {
            adsTime = 0.65f,
            sprintTime = 0.45f,
            swapTime = 0.45f,

            inspectTime = (0.25f, 0.18f),
            reloadTime = (0.55f, 0.55f, 0.55f),
            boltTime = (0.25f, 0.45f, 0.70f),

        }, new()
        {
            speed = 0.85f,
            speedAdjust = (0.85f, 0.57f, 0.94f),
        }, new()
        {
            roundPerMinute = 750,
            chamberSize = 1,
            effectiveRange = 500f,
            loudness = 3000,
            magazineWhiteList = new(){
                "STANAG_AR15_30"
            },
            muzzleVelocity = 10000f,
            muzzleDistance = 50f,
        }, new()
        {

        }
        ));
    }
    public K2C1() : base()
    {
        weaponCode = "K2C1";
        status = new Status()
        {
            name = "대우정밀 K2C1 돌격소총",
            shortName = "K2C1",
            description = "대우 정밀에서 개발 및 생산된 대한민국의 제식 돌격소총입니다. AK47에 사용되던 롱 스트로크 가스 피스톤 방식을 사용해 높은 신뢰성을 보장하고, 구조가 간단해 누구든 쉽게 다룰 수 있도록 설계되어 있습니다.",
            textureRoot = "res://Asset/guns/AKS-74U.png",
            size = new Vector2I(4, 2),

            rarerity = Rarerity.COMMON,
            category = Category.WEAPON,
            value = 61500,
            mass = 3.20f,
        };
        Initialize();
    }
}

public class AKS_74U : WeaponItem
{
    static AKS_74U()
    {

        WeaponLibrary.Set("AKS_74U", new(
        new(
            MechanismType.CLOSED_BOLT,
            MagazineType.MAGAZINE,
            BoltLockerType.ACTIVATE,
            new() { SelectorType.AUTO, SelectorType.BURST3, SelectorType.SEMI },
            CaliberType.mm5p45x39
            ),
        new()
        {
            moa = 2.5f,
            recovery = 1f,
            stance = 1f,
            strength = 1f,
            traggingSpeed = 1f,
        }, new()
        {
            adsTime = 0.65f,
            sprintTime = 0.45f,
            swapTime = 0.45f,

            inspectTime = (0.25f, 0.18f),
            reloadTime = (0.55f, 0.55f, 0.55f),
            boltTime = (0.25f, 0.45f, 0.70f),

        }, new()
        {
            speed = 0.91f,
            speedAdjust = (0.85f, 0.75f, 0.98f),
        }, new()
        {
            roundPerMinute = 690,
            chamberSize = 1,
            effectiveRange = 375f,
            loudness = 3500f,
            magazineWhiteList = new(){
                "AK74_MOLOT_30"
            },
            muzzleVelocity = 7800f,
            muzzleDistance = 50f,
        }, new()
        {

        }
        ));
    }
    public AKS_74U() : base()
    {
        weaponCode = "AKS_74U";
        status = new Status()
        {
            name = "칼라시니코프 AKS-74U 카빈소총",
            shortName = "AKS-74U",
            description = "",
            textureRoot = "res://Asset/guns/AKS-74U.png",
            size = new Vector2I(3, 2),

            rarerity = Rarerity.COMMON,
            category = Category.WEAPON,
            value = 34000,
            mass = 2.7f,
        };
        Initialize();
    }
}

public class M4A1 : WeaponItem
{
    static M4A1()
    {
        WeaponLibrary.Set("M4A1", new(
        new(
            MechanismType.CLOSED_BOLT,
            MagazineType.MAGAZINE,
            BoltLockerType.ACTIVATE,
            new() { SelectorType.AUTO, SelectorType.BURST3, SelectorType.SEMI },
            CaliberType.mm5p56x45
            ),
        new()
        {
            moa = 2.5f,
            recovery = 1f,
            stance = 1f,
            strength = 1f,
            traggingSpeed = 1f,
        }, new()
        {
            adsTime = 0.65f,
            sprintTime = 0.45f,
            swapTime = 0.45f,

            inspectTime = (0.25f, 0.18f),
            reloadTime = (0.55f, 0.55f, 0.55f),
            boltTime = (0.25f, 0.45f, 0.70f),

        }, new()
        {
            speed = 0.91f,
            speedAdjust = (0.85f, 0.75f, 0.98f),
        }, new()
        {
            roundPerMinute = 825,
            chamberSize = 1,
            effectiveRange = 375f,
            loudness = 3500f,
            magazineWhiteList = new(){
                "STANAG_AR15_30"
            },
            muzzleVelocity = 7800f,
            muzzleDistance = 50f,
        }, new()
        {

        }
        ));
       
    }
    public M4A1() : base()
    {
        weaponCode = "M4A1";
        status = new Status()
        {
            name = "Colt AR-15 M4A1 돌격소총",
            shortName = "M4A1",
            description = "",
            textureRoot = "res://Asset/guns/M4A1.png",
            size = new Vector2I(4, 2),

            rarerity = Rarerity.COMMON,
            category = Category.WEAPON,
            value = 79000,
            mass = 2.92f,
        };
        Initialize();
        magazine = new AR15_StanagMag_30(typeof(M855));
    }
}
#endregion


#region [Mag]

public class AR15_StanagMag_30 : Magazine
{
    static AR15_StanagMag_30() 
    {
        MagazineLibrary.Set("STANAG_AR15_30", new(){
            adjusts = new() { },
            whiteList = new()
            {
                CaliberType.mm5p56x45,
                CaliberType.p300,
            },
            ammoSize = 30,
        });
    }

    public AR15_StanagMag_30() : this(null) { }
    public AR15_StanagMag_30(Type ammo) : base("STANAG_AR15_30", ammo)
    {
        status = new()
        {
            name = "STANAG 30발 탄창",
            shortName = "STAN30",
            description = "",
            textureRoot = "res://Asset/guns/AR15_STANAG30.png",
            size = new Vector2I(1, 2),

            rarerity = Rarerity.COMMON,
            category = Category.MAGAZINE,
            value = 2200,
            mass = 0.13f,
        };
    }

}

#endregion

#region [Ammo]

public class M855 : Ammo
{
    static M855() 
    {
        AmmoLibrary.Set("5.56 M855", new()
        {
            adjustment = new()
            {
                accuracyRatio = 1,
                recoilRatio = 1,
                speedRatio = 1,
            },
            caliber = CaliberType.mm5p56x45,
            lethality = new()
            {
                damage = 35,
                bleeding = 21,
                suppress = 62f,
                pellitCount = 1,
                pierceLevel = 3.2f,
            },
            tracer = new()
            {
                isTraced = false,
                color = Colors.Orange,
                radius = 100,
            }
        });
    }
    public M855() : base("5.56 M855")
    {
        stackMax = 100;
        status = new ()
        {
            name = "5.56x45mm M855 보통탄",
            shortName = "M855",
            description = "",
            textureRoot = "res://Asset/guns/ImageAR.png",
            size = new Vector2I(1, 1),

            rarerity = Rarerity.COMMON,
            category = Category.AMMUNITION,
            value = 100,
            mass = 0.012f,
        };
    }
}
public class M855A1 : Ammo
{
    static M855A1()
    {
        AmmoLibrary.Set("5.56 M855A1", new()
        {
            adjustment = new()
            {
                accuracyRatio = 1.1f,
                recoilRatio = 1.1f,
                speedRatio = 1.1f,
            },
            caliber = CaliberType.mm5p56x45,
            lethality = new()
            {
                damage = 32,
                bleeding = 15,
                suppress = 55f,
                pellitCount = 1,
                pierceLevel = 4.6f,
            },
            tracer = new()
            {
                isTraced = false,
                color = Colors.Orange,
                radius = 100,
            }
        });
    }
    public M855A1() : base("5.56 M855A1")
    {
        stackMax = 100;
        status = new()
        {
            name = "5.56x45mm M855A1 철갑탄",
            shortName = "M855A1",
            description = "",
            textureRoot = "res://Asset/guns/ImageAR.png",
            size = new Vector2I(1, 1),

            rarerity = Rarerity.COMMON,
            category = Category.AMMUNITION,
            value = 250,
            mass = 0.012f,
        };
    }
}

#endregion
