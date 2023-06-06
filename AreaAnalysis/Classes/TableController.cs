using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using AreaAnalysis.Views;
using Eto.Forms;
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
            //need this parent panel to block action while modals are open
            _parentPanel = parentPanel;
            _gView = gridView;
        }

        // PUBLIC METHODS =======================================================================================

        public int GetRowIndex(TableObject tObject)
        {
            return _dTable.IndexOf(tObject);
        }
        
        public void AddColumn()
        {
            if (_dTable.Count == 0)
            {
                AddColumnToEmpty();
            }
            else
            {
                AddColumnToExisting();
            }
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

        public void RenameHeader(string oldName, string newName, GridColumn column, int index)
        {
            Type dictType = GetDictionaryValueType(oldName);

            // rename the dictionary keys in the table objects
            if (dictType != null)
            {
                foreach (var tObj in _dTable)
                {
                    tObj.ChangeColumnName(oldName, newName);
                }

                //relink the eto column
                GridColumn gCol = EtoMethods.AddColumn(_gView, dictType, newName);
                _gView.Columns.RemoveAt(index);
                _gView.Columns.Insert(index, gCol);
            }
        }

        public void DeleteColumn(string columnName, int colIndex)
        {
            foreach (var dObj in _dTable)
            {
                dObj.DeleteColumn(columnName);
            }

            _gView.Columns.RemoveAt(colIndex);
        }

        public void DeleteRow(List<int> indices)
        {
            if (_dTable.Count > indices.Count)
            {
                indices.Reverse();
                foreach (var row in indices)
                {
                    _dTable.RemoveAt(row);
                }
            }
            else
            {
                WarningMessageModal warning =
                    new WarningMessageModal("The data table requires at least one row.", "Zero rows not allowed");
                    warning.ShowModal(_gView);
            }
            
        }

        public void RenameCells(List<int> rowIndices, string colName)
        {

        }

        // PRIVATE METHODS ======================================================================================

        private void AddRowToExisting()
        {
            TableObject newTObject = new TableObject();
            _dTable.Add(newTObject);
        }

        private void AddColumnToExisting()
        {
            (Type propType, string userName, bool isLink) = ModalInfo();

            if (isLink)
            {
                TableObject newTObject = new TableObject();
                string linkName = newTObject.GetLinkName();

                _dTable.Add(newTObject);
                GridColumn gCol = EtoMethods.AddLinkColumn(_gView, linkName);
                _gView.Columns.Add(gCol);
            }
            else if (propType != null)
            {
                //update existing instances
                foreach (var existingObj in _dTable)
                {
                    existingObj.AddNewColumnToField(propType, userName);
                }
                GridColumn gCol = EtoMethods.AddColumn(_gView, propType, userName);
                _gView.Columns.Add(gCol);
            }
        }
        private void AddColumnToEmpty()
        {
            
            (Type propType, string userName, bool isLink) = ModalInfo();

            if (isLink)
            {
                TableObject newTObject = new TableObject();
                string linkName = newTObject.GetLinkName();

                _dTable.Add(newTObject);
                GridColumn gCol = EtoMethods.AddLinkColumn(_gView, linkName);
                _gView.Columns.Add(gCol);
            }
            else if (propType != null)
            {
                // create the table object with proper column
                TableObject newTObject = new TableObject(propType, userName);
                _dTable.Add(newTObject);
                GridColumn gCol = EtoMethods.AddColumn(_gView, propType, userName);
                _gView.Columns.Add(gCol);
            }
        }
        private (Type, string, bool) ModalInfo()
        {
            var columnAddModal = new ColumnAddModal();
            columnAddModal.ShowModal(_parentPanel);
            (string fieldName, string userName, bool isLink) = columnAddModal.GetColumnInfo();


            if (isLink)
            {
                // return if adding link column
                return (null, null, true);
            }
            else if (fieldName != null)
            {
                //getting the property
                string propName = GetPropertyName(fieldName);
                Type tableType = typeof(TableObject);
                PropertyInfo propInfo = tableType.GetProperty(propName);

                //getting the value type for the property dictionary
                Type propType = propInfo.PropertyType.GenericTypeArguments[1];

                return (propType, userName, false);
            }
            return (null, null, false);
        }

        // formats the UI name to property name
        private string GetPropertyName(string uiName)
        {
            return uiName.Replace(" ", "");
        }

        //gets the type of value in a given dictionary property in table object based on key

        private Type GetDictionaryValueType(string keyName)
        {
            TableObject blankObj = new TableObject();

            Type dictType = null;

            //get the property dictionary location
            PropertyInfo[] props = typeof(TableObject).GetProperties();
            foreach (var prop in props)
            {
                var dictProp = prop.GetValue(blankObj);
                if (dictProp is IDictionary)
                {
                    IDictionary dict = (IDictionary)dictProp;

                    if (dict.Contains(keyName))
                    {
                        dictType = dict.GetType().GenericTypeArguments[0];

                    }
                }
            }

            return dictType;
        }
    }
}
