using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public partial class Faction
{
    static Dictionary<string, Faction> factionList = new (){
        {"survivor",  new Faction()  },
        {"revelation",  new Faction()  },
        {"hyenas",  new Faction()  },
    };

    static List<(string, string, float)> factionFriendship = new()
    {
        ("survivor", "hyenas", -100),
        ("revelation", "survivor", 100),
        ("revelation", "hyenas", -100),
    };

    public static Faction Get(string key) => factionList[key];
    public static float FriendShipGet(string key1, string key2)
    {
        if (!factionList.ContainsKey(key1)) throw new Exception("존재하지 않는 팩션 키값입니다.");
        if (!factionList.ContainsKey(key2)) throw new Exception("존재하지 않는 팩션 키값입니다.");

        if (!factionFriendship.Exists(data =>
            (data.Item1 == key1 && data.Item2 == key2) ||
            (data.Item1 == key2 && data.Item2 == key1)
        ))
        {
            return factionFriendship.Find(data =>
                   (data.Item1 == key1 && data.Item2 == key2) ||
                   (data.Item1 == key2 && data.Item2 == key1)
               ).Item3;
        }
        else
        {
            factionFriendship.Add((key1, key2, 0));
            return 0;
        }
    }
    public static void FriendShipSet(string key1, string key2, float value)
    {
        if (!factionList.ContainsKey(key1)) throw new Exception("존재하지 않는 팩션 키값입니다.");
        if (!factionList.ContainsKey(key2)) throw new Exception("존재하지 않는 팩션 키값입니다.");

        (string, string, float) target = ("", "", 0f);

        if (factionFriendship.Exists(data => data.Item1 == key2 && data.Item2 == key1))
        {
            target = factionFriendship.Find(data => data.Item1 == key2 && data.Item2 == key1);
        }
        else if (factionFriendship.Exists(data => data.Item1 == key1 && data.Item2 == key2))
        {
            target = factionFriendship.Find(data => data.Item1 == key1 && data.Item2 == key2);
        }
        else
        {
            factionFriendship.Add((key1, key2, value));
            return;
        }

        int idx = factionFriendship.IndexOf(target);
        factionFriendship.RemoveAt(idx);
        factionFriendship.Add((key1, key2, value));
    }

}