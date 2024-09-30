using _favorClient.library;
using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _favorClient.controls
{
    public partial class HostInterface : UserInterface
    {
        [Export]
        private TextEdit nameTxt;
        [Export]
        private TextEdit pwTxt;

        [Export]
        private Button hostBtn;
        [Export]
        private Button signoutBtn;

        Action requestDisposer;
        public override void _Ready()
        {
            hostBtn.Pressed += () =>
            {
                hostBtn.Disabled = true;
                MainClient.instance.Send(new Packet(Packet.Flag.ROOM_HOST, nameTxt.Text, pwTxt.Text != "", pwTxt.Text));

                requestDisposer = MainClient.instance.AddPacketListener(Packet.Flag.ROOM_HOST_CALLBACK, packet =>
                {
                    hostBtn.SetDeferred(Button.PropertyName.Disabled, false);

                    //ROOM_JOIN_CALLBACK
                    bool isSucceed = (bool)packet.value[0];
                    if (isSucceed)
                    {
                        GD.Print("호스트 성공 : " + packet.value[1].ToString());
                        CallDeferred("ShowAcceptDialog", "호스트 성공", packet.value[1].ToString(), "확인");

                        InroomInterface.roomName = nameTxt.Text;

                        //로그인 성공에 따른 처리
                        CallDeferred("ControlExchange", "InroomInterface", "res://controls/InroomInterface.tscn");

                    }
                    else
                    {
                        GD.Print("호스트 실패 : " + packet.value[1].ToString());
                        CallDeferred("ShowAcceptDialog", "호스트 실패", packet.value[1].ToString(), "확인");
                    }

                    requestDisposer();
                });
            };
            signoutBtn.Pressed += () => {
                ControlExchange("RoomInterface", "res://controls/RoomInterface.tscn");
            };
        }



    }
}
