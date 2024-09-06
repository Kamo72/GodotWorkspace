using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _favorClient.library.DataType
{
    public partial struct CharacterData
    {
        static CharacterData()
        {
            CharacterData cd;

            #region [FUHRUR]
            cd = new CharacterData("퓌러", new(100, 10, 10),
                    "res://EntityImplemented/CharacterFuhrer/CharacterFuhrer.tscn");

            cd.AddSkill(new Skill(Skill.KeyType.PAS,
                "정의되지 않은 스킬", null,
                "아직 스킬에 대한 설명이 정의되지 않았습니다.",
                new Description(
                    "아직 스킬에 대한 설명이 정의되지 않았습니다."
                    ),
                0f));
            cd.AddSkill(new Skill(Skill.KeyType.LM,
                "정의되지 않은 스킬", null,
                "아직 스킬에 대한 설명이 정의되지 않았습니다.",
                new Description(
                    "아직 스킬에 대한 설명이 정의되지 않았습니다."
                    ),
                0f));
            cd.AddSkill(new Skill(Skill.KeyType.RM,
                "정의되지 않은 스킬", null,
                "아직 스킬에 대한 설명이 정의되지 않았습니다.",
                new Description(
                    "아직 스킬에 대한 설명이 정의되지 않았습니다."
                    ),
                0f));
            cd.AddSkill(new Skill(Skill.KeyType.SPACE,
                "정의되지 않은 스킬", null,
                "아직 스킬에 대한 설명이 정의되지 않았습니다.",
                new Description(
                    "아직 스킬에 대한 설명이 정의되지 않았습니다."
                    ),
                0f));
            cd.AddSkill(new Skill(Skill.KeyType.Q,
                "정의되지 않은 스킬", null,
                "아직 스킬에 대한 설명이 정의되지 않았습니다.",
                new Description(
                    "아직 스킬에 대한 설명이 정의되지 않았습니다."
                    ),
                0f));
            cd.AddSkill(new Skill(Skill.KeyType.E,
                "정의되지 않은 스킬", null,
                "아직 스킬에 대한 설명이 정의되지 않았습니다.",
                new Description(
                    "아직 스킬에 대한 설명이 정의되지 않았습니다."
                    ),
                0f));
            cd.AddSkill(new Skill(Skill.KeyType.R,
                "정의되지 않은 스킬", null,
                "아직 스킬에 대한 설명이 정의되지 않았습니다.",
                new Description(
                    "아직 스킬에 대한 설명이 정의되지 않았습니다."
                    ),
                0f));

            dataLib.Add(Type.FUHRER, cd);

            #endregion

            #region [HORNET]
            cd = new CharacterData("호넷", new(100, 10, 10),
                    "res://EntityImplemented/CharacterFuhrer/CharacterFuhrer.tscn");

            cd.AddSkill(new Skill(Skill.KeyType.PAS,
                "정의되지 않은 스킬", null,
                "아직 스킬에 대한 설명이 정의되지 않았습니다.",
                new Description(
                    "아직 스킬에 대한 설명이 정의되지 않았습니다."
                    ),
                0f));
            cd.AddSkill(new Skill(Skill.KeyType.LM,
                "정의되지 않은 스킬", null,
                "아직 스킬에 대한 설명이 정의되지 않았습니다.",
                new Description(
                    "아직 스킬에 대한 설명이 정의되지 않았습니다."
                    ),
                0f));
            cd.AddSkill(new Skill(Skill.KeyType.RM,
                "정의되지 않은 스킬", null,
                "아직 스킬에 대한 설명이 정의되지 않았습니다.",
                new Description(
                    "아직 스킬에 대한 설명이 정의되지 않았습니다."
                    ),
                0f));
            cd.AddSkill(new Skill(Skill.KeyType.SPACE,
                "정의되지 않은 스킬", null,
                "아직 스킬에 대한 설명이 정의되지 않았습니다.",
                new Description(
                    "아직 스킬에 대한 설명이 정의되지 않았습니다."
                    ),
                0f));
            cd.AddSkill(new Skill(Skill.KeyType.Q,
                "정의되지 않은 스킬", null,
                "아직 스킬에 대한 설명이 정의되지 않았습니다.",
                new Description(
                    "아직 스킬에 대한 설명이 정의되지 않았습니다."
                    ),
                0f));
            cd.AddSkill(new Skill(Skill.KeyType.E,
                "정의되지 않은 스킬", null,
                "아직 스킬에 대한 설명이 정의되지 않았습니다.",
                new Description(
                    "아직 스킬에 대한 설명이 정의되지 않았습니다."
                    ),
                0f));
            cd.AddSkill(new Skill(Skill.KeyType.R,
                "정의되지 않은 스킬", null,
                "아직 스킬에 대한 설명이 정의되지 않았습니다.",
                new Description(
                    "아직 스킬에 대한 설명이 정의되지 않았습니다."
                    ),
                0f));

            dataLib.Add(Type.HORNET, cd);

            #endregion

            #region [AGITATOR]
            cd = new CharacterData("어지테이터", new(100, 10, 10),
                    "res://EntityImplemented/CharacterFuhrer/CharacterFuhrer.tscn");

            cd.AddSkill(new Skill(Skill.KeyType.PAS,
                "정의되지 않은 스킬", null,
                "아직 스킬에 대한 설명이 정의되지 않았습니다.",
                new Description(
                    "아직 스킬에 대한 설명이 정의되지 않았습니다."
                    ),
                0f));
            cd.AddSkill(new Skill(Skill.KeyType.LM,
                "정의되지 않은 스킬", null,
                "아직 스킬에 대한 설명이 정의되지 않았습니다.",
                new Description(
                    "아직 스킬에 대한 설명이 정의되지 않았습니다."
                    ),
                0f));
            cd.AddSkill(new Skill(Skill.KeyType.RM,
                "정의되지 않은 스킬", null,
                "아직 스킬에 대한 설명이 정의되지 않았습니다.",
                new Description(
                    "아직 스킬에 대한 설명이 정의되지 않았습니다."
                    ),
                0f));
            cd.AddSkill(new Skill(Skill.KeyType.SPACE,
                "정의되지 않은 스킬", null,
                "아직 스킬에 대한 설명이 정의되지 않았습니다.",
                new Description(
                    "아직 스킬에 대한 설명이 정의되지 않았습니다."
                    ),
                0f));
            cd.AddSkill(new Skill(Skill.KeyType.Q,
                "정의되지 않은 스킬", null,
                "아직 스킬에 대한 설명이 정의되지 않았습니다.",
                new Description(
                    "아직 스킬에 대한 설명이 정의되지 않았습니다."
                    ),
                0f));
            cd.AddSkill(new Skill(Skill.KeyType.E,
                "정의되지 않은 스킬", null,
                "아직 스킬에 대한 설명이 정의되지 않았습니다.",
                new Description(
                    "아직 스킬에 대한 설명이 정의되지 않았습니다."
                    ),
                0f));
            cd.AddSkill(new Skill(Skill.KeyType.R,
                "정의되지 않은 스킬", null,
                "아직 스킬에 대한 설명이 정의되지 않았습니다.",
                new Description(
                    "아직 스킬에 대한 설명이 정의되지 않았습니다."
                    ),
                0f));

            dataLib.Add(Type.AGITATOR, cd);

            #endregion

        }
    }
}

