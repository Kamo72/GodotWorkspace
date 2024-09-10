
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Resources;
using System.Text;
using System.Threading.Tasks;

namespace _240823_favorServer.library.DataType
{
    public partial struct CharacterData
    {
        public CharacterData(string name, Status status, string path)
        {
            this.name = name;
            this.status = status;
            //getScene = () => ResourceLoader.Load<PackedScene>(path);
        }

        public string name;
        public Status status;
        //public Func<PackedScene> getScene;

        public Dictionary<Skill.KeyType, Skill> skillInfos = new Dictionary<Skill.KeyType, Skill>();

        //기본 스텟
        public struct Status
        {
            public Status(float hpMax, float speed, float resistance)
            {
                this.hpMax = hpMax;
                this.speed = speed;
                this.resistance = resistance;
            }

            public float hpMax;     //최대 체력
            public float speed;     //기본 이동속도
            public float resistance; //넉백저항
        }

        //스킬
        public struct Skill
        {

            public Skill(KeyType type, string name, float? cooldown, string easyDes, Description diffDes,
                params float[] stats
                )
            {
                this.type = type;
                this.name = name;
                this.cooldown = cooldown;
                this.easyDes = easyDes;
                this.diffDes = diffDes;
                this.stats = stats;
            }


            public enum KeyType
            {
                PAS, LM, RM, SPACE, Q, E, R
            }
            public KeyType type;

            public string name;
            public string easyDes;
            public float? cooldown;
            public float[] stats;
            public Description diffDes;

        }

        //특성
        public struct Trait
        {
            public string name;
            public Description diffDes;
            public int cost;
        }

        //특성 트리
        public TraitTreeData traitTree = new();
        public struct TraitTreeData
        {
            public TraitTreeData()
            {
                AddTrait(Root, new Vector2());
            }

            public Trait Root = new Trait()
            {
                name = "기본 노드"
            };

            public List<(Trait trait, Vector2 pos, string[] dependencies)> traits = new();

            public void AddTrait(Trait newTrait, Vector2 pos, params string[] dependencies) => traits.Add((newTrait, pos, dependencies));
        }


        //캐릭터 유형
        public enum Type
        {
            NONE,
            FUHRER,
            HORNET,
            AGITATOR,

        }
        static Dictionary<Type, CharacterData> dataLib = new Dictionary<Type, CharacterData>()
        {
        };

        public static CharacterData GetByType(Type type) => dataLib[type];
        public static int typeCount => dataLib.Count;

        public void AddSkill(Skill skill) => skillInfos.Add(skill.type, skill);

    }


    public struct TraitTree
    {
        public TraitTree(CharacterData.Type type)
        {
            this.type = type;
        }

        public CharacterData.Type type;
        public CharacterData.TraitTreeData traitTreeData => CharacterData.GetByType(type).traitTree;
        public int spentPoint = 0;

        public List<string> traitsList = new() { "기본 노드" };

        //string으로 하는게 맞을려나...
        public readonly bool this[string name] => traitsList.Contains(name);
        public bool IsTraitTaken(string name) => traitsList.Contains(name);
        public bool TakeTraitByName(string name, int hasPoint = 99)
        {
            bool addedAlready = traitsList.Contains(name);

            if (addedAlready)
                throw new Exception("AddTraitByName - the trait of given name is in traitsList already");

            CharacterData.Trait trait = GetTraitByName(name);

            if (hasPoint < trait.cost)
                throw new Exception("AddTraitByName - you need more points to take this trait");

            traitsList.Add(trait.name);
            spentPoint += trait.cost;
            return true;
        }
        public bool ReleaseTraitByName(string name)
        {
            if (name == "기본 노드")
                throw new Exception("ReleaseTraitByName - root node is not able to realease");

            bool releasedAlready = !traitsList.Contains(name);

            if (releasedAlready)
                throw new Exception("ReleaseTraitByName - the trait of given name is not in traitsList already");

            CharacterData.Trait trait = GetTraitByName(name);

            traitsList.Remove(name);
            spentPoint -= trait.cost;
            return true;

        }
        public bool ReleaseAll()
        {
            traitsList.Clear();
            traitsList.Add("기본 노드");
            spentPoint = 0;
            return true;
        }
        public CharacterData.Trait GetTraitByName(string traitName) => traitTreeData.traits.Find(i => i.trait.name == traitName).trait;
    }

    public struct Description
    {
        object[] data;
        public Description(params object[] strOrFuncs)
        {
            data = strOrFuncs;
        }

        public override string ToString()
        {
            string res = "";
            foreach (object obj in data)
            {
                if (obj is string str)
                    res += str;
                if (obj is Func<string> func)
                    res += func();
            }

            return res;
        }

    }
}
