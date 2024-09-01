using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _favorClient.Data
{
    public class CharacterData
    {
        public CharacterData(string name, Status status, Dictionary<Skill.KeyType, Skill> skillInfos) 
        {
            this.name = name;
            this.status = status;
            this.skillInfos = skillInfos;
        }

        public string name;
        public Status status;
        public Dictionary<Skill.KeyType, Skill> skillInfos = new Dictionary<Skill.KeyType, Skill>();


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
