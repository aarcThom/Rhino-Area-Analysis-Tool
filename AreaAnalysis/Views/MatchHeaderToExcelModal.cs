using Eto.Drawing;
using Eto.Forms;
using Rhino.UI.Forms;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AreaAnalysis.Views
{
    class MatchDataToExcelHeader : CommandDialog
    {
        private CheckBox checkBox;
        private DropDown dropDownList;
        private int selectedOption = 0;


        public MatchDataToExcelHeader(FileInfo excelFileInfo, int sheetNo, List<string> rhinoHeaders)
        {
            Padding = new Padding(10);
            Title = "Choose your Excel Sheet";
            Resizable = false;


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

    }
}
