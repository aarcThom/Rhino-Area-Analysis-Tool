using AreaAnalysis.Views;
using Eto.Forms;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rhino;

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
            AddColumnPrivate(RowDict.NameHeaderText, typeof(string));
            AddColumnPrivate(RowCell.GetLinkColumnText(), typeof(bool));
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

        public void SetLink(int rowIndex)
        {
            string linkKey = RowCell.GetLinkColumnText();
            _dTable[rowIndex][linkKey].EnableLink();
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

        // public helper functions ===============================

        public int GetRowIndex(RowDict row)
        {
            return _dTable.IndexOf(row);
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

                GridColumn gColumn = EtoFunctions.AddColumn(_gView, colName, colType);
                _gView.Columns.Add(gColumn);
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
                return (RowCell.GetLinkColumnText(), chosenType);
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
    }
}
