using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rhino;
using Rhino.Runtime.RhinoAccounts;

namespace AreaAnalysis.Classes
{
    public class Test

    {
        private static List<string> _names = new List<string>();

        public void AddNames(string name)
        {
            _names.Add(name);
        }

        public void GetNames()
        {
            RhinoApp.WriteLine("+++++++++++++++++++++++++++++++++++++++++++++++++");
            foreach (var name in _names)
            {
                RhinoApp.WriteLine(name);
            }
        }
    }

    /*

    public RowObsCollection()
        {
            CollectionChanged += Row_CollectionChanged;
        }

        private void Row_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (INotifyPropertyChanged item in e.NewItems)
                {
                    item.PropertyChanged += Item_PropertyChanged;
                }
            }

            if (e.OldItems != null)
            {
                foreach (INotifyPropertyChanged item in e.OldItems)
                {
                    item.PropertyChanged -= Item_PropertyChanged;
                }
            }
        }

        protected override void SetItem(int index, T item)
        {
            if (this[index] != null)
            {
                this[index].PropertyChanged -= Item_PropertyChanged;
            }

            base.SetItem(index, item);

            if (item != null)
            {
                item.PropertyChanged += Item_PropertyChanged;
            }
        }

        protected override void ClearItems()
        {
            foreach (T item in this)
            {
                if (item != null)
                {
                    item.PropertyChanged -= Item_PropertyChanged;
                }
            }

            base.ClearItems();
        }

        private void Item_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            // Notify listeners that a property of an object in the collection has changed.
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }
    }
    */
}
