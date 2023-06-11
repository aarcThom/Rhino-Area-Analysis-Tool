using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Remoting.Channels;
using System.Text;
using System.Threading.Tasks;
using Rhino;

namespace AreaAnalysis.Classes
{
    public class RowDict : Dictionary<string, RowCell>, INotifyPropertyChanged
    {
        // Fields ==================================================================================
        // used to keep all rows in sync on init
        private static readonly Dictionary<string, RowCell> MasterDictionary = new Dictionary<string, RowCell>(); 

        // name value for special name column & column name
        public static string NameHeader = "Name";


        // Event handlers ==========================================================================
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        // Event handler for property change in Cell objects
        private void OnCellValueChange(object sender, PropertyChangedEventArgs e)
        {
            var cell = (RowCell)sender;
            var key = GetKeyByValue(cell);
            if (key != null)
            {
                OnPropertyChanged(key);
            }
        }

        // Helper method to retrieve key by value from the dictionary
        private string GetKeyByValue(RowCell value)
        {
            foreach (var kvp in this)
            {
                if (kvp.Value == value)
                {
                    return kvp.Key;
                }
            }

            return null;
        }

        // Constructor ===========================================================================
        public RowDict(string cellName = "", Type cellType = null)
        {
            // add existing columns to this row dictionary
            foreach (var column in MasterDictionary)
            {
                if (!this.ContainsKey(column.Key))
                {
                    this.Add(column.Key, new RowCell(column.Value.GetCellType()));
                }
            }

            // adds new column to this dictionary and master dictionary if specified
            if (cellType != null && cellName != "")
            {
                this.Add(cellName, new RowCell(cellType));
            }
            else if (cellType == null ^ cellName == "")
            {
                // convert to error eventually
                RhinoApp.WriteLine("you need to provide both arguments or none"); 
            }
        }

        // Overrides ===========================================================================

        // Override the indexer property to add property change event subscription
        public new RowCell this[string key]
        {
            get => base[key];
            set
            {
                if (base.ContainsKey(key))
                {
                    base[key].PropertyChanged -= OnCellValueChange;
                }

                base[key] = value;

                if (base.ContainsKey(key))
                {
                    base[key].PropertyChanged += OnCellValueChange;
                }

                OnPropertyChanged(key);
            }
        }

        // Override the Add method to add property change event subscription
        public new void Add(string key, RowCell value)
        {
            if (base.ContainsKey(key))
            {
                base[key].PropertyChanged -= OnCellValueChange;
            }

            // need to add to master dictionary if it doesn't contain the new key
            if (!MasterDictionary.ContainsKey(key))
            {
                MasterDictionary.Add(key, value);
            }


            base.Add(key, value);

            if (base.ContainsKey(key))
            {
                base[key].PropertyChanged += OnCellValueChange;
            }

            OnPropertyChanged(key);
        }


        // Override the Remove method to remove property change event subscription
        public new void Remove(string key)
        {
            if (base.ContainsKey(key))
            {
                base[key].PropertyChanged -= OnCellValueChange;
            }

            //remove key from master dictionary if it hasn't been remove by another instance
            if (MasterDictionary.ContainsKey(key))
            {
                MasterDictionary.Remove(key);
            }

            base.Remove(key);

            OnPropertyChanged(key);
        }

        //PUBLIC METHODS ===========================================================

        public static (List<string>, List<Type>) GetCurrentColumns()
        {
            List<string> masterKeys = new List<string>();
            List<Type> masterValues = new List<Type>();
            foreach (var pair in MasterDictionary)
            {
                masterKeys.Add(pair.Key);
                masterValues.Add(pair.Value.GetCellType());
            }

            return (masterKeys, masterValues);
        }

        public void ChangeColumnName(string oldKey, string newKey)
        {
            RowCell oldCell = this[oldKey];
            
            this.Remove(oldKey);
            this.Add(newKey, oldCell);

            if (MasterDictionary.ContainsKey(oldKey))
            {
                RowCell oldMasterCell = MasterDictionary[oldKey];
                MasterDictionary.Remove(oldKey);
                MasterDictionary.Add(newKey, oldMasterCell);
            }

        }

        public void DeleteColumn(string colName)
        {
            this.Remove(colName);
        }

        public static void AddColumnToMaster(string columnName, Type columnType)
        {
            MasterDictionary.Add(columnName, new RowCell(columnType));
        }
    }

    // PRIVATE METHODS ==================================================================
}
