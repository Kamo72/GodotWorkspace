using _240823_favorServer.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace _240823_favorServer.Data
{
    public class User : IDisposable
    {
        public User(Socket socket, string id, string name) 
        {
            this.socket = socket;
            this.id = id;
            this.name = name;
        }
        public Socket socket;
        public string id;
        public string name;
        public bool isReady = false;

        public void Send(Packet packet) 
        {
            MainServer.GetInstance().Send(socket, packet);
        }

        public Room? room = null;

        public bool isInRoom => room != null;
        public int? roomIdx => isInRoom && room != null ? room.GetUserIdx(this) : -1;


        public void Dispose()
        {
            if (isInRoom) room?.UserExit(this);
            UserManager.GetInstance().SignOut(this);
        }

        public override string ToString()
        {
            return $"{id}\t{name} - room : {(isInRoom? room.name : "X")}";
        }
    }
}
