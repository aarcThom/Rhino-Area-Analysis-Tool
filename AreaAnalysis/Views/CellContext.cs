using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AreaAnalysis.Classes;
using Eto.Drawing;
using Eto.Forms;
using Rhino;

namespace AreaAnalysis.Views
{
    public class CellContext : ContextMenu
    {
        public CellContext(List<int> selectedRows, int selectedColumn, TableController tController, GridView gView)
        {
            int rowCount = selectedRows.Count();

            //delete Row Button
             ButtonMenuItem deleteRowBut = new ButtonMenuItem();
             
             if (rowCount == 1)
            {
                deleteRowBut.Text = "Delete selected Row";
            }
            else if (rowCount > 1)
            {
                deleteRowBut.Text = "Delete selected " + rowCount.ToString() + " rows";
            }


            deleteRowBut.Click += (sender, e) =>
            {
                tController.DeleteRow(selectedRows);
            };

            Items.Add(deleteRowBut);

            // rename cells button
            ButtonMenuItem renameCellsBut = new ButtonMenuItem();

            string headerName = gView.Columns[selectedColumn].HeaderText;

            if (rowCount == 1)
            {
                renameCellsBut.Text = "Change selected cell value in column '" + headerName + "'";
            }
            else if (rowCount > 1)
            {
                renameCellsBut.Text = "Change selected cell values for " + rowCount.ToString() + 
                                   " selected rows in column '" + headerName + "'";
            }

            renameCellsBut.Click += (sender, e) =>
            {
                RhinoApp.WriteLine("whatever");
            };

            if (headerName != new TableObject().GetLinkName()) // we can't rename the link status column
            {
                Items.Add(renameCellsBut);
            }
            
        }
    }
}
