using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        };
    }
    
}

public class TestBackpack : Backpack
{
    public TestBackpack()
    {
        status = new Status()
        {
            name = "테스트 아이템",
            shortName = "Test",
            description = "테스트를 위한 아이템입니다.테스트를 위한 아이템입니다.",
            textureRoot = "res://Asset/guns/ImageAR.png",
            size = new Vector2I(2, 2),
            category = Category.BACKPACK,

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
            name = "테스트 아이템",
            shortName = "Test",
            description = "테스트를 위한 아이템입니다.테스트를 위한 아이템입니다.",
            textureRoot = "res://Asset/guns/ImageAR.png",
            size = new Vector2I(2, 2),
            category = Category.RIG,

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
            name = "테스트 아이템",
            shortName = "Test",
            description = "테스트를 위한 아이템입니다.테스트를 위한 아이템입니다.",
            textureRoot = "res://Asset/guns/ImageAR.png",
            size = new Vector2I(2, 2),
            category = Category.S_CONTAINER,

        };
        storage = new Storage(new(3, 3));
    }


}