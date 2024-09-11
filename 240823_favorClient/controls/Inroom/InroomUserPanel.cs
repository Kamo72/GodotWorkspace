using _favorClient.library;
using _favorClient.library.DataType;
using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _favorClient.controls.Inroom
{
    public partial class InroomUserPanel : UserInterface
    {
        [Export]
        public RichTextLabel nameRich, readyRich;

        [Export]
        public Button prevCharBtn, nextCharBtn;

        [Export]
        public Label typeTxt;

        UserStatus? uStat = new();

        public void SetUser(string nickname)
        {
            nameRich.Text = "[font_size=25][center][color=white]" + nickname;
            readyRich.Text = "[font_size=40][center][color=darkgray]준비 중";
            SetUserStatus(null);
        }
        public void LostUser()
        {
            nameRich.Text = "[font_size=25][center][color=gray]Empty";
            readyRich.Text = "[font_size=40][center][color=darkgray]준비 중";
            SetUserStatus(null);
        }

        public void SetReady(bool isReady)
        {
            readyRich.Text = "[font_size=40][center]" + (isReady? "[color=red]준비 완료" : "[color=darkgray]준비 중");
        }

        public void SetUserStatus(UserStatus? uStat)
        {
            this.uStat = uStat;
            typeTxt.Text = uStat.HasValue ? uStat.Value.type.ToString() : "X";
        }


        public override void _Process(double delta)
        {
            bool isChangable = InroomInterface.instance.userPanels.ToList().FindIndex(panel => panel == this) == InroomInterface.instance.userIdx;

            prevCharBtn.Visible = isChangable;
            nextCharBtn.Visible = isChangable;
        }
    }
}
