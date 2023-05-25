using Eto.Drawing;
using Eto.Forms;
using Rhino.UI;
using Rhino.UI.Controls;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using AreaAnalysis.Classes;
using System.Drawing;
using System.Drawing.Text;
using System.IO;
using System.Runtime.Remoting.Channels;
using FastExcel;
using Microsoft.SqlServer.Server;

namespace AreaAnalysis.Views
{
    [System.Runtime.InteropServices.Guid("29BA961F-A5C5-48D0-A352-11D1B7F336CF")]
    public class MainPluginPanel : Panel
    {
        readonly uint _mDocumentSn = 0;

        /// <summary>
        /// Provide easy access to the SampleCsEtoPanel.GUID
        /// </summary>
        public static System.Guid PanelId => typeof(MainPluginPanel).GUID;

        /// <summary>
        /// Required public constructor with NO parameters
        /// </summary>
        public MainPluginPanel(uint documentSerialNumber)
        {
            //Rhino Panel Setup
            _mDocumentSn = documentSerialNumber;
            Title = GetType().Name;

            // setting up the document table
            UserDataTable dTable = new UserDataTable();
            char forbiddenSeparator = dTable.GetSeparator(); //can't let the user use the defined separator
            ObservableTable<string> mainStore = dTable.obsTable; 



            //creating the roomTable grid and adding the list of room objects as it's data store (source)
            var roomTable = new GridView { AllowMultipleSelection = false };
            
            roomTable.DataStore = mainStore;

            //handling the excel input
            var excelFilePath = new FilePicker();
            var filter = new FileFilter("Excel Files", ".xlsx");
            excelFilePath.Filters.Add(filter);

            var importExcel = new Button { Text = "Import Excel File" };
            importExcel.Click += (sender, e) => OnExcelButton(excelFilePath, roomTable);

            //Eto buttons for the form
            var hello_button = new Button { Text = "Hello..." };
            hello_button.Click += (sender, e) => OnHelloButton(excelFilePath);


            /*
            // adding the column cells and binding their inputs with the object's properties
            roomTable.Columns.Add(new GridColumn { 
                DataCell = new TextBoxCell { Binding = Binding.Property<Room, string>(r => r.roomName) },
                HeaderText = "Room Name", AutoSize = true, Editable = true});

            // we need to create a custom binding to convert back and forth between numbers and text
            roomTable.Columns.Add(new GridColumn { 
                DataCell = new TextBoxCell { Binding = Binding.Property<Room, string>(r => r.roomType) },
                HeaderText = "Program Type", AutoSize = true, Editable = true });

            roomTable.Columns.Add(new GridColumn {
                DataCell = new TextBoxCell { Binding = new DelegateBinding<Room, string>(
                        r => r.curArea.ToString(),
                        (r, v) => r.curArea = float.TryParse(v, out var result) ? result : r.curArea
                    ) }, 
                HeaderText = "Current Area",
                AutoSize = true, 
                Editable = false });

            roomTable.Columns.Add(new GridColumn {
                DataCell = new TextBoxCell
                {
                    Binding = new DelegateBinding<Room, string>(
                        r => r.reqArea.ToString(),
                        (r, v) => r.reqArea = float.TryParse(v, out var result) ? result : r.reqArea
                    )
                },
                HeaderText = "Required Area",
                AutoSize = true,
                Editable = true
            });

            roomTable.Columns.Add(new GridColumn {
                DataCell = new TextBoxCell
                {
                    Binding = new DelegateBinding<Room, string>(
                        r => r.floor.ToString(),
                        (r, v) => r.floor = int.TryParse(v, out var result) ? result : r.floor
                    )
                },
                HeaderText = "Floor",
                AutoSize = true,
                Editable = true
            });
            */

            // laying out the panel
            var layout = new DynamicLayout { DefaultSpacing = new Eto.Drawing.Size(5, 5), Padding = new Padding(10) };
            layout.AddSeparateRow(hello_button, null);
            layout.AddSeparateRow(new EtoDivider());
            layout.AddSeparateRow(new Label { Text = "Excel Document" });
            layout.AddSeparateRow(excelFilePath, new Label{ Text = "---->"}, importExcel, null);
            layout.AddSeparateRow(new EtoDivider());
            layout.AddSeparateRow(new Label { Text = "Rhino Objects Table" });
            layout.Add(roomTable, yscale: true);
            layout.Add(null);
            Content = layout;
        }


        public string Title { get; }

        /// <summary>
        /// Example of proper way to display a message box
        /// </summary>
        protected void OnHelloButton(FilePicker exFile)
        {
            // Use the Rhino common message box and NOT the Eto MessageBox,
            // the Eto version expects a top level Eto Window as the owner for
            // the MessageBox and will cause problems when running on the Mac.
            // Since this panel is a child of some Rhino container it does not
            // have a top level Eto Window.
            Rhino.RhinoApp.WriteLine(exFile.FilePath);
        }


        protected void OnExcelButton(FilePicker exFile, GridView rTable)
        {
            //crazy regex to match a valid file path
            Regex reggie = new Regex(@"^(?:[a-zA-Z]:|\\\\[a-zA-Z0-9_.$\]+)(?:\\[^\\/:*?""<>|]+)*\\?[^\\/:*?""<>|]*$");

            bool fileCheck = false;

            if (exFile.FilePath == "")
            {
                Dialogs.ShowMessage("You must pick an excel file to link.", "Excel file not specified.");
            } 
            else if (reggie.IsMatch(exFile.FilePath))
            {
                Dialogs.ShowMessage("Please enter a valid file path.", "Invalid file path.");
            } 
            else if (File.Exists(exFile.FilePath).Equals(false))
            {
                Dialogs.ShowMessage("This file doesn't exist.", "File doesn't exist.");
            } 
            else
            {
                fileCheck = true;
            }

            // reading the excel file
            if (fileCheck)
            {
                FileInfo excelInfo = new FileInfo(exFile.FilePath);

                // selecting the proper sheet and checking header
                var sheetChoiceDialog = new SheetChoiceModal(excelInfo);
                sheetChoiceDialog.ShowModal(this);

                var headerBool = sheetChoiceDialog.HasHeader();
                var sheetSelected = sheetChoiceDialog.GetSelectedSheet();

                //aligning the excel to the Rhino columns
                var rhinoHeaders = EtoMethods.GetGridViewHeaders(rTable);
            }
        }
    }
}
