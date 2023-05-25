using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AreaAnalysis.Classes
{
    public class Room
    {
        public string roomName { get; set; }
        public string roomType { get; set; }
        public float curArea { get; set; }

        public float reqArea { get; set; }
        public int floor { get; set; }


        //constructor
        public Room(string initRoom = "undefined", string initType = "undefined", float initcurArea = 0, float initreqArea = 0, int initFloor = 0)
        {
            roomName = initRoom;
            roomType = initType;
            curArea = initcurArea;
            reqArea = initreqArea;
            floor = initFloor;
        }
    }
}
