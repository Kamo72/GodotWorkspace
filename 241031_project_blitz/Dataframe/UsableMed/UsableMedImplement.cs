using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public partial class PrivateBandage : UsableMed
{
    static PrivateBandage()
    {
        MedStatus.Set("private_bandage", new(
                "private_bandage",
                0.2f,
                2.0f,
                true,
                new()
                {
                    (MedStatus.Effect.HEMOSTASIS, 80f),
                })
            );
    }
    public PrivateBandage() : base("private_bandage")
    {
        status = new Status() {
            name = "출혈 붕대",
            description = "민간용으로 널리 사용되는 응급 붕대입니다. 환부에 둘러 오염과 추가적인 출혈을 방지할 수 있는 가장 기본적인 지혈 수단입니다.",
            category = Category.MEDICINE,
            mass = 0.07f,
            rarerity = Rarerity.COMMON,
            shortName = "band",
            size = new(1,1),
            textureRoot = "res://Asset/IMG-Meds/catt.png",
            value = 1500,
        };
    }
}

public partial class MilitaryBandage : UsableMed
{
    static MilitaryBandage()
    {
        MedStatus.Set("military_bandage", new(
            "military_bandage",
            0.2f,
            2.5f,
            true,
            new()
            {
                (MedStatus.Effect.HEMOSTASIS, 160f),
            }));
    }
    public MilitaryBandage() : base("military_bandage")
    {
        status = new Status()
        {
            name = "군용 붕대",
            description = "대한민국 군용으로 주로 사용되는 표준 지혈 붕대입니다. 훌륭한 지혈 효과와 넉넉한 양 덕분에 상당한 출혈도 막아낼 수 있습니다.",
            category = Category.MEDICINE,
            mass = 0.08f,
            rarerity = Rarerity.COMMON,
            shortName = "milband",
            size = new(1, 1),
            textureRoot = "res://Asset/IMG-Meds/catt.png",
            value = 4500,
        };
    }
}

public partial class CatTourniquet : UsableMed
{
    static CatTourniquet()
    {
        MedStatus.Set("cat_tourniquet", new(
            "cat_tourniquet",
            0.2f,
            3.5f,
            true,
            new()
            {
                (MedStatus.Effect.HEMOSTASIS,  250f),
            }));
    }
    public CatTourniquet() : base("cat_tourniquet")
    {
        status = new Status()
        {
            name = "지혈대",
            description = "CAT 지혈대로 사지의 과다출혈을 막는데 사용되는 구급 장비입니다. 적당한 세기로 묶은체로 30분마다 상태를 확인하십시오.",
            category = Category.MEDICINE,
            mass = 0.076f,
            rarerity = Rarerity.UNCOMMON,
            shortName = "CAT",
            size = new(1, 1),
            textureRoot = "res://Asset/IMG-Meds/catt.png",
            value = 9200,
        };
    }
}

public partial class CeloxA : UsableMed
{
    static CeloxA()
    {
        MedStatus.Set("celox_a", new(
            "celox_a",
            0.2f,
            2.0f,
            true,
            new()
            {
                (MedStatus.Effect.HEMOSTASIS,  540f),
            }));
    }
    public CeloxA() : base("celox_a")
    {
        status = new Status()
        {
            name = "Celox-A",
            description = "총상, 자상 등 깊은 관통상에서 발생하는 중등도에서 중증의 출혈을 신속하게 제어하기 위해 설계된 지혈제입니다. 6g의 CELOX™ 입자가 사전 충전된 어플리케이터를 통해 출혈 부위에 직접 적용할 수 있습니다.",
            category = Category.MEDICINE,
            mass = 0.035f,
            rarerity = Rarerity.RARE,
            shortName = "CELOX",
            size = new(1, 1),
            textureRoot = "res://Asset/IMG-Meds/catt.png",
            value = 16000,
        };
    }
}



public partial class PrivateFAK : UsableMed
{
    static PrivateFAK()
    {
        MedStatus.Set("private_fak", new(
            "private_fak",
            2.0f,
            8.0f,
            true,
            new()
            {
                (MedStatus.Effect.HEALING,  400f),
            }));
    }
    public PrivateFAK() : base("private_fak")
    {
        status = new Status()
        {
            name = "민수용 응급구호키트",
            description = "민수용으로 시중에서 흔하게 구할 수 있던 FAK입니다. 여러모로 부실한게 많지만, 지금은 이런 것 하나에도 목숨이 오고가곤 하죠.",
            category = Category.MEDICINE,
            mass = 1.8f,
            rarerity = Rarerity.COMMON,
            shortName = "FAK",
            size = new(1, 2),
            textureRoot = "res://Asset/IMG-Meds/catt.png",
            value = 5500,
        };
    }
}

public partial class IFAK : UsableMed
{
    static IFAK()
    {
        MedStatus.Set("ifak", new(
            "ifak",
            0.9f,
            3.0f,
            true,
            new()
            {
                (MedStatus.Effect.HEALING,  500f),
                (MedStatus.Effect.HEMOSTASIS,  200f),
            }));
    }
    public IFAK() : base("ifak")
    {
        status = new Status()
        {
            name = "대한민국 육군 구급낭 IFAK",
            description = "병사 개개인에게 지급되는 구급낭으로 최소한의 응급처치를 위한 것들이 담겨 있습니다. 팔다리 정도는 잘려도 살 수 있을거에요.",
            category = Category.MEDICINE,
            mass = 1.2f,
            rarerity = Rarerity.UNCOMMON,
            shortName = "IFAK",
            size = new(1, 1),
            textureRoot = "res://Asset/IMG-Meds/catt.png",
            value = 8000,
        };
    }
}

public partial class AFAK : UsableMed
{
    static AFAK()
    {
        MedStatus.Set("afak", new(
            "afak",
            1.5f,
            10f,
            true,
            new()
            {
                (MedStatus.Effect.HEALING,  1000f),
                (MedStatus.Effect.HEMOSTASIS,  500f),
            }));
    }
    public AFAK() : base("afak")
    {
        status = new Status()
        {
            name = "미해병대 고급 구급낭 AFAK",
            description = "미군 해병대 의무병이 사용하는 고급 구급낭으로 보다 다양한 도구와 치료키트가 있습니다.",
            category = Category.MEDICINE,
            mass = 2.5f,
            rarerity = Rarerity.UNCOMMON,
            shortName = "AFAK",
            size = new(1, 2),
            textureRoot = "res://Asset/IMG-Meds/catt.png",
            value = 15000,
        };
    }
}

public partial class MFAK : UsableMed
{
    static MFAK()
    {
        MedStatus.Set("mfak", new(
            "mfak",
            2.5f,
            30f,
            false,
            new()
            {
                (MedStatus.Effect.HEALING,  2500f),
                (MedStatus.Effect.HEMOSTASIS,  1250f),
            }));
    }
    public MFAK() : base("mfak")
    {
        status = new Status()
        {
            name = "다중 응급처치 키트 MFAK",
            description = "MFAK",
            category = Category.MEDICINE,
            mass = 4.0f,
            rarerity = Rarerity.RARE,
            shortName = "MFAK",
            size = new(2, 2),
            textureRoot = "res://Asset/IMG-Meds/catt.png",
            value = 40000,
        };
    }
}

public partial class AmbulanceFak : UsableMed
{
    static AmbulanceFak()
    {
        MedStatus.Set("ambulance_fak", new(
            "ambulance_fak",
            4.0f,
            80f,
            false,
            new()
            {
                (MedStatus.Effect.HEALING,  4000f),
                (MedStatus.Effect.HEMOSTASIS,  2000f),
            }));
    }
    public AmbulanceFak() : base("ambulance_fak")
    {
        status = new Status()
        {
            name = "응급 구조대 FAK",
            description = "응급 구조대 FAK",
            category = Category.MEDICINE,
            mass = 8.0f,
            rarerity = Rarerity.RARE,
            shortName = "Ambul",
            size = new(3, 2),
            textureRoot = "res://Asset/IMG-Meds/catt.png",
            value = 32000,
        };
    }
}


//        //치료 아이템
//        {
//    "private_fak" , new("private_fak", 2.0f, 6.0f, true, new(){
//            (Effect.HEALING, 400f),
//        })},
//        {
//    "ifak" , new("ifak", 0.9f, 3.0f, true, new(){
//            (Effect.HEALING, 400f),
//            (Effect.HEMOSTASIS, 200f),
//        })},
//        {
//    "afak" , new("afak", 1.5f, 12.0f, true, new(){
//            (Effect.HEALING, 1000f),
//            (Effect.HEMOSTASIS, 500f),
//        })},
//        {
//    "mfak" , new("mfak", 2.1f, 30.0f, false, new(){
//            (Effect.HEALING, 2500f),
//            (Effect.HEMOSTASIS, 1250f),
//        })},
//        {
//    "ambulance_fak" , new("ambulance_fak", 2.1f, 20.0f, false, new(){
//            (Effect.HEALING, 2000f),
//            (Effect.HEMOSTASIS, 1000f),
//        })},