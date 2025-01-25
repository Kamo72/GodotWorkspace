

using Godot;
using System;
using System.Collections.Generic;

public  class TraderManager
{
    static TraderManager()
    {
        instance = new TraderManager();
    }
    public static TraderManager instance;
    private TraderManager()
    {
    }

    public Dictionary<string, Trader> traderLibrary = new() {
        { "medic", new Trader(){
            code = "medic",
            reputation = 0f,
        } }
    };

    public Dictionary<string, TraderData> traderDataLibrary = new() {
        { "medic", new TraderData(){
            products = new List<Product>(){
                new(typeof(M855), 1.0f),
                new(typeof(M855A1), 1.2f, 1),
                new(typeof(G12_Grizzly), 1.1f, 0,
                    () => TraderManager.instance.traderLibrary["medic"].reputation > 0),
                },
            reputationSeps = new(){1,3,4,4.5f}
        } },
    };

    public List<Storage> stashList = new() {
        new Storage(new(8,12)),
        new Storage(new(8,12)),
        new Storage(new(8,12)),
    };
}

public class Trader
{
    public string code = "";
    public float reputation = 0f;
    public TraderData traderData => TraderManager.instance.traderDataLibrary[code];
}

public struct TraderData
{
    //public TraderData(List<Product> products, float[] reputationSeps)
    //{
    //    this.products = products;
    //    this.reputationSeps = reputationSeps;
    //}

    public List<Product> products;
    public List<float> reputationSeps;
    public int GetReputationLv(float rep) 
    {
        int lv = 0;
        if(reputationSeps.Count == 0)
            return lv;

        foreach (float sep in reputationSeps)
        {
            if (rep >= sep)
                lv++;
            else
                break;
        }
        return lv;
    }
    public List<Product> GetValidProducts(int repLv) 
    {
        var list = new List<Product>();

        foreach (var product in products)
        {
            if (product.needReputationLv > repLv)
                continue;
            if (product.condition == null)
                continue;
            if (!product.condition())
                continue;

            list.Add(product);
        }

        return list;
    }
}

public struct Product 
{
    public Product(Type itemType, float valueRatio, int needReputationLv = 0, Func<bool> condition = null)
    {
        this.itemType = itemType;
        this.valueRatio = valueRatio;
        this.needReputationLv = needReputationLv;
        this.condition = condition;
    }

    public Type itemType;   //아이템 타입
    public float valueRatio;    //가격배율
    public int needReputationLv;    //요구 명성 수치
    public Func<bool> condition;    //조건문

}