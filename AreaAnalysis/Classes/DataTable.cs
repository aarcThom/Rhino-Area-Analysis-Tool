using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;

namespace AreaAnalysis.Classes
{

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
