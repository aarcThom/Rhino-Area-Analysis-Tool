using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Eto.Forms;
using Rhino;

namespace AreaAnalysis.Classes
{
    class EtoMethods
    {
        public static List<string> GetGridViewHeaders(GridView gView)
        {
            List<string> rhinoHeaders = new List<string>();
            foreach (var rHeader in gView.Columns)
            {
                rhinoHeaders.Add(rHeader.HeaderText);
            }
            return rhinoHeaders;
        }

        public static void AddColumn(GridView gView, string colName, PropertyInfo colProp, string colType)
        {
            DelegateBinding<TableObject, string> tableBinding;

            if (colType == "System.String")
            {
                tableBinding = new DelegateBinding<TableObject, string>(
                    t => (string)colProp.GetValue(t),
                    (t, value) => colProp.SetValue(t, value));
            }
            else if (colType == "System.Int32")
            {
                tableBinding = new DelegateBinding<TableObject, string>(
                    t => colProp.GetValue(t).ToString(),
                    (t, value) => colProp.SetValue(t, int.TryParse(value, out var result) 
                        ? result : colProp.GetValue(t)));
            }
            else
            {
                tableBinding = new DelegateBinding<TableObject, string>(
                    t => colProp.GetValue(t).ToString(),
                    (t, value) => colProp.SetValue(t, float.TryParse(value, out var result) 
                        ? result : colProp.GetValue(t)));

            }

            gView.Columns.Add(new GridColumn
            {
                DataCell = new TextBoxCell {Binding = tableBinding},
                HeaderText = colName,
                AutoSize = true,
                Editable = true
            });
        }

    }
}
