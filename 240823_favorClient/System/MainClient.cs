using Godot;
using System;
using _231018_WBNET;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Reflection.Metadata.Ecma335;
using System.Threading;

public partial class MainClient : Node
{
    public static MainClient instance = null;

    public event Action onDisconnect;
    public event Action onConnect;
    List<(Packet.Flag, Action<Packet>)> packetListeners = new List<(Packet.Flag, Action<Packet>)>();
    List<Action> listenerFreeQueue = new List<Action> ();

    public int ping = 9999;
    public bool pingMustRecv = false;
    public int state = 0;

    Client client;

    string ip = "127.0.0.1";//"209.38.25.83";
    int port = 8125;

    DateTime pingSent = DateTime.UtcNow;

    public override void _Ready()
	{
		client = new Client(ip, port, RecvDel, LogDel);
        instance = this;

        TryConnect();
    }
    public override void _Process(double delta)
	{
        PingProcess((float)delta);
    }

    float pingTimer = 0f;
    void PingProcess(float delta) 
    {
        if (state == 2)
        {
            conTryDelay -= delta;
            if (conTryDelay < 0f)
            {
                conTryDelay = 999f;
                TryConnect();
            }
        }
        else if(state == 1)
        {
            pingTimer += delta;
            if (pingMustRecv == false)
            {
                if (pingTimer > 1f)
                {
                    pingTimer  = 0f;
                    Packet packet = new Packet(Packet.Flag.PING, DateTime.UtcNow);
                    Send(packet);
                    pingMustRecv = true;
                }
            }
            else if (pingTimer > 5f)
            {
                GD.Print("connection delayed too long!");
                conTryDelay = 5f;
                state = 2;
                onDisconnect?.Invoke();
                pingMustRecv = false;
            }



        }

    }

    public float conTryDelay = 999f;
    bool TryConnect() 
    {
        bool isSucceed = client.Start();
        state = isSucceed ? 1 : 2;
        if (isSucceed == false)
        {
            onConnect?.Invoke();
            conTryDelay = 5f;
        }
        else
        {
            pingSent = DateTime.UtcNow;
            Packet packet = new Packet(Packet.Flag.PING, DateTime.UtcNow);
            Send(packet);
            onDisconnect?.Invoke();
        }

        return isSucceed;
    }
    
	void RecvDel(string msg)
    {
        try
        {
            if (msg == "") throw new Exception("Exception : packet is empty!");
            Packet packet = Packet.FromString(msg);

            foreach (var item in listenerFreeQueue)
                item.Invoke();
            listenerFreeQueue.Clear();

            foreach (var item in packetListeners) 
                if (packet.flag == item.Item1) 
                    item.Item2(packet);
            

            switch (packet.flag) 
            {
                case Packet.Flag.PONG:{
                    DateTime dt = (DateTime)packet.value[0];
                    ping = 0;
                    ping += (DateTime.UtcNow.Second - dt.Second) * 1000;
                    ping += (DateTime.UtcNow.Millisecond - dt.Millisecond);
                    pingMustRecv = false;
                } break;
            
            }
        }
        catch (Exception ex)
        {
            GD.Print(ex.ToString());
            conTryDelay = 5f;
            state = 2;
            onDisconnect?.Invoke();
            pingMustRecv = false;
            client.Stop();
        }

    }
	void LogDel(LogTypes type, string msg)
    {
        string str = "";

        if (msg.Contains("0\n2024")) return;
        if (msg.Contains("1\n2024")) return;
        //if (msg.Contains("11\n")) return;

        switch (type)
        {
            case LogTypes.START:
                str +=("[START]"); break;
            case LogTypes.STOP:
                str += ("[STOP]"); break;

            case LogTypes.SEND:
                str += ("[SEND]"); break;
            case LogTypes.RECV:
                str += ("[RECV]"); break;

            case LogTypes.CONNECT:
                str += ("[CONNECT]"); break;
            case LogTypes.DISCONNECT:
                str += ("[DISCONNECT]"); break;
        }
        str += " : " + msg;
        GD.Print(str);

        if (type == LogTypes.DISCONNECT) {
            conTryDelay = 5f;
            state = 2;
            onDisconnect?.Invoke();
            pingMustRecv = false;
        }
    }
    public void Send(Packet packet) 
    {
        client.SendData(packet.ToString());
    }
    public Action AddPacketListener(Packet.Flag flag, Action<Packet> action) 
    {
        var item = (flag, action);
        packetListeners.Add(item);

        Action disposer = () => listenerFreeQueue.Add(()=> packetListeners.Remove(item));
        return disposer;
    }

    protected override void Dispose(bool disposing)
    {
        Packet packet = new Packet(Packet.Flag.NET_CRASH);
        Send(packet);
        Thread.Sleep(100);
        client.Stop();
        base.Dispose(disposing);
    }
}
