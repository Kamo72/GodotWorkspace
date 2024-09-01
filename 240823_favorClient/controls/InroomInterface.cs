using Godot;
using _favorClient.controls.Inroom;
using _favorClient.library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;

namespace _favorClient.controls
{
    public partial class InroomInterface : UserInterface
    {
        [Export]
        private Godot.Collections.Array<InroomUserPanel> userPanels = new();

        [Export]
        private TextEdit nameTxt;

        [Export]
        private Button readyBtn;
        [Export]
        private Button exitBtn;

        [Export]
        private ItemList chatList;
        [Export]
        private TextEdit chatTxt;
        [Export]
        private Button chatBtn;

        bool isReady = false;
        public static string roomName = "";
        public static string userId = "";
        public (string name, string id, UserRoomData urd)?[] userArray
            = new (string name, string id, UserRoomData urd)?[4] { null, null, null, null };

        Action requestDisposer;
        public override void _Ready()
        {
            nameTxt.Text = roomName;

            //ROOM_EXIT 전송 및 ROOM_EXIT_CALLBACK 처리
            exitBtn.Pressed += () => {

                exitBtn.Disabled = true;
                MainClient.instance.Send(new Packet(Packet.Flag.ROOM_EXIT));

                requestDisposer = MainClient.instance.AddPacketListener(Packet.Flag.ROOM_EXIT_CALLBACK, packet =>
                {
                    exitBtn.SetDeferred(Button.PropertyName.Disabled, false);

                    //ROOM_EXIT_CALLBACK
                    bool isSucceed = bool.Parse(packet.value[0].ToString());
                    if (isSucceed)
                    {
                        GD.Print("방 퇴장 성공 : " + packet.value[1].ToString());

                        //로그인 성공에 따른 처리
                        CallDeferred("ControlExchange", "RoomInterface", "res://controls/RoomInterface.tscn");
                    }
                    else
                    {
                        GD.Print("방 퇴장 실패 : " + packet.value[1].ToString());
                        CallDeferred("ShowAcceptDialog", "방 퇴장 실패", packet.value[1].ToString(), "확인");

                        //로그인 성공에 따른 처리
                        CallDeferred("ControlExchange", "RoomInterface", "res://controls/RoomInterface.tscn");
                    }

                    requestDisposer();
                });

            };

            //ROOM_READY_SEND 전송
            readyBtn.Pressed += () => {

                isReady = !isReady;
                MainClient.instance.Send(new Packet(Packet.Flag.ROOM_READY_SEND, isReady));
            };

            //ROOM_READY_RECV에 대한 처리
            MainClient.instance.AddPacketListener(Packet.Flag.ROOM_READY_RECV, packet =>
            {
                int tIdx = int.Parse(packet.value[0].ToString());
                bool isReady = bool.Parse(packet.value[1].ToString());

                CallDeferred("AsyncUserReady", tIdx - 1, isReady);
            });

            //ROOM_USERS에 대한 처리
            MainClient.instance.AddPacketListener(Packet.Flag.ROOM_USERS, packet =>
            {
                List<(int idx, string id, string name)> newDataList =
                    (List<(int idx, string id, string name)>)packet.value[0];

                foreach (var item in newDataList)
                    if (userArray[item.idx - 1].HasValue == false)
                    {
                        userArray[item.idx - 1] = (item.name, item.id, new ());
                        CallDeferred("AsyncUserGet", item.idx, item.name);
                    }

                Godot.Collections.Array<int> toDelList = new();

                for (int i = 0; i < 4; i++)
                {
                    if (userArray[i].HasValue == false) continue;
                    var item = userArray[i].Value;

                    if (newDataList.FindIndex(i => i.id == item.id) == -1)
                        toDelList.Add(i);
                }

                CallDeferred("AsyncUserDelList", toDelList);
            });


            chatList.Draw += () => chatList.GetVScrollBar().Value = 9999;

            //ROOM_CHAT_RECV 대한 처리
            MainClient.instance.AddPacketListener(Packet.Flag.ROOM_CHAT_RECV, packet =>
            {
                string sender = packet.value[0].ToString();
                string message = packet.value[1].ToString();

                CallDeferred("AsyncAddList", $"[{sender}] {message}");
            });

            //ROOM_CHAT_SEND 전송
            chatBtn.Pressed += () => {
                if (chatTxt.Text != "")
                {
                    Packet packet = new Packet(Packet.Flag.ROOM_CHAT_SEND, GetMyName(), chatTxt.Text);
                    chatTxt.Text = "";
                    MainClient.instance.Send(packet);
                }

            };


            //ROOM_STATUS_RECV 대한 처리
            MainClient.instance.AddPacketListener(Packet.Flag.ROOM_STATUS_RECV, packet =>
            {
                //TODO
            });

            //ROOM_STATUS_RECV 대한 처리
            MainClient.instance.AddPacketListener(Packet.Flag.ROOM_COUNTDOWN, packet =>
            {
                int countdown = int.Parse(packet.value[0].ToString());

                //TODO
            });

            //ROOM_STATUS_RECV 대한 처리
            MainClient.instance.AddPacketListener(Packet.Flag.ROOM_START, packet =>
            {
                //TODO
            });
        }

        public string GetMyName()
        {
            string userName = "";

            for (int i = 0; i < 4; i++)
                if (userArray[i].HasValue)
                    if (userId == userArray[i].Value.id)
                        userName = userArray[i].Value.name;

            return userName;
        }

        public void AsyncAddList(string str)
        {

            chatList.AddItem(str);
        }

        public void AsyncUserGet(int idx, string name)
        {
            userPanels[idx].SetUser(name);
        }

        public void AsyncUserDelList(Godot.Collections.Array<int> toDelList)
        {
            foreach (var i in toDelList)
            {
                userArray[i] = null;
                userPanels[i].LostUser();
            }
        }

        public void AsyncUserReady(int idx, bool isReady)
        {
            userPanels[idx].SetReady(isReady);

        }









    }
}


public class UserRoomData
{
    public enum CharCode {
        NONE
    }
    public CharCode charCode = CharCode.NONE;


    public static UserRoomData Parse(string str)
    {
        //TODO

        return new();
    }

    public override string ToString()
    {
        string str = base.ToString();

        //TODO

        return str;
    }

}