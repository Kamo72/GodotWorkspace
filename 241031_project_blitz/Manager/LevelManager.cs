using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

public class LevelManager
{
    static LevelManager()
    {
        inst = new LevelManager();
    }
    public static LevelManager inst;

    private LevelManager()
    {


    }

    public enum Theme {
        TEST,
        ARMORY,
        HOSPITAL,
        COMPUTER,
        
    }

    public Dictionary<Theme, Dictionary<Type, float>> theme= new()
    {
        { Theme.ARMORY, new(){
            { typeof(M855), 0.6f },
            { typeof(M855A1), 0.2f },
        } },
        { Theme.TEST, new(){
            { typeof(TestItem), 0.2f },
            { typeof(TestItemSmall), 1f },
        } },
    };

    public List<Item> GetSpawnedItems(Theme theme, Vector2I size, float valueRatio) 
    {
        float valueExpect = size.X * size.Y * valueRatio * 1000;
        float valueNow = 0f;
        List<Item> itemStack = new();
        int stackedSlots = 0;

        Dictionary<Type, float> levelList = this.theme[theme];
        
        //
        float totalPoint = 0, nowPoint, randPoint;
        foreach (var value in levelList.Values)
            totalPoint += value;

        while (true) 
        {
            //예상 비용을 충분히 충족했을 때 종료
            if (valueNow > valueExpect) break;

            //채울 수 있는 칸의 50%가 충족됐을 때, 25% 확률로 종료 
            if (stackedSlots > size.X * size.Y * 0.5f)
                if (Random.Shared.NextDouble() < 0.25f)
                    break;

            randPoint = (float)Random.Shared.NextDouble();
            nowPoint = 0;

            foreach (var pair in levelList) 
            {
                //판정 성공
                if (nowPoint <= randPoint && randPoint < nowPoint + pair.Value)
                    if (Activator.CreateInstance(pair.Key) is Item item)
                    {
                        if (item is IStackable stackable)
                        {
                            stackable.stackNow = (int)Mathf.Ceil(stackable.stackMax * (0.25f + randPoint / 2f));
                            valueNow += item.status.value * (stackable.stackNow - 1);
                        }
                        valueNow += item.status.value;
                        itemStack.Add(item);
                    }

                nowPoint += pair.Value/totalPoint;
            }
        }

        return itemStack;
    }




    

}