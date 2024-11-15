using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Item;

public class TestItem : Item
{
    public TestItem()
    {
        status = new Status()
        {
            name = "테스트 아이템",
            shortName = "Test",
            description = "테스트를 위한 아이템입니다.테스트를 위한 아이템입니다.",
            textureRoot = "res://Asset/guns/ImageAR.png",
            size = new Vector2I(3, 2),

            rarerity = Rarerity.COMMON,
            category = Category.ETC,
            value = 32000,
            mass = 2.45f,
        };
    }

}

public class TestItemSmall : Item
{
    public TestItemSmall()
    {
        status = new Status()
        {
            name = "테스트 아이템 작음",
            shortName = "Test",
            description = "테스트를 위한 아이템입니다.테스트를 위한 아이템입니다.",
            textureRoot = "res://Asset/guns/ImageAR.png",
            size = new Vector2I(1,1),

            rarerity = Rarerity.COMMON,
            category = Category.ETC,
            value = 4600,
            mass = 0.15f,
        };
    }

}
public class TestBackpack : Backpack
{
    public TestBackpack()
    {
        status = new Status()
        {
            name = "테스트 가방",
            shortName = "Test",
            description = "테스트를 위한 아이템입니다.테스트를 위한 아이템입니다.",
            textureRoot = "res://Asset/guns/ImageAR.png",
            size = new Vector2I(4, 4),

            rarerity = Rarerity.COMMON,
            category = Category.BACKPACK,
            value = 20000,
            mass = 1.80f,
        };
        storage = new Storage(new(4, 8));
    }
    

}


public class TestRig : Rig
{
    public TestRig()
    {
        status = new Status()
        {
            name = "테스트 리그",
            shortName = "Test",
            description = "테스트를 위한 아이템입니다.테스트를 위한 아이템입니다.",
            textureRoot = "res://Asset/guns/ImageAR.png",
            size = new Vector2I(2, 2),

            rarerity = Rarerity.COMMON,
            category = Category.RIG,
            value = 35000,
            mass = 0.95f,

        };
        storage = new Storage(new(7, 2));
    }


}

public class TestContainer : SecContainer
{
    public TestContainer()
    {
        status = new Status()
        {
            name = "테스트 컨테이너",
            shortName = "Test",
            description = "테스트를 위한 아이템입니다.테스트를 위한 아이템입니다.",
            textureRoot = "res://Asset/guns/ImageAR.png",
            size = new Vector2I(2, 2),

            rarerity = Rarerity.COMMON,
            category = Category.S_CONTAINER,
            value = 80000,
            mass = 0.3f,
        };
        storage = new Storage(new(3, 3));
    }


}