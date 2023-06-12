using AreaAnalysis.Views;
using Eto.Forms;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rhino;
using Rhino.DocObjects;
using Rhino.Commands;

namespace AreaAnalysis.Classes
{
    public class TableController
    {
        // FIELDS ==============================================================================================
        private DataTable _dTable;
        private GridView _gView;
        private readonly Control _parentPanel;

        // CONSTRUCTOR ==========================================================================================

        public TableController(DataTable inputTable, Control parentPanel, GridView gridView)
        {
            _dTable = inputTable;
            _parentPanel = parentPanel; //need this parent panel to block action while modals are open
            _gView = gridView;
        }

        // PUBLIC METHODS =======================================================================================

        public void InitializeGridView()
        {
            _dTable.Add(new RowDict());
            AddColumnPrivate(RowDict.NameHeader, typeof(string));
            AddColumnPrivate(RowCell.GetLinkHeader(), typeof(bool));
            AddColumnPrivate(RowDict.GuidHeader, typeof(Guid));
        }

        public void AddColumn()
        {
            (string colName, Type colType) = AddColumnModalInfo();
            AddColumnPrivate(colName, colType);
        }

        public void DeleteColumn(string columnName, int colIndex)
        {
            foreach (var row in _dTable)
            {
                row.DeleteColumn(columnName);
            }
            _gView.Columns.RemoveAt(colIndex);
        }

        public void AddRow()
        {
            if (_dTable.Count == 0)
            {
                WarningMessageModal warning = new WarningMessageModal(
                    "You need to add a column first", "No properties found");
                warning.ShowModal(_parentPanel);
            }
            else
            {
                AddRowToExisting();
            }
        }

        public void DeleteRow(List<int> rowIndices)
        {
            rowIndices.Reverse();

            foreach (var ix in rowIndices)
            {
                _dTable.RemoveAt(ix);
            }

        }

        public void SetLinkStatus(int rowIndex)
        {
            string linkKey = RowCell.GetLinkHeader();
            _dTable[rowIndex][linkKey].EnableLink();
        }

        public void SetLinkObject(int rowIndex, Guid blockId)
        {
            _dTable[rowIndex][RowDict.GuidHeader].CellValue = blockId.ToString();
        }

        public void RemoveLink(Guid blockGuid)
        {
            int rowIndex = GetRowIndexFromBlockGuid(blockGuid);
            RowCell cellLinkStatus = _dTable[rowIndex][RowCell.GetLinkHeader()];
            cellLinkStatus.DisableLink();

            RowCell cellLinkGuid = _dTable[rowIndex][RowDict.GuidHeader];
            cellLinkGuid.CellValue = Guid.Empty.ToString();
        }

        public void RenameHeader(string oldName, string newName, GridColumn column, int index)
        {
            Type type = _dTable[0][oldName].GetType();

            if (type != typeof(bool))
            {
                foreach (var row in _dTable)
                {
                    row.ChangeColumnName(oldName, newName);
                }

                //relink the eto column
                GridColumn gCol = EtoFunctions.AddColumn(_gView, newName, type);
                _gView.Columns.RemoveAt(index);
                _gView.Columns.Insert(index, gCol);
            }
        }

        public void AddBlockLink(int rowIndex, RhinoDoc doc)
        {

            RowDict row = _dTable[rowIndex];
            string blockName = row[RowDict.NameHeader].CellValue;

            if (!RowUnnamed(rowIndex))
            {
                (Result selectResult, ObjRef[] objects) = RhinoFunctions.UserSelect(doc);
                if (selectResult == Result.Success)
                {
                    (Result blockResult, Guid blockGuid) = RhinoFunctions.CreateBlock(objects, blockName, doc);

                    if (blockResult == Result.Success)
                    {
                        SetLinkStatus(rowIndex);
                        SetLinkObject(rowIndex, blockGuid);

                        RhinoFunctions.AddDeleteBlockEventHandler(this, doc);
                    }
                }

            }

            else
            {
                WarningMessageModal warning =
                    new WarningMessageModal("You must name the row before linking it",
                        "Undefined row name");
                warning.ShowModal(_gView);
            }


        }



        // public helper functions ===============================

        public int GetRowIndex(RowDict row)
        {
            return _dTable.IndexOf(row);
        }

        public RowDict GetRowFromIndex(int index)
        {
            return _dTable[index];
        }

        public bool RowUnnamed(int index)
        {
            return _dTable[index][RowDict.NameHeader].CheckForDefaultName();
        }

        public List<string> GetAllRowNames()
        {
            List<string> allNames = new List<string>();

            foreach (RowDict row in _dTable)
            {
                allNames.Add(row[RowDict.NameHeader].CellValue);
            }

            return allNames;
        }

        public List<Guid> GetLinkedBlockGuids()
        {
            List<Guid> allGuids = new List<Guid>();

            foreach (RowDict row in _dTable)
            {
                Guid rowGuid = Guid.Parse(row[RowDict.GuidHeader].CellValue);
                if (rowGuid != Guid.Empty)
                {
                    allGuids.Add(rowGuid);
                }
            }
            return allGuids;
        }

        public GridView GetGridView()
        {
            return _gView;
        }



        // PRIVATE METHODS =======================================================================================

        private void AddColumnPrivate(string colName, Type colType)
        {
            if (colName != null && colType != null)
            {
                // add new column to master dictionary
                RowDict.AddColumnToMaster(colName, colType);

                //add the new columns to all the existing rows
                foreach (var row in _dTable)
                {
                    row.Add(colName, new RowCell(colType));
                }

                if (colType != typeof(Guid))
                {
                    GridColumn gColumn = EtoFunctions.AddColumn(_gView, colName, colType);
                    _gView.Columns.Add(gColumn);
                }
            }
        }

        private void AddRowToExisting()
        {
            RowDict row = new RowDict();
            _dTable.Add(row);
        }

        private (string colName, Type colType) AddColumnModalInfo()
        {
            var columnAddModal = new ColumnAddModal();
            columnAddModal.ShowModal(_parentPanel);

            (Type chosenType, string userName) = columnAddModal.GetColumnInfo();

            if (chosenType == typeof(bool))
            {
                return (RowCell.GetLinkHeader(), chosenType);
            }
            else if (userName != null)
            {
                return (userName, chosenType);
            }
            else
            {
                return (null, null);
            }
        }

        private int GetRowIndexFromBlockGuid(Guid blockIndex)
        {
            foreach (var row in _dTable)
            {
                if (row[RowDict.GuidHeader].CellValue == blockIndex.ToString())
                {
                    return _dTable.IndexOf(row);
                }
            }

            return -1;
        }
    }
}
