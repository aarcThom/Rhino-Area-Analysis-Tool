﻿using Eto.Drawing;
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
using Color = Eto.Drawing.Color;

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


            // SETTING UP DOCUMENT TABLE AND GRID VIEW =================================================================================
            TableObject test = new TableObject();

            DataTable mainStore = new DataTable();
            var roomTable = new GridView { AllowColumnReordering = true };
            roomTable.DataStore = mainStore;

            // data table display settings
            roomTable.GridLines = GridLines.Both;

            //Eto button to add column
            var addColumnButton = new Button { Text = "Add Column to Table" };
            addColumnButton.Click += (sender, e) => OnAddColumnButton();

            void OnAddColumnButton()
            {
                var columnAddDialog = new ColumnAddModal();
                columnAddDialog.ShowModal(this);
                /*
                (string colName, PropertyInfo colProp, string colType) = columnAddDialog.GetColumnInfo();

                //add column if not null
                if (colName != null && colProp != null && colType != null)
                {
                    EtoMethods.AddColumn(roomTable, colName, colProp, colType);
                }
                */

            }


            //TEST BUTTON ==============================================================================================================

            var testButton = new Button { Text = "Get Object Descriptios" };
            testButton.Click += (sender, e) => OnTestButton();

            void OnTestButton()
            {
                foreach (var yo in test.GetFieldsDescriptions())
                {
                    RhinoApp.WriteLine(yo);
                }
            }

            var testButton2 = new Button { Text = "Get Object field names" };
            testButton2.Click += (sender, e) => OnTestButton2();

            void OnTestButton2()
            {
                foreach (var yo in test.GetFieldsNames())
                {
                    RhinoApp.WriteLine(yo);
                }
            }

            var testButton3 = new Button { Text = "Change a table object" };
            testButton3.Click += (sender, e) => OnTestButton3();

            void OnTestButton3()
            {
                test.TextField.Add("hello","jello");
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
            layout.AddSeparateRow(testButton, testButton2, testButton3, null);
            layout.AddSeparateRow(new EtoDivider());
            layout.AddSeparateRow(new Label { Text = "Excel Document" });
            layout.AddSeparateRow(excelFilePath, new Label { Text = "---->" }, importExcel, null);
            layout.AddSeparateRow(new EtoDivider());
            layout.AddSeparateRow(new Label { Text = "Rhino Objects Table" });
            layout.AddSeparateRow(addColumnButton, null);
            //layout.AddSeparateRow(addColumnButton, null);
            layout.Add(roomTable, yscale: true);
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
