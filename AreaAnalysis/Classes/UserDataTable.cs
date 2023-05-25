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
using Rhino.UI;

namespace AreaAnalysis.Classes
{
    //extending the ObservableCollection so that changes inside the lists are registered
    //This is intended to have the following lists:
    //00: Column Name
    //01: Editable
    //02 --> onwards: values

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
        // the Rhino UserData values
        private readonly string _stringSection = "AARCAreaAnalysisTool"; //make sure we have a unique key
        private readonly char _valueSeparator = '~';

        //The rhino doc
        private RhinoDoc doc = RhinoDoc.ActiveDoc;

        // the plugin observableTable to keep track of the changes
        public ObservableTable<string> obsTable = new ObservableTable<string>();

        //the required header lists for the obstable:
        private readonly List<string> headerList = new List<string> { "header", "editable" };


        //constructor
        public UserDataTable()
        {
            //add the required headers to the obsTable
            foreach (var _ in headerList)
            {
                obsTable.Add(new BindingList<string>());
            }

            // check if the string section exists in the current document and setup if not
            var userSections = doc.Strings.GetSectionNames();

            if (!userSections.Contains(_stringSection))
            {
                foreach (var header in headerList)
                {
                    doc.Strings.SetString(_stringSection, header, "");
                }
            }

            //populate the obsTable

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
