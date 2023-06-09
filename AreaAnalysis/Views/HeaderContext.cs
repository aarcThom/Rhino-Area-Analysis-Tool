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
        public HeaderContext(Control blockedMenu, RevTableController tControl, 
            GridColumn column, List<string> headerNames)
        {
            string headerName = column.HeaderText;

            //getting the column index
            GridView gView = (GridView)blockedMenu;
            int columnIndex = gView.Columns.IndexOf(column);

            // allowable functions for non-link columns
            if (headerName != RowCell.GetLinkColumnText())
            {
                ButtonMenuItem renameHeadBut = new ButtonMenuItem() { Text = "Rename header \"" + headerName + "\"" };
                ButtonMenuItem addDependentBut = new ButtonMenuItem() { Text = "Add dependent column to \"" + headerName + "\"" };

                renameHeadBut.Click += (sender, e) =>
                {
                    RenameHeaderModal renameHeader = new RenameHeaderModal(headerName, tControl, column, headerNames);
                    renameHeader.ShowModal(blockedMenu);

                };

                addDependentBut.Click += (sender, e) => RhinoApp.WriteLine("added dependent");

                Items.Add(renameHeadBut);
                Items.Add(addDependentBut);
            }


            //allowable functions for all!

            ButtonMenuItem deleteHeadBut = new ButtonMenuItem() { Text = "Delete header \"" + headerName + "\"" };


            deleteHeadBut.Click += (sender, e) =>
            {
                DeleteColumnModal deleteCol = new DeleteColumnModal(headerName, tControl);
                deleteCol.ShowModal(blockedMenu);
            };

            Items.Add(deleteHeadBut);
        }
    } 
}
