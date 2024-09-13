using Godot;
using _favorClient.controls.Inroom;
using _favorClient.library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using _favorClient.library.DataType;
using _favorClient.System.Ingame;
using static Godot.Projection;

namespace _favorClient.controls
{
    public partial class InroomInterface : UserInterface
    {
        public static InroomInterface instance = null;


        [Export]
        public Godot.Collections.Array<InroomUserPanel> userPanels = new();

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

        public static string roomName = "";
        public static string userId = "";
        public string userName
        {
            get
            {
                for (int i = 0; i < 4; i++)
                    if (userArray[i].HasValue)
                        if (userId == userArray[i].Value.id)
                            return userArray[i].Value.name;
                return "";
            }
        }
        public int userIdx { get {

                for (int i = 0; i < userArray.Length; i++)
                    if (userArray[i].HasValue)
                        if (userArray[i].Value.id == userId)
                            return i;
                        
                return -1;
            } }

        public (string name, string id, UserStatus? urd)?[] userArray
            = new (string name, string id, UserStatus? urd)?[4] { null, null, null, null };
        public int usersCount
        {
            get
            {
                int count = 0;
                foreach (var item in userArray)
                    if (item.HasValue) count++;
                return count;
            }
        }
        UserStatus userStatus;
        bool isReady = false;

        Action requestDisposer;
        List<Action> disposerCollecter = new();
        public override void _Ready()
        {
            instance = this;

            Action tAct;
            nameTxt.Text = roomName;
            userStatus = new UserStatus(userId, userName, userIdx, -1, CharacterData.Type.NONE);

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
            tAct = MainClient.instance.AddPacketListener(Packet.Flag.ROOM_READY_RECV, packet =>
            {
                int tIdx = int.Parse(packet.value[0].ToString());
                bool isReady = bool.Parse(packet.value[1].ToString());

                CallDeferred("AsyncUserReady", tIdx - 1, isReady);
            });
            disposerCollecter.Add(tAct);

            //ROOM_USERS에 대한 처리
            tAct = MainClient.instance.AddPacketListener(Packet.Flag.ROOM_USERS, packet =>
            {
                //GD.Print($"[userId : {userId}]");
                //for (int i = 0; i < 4; i++) 
                //{
                //    GD.Print($"[{i}] : {(userArray[i].HasValue? userArray[i].Value : null)}");
                //}

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
            disposerCollecter.Add(tAct);

            chatList.Draw += () => chatList.GetVScrollBar().Value = 9999;

            //ROOM_CHAT_RECV 대한 처리
            tAct = MainClient.instance.AddPacketListener(Packet.Flag.ROOM_CHAT_RECV, packet =>
            {
                string sender = packet.value[0].ToString();
                string message = packet.value[1].ToString();

                CallDeferred("AsyncAddChat", $"[{sender}] {message}");
            });
            disposerCollecter.Add(tAct);

            //ROOM_CHAT_SEND 전송
            chatBtn.Pressed += () => {
                if (chatTxt.Text != "")
                {
                    Packet packet = new Packet(Packet.Flag.ROOM_CHAT_SEND, userName, chatTxt.Text);
                    chatTxt.Text = "";
                    MainClient.instance.Send(packet);
                }
            };

            //ROOM_STATUS_RECV 대한 처리
            tAct = MainClient.instance.AddPacketListener(Packet.Flag.ROOM_STATUS_RECV, packet =>
            {
                int idx = int.Parse(packet.value[0].ToString());
                UserStatus uStat = UserStatus.Parse(packet.value[1].ToString());

                var panel = userPanels[idx];
                panel.SetUserStatus(uStat);
            });
            disposerCollecter.Add(tAct);

            //ROOM_COUNTDOWN 대한 처리
            tAct = MainClient.instance.AddPacketListener(Packet.Flag.ROOM_COUNTDOWN, packet =>
            {
                int countdown = int.Parse(packet.value[0].ToString());

                if (countdown == 0)
                {
                    CallDeferred("AsyncAddChat", $"곧 게임이 시작됩니다!");
                    return;
                }
                else if (countdown == -1)
                {
                    CallDeferred("AsyncAddChat", $"게임 시작이 취소되었습니다.");
                    return;

                }

                CallDeferred("AsyncAddChat", $"게임 시작까지 {countdown}초...");
            });
            disposerCollecter.Add(tAct);

            //ROOM_START 대한 처리
            tAct = MainClient.instance.AddPacketListener(Packet.Flag.ROOM_START, packet =>
            {
                chatBtn.SetDeferred(Button.PropertyName.Disabled, true);
                exitBtn.SetDeferred(Button.PropertyName.Disabled, true);
                readyBtn.SetDeferred(Button.PropertyName.Disabled, true);

                int hostIdx = int.Parse(packet.value[0].ToString()) - 1;

                if (userIdx == -1) throw new Exception("myIdx가 -1...????????");

                bool isHost = hostIdx == userIdx;
                
                //RPC 연결 시작
                if (isHost) CallDeferred("AsyncRpcHost");

            });
            disposerCollecter.Add(tAct);

            chatBtn.Disabled = false;
            exitBtn.Disabled = false;
            readyBtn.Disabled = false;

            //ROOM_RPC_RECV 대한 처리
            tAct = MainClient.instance.AddPacketListener(Packet.Flag.ROOM_RPC_RECV, packet =>
            {

                string ip = packet.value[0].ToString();
                int port = int.Parse(packet.value[1].ToString());

                //RPC 연결 시작
                CallDeferred("AsyncRpcJoin", ip, port);

            });
            disposerCollecter.Add(tAct);


            //userPanels의 isMy를 세팅
            for (int i = 0; i < 4; i++)
            {
                //userPanels[i].isChangable = () => i == userIdx && !isReady;
                userPanels[i].prevCharBtn.Pressed += OnPressPrevChar;
                userPanels[i].nextCharBtn.Pressed += OnPressNextChar;
            }
        }


        void BroadcastUserStatus()
        {
            Packet packet = new Packet(Packet.Flag.ROOM_STATUS_SEND, userStatus);
            MainClient.instance.Send(packet);
        }
        void OnUserStatusChanged() 
        {
            userPanels[userIdx].SetUserStatus(userStatus);
            BroadcastUserStatus();
        }

        void OnPressNextChar()
        {
            int nowIdx = (int)userStatus.type;
            int maxIdx = CharacterData.typeCount;
            int newIdx = (nowIdx + 1) >= maxIdx ? 0 : (nowIdx + 1);

            userStatus.type = (CharacterData.Type)newIdx;
            userStatus.traitTree = new TraitTree((CharacterData.Type)newIdx);

            OnUserStatusChanged();
        }
        void OnPressPrevChar()
        {
            int nowIdx = (int)userStatus.type;
            int maxIdx = CharacterData.typeCount;
            int newIdx = (nowIdx - 1) <= -1 ? maxIdx-1 : (nowIdx - 1);

            userStatus.type = (CharacterData.Type)newIdx;
            userStatus.traitTree = new TraitTree((CharacterData.Type)newIdx);

            OnUserStatusChanged();
        }

        public void AsyncAddChat(string str)
        {
            chatList.AddItem(str);
            chatList.GetVScrollBar().Value = 9999;
        }
        public void AsyncUserReady(int idx, bool isReady)
        {
            userPanels[idx].SetReady(isReady);
        }
        
        public void AsyncUserGet(int idx, string name)
        {
            if (idx == userIdx)
            {
                userStatus.idx = idx;
                userArray[idx] = (userName, userId, userStatus);
            }

            userPanels[idx-1].SetUser(name);
            BroadcastUserStatus();
        }
        public void AsyncUserDelList(Godot.Collections.Array<int> toDelList)
        {
            foreach (var i in toDelList)
            {
                userArray[i] = null;
                userPanels[i].LostUser();
            }
        }

        public void AsyncRpcHost()
        {
            RpcManager rpcM = GetNode("../RpcManager") as RpcManager;

            if (rpcM == null) throw new Exception("Rpc Manager를 찾지 못했습니다.");

            rpcM.userStatus = () => userStatus;
            bool res = rpcM.DoHost();

            if (res == false) throw new Exception("Rpc Host 중 문제가 발생했습니다.");

            var address = rpcM.GetIpAddress();

            Packet sPacket = new Packet(Packet.Flag.ROOM_RPC_SEND, address.ip, address.port);
            MainClient.instance.Send(sPacket);
            GD.PushWarning("Rpc Host에 성공했습니다.");


        }
        public void AsyncRpcJoin(string ip, int port)
        {
            RpcManager rpcM = GetNode("../RpcManager") as RpcManager;

            if (rpcM == null) throw new Exception("Rpc Manager를 찾지 못했습니다.");

            rpcM.userStatus = () => userStatus;
            bool res = rpcM.DoJoin(ip, port);

            if (res == false) throw new Exception("Rpc Join 중 문제가 발생했습니다.");


            GD.PushWarning("Rpc Join에 성공했습니다.");

        }
    }
}
