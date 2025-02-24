using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class PTBL2 : Plate 
{
    public PTBL2() :
        base(4.8f, 0.96f, 350.0f)
    {
        status = new Status() { 
            name = "PTBL2 복합 방탄판",
            shortName = "PTBL2",
            description = "PTBL2 방탄판은 러시아제 방탄 플레이트로, 방탄복에 삽입하여 총기 탄환을 방어하는 역할을 합니다. 세라믹과 UHMWPE(초고분자량 폴리에틸렌) 복합재로 제작되었으며, NIJ Level III 또는 IV 수준의 방호력을 제공하는 것으로 알려져 있습니다. 주로 군과 특수부대에서 사용되며, 7.62mm 철갑탄 방어가 가능하다고 평가됩니다.",

            category = Category.PLATE,
            rarerity = Rarerity.RARE,

            mass = 3.4f,
            size = new (2,2),
            value = 248000,

            textureRoot = "res://Asset/IMG-Meds/catt.png",
        };
    }
}
public class K2_Plate : Plate
{
    public K2_Plate() :
        base(4.3f, 0.97f, 300.0f)
    {
        status = new Status()
        {
            name = "대한민국 육군 K-2 방탄판",
            shortName = "K-2",
            description = "국군 표준 방탄판으로, K-2 방탄복에 삽입하여 사용. 7.62mm 철갑탄에는 취약하여 상위 등급 방탄판으로 대체되는 추세.",

            rarerity = Rarerity.RARE,
            category = Category.PLATE,

            mass = 2.7f,
            size = new(2, 2),
            value = 181000,
            
            textureRoot = "res://Asset/IMG-Meds/catt.png",
        };
    }
}
public class K3_Plate : Plate
{
    public K3_Plate() :
        base(6.2f, 0.93f, 450.0f)
    {
        status = new Status()
        {
            name = "대한민국 육군 K-3 방탄판",
            shortName = "K-3",
            description = "기존 K-2보다 향상된 방탄 성능 제공. 2020년대 이후 특수부대 및 정예 부대에서 확대 도입.",

            rarerity = Rarerity.UNIQUE,
            category = Category.PLATE,

            mass = 3.6f,
            size = new(2, 2),
            value = 520000,
            
            textureRoot = "res://Asset/IMG-Meds/catt.png",
        };
    }
}
public class FMA_SAPI : Plate
{
    public FMA_SAPI() :
        base(5.0f, 0.96f, 280.0f)
    {
        status = new Status()
        {
            name = "FMA 컨트롤 IIIA SAPI 방탄판",
            shortName = "SAPI",
            description = "기존 K-2보다 향상된 방탄 성능 제공. 2020년대 이후 특수부대 및 정예 부대에서 확대 도입.",

            rarerity = Rarerity.RARE,
            category = Category.PLATE,

            mass = 0.550f,
            size = new(2, 2),
            value = 305000,
            
            textureRoot = "res://Asset/IMG-Meds/catt.png",
        };
    }
}
public class API_BZ : Plate
{
    public API_BZ() :
        base(5.4f, 0.98f, 460.0f)
    {
        status = new Status()
        {
            name = "API-BZ 방탄판",
            shortName = "API-BZ",
            description = "기존 K-2보다 향상된 방탄 성능 제공. 2020년대 이후 특수부대 및 정예 부대에서 확대 도입.",

            rarerity = Rarerity.RARE,
            category = Category.PLATE,

            mass = 2.1f,
            size = new(2, 2),
            value = 356000,
            
            textureRoot = "res://Asset/IMG-Meds/catt.png",
        };
    }
}