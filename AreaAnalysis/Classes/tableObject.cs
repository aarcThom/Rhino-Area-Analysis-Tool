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

    public class DataTable : ObservableCollection<TableObject>
    {
        public DataTable()
        {
            // Subscribe to property change events of TableObject items
            foreach (var item in Items)
            {
                item.PropertyChanged += TableObject_PropertyChanged;
            }
        }

        private void TableObject_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            // Trigger collection change event when a property changes in TableObject
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        protected override void ClearItems()
        {
            // Unsubscribe from property change events of TableObject items before clearing the collection
            foreach (var item in Items)
            {
                item.PropertyChanged -= TableObject_PropertyChanged;
            }

            base.ClearItems();
        }

        protected override void InsertItem(int index, TableObject item)
        {
            // Subscribe to property change event of the new item before inserting it into the collection
            item.PropertyChanged += TableObject_PropertyChanged;

            base.InsertItem(index, item);
        }

        protected override void RemoveItem(int index)
        {
            // Unsubscribe from property change event of the item being removed
            Items[index].PropertyChanged -= TableObject_PropertyChanged;

            base.RemoveItem(index);
        }

        protected override void SetItem(int index, TableObject item)
        {
            // Unsubscribe from property change event of the old item
            Items[index].PropertyChanged -= TableObject_PropertyChanged;

            // Subscribe to property change event of the new item
            item.PropertyChanged += TableObject_PropertyChanged;

            base.SetItem(index, item);
        }
    }
}
