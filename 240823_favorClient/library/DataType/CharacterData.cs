using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace _favorClient.library.DataType
{
    public struct CharacterData
    {
        public CharacterData(string name, Status status, Dictionary<Skill.KeyType, Skill> skillInfos)
        {
            this.name = name;
            this.status = status;
            this.skillInfos = skillInfos;
        }

        public string name;
        public Status status;
        //얘가 딕셔너리면 안되는데 씨발
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
        public TraitTreeData traitTree = new ();
        public struct TraitTreeData
        {
            public TraitTreeData() 
            {
                AddTrait(Root, new Vector2());
            }

            public Trait Root = new Trait() { 
                name = "기본 노드"
            };

            public List<(Trait trait, Vector2 pos, string[] dependencies)> traits = new();

            public void AddTrait(Trait newTrait, Vector2 pos, params string[] dependencies) => traits.Add((newTrait, pos, dependencies));
        }



        public enum Type
        {
            NONE,
            WARRIOR,
        }
        static Dictionary<Type, CharacterData> dataLib = new Dictionary<Type, CharacterData>()
        {
            {Type.NONE,
                new CharacterData("None", new(100, 10, 10),
                    new(){
                        { Skill.KeyType.PAS, new Skill(Skill.KeyType.PAS,
                            "정의되지 않은 스킬", null,
                            "아직 스킬에 대한 설명이 정의되지 않았습니다.",
                            new Description(
                                "아직 스킬에 대한 설명이 정의되지 않았습니다."
                                ),
                            0f)},

                        { Skill.KeyType.LM, new Skill(Skill.KeyType.LM,
                            "정의되지 않은 스킬", null,
                            "아직 스킬에 대한 설명이 정의되지 않았습니다.",
                            new Description(
                                "아직 스킬에 대한 설명이 정의되지 않았습니다."
                                ),
                            0f)},

                        { Skill.KeyType.RM, new Skill(Skill.KeyType.RM,
                            "정의되지 않은 스킬", null,
                            "아직 스킬에 대한 설명이 정의되지 않았습니다.",
                            new Description(
                                "아직 스킬에 대한 설명이 정의되지 않았습니다."
                                ),
                            0f)},

                        { Skill.KeyType.SPACE, new Skill(Skill.KeyType.SPACE,
                            "정의되지 않은 스킬", null,
                            "아직 스킬에 대한 설명이 정의되지 않았습니다.",
                            new Description(
                                "아직 스킬에 대한 설명이 정의되지 않았습니다."
                                ),
                            0f)},

                        { Skill.KeyType.Q, new Skill(Skill.KeyType.Q,
                            "정의되지 않은 스킬", null,
                            "아직 스킬에 대한 설명이 정의되지 않았습니다.",
                            new Description(
                                "아직 스킬에 대한 설명이 정의되지 않았습니다."
                                ),
                            0f)},

                        { Skill.KeyType.E, new Skill(Skill.KeyType.E,
                            "정의되지 않은 스킬", null,
                            "아직 스킬에 대한 설명이 정의되지 않았습니다.",
                            new Description(
                                "아직 스킬에 대한 설명이 정의되지 않았습니다."
                                ),
                            0f)},

                        { Skill.KeyType.R, new Skill(Skill.KeyType.R,
                            "정의되지 않은 스킬", null,
                            "아직 스킬에 대한 설명이 정의되지 않았습니다.",
                            new Description(
                                "아직 스킬에 대한 설명이 정의되지 않았습니다."
                                ),
                            0f)},
                    }
                )
            },
            //TODO
        };

        public static CharacterData GetByType(Type type) => dataLib[type];


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
