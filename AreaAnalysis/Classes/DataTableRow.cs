using Rhino;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AreaAnalysis.Classes
{
    public class DataTableRow
    {
        /*
        
        private static List<TableCell> _columnsInTable = new List<TableCell>();

        public DataTableRow(string inputCellName = "", Type inputCellType = null)
        {
            CellsInRow = new RowObsCollection<TableCell>();

            //add existing cells to new row object
            foreach (var column in _columnsInTable)
            {
                var colType = column.GetCellType();
                var colName = column.CellName;
                CellsInRow.Add(new TableCell(colType, colName));

            }

            //add new cells to row
            if (inputCellType == null || inputCellName == "")
            {
                RhinoApp.WriteLine("YOU MUST PROVIDE A CELL TYPE AND NAME IF CREATING A ROW WITH A NEW COLUMN"); // replace this with an error message or modal
            }
            else
            {
                CellsInRow.Add(new TableCell(inputCellType, inputCellName));
                _columnsInTable.Add(new TableCell(inputCellType, inputCellName));
            }
        }

        public RowObsCollection<TableCell> CellsInRow { get; set; }

        public void GetAllCells()
        {
            foreach (var cell in CellsInRow)
            {
                RhinoApp.WriteLine(cell.CellValue);
            }
        }
*/
    }
}
