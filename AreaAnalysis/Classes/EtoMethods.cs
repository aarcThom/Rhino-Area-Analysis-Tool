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
using Rhino.Geometry;

namespace AreaAnalysis.Classes
{
    class EtoMethods
    {

        public static GridColumn AddLinkColumn(GridView gView,string linkName)
        {
            GridColumn newCol = new GridColumn
            {
                HeaderText = linkName,
                DataCell = new TextBoxCell { Binding = Binding.Property((TableObject t) => t.LinkStatus) },
                Editable = false,
                AutoSize = true,
                ID = linkName
            };
            return newCol;
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
                ID = userKey,
                Sortable = true

            };

            return newGColumn;
        }

        public static void CellClick(object sender, GridCellMouseEventArgs e,
            Control blockControl, TableController tControl)
        {
            GridView gView = sender as GridView;

            if (e.Buttons == MouseButtons.Alternate && e.Modifiers == Keys.None && e.Column >= 0 && e.Row >= 0)
            {
                List<int> selectedIndices = new List<int>();

                var selectedRows = gView.SelectedItems;

                if (selectedRows.Count() > 1)
                {
                    foreach (TableObject row in selectedRows)
                    {
                        selectedIndices.Add(tControl.GetRowIndex(row));
                    }
                }
                else
                {
                    selectedIndices.Add(e.Row);
                }

                CellContext cMenu = new CellContext(selectedIndices, tControl);
                cMenu.Show(gView, e.Location);
            }

            

        }


        public static void HeaderClick(object sender, GridColumnEventArgs e, 
            Control blockControl, TableController tControl, MouseEventArgs m)
        {
            GridView gView = sender as GridView;
            
            List<string> headerNames =  new List<string>();

            foreach (var col in gView.Columns)
            {
                headerNames.Add(col.HeaderText);
            }

            GridColumn clickedHeader = e.Column;
            int columnIx = gView.Columns.IndexOf(clickedHeader);

            if (clickedHeader != null)
            {
                HeaderContext cMenu = new HeaderContext(
                    clickedHeader.HeaderText, blockControl, tControl, clickedHeader, 
                    columnIx, headerNames);
                cMenu.Show(gView, m.Location);
            }
        }
    }
}
