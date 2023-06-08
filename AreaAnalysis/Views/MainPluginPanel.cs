using Eto.Drawing;
using Eto.Forms;
using Rhino.UI;
using Rhino.UI.Controls;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
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
using Rhino;
using System.Collections.ObjectModel;
using System.Data.Common;
using System.Dynamic;
using System.Reflection;
using Cell = Eto.Forms.Cell;
using Color = Eto.Drawing.Color;
using Font = Eto.Drawing.Font;
using TableCell = AreaAnalysis.Classes.RowCell;

namespace AreaAnalysis.Views
{
    [System.Runtime.InteropServices.Guid("29BA961F-A5C5-48D0-A352-11D1B7F336CF")]
    public class MainPluginPanel : Panel
    {
        // Rhino default setup=======================================
        readonly uint _mDocumentSn = 0;

        // Provide easy access to the SampleCsEtoPanel.GUID
        public static Guid PanelId => typeof(MainPluginPanel).GUID;

        //Mouse event field for interacting with gridview
        private MouseEventArgs _mouse;


        public MainPluginPanel(uint documentSerialNumber)
        {
            //Rhino Panel Setup
            _mDocumentSn = documentSerialNumber;
            Title = GetType().Name;

            // SETTING UP DOCUMENT TABLE AND GRID VIEW =================================================================================

            DataTable mainStore = new DataTable();

            //linking the eto grid view
            var gridView = new GridView
            {
                AllowColumnReordering = true,
                AllowMultipleSelection = true,
                
            };

            gridView.DataStore = mainStore;

            // data table display settings
            //gridView.GridLines = GridLines.Both;
            gridView.RowHeight = 20;


            
            //initializing the data controller
            RevTableController tableController = new RevTableController(mainStore, this, gridView);

            // room table events--------------------------------------------------------------------
            // handle clicks on room table

            //format the cells so link symbols are red and green
            gridView.CellFormatting += (sender, e) =>
            {
                RowDict cell = e.Item as RowDict;
                if (cell != null)
                {
                    if (cell[e.Column.HeaderText].CellValue == RowCell.UnLinkedSymbol)
                    {
                        e.ForegroundColor = Colors.Red;
                    }
                    else if (cell[e.Column.HeaderText].CellValue == RowCell.LinkedSymbol)
                    {
                        e.ForegroundColor = Colors.Green;
                    }
                }
            };

            //adding alternating background colors
            gridView.CellFormatting += (sender, e) =>
            {
                if (e.Row % 2 == 0)
                {
                    e.BackgroundColor = Color.Parse("#ccecff");
                }
            };


            //header right click
            gridView.MouseDown += (sender, args) =>
            {
                string columns = "";
                foreach (var col in gridView.Columns)
                {
                    columns += $"{col.HeaderText}__"; 
                }
                RhinoApp.WriteLine(columns);    

                if (args.Buttons == MouseButtons.Alternate && args.Location.Y <= gridView.RowHeight)
                {
                   
                }
            };

            /*
            roomTable.MouseDown += (sender, e) =>
            {
                _mouse = e;
            };

            roomTable.CellClick += (sender, e) =>
            {
                EtoMethods.CellClick(sender, e, this, tableController);
            };


            roomTable.ColumnHeaderClick += (sender, e) =>
            {
                EtoMethods.HeaderClick(sender, e, this, tableController, _mouse);
            };
            */


            //TEST BUTTON ==============================================================================================================

            var testButton = new Button { Text = "Add a Column" };
            testButton.Click += (sender, e) => OnTestButton();

            void OnTestButton()
            {
                tableController.AddColumn();
            }

            var testButton2 = new Button { Text = "Print row values" };
            testButton2.Click += (sender, e) => OnTestButton2();

            void OnTestButton2()
            {
                foreach (var row in mainStore)
                {
                    RhinoApp.WriteLine("++++++++++++");
                    foreach (var val in row.Values)
                    {
                        RhinoApp.Write(val.CellValue);
                    }
                }
            }

            var testButton3 = new Button { Text = "Get master keys" };
            testButton3.Click += (sender, e) => OnTestButton3();

            void OnTestButton3()
            {
            }

            var testButton4 = new Button { Text = "Add row" };
            testButton4.Click += (sender, e) => OnTestButton4();

            void OnTestButton4()
            {
                tableController.AddRow();
            }



            // EXCEL INPUT ==============================================================================================================

            //handling the excel input
            var excelFilePath = new FilePicker();
            var filter = new FileFilter("Excel Files", ".xlsx");
            excelFilePath.Filters.Add(filter);

            var importExcel = new Button { Text = "Import Excel File" };
            importExcel.Click += (sender, e) => OnExcelButton(excelFilePath);

            // PANEL LAYOUT ==============================================================================================================

            var layout = new DynamicLayout { DefaultSpacing = new Eto.Drawing.Size(5, 5), Padding = new Padding(10) };
            layout.AddSeparateRow(testButton, testButton2, testButton3,testButton4, null);
            layout.AddSeparateRow(new EtoDivider());
            layout.AddSeparateRow(new Label { Text = "Excel Document" });
            layout.AddSeparateRow(excelFilePath, new Label { Text = "---->" }, importExcel, null);
            layout.AddSeparateRow(new EtoDivider());
            layout.AddSeparateRow(new Label { Text = "Rhino Objects Table" });
            //layout.AddSeparateRow(addColumnButton, null);
            layout.Add(gridView, yscale: true);
            layout.Add(null);
            Content = layout;

        }


        public string Title { get; }

        /// <summary>
        /// Example of proper way to display a message box
        /// </summary>



        protected void OnExcelButton(FilePicker exFile)
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
            }
        }
    }
}
