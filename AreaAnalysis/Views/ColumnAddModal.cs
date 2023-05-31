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
        private readonly DropDown _dropDownList;
        private TextArea _dropDesc = new TextArea();

        private string _chosenFieldName;
        private string _chosenUserName;
        private TableObject _tableObject = new TableObject();


        public ColumnAddModal()
        {
            //Setting the modal title
            Title = "Add a column";

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


            // adding the dropdown to the layout
            ModalLayout.Items.Insert(0, dropLabel);
            ModalLayout.Items.Insert(1, _dropDownList);
            ModalLayout.Items.Insert(2, _dropDesc);

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

        private void DropDown_Closed(List<string> fNames, List<string> fDescriptions)
        {
            int choiceIndex = _dropDownList.SelectedIndex;

            _chosenFieldName = fNames[choiceIndex];
            _dropDesc.Text = fDescriptions[choiceIndex];

        }
    }
}
