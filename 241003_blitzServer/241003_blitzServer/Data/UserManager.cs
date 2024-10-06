using _241003_blitzServer.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using Timer = System.Timers.Timer;

namespace _241003_blitzServer.Data
{
    public class UserManager
    {
        static UserManager? instance = null;
        public static UserManager GetInstance()
        {
            if (instance == null)
                instance = new();
            return instance;
        }

        private UserManager()
        {
            refreshTimer = new Timer(1000);
            refreshTimer.Elapsed += RefreshFunc;
            refreshTimer.Start();
        }
        ~UserManager() 
        {
            refreshTimer.Stop();
            refreshTimer.Dispose();
        }

        Timer refreshTimer;
        private List<User> userList = new List<User>();


        void RefreshFunc(object sender, ElapsedEventArgs args) 
        {
            List<User> toSignOut = new List<User>();

            foreach (var item in userList)
            {
                bool isFine;

                 try
                {
                    isFine = !(item.socket.Poll(1, SelectMode.SelectRead) && item.socket.Available == 0);
                }
                catch (SocketException) { isFine = false; }
                catch (ObjectDisposedException) { isFine = false; }

                if (isFine == false)
                    toSignOut.Add(item);
            }

            foreach (var item in toSignOut)
                SignOut(item);

            var dataList = RoomManager.GetInstance().GetRoomDataList();
            Packet packet = new Packet(Packet.Flag.ROOM_LIST, dataList);

            foreach (var item in userList)
                if(!item.isInRoom)
                    item.Send(packet);
            
        }


        public User FindById(string id) 
        {
            return userList.Find(i => i.id == id);
        }

        public User FindBySocket(Socket socket)
        {
            return userList.Find(i => i.socket == socket);
        }

        public bool SignIn(Socket socket, string id, string name)
        {
            if (FindBySocket(socket) != null) return false;
            if (FindById(id) != null) return false;

            User user = new User(socket, id, name);
            userList.Add(user);

            return true;
        }

        public bool SignOut(User user)
        {
            if (userList.Contains(user) == false) return false;

            if (user.isInRoom)
                user.room.UserExit(user);
            userList.Remove(user);

            return true;
        }
    }
}
