using _231018_WBNET;
using _240823_favorServer.Data;
using _240823_favorServer.Library.DataType;
using _240823_favorServer.Library.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using static Packet;

namespace _240823_favorServer.System
{
    public class MainServer
    {
        static MainServer? mainServer = null;
        public static MainServer GetInstance()
        {
            if (mainServer == null)
                mainServer = new();
            return mainServer;
        }
        private Server server;
        int port = 8125;

        private MainServer()
        {
            server = new Server(port, RecvDel, LogDel);
            server.Start();
        }

        public void RecvDel(Socket socket, string msg)
        {
            try
            {
                if (msg == "") return;

                string ipStr = socket.RemoteEndPoint == null ? "unableToKnow" : socket.RemoteEndPoint.AddressFamily.ToString();

                Packet gotPacket = Packet.FromString(msg);

                switch (gotPacket.flag)
                {
                    case Packet.Flag.PING:
                        {
                            Packet sendPacket = new Packet(Packet.Flag.PONG, DateTime.UtcNow);
                            Send(socket, sendPacket);
                        }
                        break;

                    case Packet.Flag.ACCOUNT_SIGNUP:
                        {
                            Packet sendPacket;
                            string id = gotPacket.value[0].ToString();
                            string name = gotPacket.value[1].ToString();
                            string pw = gotPacket.value[2].ToString();

                            var foundUser = SQLiteManager.GetUser(id);

                            if (foundUser.HasValue)
                            {
                                sendPacket = new Packet(Packet.Flag.ACCOUNT_SIGNUP_CALLBACK, false, "이미 같은 아이디로 계정이 존재합니다.");
                                Send(socket, sendPacket);
                                break;
                            }

                            if (id == "" || name == "" || pw == "")
                            {
                                sendPacket = new Packet(Packet.Flag.ACCOUNT_SIGNUP_CALLBACK, false, "아이디, 비밀번호, 닉네임을 비울 수 없습니다.");
                                Send(socket, sendPacket);
                                break;
                            }

                            SQLiteManager.AddUser(id, name, pw);

                            sendPacket = new Packet(Packet.Flag.ACCOUNT_SIGNUP_CALLBACK, true, "계정 생성에 성공했습니다.");
                            Send(socket, sendPacket);
                        }
                        break;
                    case Packet.Flag.ACCOUNT_SIGNIN:
                        {
                            Packet sendPacket;
                            string id = gotPacket.value[0].ToString();
                            string pw = gotPacket.value[1].ToString();

                            var foundUser = SQLiteManager.GetUserByIdAndPw(id, pw);

                            if (id == "" || pw == "")
                            {
                                sendPacket = new Packet(Packet.Flag.ACCOUNT_SIGNIN_CALLBACK, false, "올바르지 않은 아이디 또는 비밀번호입니다.");
                                Send(socket, sendPacket);
                                return;
                            }

                            if (foundUser.HasValue == false)
                            {
                                sendPacket = new Packet(Packet.Flag.ACCOUNT_SIGNIN_CALLBACK, false, "아이디 또는 비밀번호가 잘못됐습니다.");
                                Send(socket, sendPacket);
                                return;
                            }

                            //로그인 성공 처리
                            bool res =  UserManager.GetInstance().SignIn(socket, id, foundUser.Value.name);

                            if (res == false)
                            {
                                sendPacket = new Packet(Packet.Flag.ACCOUNT_SIGNIN_CALLBACK, false, "이미 점유된 아이디 또는 소켓입니다.");
                                Send(socket, sendPacket);
                                return;
                            }



                            sendPacket = new Packet(Packet.Flag.ACCOUNT_SIGNIN_CALLBACK, true, $"로그인에 성공했습니다. 환영합니다. {foundUser.Value.name}");
                            Send(socket, sendPacket);
                        }
                        return;
                    case Packet.Flag.ACCOUNT_SIGNOUT:
                        {
                            Packet sendPacket;

                            User user = UserManager.GetInstance().FindBySocket(socket);

                            if (user == null)
                            {
                                sendPacket = new Packet(Packet.Flag.ACCOUNT_SIGNOUT_CALLBACK, false, "해당 소켓에 연결된 계정이 존재하지 않습니다.");
                                Send(socket, sendPacket);
                                return;
                            }

                            bool res = UserManager.GetInstance().SignOut(user);

                            if (res == false)
                            {
                                sendPacket = new Packet(Packet.Flag.ACCOUNT_SIGNOUT_CALLBACK, false, "로그아웃에 실패했습니다.");
                                Send(socket, sendPacket);
                                return;
                            }

                            sendPacket = new Packet(Packet.Flag.ACCOUNT_SIGNOUT_CALLBACK, true, "로그아웃에 성공했습니다.");
                            Send(socket, sendPacket);
                        }
                        return;

                    case Packet.Flag.ROOM_HOST: {

                            Packet sendPacket;
                            string roomName = gotPacket.value[0].ToString();
                            bool isPw = bool.Parse(gotPacket.value[1].ToString());
                            string pw = gotPacket.value[2].ToString();

                            User user = UserManager.GetInstance().FindBySocket(socket);

                            if (user == null)
                            {
                                sendPacket = new Packet(Packet.Flag.ROOM_HOST_CALLBACK, false, "해당 소켓에 연결된 계정이 존재하지 않습니다.");
                                Send(socket, sendPacket);
                                return;
                            }

                            if (roomName == "")
                            {
                                sendPacket = new Packet(Packet.Flag.ROOM_HOST_CALLBACK, false, "방 이름은 비워둘 수 없습니다.");
                                Send(socket, sendPacket);
                                return;
                            }

                            if (isPw && pw == "")
                            {
                                sendPacket = new Packet(Packet.Flag.ROOM_HOST_CALLBACK, false, "비밀번호를 비워둘 수 없습니다.");
                                Send(socket, sendPacket);
                                return;
                            }

                            bool res = RoomManager.GetInstance().Host(user, roomName, isPw, pw);

                            if (res == false)
                            {
                                sendPacket = new Packet(Packet.Flag.ROOM_HOST_CALLBACK, false, "방 생성에 실패했습니다.");
                                Send(socket, sendPacket);
                                return;
                            }

                            sendPacket = new Packet(Packet.Flag.ROOM_HOST_CALLBACK, true, "방 생성에 성공했습니다.");
                            Send(socket, sendPacket);
                        } break;
                    case Packet.Flag.ROOM_JOIN:
                        {
                            Packet sendPacket;
                            int roomIdx = int.Parse(gotPacket.value[0].ToString());
                            string pw = gotPacket.value[1].ToString();

                            User user = UserManager.GetInstance().FindBySocket(socket);

                            if (user == null)
                            {
                                sendPacket = new Packet(Packet.Flag.ROOM_JOIN_CALLBACK, false, "해당 소켓에 연결된 계정이 존재하지 않습니다.");
                                Send(socket, sendPacket);
                                return;
                            }

                            Room room = RoomManager.GetInstance().GetRoomByIdx(roomIdx);

                            if (room == null)
                            {
                                sendPacket = new Packet(Packet.Flag.ROOM_JOIN_CALLBACK, false, "해당 방이 존재하지 않습니다.");
                                Send(socket, sendPacket);
                                return;
                            }

                            bool res = RoomManager.GetInstance().Join(user, room,  pw);

                            if (res == false)
                            {
                                sendPacket = new Packet(Packet.Flag.ROOM_JOIN_CALLBACK, false, "방 참가에 실패했습니다.");
                                Send(socket, sendPacket);
                                return;
                            }

                            sendPacket = new Packet(Packet.Flag.ROOM_JOIN_CALLBACK, true, "방 참가에 성공했습니다.");
                            Send(socket, sendPacket);

                        } break;
                    case Packet.Flag.ROOM_EXIT:
                        {
                            Packet sendPacket;

                            User user = UserManager.GetInstance().FindBySocket(socket);

                            if (user == null)
                            {
                                sendPacket = new Packet(Packet.Flag.ROOM_EXIT_CALLBACK, false, "해당 소켓에 연결된 계정이 존재하지 않습니다.");
                                Send(socket, sendPacket);
                                return;
                            }

                            Room room = RoomManager.GetInstance().GetRoomByUser(user);

                            if (room == null)
                            {
                                sendPacket = new Packet(Packet.Flag.ROOM_EXIT_CALLBACK, false, "해당 방을 찾지 못했습니다.");
                                Send(socket, sendPacket);
                                return;
                            }

                            bool res = room.UserExit(user);

                            if (res == false)
                            {
                                sendPacket = new Packet(Packet.Flag.ROOM_EXIT_CALLBACK, false, "방 퇴장에 실패했습니다.");
                                Send(socket, sendPacket);
                                return;
                            }

                            sendPacket = new Packet(Packet.Flag.ROOM_EXIT_CALLBACK, true, "방 퇴장에 성공했습니다.");
                            Send(socket, sendPacket);


                        }
                        break;
                    case Packet.Flag.ROOM_CHAT_SEND:
                        {
                            Packet sendPacket;

                            User user = UserManager.GetInstance().FindBySocket(socket);
                            string content = gotPacket.value[0].ToString();

                            if (user == null)
                            {
                                Console.WriteLine("ROOM_CHAT_SEND ERROR : 해당 소켓에 연결된 계정이 존재하지 않습니다.");
                                return;
                            }

                            Room room = RoomManager.GetInstance().GetRoomByUser(user);

                            if (room == null)
                            {
                                Console.WriteLine("ROOM_CHAT_SEND ERROR : 해당 방을 찾지 못했습니다.");
                                return;
                            }

                            bool res = room.BroadcastChat(user, content);

                            if (res == false)
                            {
                                Console.WriteLine("ROOM_CHAT_SEND ERROR : 메세지 전달에 실패했습니다.");
                                return;
                            }
                        }
                        break;
                    case Packet.Flag.ROOM_READY_SEND:
                    {
                        Packet sendPacket;

                        User user = UserManager.GetInstance().FindBySocket(socket);
                        bool isReady = bool.Parse(gotPacket.value[0].ToString());

                        if (user == null)
                        {
                            Console.WriteLine("ROOM_CHAT_SEND ERROR : 해당 소켓에 연결된 계정이 존재하지 않습니다.");
                            return;
                        }

                        Room room = RoomManager.GetInstance().GetRoomByUser(user);

                        if (room == null)
                        {
                            Console.WriteLine("ROOM_CHAT_SEND ERROR : 해당 방을 찾지 못했습니다.");
                            return;
                        }

                        bool res = room.BroadcastReady(user, isReady);

                        if (res == false)
                        {
                            Console.WriteLine("ROOM_CHAT_SEND ERROR : 준비 패킷 전달에 실패했습니다.");
                            return;
                        }

                    } break;
                    case Packet.Flag.ROOM_STATUS_SEND:
                        {
                            Packet sendPacket;

                            User user = UserManager.GetInstance().FindBySocket(socket);
                            UserStatus status = (UserStatus)gotPacket.value[0];

                            if (user == null)
                            {
                                Console.WriteLine("ROOM_STATUS_SEND ERROR : 해당 소켓에 연결된 계정이 존재하지 않습니다.");
                                return;
                            }

                            Room room = RoomManager.GetInstance().GetRoomByUser(user);

                            if (room == null)
                            {
                                Console.WriteLine("ROOM_STATUS_SEND ERROR : 해당 방을 찾지 못했습니다.");
                                return;
                            }

                            bool res = room.BroadcastStatus(user, status);

                            if (res == false)
                            {
                                Console.WriteLine("ROOM_STATUS_SEND ERROR : 메세지 전달에 실패했습니다.");
                                return;
                            }


                        } break;
                    case Packet.Flag.ROOM_RPC_SEND: {

                            Packet sendPacket;

                            User user = UserManager.GetInstance().FindBySocket(socket);
                            string ip = gotPacket.value[0].ToString();
                            int port = (int)gotPacket.value[1];

                            if (user == null)
                            {
                                Console.WriteLine("ROOM_RPC_SEND ERROR : 해당 소켓에 연결된 계정이 존재하지 않습니다.");
                                return;
                            }

                            Room room = RoomManager.GetInstance().GetRoomByUser(user);

                            if (room == null)
                            {
                                Console.WriteLine("ROOM_RPC_SEND ERROR : 해당 방을 찾지 못했습니다.");
                                return;
                            }

                            bool res = room.BroadcastRpcRecv(user, ip, port);

                            if (res == false)
                            {
                                Console.WriteLine("ROOM_RPC_SEND ERROR : 메세지 전달에 실패했습니다.");
                                return;
                            }

                        }
                        break;
                    
                    case Packet.Flag.DEBUG_FAST_LOGIN: {
                            Packet sendPacket;
                            (string id, string name, string pw)? foundUser;

                            foundUser = SQLiteManager.GetUserByIdAndPw("testid", "testpw");
                            if (foundUser.HasValue)
                                if (UserManager.GetInstance().SignIn(socket, foundUser.Value.id, foundUser.Value.name))
                                {
                                    sendPacket = new Packet(Packet.Flag.DEBUG_FAST_LOGIN_CALLBACK, foundUser.Value.id, foundUser.Value.name);
                                    Send(socket, sendPacket);
                                    return;
                                }

                            foundUser = SQLiteManager.GetUserByIdAndPw("testid2", "testpw");
                            if (foundUser.HasValue)
                                if (UserManager.GetInstance().SignIn(socket, foundUser.Value.id, foundUser.Value.name))
                                {
                                    sendPacket = new Packet(Packet.Flag.DEBUG_FAST_LOGIN_CALLBACK, foundUser.Value.id, foundUser.Value.name);
                                    Send(socket, sendPacket);
                                    return;
                                }

                            foundUser = SQLiteManager.GetUserByIdAndPw("testid3", "testpw");
                            if (foundUser.HasValue)
                                if (UserManager.GetInstance().SignIn(socket, foundUser.Value.id, foundUser.Value.name))
                                {
                                    sendPacket = new Packet(Packet.Flag.DEBUG_FAST_LOGIN_CALLBACK, foundUser.Value.id, foundUser.Value.name);
                                    Send(socket, sendPacket);
                                    return;
                                }

                            //foundUser = SQLiteManager.GetUserByIdAndPw("testid4", "testpw");
                            //if (foundUser.HasValue)
                            //    if (UserManager.GetInstance().SignIn(socket, foundUser.Value.id, foundUser.Value.name))
                            //    {
                            //        sendPacket = new Packet(Packet.Flag.DEBUG_FAST_LOGIN_CALLBACK, foundUser.Value.id, foundUser.Value.name);
                            //        Send(socket, sendPacket);x
                            //        return;
                            //    }


                            sendPacket = new Packet(Packet.Flag.NET_CRASH);
                            Send(socket, sendPacket);
                        } break;

                    case Packet.Flag.DEBUG_FAST_JOIN:
                        {
                            Packet sendPacket;

                            int roomIdx = 0;
                            Room? room = RoomManager.GetInstance().GetRoomByIdx(roomIdx);

                            if (room == null)
                            {
                                RoomManager.GetInstance().Host(UserManager.GetInstance().FindBySocket(socket), "testRoom", false, "");
                            }
                            else {
                                room.UserEnter(UserManager.GetInstance().FindBySocket(socket));
                            }
                            sendPacket = new Packet(Flag.DEBUG_FAST_JOIN_CALLBACK, "testRoom");
                            Send(socket, sendPacket);


                        } break;
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        public void LogDel(Socket socket, LogTypes type, string msg)
        {
            string str = "";
            string ipStr = socket.RemoteEndPoint == null ? "unableToKnow" : socket.RemoteEndPoint.AddressFamily.ToString();


            if (msg.Contains("0\n2024")) return;
            if (msg.Contains("1\n2024")) return;
            if (msg.Contains("11\n")) return;


            Console.Write($"{ipStr} ");
            switch (type)
            {
                case LogTypes.START:
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write("[START]"); break;
                case LogTypes.STOP:
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write("[STOP]"); break;

                case LogTypes.SEND:
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.Write("[SEND]"); break;
                case LogTypes.RECV:
                    Console.ForegroundColor = ConsoleColor.Blue;
                    Console.Write("[RECV]"); break;

                case LogTypes.CONNECT:
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write("[CONNECT]"); break;
                case LogTypes.DISCONNECT:
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write("[DISCONNECT]"); break;
            }

            str += " : " + msg;
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(str);
        }

        public void Send(Socket socket, Packet packet)
        {
            string msg = packet.ToString();
            server.SendData(socket, msg);
        }


    }
}
