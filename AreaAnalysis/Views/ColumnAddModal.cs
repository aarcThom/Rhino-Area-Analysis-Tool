using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Remoting.Channels;
using Eto.Forms;
using AreaAnalysis.Classes;
using System.Text.RegularExpressions;

namespace AreaAnalysis.Views
{
    internal class ColumnAddModal : BaseModal
    {
        private readonly DropDown _dropDownList;
        private readonly TextArea _dropDesc = new TextArea();

        private readonly TextBox _userNameBox = new TextBox();

        private string _chosenFieldName;
        private string _chosenUserName;
        private readonly TableObject _tableObject = new TableObject();


        public ColumnAddModal()
        {
            //Setting the modal title
            Title = "Add a column";

            //getting table object info
            List<string> fieldNames = _tableObject.GetFieldsNames();
            List<string> fieldDescriptions = _tableObject.GetFieldsDescriptions();
            
            //create the dropdown label, dropdown, and description
            Label dropLabel = new Label { Text = "What sort of column do you want to add?" };

            _dropDownList = new DropDown();
            foreach (string name in fieldNames)
            {
                _dropDownList.Items.Add(name);
            }

            _dropDownList.SelectedIndex = 0;

            _dropDesc.ReadOnly = true;
            _dropDesc.Width = 220;

            //closing event for dropdown
            _dropDownList.DropDownClosed += (sender, args) => DropDown_Closed(fieldNames, fieldDescriptions);
            //opening event for dropdown
            _dropDownList.LoadComplete += (sender, args) => DropDown_Closed(fieldNames, fieldDescriptions);


            //adding the text field for the user name and label
            Label userNameLabel = new Label { Text = "Choose a name for the column" };
            _userNameBox.PlaceholderText = "Column name";

            //event for username box
            _userNameBox.TextChanged += (sender, args) => TextBoxChanged();

            // adding the dropdown to the layout
            ModalLayout.Items.Insert(0, dropLabel);
            ModalLayout.Items.Insert(1, _dropDownList);
            ModalLayout.Items.Insert(2, _dropDesc);
            ModalLayout.Items.Insert(3, userNameLabel);
            ModalLayout.Items.Insert(4, _userNameBox);

        }

        // get the info
        public (string, string) GetColumnInfo() => (_chosenFieldName, _chosenUserName);

        //wiping return info if cancelled
        protected override void OnCancelButtonClicked()
        {
            _chosenFieldName = null;
            _chosenUserName = null;

            base.OnCancelButtonClicked();
        }

        // need to provide a column name
        protected override void OnOKButtonClicked()
        {
            List<string> existingKeys = _tableObject.GetKeys();

            if (_userNameBox.Text == "")
            {
                WarningMessageModal warning = new WarningMessageModal("You must define column name",
                    "Empty column name");
                warning.ShowModal(this);
            }
            else if (existingKeys.Contains(_chosenUserName))
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

        private void DropDown_Closed(List<string> fNames, List<string> fDescriptions)
        {
            int choiceIndex = _dropDownList.SelectedIndex;

            _chosenFieldName = fNames[choiceIndex];
            _dropDesc.Text = fDescriptions[choiceIndex];

        }

        private void TextBoxChanged()
        {
            _chosenUserName = _userNameBox.Text;
        }
    }
}
