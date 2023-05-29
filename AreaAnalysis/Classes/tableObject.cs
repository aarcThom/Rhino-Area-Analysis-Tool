using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace AreaAnalysis.Classes
{
    public class TableObject : INotifyPropertyChanged
    {
        // PROPERTIES =========================================================
        // string properties
        private string _roomName = "undefined";
        private string _roomType = "undefined";
        private string _roomId = "undefined";

        // guid
        private Guid _rhinoGuid;

        // dimensional properties
        private float _currentArea = 0;
        private float _roomHeight = 0;

        // location properties
        private int _floor = 0;
        private string _roomNumber = "undefined";

        // target properties
        private float _targetArea = 0;

        // PROPERTY CHANGE NOTIFICATION IMPLEMENTATION =============================
        public event PropertyChangedEventHandler PropertyChanged;

        //GETTERS SETTERS W/ PROPERTY CHANGE
        public string RoomName
        {
            get => _roomName;
            set
            {
                _roomName = value;
                OnPropertyChanged();
            }
        }

        public string RoomType
        {
            get => _roomType;
            set
            {
                _roomType = value;
                OnPropertyChanged();
            }
        }

        public string RoomId
        {
            get => _roomId;
            set
            {
                _roomId = value;
                OnPropertyChanged();
            }
        }

        public Guid RhinoGuid
        {
            get => _rhinoGuid;
            set
            {
                _rhinoGuid = value;
                OnPropertyChanged();
            }
        }

        public float CurrentArea
        {
            get => _currentArea;
            set
            {
                _currentArea = value;
                OnPropertyChanged();
            }
        }

        public float RoomHeight
        {
            get => _roomHeight;
            set
            {
                _roomHeight = value;
                OnPropertyChanged();
            }
        }

        public int Floor
        {
            get => _floor;
            set
            {
                _floor = value;
                OnPropertyChanged();
            }
        }

        public string RoomNumber
        {
            get => _roomNumber;
            set
            {
                _roomNumber = value;
                OnPropertyChanged();
            }
        }

        public float TargetArea
        {
            get => _targetArea;
            set
            {
                _targetArea = value;
                OnPropertyChanged();
            }
        }


        //Property change method to raise the event ========================================
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
