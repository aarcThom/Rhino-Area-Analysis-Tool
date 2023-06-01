using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Eto.Drawing;
using Eto.Forms;
using Rhino;

namespace AreaAnalysis.Views
{
    public class HeaderContext : ContextMenu
    {
        public HeaderContext(string headerName)
        {
            ButtonMenuItem renameHeader = new ButtonMenuItem() { Text = "Rename header \"" + headerName + "\"" };
            ButtonMenuItem deleteHeader = new ButtonMenuItem() { Text = "Delete header \"" + headerName + "\"" };

            renameHeader.Click += (sender, e) => RhinoApp.WriteLine("clicked rename");
            deleteHeader.Click += (sender, e) => RhinoApp.WriteLine("clicked delete");

            Items.Add(renameHeader);
            Items.Add(deleteHeader);
        }
    }
}
