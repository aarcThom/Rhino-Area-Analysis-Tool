using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace AreaAnalysis.Classes
{
    public class RoomMethods
    {
        public static List<Room> CreateEmptyRoomList(int values) 
        { 
            List<Room> roomList = new List<Room>();
            for (int i = 0; i < values; i++) {
                Room emptyRoom = new Room();
                roomList.Add(emptyRoom);
            }
        return roomList;
        }
    }


}
