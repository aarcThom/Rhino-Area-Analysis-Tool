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
        public HeaderContext(string headerName, Control blockedMenu, TableController tControl, GridColumn column, int index)
        {
            ButtonMenuItem renameHeadBut = new ButtonMenuItem() { Text = "Rename header \"" + headerName + "\"" };
            ButtonMenuItem deleteHeadBut = new ButtonMenuItem() { Text = "Delete header \"" + headerName + "\"" };
            ButtonMenuItem addDependentBut = new ButtonMenuItem() { Text = "Add dependent column to \"" + headerName + "\"" };

            renameHeadBut.Click += (sender, e) =>
            {
                RenameHeaderModal renameHeader = new RenameHeaderModal(headerName, tControl, column, index);
                renameHeader.ShowModal(blockedMenu);
                
            };

            deleteHeadBut.Click += (sender, e) =>
            {
                DeleteColumnModal deleteCol = new DeleteColumnModal(headerName, index, tControl);
                deleteCol.ShowModal(blockedMenu);
            };



            addDependentBut.Click += (sender, e) => RhinoApp.WriteLine("added dependent");

            Items.Add(renameHeadBut);
            Items.Add(deleteHeadBut);
            Items.Add(addDependentBut);
        }
    }
}
