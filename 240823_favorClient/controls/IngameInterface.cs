using _favorClient.controls.Ingame;
using _favorClient.library;
using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _favorClient.controls
{
    public partial class IngameInterface : UserInterface
    {
        [ExportCategory("Skills")]
        [Export]//패시브
        public IngameSkillPanel skillPassive;
        [Export]//우클
        public IngameSkillPanel skillLM;
        [Export]//좌클
        public IngameSkillPanel skillRM;
        [Export]//Q 스킬
        public IngameSkillPanel skillQ;
        [Export]//E 스킬
        public IngameSkillPanel skillE;
        [Export]//R 궁극기
        public IngameSkillPanel skillR;

        [ExportCategory("Dynamic")]
        [Export]
        public int a;

        [ExportCategory("MyInfo")]
        [Export]
        public int aa;

        [ExportCategory("TeamInfo")]
        [Export]
        public int aaa;

        [ExportCategory("BossInfo")]
        [Export]
        public int aaaa;


        public List<IngameSkillPanel> skillPanels;
        public override void _Ready()
        {
            skillPanels = new() {
                skillPassive, skillLM, skillRM, skillE, skillQ, skillR
            };

            foreach (var skillPanel in skillPanels)
                skillPanel.SetVisible(false);
        }


    }
}



