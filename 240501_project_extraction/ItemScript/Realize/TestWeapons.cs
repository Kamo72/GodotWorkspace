

using System.Collections.Generic;
using Godot;

public class TestDMR : WeaponItem
{
    public TestDMR()
    {
        //prefabRoot = "TestDMR";
        status.textureRoot = "res://AssetSprite/Weapon/ImageDMR.png";
    }
    public override WeaponStatus weaponStatus => new WeaponStatus()
    {
        detailDt = new WeaponStatus.DetailData(){
            basicDamage = 190f,
            muzzleSpeed = 130f,
            roundPerMinute = 250,
            basicMag = 10,
        },
        timeDt = new WeaponStatus.TimeData(){
            swapTime = 0.860f,
            reloadTime = (0f,0f,1.520f),
        },
        typeDt = new WeaponStatus.TypeData(){
            mechanismType = MechanismType.CLOSED_BOLT,
            boltLockerType = BoltLockerType.ACTIVATE,
            magazineType = MagazineType.MAGAZINE,
            selectorList = new List<SelectorType>{SelectorType.SEMI},
            caliberType = CaliberType.mm7p62x51,
        },
        aimDt = new WeaponStatus.AimData(){
            ads = new WeaponStatus.AimData.AdsData(){
                adsName = "",
                moa = 0.3f,
                recoil = new WeaponStatus.AimData.AdsData.AdsRecoilData(){
                    fix = new Vector2(0.6f, 4.2f),
                    random = new Vector2(0.2f, 0.3f),
                    recovery = 1.0f,
                    strengthAdjust = (0.75f, 1.35f),
                },
                stance = new WeaponStatus.AimData.AdsData.AdsStancelData(){
                    accuracy = 1f,
                    accuracyAdjust = (0.75f, 1.35f),
                },
            },
            hip = new WeaponStatus.AimData.HipData(){
                traggingSpeed = 5.0f,
                recoil = new WeaponStatus.AimData.HipData.HipRecoilData(){
                    recovery = 3.0f,
                    recoveryAdjust = (0.75f, 1.35f),
                    strength = 5.0f,
                },
                stance = new WeaponStatus.AimData.HipData.HipStancelData(){
                    recovery = 5.0f,
                    accuracy = 43.0f,
                    accuracyAdjust = (0.5f, 1.7f),
                },
            },
        },
        moveDt = new WeaponStatus.MovementData(){
            speed = 0.85f,
            speedAdjust = (0.75f, 0.64f, 1.70f)
        },
        attachDt = new WeaponStatus.AttachData(){},
    };

}

public class TestAR : WeaponItem
{
    public TestAR()
    {
        //prefabRoot = "TestAR";
        status.textureRoot = "res://AssetSprite/Weapon/ImageAR.png";
    }
    public override WeaponStatus weaponStatus => new WeaponStatus()
    {
        detailDt = new WeaponStatus.DetailData(){
            basicDamage = 70f,
            muzzleSpeed = 90f,
            roundPerMinute = 650,
            basicMag = 30,
        },
        timeDt = new WeaponStatus.TimeData(){
            swapTime = 0.720f,
            reloadTime = (0f,0f,2.150f),
        },
        typeDt = new WeaponStatus.TypeData(){
            mechanismType = MechanismType.CLOSED_BOLT,
            boltLockerType = BoltLockerType.ACTIVATE,
            magazineType = MagazineType.MAGAZINE,
            selectorList = new List<SelectorType>{SelectorType.AUTO},
            caliberType = CaliberType.mm5p56x45,
        },
        aimDt = new WeaponStatus.AimData(){
            ads = new WeaponStatus.AimData.AdsData(){
                adsName = "",
                moa = 1.1f,
                recoil = new WeaponStatus.AimData.AdsData.AdsRecoilData(){
                    fix = new Vector2(0.4f, 1.7f),
                    random = new Vector2(0.4f, 0.4f),
                    recovery = 2.0f,
                    strengthAdjust = (0.75f, 1.35f),
                },
                stance = new WeaponStatus.AimData.AdsData.AdsStancelData(){
                    accuracy = 50.0f,
                    accuracyAdjust = (0.75f, 1.35f),
                },
            },
            hip = new WeaponStatus.AimData.HipData(){
                traggingSpeed = 7.5f,
                recoil = new WeaponStatus.AimData.HipData.HipRecoilData(){
                    recovery = 5.0f,
                    recoveryAdjust = (0.75f, 1.35f),
                    strength = 2.2f,
                },
                stance = new WeaponStatus.AimData.HipData.HipStancelData(){
                    recovery = 8.0f,
                    accuracy = 25.0f,
                    accuracyAdjust = (0.5f, 1.7f),
                },
            },
        },
        moveDt = new WeaponStatus.MovementData(){
            speed = 0.89f,
            speedAdjust = (0.80f, 0.75f, 1.75f)
        },
        attachDt = new WeaponStatus.AttachData(){},
    };
}

public class TestSMG : WeaponItem
{
    public TestSMG()
    {
        //prefabRoot = "TestSMG";
        status.textureRoot = "res://AssetSprite/Weapon/ImageSMG.png";
    }
    public override WeaponStatus weaponStatus => new WeaponStatus()
    {
        detailDt = new WeaponStatus.DetailData(){
            basicDamage = 60f,
            muzzleSpeed = 70f,
            roundPerMinute = 800,
            basicMag = 30,
        },
        timeDt = new WeaponStatus.TimeData(){
            swapTime = 0.460f,
            reloadTime = (0f,0f, 1.720f),
        },
        typeDt = new WeaponStatus.TypeData(){
            mechanismType = MechanismType.CLOSED_BOLT,
            boltLockerType = BoltLockerType.ACTIVATE,
            magazineType = MagazineType.MAGAZINE,
            selectorList = new List<SelectorType>{SelectorType.AUTO},
            caliberType = CaliberType.mm9x18,
        },
        aimDt = new WeaponStatus.AimData(){
            ads = new WeaponStatus.AimData.AdsData(){
                adsName = "",
                moa = 2.5f,
                recoil = new WeaponStatus.AimData.AdsData.AdsRecoilData(){
                    fix = new Vector2(0.6f, 1.2f),
                    random = new Vector2(0.7f, 0.7f),
                    recovery = 3.0f,
                    strengthAdjust = (0.75f, 1.35f),
                },
                stance = new WeaponStatus.AimData.AdsData.AdsStancelData(){
                    accuracy = 65.0f,
                    accuracyAdjust = (0.75f, 1.35f),
                },
            },
            hip = new WeaponStatus.AimData.HipData(){
                traggingSpeed = 10.0f,
                recoil = new WeaponStatus.AimData.HipData.HipRecoilData(){
                    recovery = 7.0f,
                    recoveryAdjust = (0.75f, 1.35f),
                    strength = 2.2f,
                },
                stance = new WeaponStatus.AimData.HipData.HipStancelData(){
                    recovery = 12.0f,
                    accuracy = 32.0f,
                    accuracyAdjust = (0.5f, 1.7f),
                },
            },
        },
        moveDt = new WeaponStatus.MovementData(){
            speed = 0.93f,
            speedAdjust = (0.83f, 0.79f, 1.80f)
        },
        attachDt = new WeaponStatus.AttachData(){},
    };
}

public class TestHG : WeaponItem
{
    public TestHG()
    {
        //prefabRoot = "TestHG";
        status.textureRoot = "res://AssetSprite/Weapon/ImageHG.png";
    }
    public override WeaponStatus weaponStatus => new WeaponStatus()
    {
        detailDt = new WeaponStatus.DetailData(){
            basicDamage = 80f,
            muzzleSpeed = 60f,
            roundPerMinute = 450,
            pellitAmount = 12,
            basicMag = 18,
        },
        timeDt = new WeaponStatus.TimeData(){
            swapTime = 0.320f,
            reloadTime = (0f,0f,1.0f),
        },
        typeDt = new WeaponStatus.TypeData(){
            mechanismType = MechanismType.CLOSED_BOLT,
            boltLockerType = BoltLockerType.ACTIVATE,
            magazineType = MagazineType.MAGAZINE,
            selectorList = new List<SelectorType>{SelectorType.SEMI},
            caliberType = CaliberType.mm9x18,
        },
        aimDt = new WeaponStatus.AimData(){
            ads = new WeaponStatus.AimData.AdsData(){
                adsName = "",
                moa = 2.2f,
                recoil = new WeaponStatus.AimData.AdsData.AdsRecoilData(){
                    fix = new Vector2(2.0f, 9.0f),
                    random = new Vector2(2.8f, 2.8f),
                    recovery = 1.2f,
                    strengthAdjust = (0.75f, 1.35f),
                },
                stance = new WeaponStatus.AimData.AdsData.AdsStancelData(){
                    accuracy = 65.0f,
                    accuracyAdjust = (0.75f, 1.35f),
                },
            },
            hip = new WeaponStatus.AimData.HipData(){
                traggingSpeed = 10.0f,
                recoil = new WeaponStatus.AimData.HipData.HipRecoilData(){
                    recovery = 7.0f,
                    recoveryAdjust = (0.75f, 1.35f),
                    strength = 2.2f,
                },
                stance = new WeaponStatus.AimData.HipData.HipStancelData(){
                    recovery = 12.0f,
                    accuracy = 32.0f,
                    accuracyAdjust = (0.5f, 1.7f),
                },
            },
        },
        moveDt = new WeaponStatus.MovementData(){
            speed = 0.87f,
            speedAdjust = (0.79f, 0.75f, 1.75f)
        },
        attachDt = new WeaponStatus.AttachData(){},
    };
}



public class TestSG : WeaponItem
{
    public TestSG()
    {
        //prefabRoot = "TestSG";
        status.textureRoot = "res://AssetSprite/Weapon/ImageSG.png";
    }
    public override WeaponStatus weaponStatus => new WeaponStatus()
    {
        detailDt = new WeaponStatus.DetailData(){
            basicDamage = 15,
            muzzleSpeed = 45f,
            roundPerMinute = 100,
            pellitAmount = 12,
            basicMag = 8,
        },
        timeDt = new WeaponStatus.TimeData(){
            swapTime = 0.320f,
            reloadTime = (0f,0f,1.0f),
        },
        typeDt = new WeaponStatus.TypeData(){
            mechanismType = MechanismType.MANUAL_RELOAD,
            boltLockerType = BoltLockerType.LOCK_TO_FIRE,
            magazineType = MagazineType.INTERNAL,
            selectorList = new List<SelectorType>{SelectorType.SEMI},
            caliberType = CaliberType.g12,
        },
        aimDt = new WeaponStatus.AimData(){
            ads = new WeaponStatus.AimData.AdsData(){
                adsName = "",
                moa = 7.0f,
                recoil = new WeaponStatus.AimData.AdsData.AdsRecoilData(){
                    fix = new Vector2(2.0f, 9.0f),
                    random = new Vector2(2.8f, 2.8f),
                    recovery = 1.2f,
                    strengthAdjust = (0.75f, 1.35f),
                },
                stance = new WeaponStatus.AimData.AdsData.AdsStancelData(){
                    accuracy = 65.0f,
                    accuracyAdjust = (0.75f, 1.35f),
                },
            },
            hip = new WeaponStatus.AimData.HipData(){
                traggingSpeed = 10.0f,
                recoil = new WeaponStatus.AimData.HipData.HipRecoilData(){
                    recovery = 7.0f,
                    recoveryAdjust = (0.75f, 1.35f),
                    strength = 2.2f,
                },
                stance = new WeaponStatus.AimData.HipData.HipStancelData(){
                    recovery = 12.0f,
                    accuracy = 32.0f,
                    accuracyAdjust = (0.5f, 1.7f),
                },
            },
        },
        moveDt = new WeaponStatus.MovementData(){
            speed = 0.87f,
            speedAdjust = (0.79f, 0.75f, 1.75f)
        },
        attachDt = new WeaponStatus.AttachData(){},
    };
}