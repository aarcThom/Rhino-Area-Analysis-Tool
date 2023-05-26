using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rhino;
using Rhino.DocObjects;
using Rhino.DocObjects.Tables;
using Rhino.UI;

namespace AreaAnalysis.Classes
{
    //extending the ObservableCollection so that changes inside the lists are registered

    public class ObservableTable<T> : ObservableCollection<BindingList<T>>
    {
        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            base.OnCollectionChanged(e);

            if (e.Action == NotifyCollectionChangedAction.Add || e.Action == NotifyCollectionChangedAction.Remove)
            {
                if (e.OldItems != null)
                {
                    foreach (var innerList in e.OldItems)
                    {
                        if (innerList is BindingList<T> bindingList)
                            bindingList.ListChanged -= InnerListChanged;
                    }
                }

                if (e.NewItems != null)
                {
                    foreach (var innerList in e.NewItems)
                    {
                        if (innerList is BindingList<T> bindingList)
                            bindingList.ListChanged += InnerListChanged;
                    }
                }
            }
        }

        private void InnerListChanged(object sender, ListChangedEventArgs e)
        {
            // Forward the inner list change notification
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }
    }

    class UserDataTable
    {
        //The rhino doc
        private RhinoDoc doc = RhinoDoc.ActiveDoc;

        // the Rhino UserData values
        private readonly string _stringSection = "AARCAreaAnalysisTool"; //make sure we have a unique key
        private readonly char _valueSeparator = '~';
        private readonly string _stringSeperator = "~";
        private readonly string _initStringTableKey = "active";

        // the plugin observableTable to keep track of the changes ....
        public ObservableTable<string> obsTable = new ObservableTable<string>();

        // ... and the corresponding column info lists for the observable table
        public List<string> columnHeaders = new List<string>();
        private List<bool> columnEditable = new List<bool>();
        private List<bool> columnIsNumber = new List<bool>(); 


        //constructor
        public UserDataTable()
        {

            // check if the string section exists in the current document and setup if not
            var userSections = doc.Strings.GetSectionNames();

            if (userSections.Contains(_stringSection) == false)
            {
                doc.Strings.SetString(_stringSection, _initStringTableKey, _stringSeperator) ; //add the init section to the stringtable
            }

        }

        public BindingList<string> AddColumn(string columnName, bool isEditable, bool isNumber)
        {
            //add the proper info to the column info lists and add a new empty list to the obsTable
            columnHeaders.Add(columnName);
            columnEditable.Add(isEditable);
            columnIsNumber.Add(isNumber);

            BindingList<string> newColumn = new BindingList<string>();

            obsTable.Add(newColumn);

            return newColumn;
        }

        private List<string> SplitEntryValues(string val)
        {
            var splitArr = val.Split(_valueSeparator);
            List<string> values = new List<string>();

            foreach (string sub in splitArr)
            {
                values.Add(sub);
            }

            return values;
        }

        public char GetSeparator()
        {
            return _valueSeparator;
        }
    }
}
