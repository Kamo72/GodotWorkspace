using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _240823_favorServer.Data
{
    public class RoomManager
    {
        static RoomManager? instance = null;
        public static RoomManager GetInstance()
        {
            if (instance == null)
                instance = new();
            return instance;
        }


        static List<Room> roomList = new List<Room>();

        public int incremental = 0;
        public bool Host(User user, string name, bool isPw, string pw)
        {
            Room room = new Room(incremental++, name, isPw, pw);
            roomList.Add(room);

            bool res = room.UserEnter(user, pw);
            room.host = room.GetUserIdx(user);
            
            return res;
        }

        public bool RemoveByIdx(int idx) 
        {
            Room room = roomList.Find(r => r.idx == idx);
            if (room == null) return false;

            roomList.Remove(room);
            return true;
        }
        public bool Remove(Room room)
        {
            if (room == null) return false;
            if (roomList.Contains(room) == false) return false;

            roomList.Remove(room);
            return true;
        }

        public Room? GetRoomByIdx(int idx)
        {
            Room? room = roomList.Find(r => r.idx == idx);
            return room;
        }
        public Room? GetRoomByUser(User user)
        {
            Room? room = roomList.Find(r => r.UserExist(user));
            return room;
        }
        public Room? GetAnyRoom() => roomList.Count == 0? null : roomList[0];
        

        public bool Join(User user, Room room, string pw = "") 
        {
            if (room == null) return false;
            if (user == null) return false;
            if (roomList.Contains(room) == false) return false;

            bool res = room.UserEnter(user, pw);
            return res;
        }

        public List<(int idx, string name, bool isPw, int state, int userCount)> GetRoomDataList() 
        {
            var roomDataList = new List<(int idx, string name, bool isPw, int state, int userCount)>();

            foreach (var room in roomList) 
            {
                var data = room.GetRoomData();
                roomDataList.Add(data);
            }

            return roomDataList;
        }





    }
}
