using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Channels;
using System.Text;
using System.Threading.Tasks;
using AreaAnalysis.Classes;
using Eto.Drawing;
using Eto.Forms;
using Rhino;

namespace AreaAnalysis.Views
{
    public class CellContext : ContextMenu
    { 
        public CellContext(List<int> selectedRows, int selectedColumn, TableController tController, GridView gView, RhinoDoc doc)
        {
            int rowCount = selectedRows.Count();
            
            // FOR ALL COLUMNS===================================================================
            //delete Row Button
             ButtonMenuItem deleteRowBut = new ButtonMenuItem();
             
             if (rowCount == 1)
            {
                deleteRowBut.Text = "Delete selected Row";
            }
            else if (rowCount > 1)
            {
                deleteRowBut.Text = "Delete selected " + rowCount.ToString() + " rows";
            }

            
            deleteRowBut.Click += (sender, e) =>
            {
                tController.DeleteRow(selectedRows);
            };

            Items.Add(deleteRowBut);


            string colHeaderName = gView.Columns[selectedColumn].HeaderText;
            

            //FOR NON-SPECIAL COLUMNS

            if (colHeaderName != RowCell.GetLinkHeader() && colHeaderName != RowDict.NameHeader) // we can't rename the link status column
            {

                // rename cells button
                ButtonMenuItem renameCellsBut = new ButtonMenuItem();

                if (rowCount == 1)
                {
                    renameCellsBut.Text = "Change selected cell value in column '" + colHeaderName + "'";
                }
                else if (rowCount > 1)
                {
                    renameCellsBut.Text = "Change selected cell values for " + rowCount.ToString() +
                                          " selected rows in column '" + colHeaderName + "'";
                }

                renameCellsBut.Click += (sender, e) =>
                {
                    RhinoApp.WriteLine("whatever");
                };

                Items.Add(renameCellsBut);
            }

            //FOR THE LINK COLUMN
            string rowName = tController.GetAllRowNames()[selectedRows[0]];
            if (colHeaderName == RowCell.GetLinkHeader() && rowCount == 1)
            {
                // row has no link yet
                if (!tController.GetRowFromIndex(selectedRows[0])[RowCell.GetLinkHeader()].GetLinkStatus())
                {
                    // LinkButton
                    ButtonMenuItem linkBlockBut = new ButtonMenuItem();
                    linkBlockBut.Text = "Link Rhino geometry to this row";

                    linkBlockBut.Click += (sender, e) =>
                    {
                        tController.AddBlockLink(selectedRows[0], doc);
                    };


                    Items.Add(linkBlockBut);
                }
                

            }







        }
    } 
}
