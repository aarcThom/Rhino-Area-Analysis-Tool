using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using FastExcel;
using FastExcelObj = FastExcel.FastExcel;
using Rhino.UI;
using Rhino.Commands;

namespace AreaAnalysis.Classes
{
    internal class ExcelReader
    {
        public FileInfo InputExcel;

        private Worksheet _currentSheet;

        //constructor
        public ExcelReader(FileInfo excelFile)
        {
            InputExcel = excelFile;
        }

        //grab the list of sheets in the chosen excel file
        public List<string> GetSheetList()
        {
            List<string> result = new List<string>();


            try
            {
                using (FastExcelObj fastExcel = new FastExcelObj(InputExcel, true))
                {
                    foreach (var sheet in fastExcel.Worksheets)
                    {
                        result.Add(sheet.Name);
                    }
                }
            }
            catch (IOException ex)
            {
                Dialogs.ShowMessage(
                    "Something went wrong. You probably left the excel file open. Please close it and try again. System message = " +
                    ex.Message, "File read error.");
            }
            return result;
        }


        public List<List<string>> SetWorksheet(int sheetNo, bool hasHeader)
        {
            using (FastExcelObj fastExcel = new FastExcelObj(InputExcel, true))
            {
                _currentSheet = fastExcel.Read(sheetNo);
                return ReadSheet(hasHeader);
            }
        }

        //reads through the rows and makes a list of lists with cell values
        private List<List<string>> ReadSheet(bool header)
        {
            List<List<string>> result = new List<List<string>>();

            int count = 0;
            foreach (var row in _currentSheet.Rows)
            {
                List<string> rowList = new List<string>();

                if (header && count != 0 || !header)
                {
                    foreach (var cell in row.Cells)
                    {
                        rowList.Add(cell.Value.ToString());
                    }
                }
                count++;
                result.Add(rowList);
            }

            return result;
        }
    }
}
