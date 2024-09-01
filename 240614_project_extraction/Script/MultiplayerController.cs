using Godot;
using Godot.Collections;
using System;
using System.Linq;

public partial class MultiplayerController : Control
{
    [Export]
    private Button hostButton;
    [Export]
    private Button joinButton;
    [Export]
    private Button startButton;
    [Export]
    private LineEdit nameInsert;


    [Export]
    private Button sendButton;
    [Export]
    private LineEdit textInsert;

    [Export]
    private ItemList chatBox;
    [Export]
    private ItemList userBox;

    [Export]
    private int _port = 8910;
    [Export]
    private string _ip = "209.38.25.83";

    private ENetMultiplayerPeer _peer;
    private int _playerId;

    public override void _Ready()
    {
        Multiplayer.PeerConnected += PlayerConnected;
        Multiplayer.PeerDisconnected += PlayerDisconnected;
        Multiplayer.ConnectedToServer += ConnectionSuccessful;
        Multiplayer.ConnectionFailed += ConnectionFailed;
        if (OS.GetCmdlineArgs().Contains("--server")) 
        {
            HostGame();
        }

        joinButton.Pressed += OnJoinPressed;
        hostButton.Pressed += OnHostPressed;
        sendButton.Pressed += OnSendPressed;
        startButton.Pressed += OnStartPressed;
        chatBox.Draw += () => chatBox.GetVScrollBar().Value = 200000;
    }

    private void ConnectionFailed()
    {
        chatBox.AddItem("Connection FAILED.");
        chatBox.AddItem("Could not connect to server.");
    }

    private void ConnectionSuccessful()
    {
        chatBox.AddItem("Connection SUCCESSFULL.");


        chatBox.AddItem($"{_playerId} : Sending player information to server.");
        chatBox.AddItem($"{_playerId} : Id: {_playerId}");

        userBox.AddItem(Multiplayer.GetUniqueId().ToString());
        RpcId(1, "SendPlayerInformation", nameInsert.Text, Multiplayer.GetUniqueId());
    }

    private void PlayerConnected(long id)
    {
        chatBox.AddItem($"{_playerId} : Player <{id}> connected.");
        userBox.AddItem(id.ToString());
    }
    
    private void PlayerDisconnected(long id)
    {
        chatBox.AddItem($"{_playerId} : Player <{id}> disconnected.");
        GameManager.players.Remove(GameManager.players.Where(i => i.Id == id).First<PlayerInfo>());
        var players = GetTree().GetNodesInGroup("Player");

        foreach (var item in players)
        {
            if (item.Name == id.ToString()) 
            {
                item.QueueFree();
            }
            
        }

        for (int i = 0; i < userBox.ItemCount; i++)
        {
            string rawText = userBox.GetItemText(i);
            int idIn = int.Parse(rawText);
            if (id == idIn)
            {
                userBox.RemoveItem(i);
                break;
            }
        }

    }


    public void OnHostPressed()
    {
        CreateServer();
    }

    public void OnJoinPressed()
    {
        ConnectToServer();
    }

    public void OnSendPressed()
    {

        if (textInsert.Text == "") return;

        chatBox.AddItem(textInsert.Text);
        Rpc("SendChat", textInsert.Text);
        textInsert.Text = "";

    }

    public void OnStartPressed() {
        Rpc("StartGame");
    }


    private void HostGame()
    {
        _peer = new ENetMultiplayerPeer();
        var status = _peer.CreateServer(_port, 2);
        if (status != Error.Ok)
        {
            chatBox.AddItem("Creating server FAILED : " + status);
            return;
        }

        _peer.Host.Compress(ENetConnection.CompressionMode.RangeCoder);
        Multiplayer.MultiplayerPeer = _peer;
    }

    public void CreateServer()
    {
        HostGame();
        SendPlayerInformation(nameInsert.Text, 1);
        chatBox.AddItem("Creating server SUCCEED.");
        userBox.AddItem(Multiplayer.GetUniqueId().ToString());
    }

    public void ConnectToServer()
    {
        _peer = new ENetMultiplayerPeer();
        var status = _peer.CreateClient(_ip, _port);
        if (status != Error.Ok)
        {
            chatBox.AddItem("Creating client FAILED.");
            return;
        }

        _peer.Host.Compress(ENetConnection.CompressionMode.RangeCoder);
        Multiplayer.MultiplayerPeer = _peer;
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
    }

    [Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal =false, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable)]
    private void SendChat(string msg) 
    {
        chatBox.AddItem(msg);
    }

    [Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable)]
    private void StartGame()
    {
        foreach (var player in GameManager.players) {
            GD.Print($"{player.Name}({player.Id}) is Playing");
        }

        var packedScene = ResourceLoader.Load<PackedScene>("res://Scenes/Ingame.tscn");
        var scene = packedScene.Instantiate<Node2D>();
        GetTree().Root.AddChild(scene);
        this.Hide();
    }

    [Rpc(MultiplayerApi.RpcMode.AnyPeer)]
    private void SendPlayerInformation(string name, int id) 
    {
        PlayerInfo playerInfo = new PlayerInfo()
        {
            Name = name,
            Id = id
        };

        if (!GameManager.players.Contains(playerInfo))
            GameManager.players.Add(playerInfo);
        

        if (Multiplayer.IsServer()) 
            foreach (var player in GameManager.players)
                Rpc("SendPlayerInformation", player.Name, player.Id);

    }
}