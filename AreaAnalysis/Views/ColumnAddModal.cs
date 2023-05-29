using System;
using System.Collections.Generic;
using System.Reflection;
using Eto.Forms;
using AreaAnalysis.Classes;
using System.Text.RegularExpressions;

namespace AreaAnalysis.Views
{
    internal class ColumnAddModal : BaseModal
    {
        private  readonly DropDown _dropDownList;
        private string _selectedEnglishName;
        private PropertyInfo _selectedProp;
        private string _selectedType;


        public ColumnAddModal()
        {
            //Setting the modal title
            Title = "Add a column";

            // getting the TableObject Properties
            (List<string> tableTypes, List<string> tableNames, 
                List<PropertyInfo> tableRawProps) = FormatProperties();

            //create the dropdown list and label
            _dropDownList = new DropDown();
            foreach (string name in tableNames)
            {
                _dropDownList.Items.Add(name);
            }
            _dropDownList.SelectedIndex = 0;

            //closing event for dropdown
            _dropDownList.DropDownClosed += (sender, args) => DropDown_Closed(tableNames, tableRawProps, tableTypes );
            //opening event for dropdown
            _dropDownList.LoadComplete += (sender, args) => DropDown_Closed(tableNames, tableRawProps, tableTypes);

            Label dropLabel = new Label { Text = "What sort of column do you want to add?" };

            // adding the dropdown to the layout
            ModalLayout.Items.Insert(0,dropLabel);
            ModalLayout.Items.Insert(1, _dropDownList);

        }

        // get the info
        public (string, PropertyInfo, string) GetColumnInfo() => (_selectedEnglishName, _selectedProp, _selectedType);

        //wiping return info if cancelled
        protected override void OnCancelButtonClicked()
        {
            _selectedType = null;
            _selectedProp = null;
            _selectedType = null;

            base.OnCancelButtonClicked();
        }

        private void DropDown_Closed(List<string> names, List<PropertyInfo> props, List<string> tTypes)
        {
            int choiceIndex = _dropDownList.SelectedIndex;

            _selectedEnglishName = names[choiceIndex];
            _selectedProp = props[choiceIndex];
            _selectedType = tTypes[choiceIndex];
        }

        private (List<string> propTypes, List<string> formattedNames, List<PropertyInfo> propNames) FormatProperties()
        {
            Type tObjType = typeof(TableObject);

            PropertyInfo[] properties = tObjType.GetProperties();

            List<string> formattedNames = new List<string>();
            List<PropertyInfo> propNames = new List<PropertyInfo>();
            List<string> propTypes = new List<string>();

            foreach (PropertyInfo prop in properties)
            {
                if (prop.PropertyType != typeof(Guid))
                {
                    //add a space before each capital letter
                    formattedNames.Add(Regex.Replace(prop.Name, @"(?<!^)([A-Z])", " $1"));
                    
                    propNames.Add(prop);

                    propTypes.Add(prop.PropertyType.ToString());
                }
                
            }

            return (propTypes, formattedNames, propNames);
        }

    }
}
