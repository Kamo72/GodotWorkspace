
using System.Net.Sockets;

namespace _231018_WBNET
{
    public delegate void ClientLogDel(LogTypes logType, string msg);
    public delegate void ClientRecvDel(string msg);

    public delegate void ServerLogDel(Socket socket, LogTypes logType, string msg);
    public delegate void ServerRecvDel(Socket socket, string msg);

    public enum LogTypes
    {
        STOP,
        START,
        CONNECT,
        DISCONNECT,
        RECV,
        SEND
    }
}