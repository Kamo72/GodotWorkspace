using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _favorClient.Entity.Character_Test
{
    public partial class CharacterFuhrer : Character
    {




        protected override void DoAttackMain(Vector2 from, Vector2 to)
        {
        }

        protected override void DoAttackSub(Vector2 from, Vector2 to)
        {
        }

        protected override void DoDodge(Vector2 from, Vector2 to)
        {
        }

        protected override void DoSkillMain(Vector2 from, Vector2 to)
        {
        }

        protected override void DoSkillSub(Vector2 from, Vector2 to)
        {
        }

        protected override void DoUltimate(Vector2 from, Vector2 to)
        {
        }

        protected virtual void ActionProcess()
        {
            switch (action.state)
            {
            case "basicAttack":
                {

                }
                break;
            case "subAttack":
                {

                }
                break;
            case "dodge":
                {

                }
                break;
            case "mainSkill":
                 {

                }
                break;
            case "subSkill":
                {

                }
                break;
                case "ultimate":
                    {

                }
                break;
            }
        }
    }
}
