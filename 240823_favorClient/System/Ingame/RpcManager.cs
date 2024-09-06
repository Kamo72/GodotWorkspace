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

namespace _favorClient.System.Ingame
{

    public partial class RpcManager : Node
    {
        static RpcManager() 
        {
            _ = GetIp();
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


        static int _port = 8910;
        static string _ip;

        private ENetMultiplayerPeer _peer;
        private int _playerId;
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
            GD.Print("Connection FAILED.");
            GD.Print("Could not connect to server.");
        }

        //Connection Succeed Event
        void ConnectionSuccessful()
        {
            GD.Print("Connection SUCCESSFULL.");

            GD.Print($"{_playerId} : Sending player information to server.");
            GD.Print($"{_playerId} : Id: {_playerId}");

            //userBox.AddItem(Multiplayer.GetUniqueId().ToString());

            //Send PlayerInfo to Host(1)
            UserStatus uStat = userStatus().Value;
            uStat.rpcId = Multiplayer.GetUniqueId();
            
            RpcId(1, "SendPlayerInformation", uStat.ToString());
        }

        //Connection Player Event
        void PlayerConnected(long id)
        {
            GD.Print($"{_playerId} : Player <{id}> connected.");
            //userBox.AddItem(id.ToString());
        }

        //Disconnection Player Event
        void PlayerDisconnected(long id)
        {
            GD.Print($"{_playerId} : Player <{id}> disconnected.");

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
            _peer = new ENetMultiplayerPeer();
            var status = _peer.CreateClient(ip, port);
            if (status != Error.Ok)
            {
                GD.Print("Creating client FAILED.");
                return false;
            }

            _peer.Host.Compress(ENetConnection.CompressionMode.RangeCoder);
            Multiplayer.MultiplayerPeer = _peer;
            return true;
        }

        //Set Mutiplayer as Host
        public bool DoHost()
        {
            _peer = new ENetMultiplayerPeer();
            var status = _peer.CreateServer(_port, 3);
            if (status != Error.Ok)
            {
                GD.Print("Creating server FAILED : " + status);
                return false;
            }

            _peer.Host.Compress(ENetConnection.CompressionMode.RangeCoder);
            Multiplayer.MultiplayerPeer = _peer;

            SendPlayerInformation(new UserStatus().ToString());
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
            //foreach (var player in IngameManager.players)
            //{
            //    GD.Print($"{player.Name}({player.Id}) is Playing");
            //}

            var packedScene = ResourceLoader.Load<PackedScene>("res://Scenes/Ingame.tscn");
            var scene = packedScene.Instantiate<Node2D>();
            GetTree().Root.AddChild(scene);
            // this.Hide();
        }

        //RPC Send Players Info
        [Rpc(MultiplayerApi.RpcMode.AnyPeer)]
        private void SendPlayerInformation(string packet)
        {
            //Create PlayerInfo
            UserStatus? playerInfo = UserStatus.Parse(packet);

            //Add PlayerInfo to Collection
            if (!IngameManager.players.Contains(playerInfo))
                IngameManager.players[playerInfo.Value.idx] = playerInfo;

            //If Server, BroadCast PlayerInfo
            if (Multiplayer.IsServer())
                foreach (var player in IngameManager.players)
                    Rpc("SendPlayerInformation", player.ToString());

            foreach (var player in IngameManager.players)
                if (player.HasValue)
                    GD.Print($"[{player.Value.idx}] NAME : \t{player.Value.name}\tRPC : {player.Value.rpcId}\tTYPE : {player.Value.type}");


        }
    }

}