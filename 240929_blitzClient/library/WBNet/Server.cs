using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace _231018_WBNET
{

    public class Server
    {
        private Socket server;

        private List<Socket> socketList = new List<Socket>();

        private readonly int port;

        private ServerRecvDel recvDel;

        private ServerLogDel logDel;

        public event Action<Socket> onDisconnect;

        private Thread runThread;

        public Server(int port, ServerRecvDel recvDel, ServerLogDel logDel)
        {
            this.port = port;
            this.recvDel = recvDel;
            this.logDel = logDel;
        }

        public bool Start()
        {
            try
            {
                server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                IPEndPoint localEP = new IPEndPoint(IPAddress.Any, port);
                server.Bind(localEP);
                server.Listen(20);
                runThread = new Thread(ThreadRun);
                runThread.Start();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                Console.ReadKey();
                return false;
            }
        }

        public void Stop()
        {
            try
            {
                server.Close();
                runThread.Abort();
                logDel(server, LogTypes.STOP, "서버 종료");
            }
            catch (Exception ex)
            {
                logDel(server, LogTypes.STOP, "오류 : " + ex.ToString());
            }
        }

        public void ThreadRun()
        {
            Console.WriteLine("서버 시작... 클라이언트 접속 대기 중...");
            IPEndPoint iPEndPoint = (IPEndPoint)server.LocalEndPoint;
            string msg = $"내 주소! : {iPEndPoint.Address} : {iPEndPoint.Port}";
            logDel(server, LogTypes.START, msg);
            while (true)
            {
                try
                {
                    Socket socket = server.Accept();
                    IPEndPoint iPEndPoint2 = (IPEndPoint)socket.LocalEndPoint;
                    msg = $"상대 주소! : {iPEndPoint2.Address} : {iPEndPoint2.Port}";
                    logDel(socket, LogTypes.CONNECT, msg);
                    Thread thread = new Thread(ThreadWork);
                    thread.Start(socket);
                    thread.IsBackground = true;
                    socketList.Add(socket);
                }
                catch (Exception ex)
                {
                    //logDel(server, LogTypes.CONNECT, "에러 : " + ex.ToString());
                }
            }
        }

        public void ThreadWork(object obj)
        {
            Socket socket = obj as Socket;
            while (true)
            {
                try
                {
                    byte[] data = null;
                    if (!ReceiveData(socket, ref data))
                    {
                        throw new Exception("ReceiveData failed");
                    }

                    string @string = Encoding.UTF8.GetString(data);
                    if (@string == "") throw new Exception("ReceiveData is empty");
                    recvDel(socket, @string);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    socketList.Remove(socket);
                    socket.Close();
                    return;
                }
            }
        }

        public bool SendDataAll(string msg, Socket client, bool isuser)
        {
            if (isuser)
            {
                foreach (Socket socket in socketList)
                {
                    SendData(socket, msg);
                }
            }
            else
            {
                foreach (Socket socket2 in socketList)
                {
                    if (client != socket2)
                    {
                        SendData(socket2, msg);
                    }
                }
            }

            return true;
        }

        public bool SendData(Socket client, string msg)
        {
            try
            {
                byte[] bytes = Encoding.UTF8.GetBytes(msg);
                SendData(client, bytes);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private void SendData(Socket client, byte[] data)
        {
            try
            {
                int num = 0;
                int num2 = data.Length;
                int num3 = num2;
                int num4 = 0;
                byte[] array = new byte[4];
                array = BitConverter.GetBytes(num2);
                num4 = client.Send(array);
                while (num < num2)
                {
                    num4 = client.Send(data, num, num3, SocketFlags.None);
                    num += num4;
                    num3 -= num4;
                }

                logDel(client, LogTypes.SEND, "메세지 전송됨 " + Encoding.UTF8.GetString(data));
            }
            catch (Exception ex)
            {
                logDel(client, LogTypes.SEND, "오류 : " + ex.ToString());
            }
        }

        private bool ReceiveData(Socket client, ref byte[] data)
        {
            try
            {
                int num = 0;
                int num2 = 0;
                int num3 = 0;
                int num4 = 0;
                byte[] array = new byte[4];
                num4 = client.Receive(array, 0, 4, SocketFlags.None);
                num2 = BitConverter.ToInt32(array, 0);
                num3 = num2;
                data = new byte[num2];
                while (num < num2)
                {
                    num4 = client.Receive(data, num, num3, SocketFlags.None);
                    if (num4 == 0)
                    {
                        break;
                    }

                    num += num4;
                    num3 -= num4;
                }

                logDel(client, LogTypes.RECV, "메세지 수령 : " + Encoding.UTF8.GetString(data));
                return true;
            }
            catch (Exception ex)
            {
                logDel(client, LogTypes.DISCONNECT, "오류 : " + ex);
                if (onDisconnect != null) onDisconnect(client);
                socketList.Remove(client);
                client.Close();
                return false;
            }
        }
    }
}