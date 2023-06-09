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
using System.Windows.Forms;
using Button = Eto.Forms.Button;
using Cell = Eto.Forms.Cell;
using Color = Eto.Drawing.Color;
using Font = Eto.Drawing.Font;
using Keys = Eto.Forms.Keys;
using Label = Eto.Forms.Label;
using MouseButtons = Eto.Forms.MouseButtons;
using MouseEventArgs = Eto.Forms.MouseEventArgs;
using Padding = Eto.Drawing.Padding;
using Panel = Eto.Forms.Panel;
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

        public MainPluginPanel(uint documentSerialNumber)
        {
            //Rhino Panel Setup
            _mDocumentSn = documentSerialNumber;
            Title = GetType().Name;

            // SETTING UP DOCUMENT TABLE AND GRID VIEW =================================================================================

            DataTable mainStore = new DataTable();

            //linking the eto grid view
            var gridView = new RhinoGridView
            {
                AllowColumnReordering = true,
                AllowMultipleSelection = true,
                DataStore = mainStore,
                RowHeight = 20,
                Border = BorderType.Line
            };


            //drawable outline
            /*
            var drawable = new Drawable
            {
                Content = gridView,
                Padding = new Padding(2)
            };
            var pen = new Eto.Drawing.Pen(Colors.LightGrey, 2);


            drawable.Paint += (sender, e) =>
            {

                var rect = new Eto.Drawing.Rectangle(new Eto.Drawing.Size(drawable.Size.Width-2,drawable.Size.Height - 2));
                e.Graphics.DrawRectangle(pen, rect);
            };
            */


            //initializing the data controller
            TableController tableController = new TableController(mainStore, this, gridView);

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
                e.BackgroundColor = (e.Row % 2 == 0)? Color.Parse("#ccecff") : Colors.White;
            };

            gridView.SelectedRowsChanged += (sender, e) =>
            {
                foreach (var row in gridView.SelectedRows)
                {
                    RhinoApp.WriteLine(row.ToString());
                }
            };



            gridView.ColumnHeaderRightClick += (sender, e) =>
            {
                EtoMethods.HeaderRightClick(sender, e.Column, tableController, e.MouseArgs);
            };


            gridView.CellClick += (sender, e) =>
            {
                if (e.Buttons == MouseButtons.Alternate && e.Modifiers == Keys.None 
                                                        && e.Column >= 0 && e.Row >= 0)
                {
                    EtoMethods.CellRightClick(sender, e, this, tableController);
                }
            };



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
                int count = 1;
                foreach (var row in mainStore)
                {
                    String leader = $"Row {count} = ";
                    foreach (var val in row.Values)
                    {
                        leader += $"{val.CellValue} / ";
                    }

                    count++;
                    RhinoApp.WriteLine(leader);
                }
                RhinoApp.WriteLine("-----------------------------");
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

            var layout = new DynamicLayout() { DefaultSpacing = new Eto.Drawing.Size(5, 5), Padding = new Padding(10)};
            layout.AddSeparateRow(testButton, testButton2, testButton3,testButton4, null);
            layout.AddSeparateRow(new EtoDivider());
            layout.AddSeparateRow(new Label { Text = "Excel Document" });
            layout.AddSeparateRow(excelFilePath, new Label { Text = "---->" }, importExcel, null);
            layout.AddSeparateRow(new EtoDivider());
            layout.AddSeparateRow(new Label { Text = "Rhino Objects Table" });
            //layout.Add(drawable, yscale:true); if you want to surround the gview with a drawable
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
