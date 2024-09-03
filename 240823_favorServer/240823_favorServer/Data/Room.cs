using _240823_favorServer.Library.DataType;
using _240823_favorServer.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Timers;
using Timer = System.Timers.Timer;

namespace _240823_favorServer.Data
{
    public class Room : IDisposable
    {
        public Room(int idx, string name = "TestRoom", bool isPw = false, string pw = "") 
        {
            this.idx = idx;
            this.name = name;
            this.isPw = isPw;
            this.pw = pw;

            Refreshtimer = new Timer(1000);
            Refreshtimer.Elapsed += BroadcastUsers;
            Refreshtimer.Start();
        }

        public int idx;
        public string name;
        public bool isPw;
        public string pw;
        public State playState = State.NONE;

        public Timer Refreshtimer;

        public const int userMax = 4;
        public int host = 0;
        public int userCount =>
            (users[0] != null ? 1 : 0) +
            (users[1] != null ? 1 : 0) +
            (users[2] != null ? 1 : 0) +
            (users[3] != null ? 1 : 0);

        User[] users = new User[userMax];
        

        public bool UserEnter(User user, string pw = "")
        {
            if (user.isInRoom == true) throw new Exception();
            if (userCount == userMax)  throw new Exception();
            if (users.Contains(user))  throw new Exception();
            if (isPw && pw != this.pw) throw new Exception();

            for (int i = 0; i < userMax; i++) 
                if (users[i] == null)
                {
                    users[i] = user;
                    user.room = this;
                    user.room = this;
                    user.isReady = true;
                    return true;
                }
            

            return false;
        }

        public bool UserExit(User user)
        {
            if (user.isInRoom == false) return false;
            if (userCount == 0) return false;
            if (!users.Contains(user)) return false;

            for (int i = 0; i < userMax; i++)
            {
                if (users[i] == user)
                {
                    users[i] = null;
                    user.room = null;
                    user.isReady = false;
    
                    //더이상 사람이 없을 경우 방 파괴
                    if (userCount == 0)
                        Dispose();

                    //호스트 이전 코드
                    else if (i == host)
                        for (int t = 0; t < userMax; t++)
                            if (users[t] != null)
                            {
                                host = t;
                                break;
                            }

                    return true;
                }
            }

            return false;
        }

        public int GetUserIdx(User user)
        {
            if (user.isInRoom == false) return -1;
            if (userCount == 0) return -1;
            if (!users.Contains(user)) return -1;

            for (int i = 0; i < userMax; i++)
            {
                if (users[i] == user)
                    return i + 1;
            }

            return -1;
        }

        public User? GetUserByIdx(int idx)
        {
            return users[idx];
        }

        public bool UserExist(User user) {

            //Console.WriteLine(user.ToString());

            return users.Contains(user);
        }

        public (int idx, string name, bool isPw, int state, int userCount) GetRoomData() 
        {
            return (idx, name, isPw, (int)playState, userCount);
        }

        public bool BroadcastChat(User user, string msg)
        {
            if (user.isInRoom == false) return false;
            if (userCount == 0) return false;
            if (!users.Contains(user)) return false;

            Packet packet = new Packet(Packet.Flag.ROOM_CHAT_RECV, user.name, msg);

            for (int i = 0; i < userMax; i++)
                if (users[i] != null)
                    users[i].Send(packet);
               
            return true;
        }

        public bool BroadcastReady(User user, bool isReady)
        {
            if (user.isInRoom == false) return false;
            if (userCount == 0) return false;
            if (!users.Contains(user)) return false;

            
            Packet packet = new Packet(Packet.Flag.ROOM_READY_RECV, GetUserIdx(user), isReady);

            for (int i = 0; i < userMax; i++)
                if (users[i] != null) users[i].Send(packet);

            user.isReady = isReady;
            SetCountdown(IsEveryoneReady());

            return true;
        }

        public bool BroadcastStatus(User user, UserStatus status)
        {
            if (user.isInRoom == false) return false;
            if (userCount == 0) return false;
            if (!users.Contains(user)) return false;

            int userIdx = GetUserIdx(user);

            Packet packet = new Packet(Packet.Flag.ROOM_STATUS_SEND, userIdx, status);

            for (int i = 0; i < userMax; i++)
                if (users[i] != null)
                    users[i].Send(packet);

            return true;
        }

        void BroadcastUsers(object sender, ElapsedEventArgs args) 
        {
            List<(int, string, string)> userDatas = new List<(int, string, string)> ();

            for (int i = 0; i < userMax; i++)
                if (users[i] != null) 
                    userDatas.Add((i + 1, users[i].id, users[i].name));


            Packet packet = new Packet(Packet.Flag.ROOM_USERS, userDatas);

            for (int i = 0; i < userMax; i++)
                if (users[i] != null) users[i].Send(packet);
        }

        public bool BroadcastRpcRecv(User user, string ip, int port)
        {
            if (user.isInRoom == false) return false;
            if (userCount == 0) return false;
            if (!users.Contains(user)) return false;

            int userIdx = GetUserIdx(user);

            Packet packet = new Packet(Packet.Flag.ROOM_RPC_RECV, ip, port);

            for (int i = 0; i < userMax; i++)
                if (users[i] != null && users[i] != user)
                    users[i].Send(packet);

            return true;

        }

        int countdownSec = -1;
        Timer countdownTimer;

        void SetCountdown(bool isTrue)
        {
            if (isTrue && countdownTimer == null)
            {
                countdownSec = 10;
                countdownTimer = new Timer(1000);
                countdownTimer.Elapsed += (s, e) =>
                {
                    if (countdownSec == 0)
                    {
                        RpcConnect();
                        return;
                    }

                    Packet packet = new Packet(Packet.Flag.ROOM_COUNTDOWN, countdownSec--);

                    for (int i = 0; i < userMax; i++)
                        if (users[i] != null)
                            users[i].Send(packet);
                        
                };
                countdownTimer.Start();

            }
            else if (!isTrue && countdownTimer != null)
            {
                Packet packet = new Packet(Packet.Flag.ROOM_COUNTDOWN, -1);

                for (int i = 0; i < userMax; i++)
                    if (users[i] != null)
                        users[i].Send(packet);

                countdownTimer.Stop();
                countdownTimer.Dispose();
                countdownTimer = null;
            }
        }

        bool IsEveryoneReady() 
        {
            foreach (var user in users)
                if (user != null)
                    if (user.isReady == false)
                        return false;
            
            return true;
        }

        void RpcConnect()
        {
            Packet packet = new Packet(Packet.Flag.ROOM_START, host);

            for (int i = 0; i < userMax; i++)
                if (users[i] != null)
                    users[i].Send(packet);

            countdownTimer.Stop();
            countdownTimer.Dispose();
            countdownTimer = null;
        }

        public void Dispose()
        {
            for (int i = 0; i < userMax; i++)
                if (users[i] != null)
                    UserExit(users[i]);

            Refreshtimer.Stop();
            Refreshtimer.Dispose();
            RoomManager.GetInstance().Remove(this);
        }

        public enum State
        {
            NONE,
            READY,
            INITIATE,
            INGAME,
        }

    }
}
