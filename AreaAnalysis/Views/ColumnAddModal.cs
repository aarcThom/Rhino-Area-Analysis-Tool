using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Remoting.Channels;
using Eto.Forms;
using AreaAnalysis.Classes;
using System.Text.RegularExpressions;
using Eto.Drawing;

namespace AreaAnalysis.Views
{
    internal class ColumnAddModal : BaseModal
    {
        private readonly DropDown _dropDownList;
        private readonly TextArea _dropDesc = new TextArea();

        private readonly Label _userNameLabel;

        private readonly TextBox _userNameBox = new TextBox();
        private Color _textColor;

        private Type _chosenType;
        private string _chosenUserName;

        private readonly List<string> _existingKeys;
        private readonly List<Type> _existingTypes;


        public ColumnAddModal()
        {
            //Setting the modal title
            Title = "Add a column";

            Height = 250;

            //getting cells info - ie. what type of cells have I implemented
            (List<string> fieldNames, List<string> fieldDescriptions, List<Type> fieldTypes) = RowCell.GetColumns();

            //getting the existing column names and types
            (_existingKeys, _existingTypes) = RowDict.GetCurrentColumns();

            //getting the default text color
            _textColor = _userNameBox.TextColor;
            
            //create the dropdown label, dropdown, and description
            Label dropLabel = new Label { Text = "What sort of column do you want to add?" };

            _dropDownList = new DropDown();

            // removing option to add a second link column
            if (_existingTypes.Contains(typeof(bool)))
            {
                int delIndex = fieldTypes.IndexOf(typeof(bool));
                fieldTypes.RemoveAt(delIndex);
                fieldDescriptions.RemoveAt(delIndex);
                fieldNames.RemoveAt(delIndex);
            }


            // populating the dropdown list
            foreach (string name in fieldNames)
            {
                _dropDownList.Items.Add(name);
            }

            _dropDownList.SelectedIndex = 0;

            _dropDesc.ReadOnly = true;
            _dropDesc.Width = 220;

            //closing event for dropdown
            _dropDownList.DropDownClosed += (sender, args) => DropDown_Closed(fieldTypes, fieldDescriptions);
            //opening event for dropdown
            _dropDownList.LoadComplete += (sender, args) => DropDown_Closed(fieldTypes, fieldDescriptions);


            //adding the text field for the user name and label
            _userNameLabel = new Label { Text = "Choose a name for the column" };
            _userNameBox.PlaceholderText = "Column name";

            //event for username box
            _userNameBox.TextChanged += (sender, args) => TextBoxChanged();

            // adding the dropdown to the layout
            ModalLayout.Items.Insert(0, dropLabel);
            ModalLayout.Items.Insert(1, _dropDownList);
            ModalLayout.Items.Insert(2, _dropDesc);
            ModalLayout.Items.Insert(3, _userNameLabel);
            ModalLayout.Items.Insert(4, _userNameBox);

        }

        // get the info
        public (Type, string) GetColumnInfo() => (_chosenType, _chosenUserName);

        //wiping return info if cancelled
        protected override void OnCancelButtonClicked()
        {
            _chosenType = null;
            _chosenUserName = null;

            base.OnCancelButtonClicked();
        }

        // need to provide a column name
        protected override void OnOKButtonClicked()
        {

            if (_userNameBox.Text == "" && _chosenType != typeof(bool))
            {
                WarningMessageModal warning = new WarningMessageModal("You must define column name",
                    "Empty column name");
                warning.ShowModal(this);
            }
            else if (_existingKeys.Contains(_chosenUserName))
            {
                WarningMessageModal warning = new WarningMessageModal("New column must have a unique name",
                    "Duplicate columns");
                warning.ShowModal(this);
            }
            else
            {
                base.OnOKButtonClicked();
            }
        }

        private void DropDown_Closed(List<Type> fTypes, List<string> fDescriptions)
        {
            int choiceIndex = _dropDownList.SelectedIndex;

            _chosenType = fTypes[choiceIndex];
            _dropDesc.Text = fDescriptions[choiceIndex];


            // hiding column name for special column types
            if (_chosenType == typeof(bool))
            {

                _userNameBox.Enabled = false;
                _userNameBox.Visible = false;

                _userNameLabel.Enabled = false;
                _userNameLabel.Visible = false;


            }
            else
            {

                _userNameBox.Enabled = true;
                _userNameBox.Visible = true;

                _userNameLabel.Enabled = true;
                _userNameLabel.Visible = true;
            }

        }

        private void TextBoxChanged()
        {
            _chosenUserName = _userNameBox.Text;
        }
    }
}
