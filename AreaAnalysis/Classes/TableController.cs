using System;
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
        // PRIVATE METHODS ======================================================================================

        private void AddRowToExisting()
        {
            TableObject newTObject = new TableObject();
            _dTable.Add(newTObject);
        }

        private void AddColumnToExisting()
        {
            (Type propType, string userName) = ModalInfo();

            if (propType != null)
            {
                //update existing instances
                foreach (var existingObj in _dTable)
                {
                    existingObj.AddNewField(propType, userName);
                }
                EtoMethods.AddColumn(_gView, propType, userName);
            }
        }
        private void AddColumnToEmpty()
        {
            
            (Type propType, string userName) = ModalInfo();

            if (propType != null)
            {
                // create the table object with proper column
                TableObject newTObject = new TableObject(propType, userName);
                _dTable.Add(newTObject);
                EtoMethods.AddColumn(_gView, propType, userName);
            }
        }
        private (Type, string) ModalInfo()
        {
            var columnAddModal = new ColumnAddModal();
            columnAddModal.ShowModal(_parentPanel);
            (string fieldName, string userName) = columnAddModal.GetColumnInfo();

            if (fieldName != null)
            {
                //getting the property
                string propName = GetPropertyName(fieldName);
                Type tableType = typeof(TableObject);
                PropertyInfo propInfo = tableType.GetProperty(propName);

                //getting the value type for the property dictionary
                Type propType = propInfo.PropertyType.GenericTypeArguments[1];

                return (propType, userName);
            }
            return (null, null);
        }

        // formats the UI name to property name
        private string GetPropertyName(string uiName)
        {
            return uiName.Replace(" ", "");
        }
    }
}
