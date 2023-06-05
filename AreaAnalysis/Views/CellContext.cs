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
        public CellContext(List<int> selectedRows, TableController tController)
        {
            int rowCount = selectedRows.Count();

            //buttons for all row selected counts
             ButtonMenuItem deleteHeadBut = new ButtonMenuItem();


            if (rowCount == 1)
            {
                deleteHeadBut.Text = "Delete selected Row";
            }
            else if (rowCount > 1)
            {
                deleteHeadBut.Text = "Delete selected " + rowCount.ToString() + " rows";
            }


            deleteHeadBut.Click += (sender, e) =>
            {
                tController.DeleteRow(selectedRows);
            };

            Items.Add(deleteHeadBut);
            
        }
    }
}
