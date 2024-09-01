using _favorClient.library;
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

        public void SetUser(string nickname)
        {
            nameRich.Text = "[font_size=25][center][color=white]" + nickname;
            readyRich.Text = "[font_size=40][center][color=darkgray]준비 중";

        }

        public void LostUser()
        {
            nameRich.Text = "[font_size=25][center][color=gray]Empty";
            readyRich.Text = "[font_size=40][center][color=darkgray]준비 중";

        }

        public void SetReady(bool isReady)
        {
            readyRich.Text = "[font_size=40][center]" + (isReady? "[color=red]준비 완료" : "[color=darkgray]준비 중");
        }

        public void GetStatus(UserRoomData data)
        {
        
        }


    }
}
