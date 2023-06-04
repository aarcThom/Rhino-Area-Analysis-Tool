using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using AreaAnalysis.Views;
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

        public static GridColumn AddColumn(GridView gView, Type columnType, string userKey)
        {
            DelegateBinding<TableObject, string> tableBinding;


            if (columnType == typeof(string))
            {

                tableBinding = new DelegateBinding<TableObject, string>(
                    binding => binding.TextField[userKey],
                    (binding, value) => { binding.TextField[userKey] = value; });
            }


            else if (columnType == typeof(int))
            {
                tableBinding = new DelegateBinding<TableObject, string>(
                    binding => binding.IntegerField[userKey].ToString(),
                    (binding, value) =>
                    {
                        if (int.TryParse(value, out int intValue))
                        {
                            binding.IntegerField[userKey] = intValue;
                        }
                        else
                        {
                            binding.IntegerField[userKey] = binding.IntegerField[userKey];
                        }
                    });
            }
            else // if (colProp == typeof(float))
            {
                tableBinding = new DelegateBinding<TableObject, string>(
                    binding => binding.NumberField[userKey].ToString(),
                    (binding, value) =>
                    {
                        if (float.TryParse(value, out float floatValue))
                        {
                            binding.NumberField[userKey] = floatValue;
                        }
                        else
                        {
                            binding.NumberField[userKey] = binding.NumberField[userKey];
                        }
                    });

            }

            GridColumn newGColumn = new GridColumn
            {
                DataCell = new TextBoxCell { Binding = tableBinding },
                HeaderText = userKey,
                AutoSize = true,
                Editable = true,
                ID = userKey

            };

            return newGColumn;
        }

        public static void MouseClickHandler(object sender, MouseEventArgs e, GridView gView,
            Control blockControl, TableController tControl)
        {
            if (e.Buttons == MouseButtons.Alternate && e.Modifiers == Keys.None &&
                e.Location.Y <= gView.RowHeight && gView.Columns.Count > 0)
            {
                HeaderRightClick(sender, e, gView, blockControl, tControl);
            }
        }

        private static void HeaderRightClick(object sender, MouseEventArgs e, GridView gView, 
            Control blockControl, TableController tControl)
        {
            // Get the column index from the X-coordinate of the mouse click
            GridColumn clickedHeader = null;

            int prevWidth = 0;
            int currWidth = 0;
            int columnIx = 0;
            foreach (var column in gView.Columns)
            {
                currWidth += column.Width;
                if (e.Location.X < currWidth && e.Location.X > prevWidth)
                {
                    clickedHeader = column;
                    break;
                }

                prevWidth += column.Width;
                columnIx++;
            }

            RhinoApp.WriteLine(columnIx.ToString());

            if (clickedHeader != null)
            {
                HeaderContext cMenu = new HeaderContext(
                    clickedHeader.HeaderText, blockControl, tControl, clickedHeader, columnIx);
                cMenu.Show(gView, e.Location);
            }
        }
    }
}
