using _240823_favorServer.System;
using System;
using System.Collections.Generic;
using System.Linq;
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
            Refreshtimer.Elapsed += RefreshFunc;
            Refreshtimer.Start();
        }

        public int idx;
        public string name;
        public bool isPw;
        public string pw;
        public PlayState playState = PlayState.NONE;

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

                    if (userCount == 0)
                        Dispose();
                    else if (i == host) 
                    {
                        //TODO
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

            return true;
        }

        public bool BroadcastStatus(User user, object status)
        {
            //TODO
            return false;
        }

        void RefreshFunc(object sender, ElapsedEventArgs args) 
        {
            List<(int, string, string)> userDatas = new List<(int, string, string)> ();

            for (int i = 0; i < userMax; i++)
                if (users[i] != null) 
                    userDatas.Add((i + 1, users[i].id, users[i].name));


            Packet packet = new Packet(Packet.Flag.ROOM_USERS, userDatas);

            for (int i = 0; i < userMax; i++)
                if (users[i] != null) users[i].Send(packet);
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

        public enum PlayState
        {
            NONE,
            READY,
            INITIATE,
            INGAME,
        }



    }
}
