using Eto.Drawing;
using Eto.Forms;
using Rhino.UI.Forms;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AreaAnalysis.Classes;

namespace AreaAnalysis.Views
{
    class SheetChoiceModal : BaseModal
    {
        private CheckBox checkBox;
        private DropDown dropDownList;
        private int selectedOption = 0;


        public SheetChoiceModal(FileInfo excelFileInfo)
        {
            // getting the sheet list
            ExcelMethods excelFile = new ExcelMethods(excelFileInfo);
            List<string> sheetList = excelFile.GetSheetList();


            //setting up the modal
            Padding = new Padding(10);
            Title = "Choose your Excel Sheet";
            Resizable = false;

            // Create the checkbox
            checkBox = new CheckBox { Text = "Does the Sheet have a header?" };

            //create the dropdown list
            dropDownList = new DropDown();
            foreach (var sheet in sheetList)
            {
                dropDownList.Items.Add(sheet);
            }
            dropDownList.SelectedIndex = 0;

            Content = new StackLayout()
            {
                Padding = new Padding(0),
                Spacing = 6,
                Items =
                {
                  new Label { Text="Please choose what excel worksheet you want to reference from " + excelFileInfo.Name },
                  dropDownList,
                  checkBox
                }
            };

            //handling the closing event
            Closed += SheetChoiceModal_Closed;
        }

        private void SheetChoiceModal_Closed(object sender, EventArgs e)
        {
            selectedOption = dropDownList.SelectedIndex + 1; //need to add one as excel starts numbering at 1...
        }

        public int GetSelectedSheet()
        {
            return selectedOption;
        }

        public bool HasHeader()
        {
            return checkBox.Checked ?? false;
        }


    }
}
