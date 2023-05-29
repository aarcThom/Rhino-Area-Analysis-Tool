using Eto.Forms;
using System.Collections.Generic;
using System.IO;
using AreaAnalysis.Classes;

namespace AreaAnalysis.Views
{
    class SheetChoiceModal : BaseModal
    {
        private readonly CheckBox _checkBox;
        private readonly DropDown _dropDownList;
        private int _selectedOption = 0;


        public SheetChoiceModal(FileInfo excelFileInfo)
        {
            //Modal title
            Title = "Choose your Excel Sheet";

            // getting the sheet list
            ExcelMethods excelFile = new ExcelMethods(excelFileInfo);
            List<string> sheetList = excelFile.GetSheetList();


            // Create the checkbox
            _checkBox = new CheckBox { Text = "Does the Sheet have a header?" };

            //create the dropdown list
            _dropDownList = new DropDown();
            foreach (var sheet in sheetList)
            {
                _dropDownList.Items.Add(sheet);
            }
            _dropDownList.SelectedIndex = 0;
            
            //adding the dropdown events
            _dropDownList.DropDownClosed += (sender, e) => SheetChoice();
            _dropDownList.LoadComplete += (sender, e) => SheetChoice();

            //add objects to modal
            Label dropLabel = new Label
                { Text = "Please choose what excel worksheet you want to reference from " + excelFileInfo.Name };
            ModalLayout.Items.Insert(0, dropLabel);
            ModalLayout.Items.Insert(1, _checkBox);
            ModalLayout.Items.Insert(1, _dropDownList);

        }

        //resetting the selected value if canceled
        protected override void OnCancelButtonClicked()
        {
            _selectedOption = 0;
            base.OnCancelButtonClicked();
        }

        private void SheetChoice()
        {
            _selectedOption = _dropDownList.SelectedIndex + 1; //need to add one as excel starts numbering at 1...
        }

        public int GetSelectedSheet()
        {
            return _selectedOption;
        }

        public bool HasHeader()
        {
            return _checkBox.Checked ?? false;
        }


    }
}
