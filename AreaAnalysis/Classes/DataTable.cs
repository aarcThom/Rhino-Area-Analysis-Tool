using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;

namespace AreaAnalysis.Classes
{

    public class DataTable : ObservableCollection<RowDict>
    {
        private readonly NotifyCollectionChangedAction action = NotifyCollectionChangedAction.Reset;
        public DataTable()
        {
            // Subscribe to property change events of TableObject items
            foreach (var item in Items)
            {
                item.PropertyChanged += Row_PropertyChanged;
            }
        }

        private void Row_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            // Trigger collection change event when a property changes in TableObject
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(action));
        }

        protected override void ClearItems()
        {
            // Unsubscribe from property change events of TableObject items before clearing the collection
            foreach (var item in Items)
            {
                item.PropertyChanged -= Row_PropertyChanged;
            }

            base.ClearItems();
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(action));
        }

        protected override void InsertItem(int index, RowDict item)
        {
            // Subscribe to property change event of the new item before inserting it into the collection
            item.PropertyChanged += Row_PropertyChanged;

            base.InsertItem(index, item);
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(action));
        }

        public new void Add(RowDict item)
        {
            // Subscribe to property change event of the new item before adding it into the collection
            item.PropertyChanged += Row_PropertyChanged;
            base.Add(item);
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(action));
        }

        protected override void RemoveItem(int index)
        {
            // Unsubscribe from property change event of the item being removed
            Items[index].PropertyChanged -= Row_PropertyChanged;

            base.RemoveItem(index);
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(action));
        }

        protected override void SetItem(int index, RowDict item)
        {
            // Unsubscribe from property change event of the old item
            Items[index].PropertyChanged -= Row_PropertyChanged;

            // Subscribe to property change event of the new item
            item.PropertyChanged += Row_PropertyChanged;

            base.SetItem(index, item);
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(action));
        }
    }
}
