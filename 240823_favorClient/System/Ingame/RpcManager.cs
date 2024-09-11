using _favorClient.controls;
using _favorClient.library.DataType;
using Godot;
using Godot.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using HttpClient = System.Net.Http.HttpClient;
using System.Runtime.InteropServices;

namespace _favorClient.System.Ingame
{

    public partial class RpcManager : Node
    {
        static RpcManager() 
        {

            #pragma warning disable CS0162 // 접근할 수 없는 코드가 있습니다.
            if (true)
            {
                GD.PushWarning("Connection mode is LOCAL, aware if you trying to connect public");
                _ip = "127.0.0.1";
            }
            else
            {
                GD.PushWarning("Connection mode is PUBLIC, aware if you trying to debug program");
                _ = GetIp();
            }
            #pragma warning restore CS0162 // 접근할 수 없는 코드가 있습니다.


        }

        public static async Task GetIp() 
        {
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    string externalIp = await client.GetStringAsync("https://api.ipify.org");
                    _ip = externalIp;
                    GD.Print($"External IP: {externalIp}");
                }
                catch (Exception ex)
                {
                    GD.PrintErr($"Error fetching external IP: {ex.Message}");
                }
            }

        }

        //[DllImport("kernel32")]
        //public static extern Int32 GetCurrentProcessId();


        static int _port = 8910;
        static string _ip;

        private ENetMultiplayerPeer _peer;

        public bool isHost = false;
        public Func<UserStatus?> userStatus = () => null;

        public override void _Ready()
        {
            //Set Multiplayer Listeners
            Multiplayer.PeerConnected += PlayerConnected;
            Multiplayer.PeerDisconnected += PlayerDisconnected;
            Multiplayer.ConnectedToServer += ConnectionSuccessful;
            Multiplayer.ConnectionFailed += ConnectionFailed;

        }

        //Connection Failed Event
        void ConnectionFailed()
        {
            GD.PushWarning("Connection FAILED.");
            GD.PushWarning("Could not connect to server longer.");
        }

        //Connection Succeed Event
        void ConnectionSuccessful()
        {
            GD.PushWarning("Connection SUCCESSFULL.");
            GD.PushWarning($"Sending player information to server.");

            //Send PlayerInfo to Host(1)
            UserStatus uStat = userStatus().Value;
            uStat.rpcId = Multiplayer.GetUniqueId();
            uStat.idx = InroomInterface.instance.userIdx;
            uStat.name = InroomInterface.instance.userName;

            GD.PushWarning("userIdx : " + uStat.idx);
            GD.PushWarning("SendPlayerInformation : " + uStat.ToString());
            RpcId(1, "SendPlayerInformation", uStat.ToString());
        }

        //Connection Player Event
        void PlayerConnected(long id)
        {
            GD.PushWarning($"{Multiplayer.GetUniqueId()} : Player <{id}> connected.");
            //userBox.AddItem(id.ToString());
        }

        //Disconnection Player Event
        void PlayerDisconnected(long id)
        {
            GD.PushWarning($"{Multiplayer.GetUniqueId()} : Player <{id}> disconnected.");

            for (int i = 0; i < 4; i++)
                if (IngameManager.players[i].HasValue)
                    if (IngameManager.players[i].Value.rpcId == id)
                    {
                        IngameManager.instance.DespawnChar(i);
                        IngameManager.players[i] = null;
                    }
        }

        //Extract Ip and Port
        public (string ip, int port) GetIpAddress() => (_ip, _port);
        
        //BroadCast StartGame
        public void DoStart()
        {
            Rpc("StartGame");
        }


        //Set Mutiplayer as Client
        public bool DoJoin(string ip, int port)
        {
            GD.PushWarning($"[JOIN]\tIP : {ip}\t PORT : {port}");

            _peer = new ENetMultiplayerPeer();
            var status = _peer.CreateClient(ip, port);
            if (status != Error.Ok)
            {
                GD.PushWarning("Creating client FAILED.");
                return false;
            }

            _peer.Host.Compress(ENetConnection.CompressionMode.RangeCoder);
            Multiplayer.MultiplayerPeer = _peer;
            return true;
        }

        //Set Mutiplayer as Host
        public bool DoHost()
        {
            GD.PushWarning($"[HOST]\tIP : {_ip}\t PORT : {_port}");

            _peer = new ENetMultiplayerPeer();
            var status = _peer.CreateServer(_port, 3);
            if (status != Error.Ok)
            {
                GD.PushWarning("Creating server FAILED : " + status);
                return false;
            }

            _peer.Host.Compress(ENetConnection.CompressionMode.RangeCoder);
            Multiplayer.MultiplayerPeer = _peer;

            UserStatus uStat = userStatus().Value;
            uStat.rpcId = Multiplayer.GetUniqueId();
            uStat.name = InroomInterface.instance.userName;


            GD.PushWarning("userIdx : " + uStat.idx);
            GD.PushWarning("SendPlayerInformation : " + uStat.ToString());
            SendPlayerInformation(uStat.ToString());
            return true;
        }


        //RPC Send Message
        [Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = false, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable)]
        private void SendChat(string msg)
        {
            GD.Print(msg);
        }

        //RPC Initiate Play Game
        [Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable)]
        private void StartGame()
        {
            GD.PushWarning($"START GAME called!");
            var packedScene = ResourceLoader.Load<PackedScene>("res://Scene/Ingame.tscn");
            var scene = packedScene.Instantiate<Node2D>();
            GetTree().Root.AddChild(scene);

            InroomInterface.instance.SetVisible(false);
        }

        //RPC Send Players Info
        [Rpc(MultiplayerApi.RpcMode.AnyPeer)]
        private void SendPlayerInformation(string packet)
        {
            if (packet == "") return;

            //Create PlayerInfo
            UserStatus? playerInfo = UserStatus.Parse(packet);

            GD.PushWarning($"[SendPlayerInformation] [{playerInfo.Value.idx}] NAME : \t{playerInfo.Value.name}\tRPC : {playerInfo.Value.rpcId}\tTYPE : {playerInfo.Value.type}");
            
            //Add PlayerInfo to Collection
            if (!IngameManager.players.Contains(playerInfo))
                IngameManager.players[playerInfo.Value.idx] = playerInfo;


            //view info
            foreach (var player in IngameManager.players)
                if (player.HasValue)
                    GD.Print($"[{player.Value.idx}] NAME : \t{player.Value.name}\tRPC : {player.Value.rpcId}\tTYPE : {player.Value.type}");

            //If Server, BroadCast PlayerInfo
            if (Multiplayer.IsServer())
            {
                foreach (var player in IngameManager.players)
                    Rpc("SendPlayerInformation", player.ToString());

                if (IngameManager.playersCount == InroomInterface.instance.usersCount)
                    Rpc("StartGame");
            }
        }
    }

}