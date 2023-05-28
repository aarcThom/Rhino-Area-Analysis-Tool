using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using Eto.Drawing;
using Eto.Forms;
using AreaAnalysis.Classes;
using Rhino.UI.Forms;
using System.Text.RegularExpressions;
using Rhino;

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
            // getting the TableObject Properties
            (List<string> tableTypes, List<string> tableNames, 
                List<PropertyInfo> tableRawProps) = FormatProperties();



            //setting up the modal
            Padding = new Padding(10);
            Title = "Choose what kind of column you want to add";
            Resizable = false;


            //create the dropdown list
            _dropDownList = new DropDown();
            foreach (string name in tableNames)
            {
                _dropDownList.Items.Add(name);
            }
            _dropDownList.SelectedIndex = 0;

            Content = new StackLayout()
            {
                Padding = new Padding(0),
                Spacing = 6,
                Items =
                {
                    new Label { Text = "What sort of column do you want to add?" },
                    _dropDownList
                }
            };

            //handling the closing event
            Closed += (sender, e) => SheetChoiceModal_Closed(tableNames, tableRawProps, tableTypes);
        }

        public (string, PropertyInfo, string) GetColumnInfo() => (_selectedEnglishName, _selectedProp, _selectedType);

        private void SheetChoiceModal_Closed(List<string> names, List<PropertyInfo> props, List<string> tTypes)
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
