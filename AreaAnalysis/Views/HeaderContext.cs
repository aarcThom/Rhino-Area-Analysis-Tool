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
    public class HeaderContext : ContextMenu
    {
        public HeaderContext(Control blockedMenu, TableController tControl, 
            GridColumn column, List<string> headerNames)
        {
            string headerName = column.HeaderText;

            //getting the column index
            GridView gView = (GridView)blockedMenu;
            int columnIndex = gView.Columns.IndexOf(column);


            ButtonMenuItem renameHeadBut = new ButtonMenuItem() { Text = "Rename header \"" + headerName + "\"" };
            ButtonMenuItem addDependentBut = new ButtonMenuItem() { Text = "Add dependent column to \"" + headerName + "\"" };
            ButtonMenuItem deleteHeadBut = new ButtonMenuItem() { Text = "Delete header \"" + headerName + "\"" };


            renameHeadBut.Click += (sender, e) =>
            {
                RenameHeaderModal renameHeader = new RenameHeaderModal(headerName, tControl, column, headerNames, columnIndex);
                renameHeader.ShowModal(blockedMenu);

            };

            addDependentBut.Click += (sender, e) => RhinoApp.WriteLine("added dependent");

            deleteHeadBut.Click += (sender, e) =>
            {
                DeleteColumnModal deleteCol = new DeleteColumnModal(headerName, tControl, columnIndex);
                deleteCol.ShowModal(blockedMenu);
            };

            Items.Add(renameHeadBut);
            Items.Add(addDependentBut);
            Items.Add(deleteHeadBut);
        }
    } 
}
