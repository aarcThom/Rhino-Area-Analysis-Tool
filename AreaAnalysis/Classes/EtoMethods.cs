using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Eto.Forms;

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

        public static void AddGridColumn(GridView rTable, BindingList<string> row, string colName, 
        string editable, string type)
        {
            if (type == "number")
            {
                rTable.Columns.Add(new GridColumn
                {
                    DataCell = new TextBoxCell
                    {
                        Binding = new DelegateBinding<BindingList<string>, string>(
                            r => r.curArea.ToString(),
                            (r, v) => r.curArea = float.TryParse(v, out var result) ? result : r.curArea
                        )
                    },
                    HeaderText = "Current Area",
                    AutoSize = true,
                    Editable = false
                });
            }
        }
    }
}
