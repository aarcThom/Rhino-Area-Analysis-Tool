using Eto.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AreaAnalysis.Views;
using Rhino;
using Rhino.Commands;
using Rhino.DocObjects;

namespace AreaAnalysis.Classes
{
    public class EtoFunctions
    {
        public static GridColumn AddColumn(GridView gView, string columnName, Type columnType)
        {
            DelegateBinding<RowDict, string> tableBinding = new DelegateBinding<RowDict, string>(
                binding => binding[columnName].CellValue,
                (binding, value) => { binding[columnName].CellValue = value; });


            //types of columns that aren't editable
            bool editable = columnType != typeof(bool);

            GridColumn newGColumn = new GridColumn
            {
                DataCell = new TextBoxCell { Binding = tableBinding },
                HeaderText = columnName,
                AutoSize = true,
                Editable = editable,
                ID = columnName,
                Sortable = true

            };

            return newGColumn;
        }

        public static void HeaderRightClick(object sender, GridColumn clickedHeader, TableController tControl, MouseEventArgs m)
        {
            GridView gView = sender as GridView;

            List<string> headerNames = new List<string>();

            foreach (var col in gView.Columns)
            {
                headerNames.Add(col.HeaderText);
            }


            if (clickedHeader != null)
            {
                HeaderContext cMenu = new HeaderContext(gView, tControl, clickedHeader, headerNames);
                cMenu.Show(gView, m.Location);
            }
        }

        public static void CellRightClick(object sender, GridCellMouseEventArgs args, Control mainPanel,
            TableController tControl)
        {
            GridView gView = sender as GridView; 

            List<int> selectedRowIndices = new List<int>();

            var selectedRows = gView.SelectedItems;

            if (selectedRows.Count() > 1)
            {
                foreach (RowDict row in selectedRows)
                {
                    selectedRowIndices.Add(tControl.GetRowIndex(row));
                }
            }
            else
            {
                selectedRowIndices.Add(args.Row);
            }

            CellContext cMenu = new CellContext(selectedRowIndices, args.Column, tControl, gView);
            cMenu.Show(gView, args.Location);
        }

        public static void LinkRightClick(object sender, GridCellMouseEventArgs args, Control mainPanel,
            TableController tControl, RhinoDoc doc)
        {
            GridView gView = sender as GridView;

            int rowIndex = args.Row;

            RowDict row = args.Item as RowDict;
            string blockName = row[RowDict.NameHeader].CellValue;

            if (!tControl.RowUnnamed(rowIndex))
            {
                (Result selectResult, ObjRef[] objects) = RhinoFunctions.UserSelect(doc);
                if (selectResult == Result.Success)
                {
                   (Result blockResult, Guid blockGuid) = RhinoFunctions.CreateBlock(objects, blockName, doc);

                   if (blockResult == Result.Success)
                   {
                       tControl.SetLinkStatus(rowIndex);
                       tControl.SetLinkObject(rowIndex, blockGuid);

                       RhinoFunctions.AddDeleteBlockEventHandler(tControl, doc);
                   }
                }
                
            }

            else
            {
                WarningMessageModal warning =
                    new WarningMessageModal("You must name the row before linking it",
                        "Undefined row name");
                warning.ShowModal(gView);
            }
            
            
        }
    }
}
