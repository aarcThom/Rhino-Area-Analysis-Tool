using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AreaAnalysis.Classes
{
    public class tableObject
    {
        // string properties
        public string roomName { get; set; }
        public string roomType { get; set; }
        public string roomID { get; set; }

        // guid
        public Guid rhinoGUID { get; set; }

        // dimensional properties
        public float CurrentArea { get; set; }
        public float roomHeight { get; set; }

        // location properties
        public int floor { get; set; } = 0;
        public int roomNumber { get; set; }

        // target properties
        public float targetArea { get; set; }

    }
}
