using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace _231018_WBNET
{

    public class Client
    {
        private Socket server;

        private readonly string SERVER_IP;

        private readonly int PORT;

        public ClientRecvDel recvDel { get; set; }

        public ClientLogDel logDel { get; set; }

        public Client(string ip, int port, ClientRecvDel recvDel, ClientLogDel logDel)
        {
            SERVER_IP = ip;
            PORT = port;
            this.recvDel = recvDel;
            this.logDel = logDel;
        }

        public bool Start()
        {
            try
            {
                server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                IPEndPoint remoteEP = new IPEndPoint(IPAddress.Parse(SERVER_IP), PORT);
                server.Connect(remoteEP);
                logDel(LogTypes.CONNECT, "서버 연결 성공");
                Thread thread = new Thread(RecvThread);
                thread.IsBackground = true;
                thread.Start(server);
                Console.WriteLine("서버에 접속...");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return false;
            }
        }

        public void Stop()
        {
            IPEndPoint iPEndPoint = (IPEndPoint)server.RemoteEndPoint;
            logDel(LogTypes.DISCONNECT, "서버 연결 종료 [주소] " + iPEndPoint.Address?.ToString() + " / [포트] " + iPEndPoint.Port);
            server.Close();
        }

        public bool SendData(string msg)
        {
            try
            {
                byte[] bytes = Encoding.UTF8.GetBytes(msg);
                SendData(bytes);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private void SendData(byte[] data)
        {
            try
            {
                int num = 0;
                int num2 = data.Length;
                int num3 = num2;
                int num4 = 0;
                byte[] array = new byte[4];
                array = BitConverter.GetBytes(num2);
                num4 = server.Send(array);
                while (num < num2)
                {
                    num4 = server.Send(data, num, num3, SocketFlags.None);
                    num += num4;
                    num3 -= num4;
                }

                logDel(LogTypes.SEND, "메세지 전송됨 " + Encoding.UTF8.GetString(data));
            }
            catch (Exception ex)
            {
                logDel(LogTypes.DISCONNECT, "오류 : " + ex.ToString());
                server.Close();
            }
        }

        private bool ReceiveData(ref byte[] data)
        {
            try
            {
                int num = 0;
                int num2 = 0;
                int num3 = 0;
                int num4 = 0;
                byte[] array = new byte[4];
                num4 = server.Receive(array, 0, 4, SocketFlags.None);
                num2 = BitConverter.ToInt32(array, 0);
                num3 = num2;
                data = new byte[num2];
                while (num < num2)
                {
                    num4 = server.Receive(data, num, num3, SocketFlags.None);
                    if (num4 == 0)
                    {
                        break;
                    }

                    num += num4;
                    num3 -= num4;
                }

                logDel(LogTypes.RECV, "메세지 수령 : " + Encoding.UTF8.GetString(data));
                return true;
            }
            catch (Exception ex)
            {
                logDel(LogTypes.RECV, "오류 : " + ex);
                return false;
            }
        }

        public void RecvThread(object obj)
        {
            try
            {
                while (true)
                {
                    byte[] data = null;
                    ReceiveData(ref data);
                    string @string = Encoding.UTF8.GetString(data);
                    recvDel(@string);
                }
            }
            catch (Exception value)
            {
                Console.WriteLine(value);
            }
        }
    }
}