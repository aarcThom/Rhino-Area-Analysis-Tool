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

        private Label _userNameLabel;

        private readonly TextBox _userNameBox = new TextBox();
        private Color _textColor;

        private string _chosenFieldName;
        private string _chosenUserName;
        private readonly TableObject _tableObject = new TableObject();

        private readonly List<string>_existingKeys;

        private readonly string _linkName;
        private bool _isLink = false;


        public ColumnAddModal()
        {
            //Setting the modal title
            Title = "Add a column";

            //getting table object info
            List<string> fieldNames = _tableObject.GetFieldsNames();
            List<string> fieldDescriptions = _tableObject.GetFieldsDescriptions();
            (_existingKeys, _linkName) = _tableObject.GetKeys();

            //getting the default text color
            _textColor = _userNameBox.TextColor;
            
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
        public (string, string, bool) GetColumnInfo() => (_chosenFieldName, _chosenUserName, _isLink);

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

            if (_userNameBox.Text == "" && _isLink == false)
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

        private void DropDown_Closed(List<string> fNames, List<string> fDescriptions)
        {
            int choiceIndex = _dropDownList.SelectedIndex;

            _chosenFieldName = fNames[choiceIndex];
            _dropDesc.Text = fDescriptions[choiceIndex];


            // hiding column name for special column types
            if (_chosenFieldName == _linkName)
            {
                _isLink = true;

                _userNameBox.Enabled = false;
                _userNameBox.Visible = false;

                _userNameLabel.Enabled = false;
                _userNameLabel.Visible = false;


            }
            else
            {
                _isLink = false;

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
