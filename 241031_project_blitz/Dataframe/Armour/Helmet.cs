using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class K1_Helmet : Helmet
{
    public K1_Helmet() :
        base(3.2f, 0.95f, 200.0f)
    {
        status = new Status()
        {
            name = "K-1 방탄모",
            shortName = "K-2",
            description = "대한민국 국군에서 가장 오랫동안 사용된 방탄모. 아라미드 섬유(Kevlar) 소재로 제작.",

            rarerity = Rarerity.UNCOMMON,
            category = Category.HELMET,

            mass = 1.4f,
            size = new(2, 2),
            value = 41000,

            textureRoot = "res://Asset/IMG-Meds/catt.png",
        };
    }
}

public class K2_Helmet : Helmet
{
    public K2_Helmet() :
        base(4.5f, 0.94f, 240.0f)
    {
        status = new Status()
        {
            name = "K-2 방탄모",
            shortName = "K-2",
            description = "K-1 방탄모의 후속 모델로 개선된 방탄 성능 제공. 현재 대한민국 육군, 해병대에서 주력으로 사용 중.",

            rarerity = Rarerity.RARE,
            category = Category.HELMET,

            mass = 1.6f,
            size = new(2, 2),
            value = 87000,

            textureRoot = "res://Asset/IMG-Meds/catt.png",
        };
    }
}

public class K3_Helmet : Helmet
{
    public K3_Helmet() :
        base(5.3f, 0.94f, 300.0f)
    {
        status = new Status()
        {
            name = "K-3 방탄모",
            shortName = "K-3",
            description = "K-2 방탄모의 개선형으로 특수부대 중심으로 보급. FAST(High Cut) 스타일로 설계되어 통신장비 장착 용이. 공수부대, 특수전사령부, 대테러부대 등에서 사용.",

            rarerity = Rarerity.UNIQUE,
            category = Category.HELMET,

            mass = 1.7f,
            size = new(2, 2),
            value = 185000,

            textureRoot = "res://Asset/IMG-Meds/catt.png",
        };
    }
}

public class FastHelmet : Helmet
{
    public FastHelmet() :
        base(4.8f, 0.96f, 320.0f)
    {
        status = new Status()
        {
            name = "FAST 방탄모",
            shortName = "FAST",
            description = "경량화 설계로 장시간 착용에 유리. 대한민국 특수전사령부, 해군 특수전전단(UDT/SEAL), 육군 특공연대 등에서 사용.",

            rarerity = Rarerity.UNIQUE,
            category = Category.HELMET,

            mass = 1.4f,
            size = new(2, 2),
            value = 160000,

            textureRoot = "res://Asset/IMG-Meds/catt.png",
        };
    }
}