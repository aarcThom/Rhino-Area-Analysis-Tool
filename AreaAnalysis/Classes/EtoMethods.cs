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

        public static void AddColumntoGridView(GridView gView, string header, bool isEditable, bool isNumber, BindingList<string> col)
        {
            if (isNumber)
            {
                Rhino.RhinoApp.WriteLine("It's a number!");
            }
            else
            {
                GridColumn gColumn = new GridColumn();
                gColumn.HeaderText = header;
                gColumn.DataCell = new TextBoxCell("string");
            }
        }

    }
}
