using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Eto.Forms;

namespace AreaAnalysis.Classes
{
    public class CustomGridColumnEventArgs : GridColumnEventArgs
    {
        public MouseEventArgs MouseArgs { get; }

        public CustomGridColumnEventArgs(GridColumn column, MouseEventArgs mouseArgs)
            : base(column)
        {
            MouseArgs = mouseArgs;
        }
    }

    public class RhinoGridView : GridView
    {   
        
        public event EventHandler<CustomGridColumnEventArgs> ColumnHeaderRightClick;

        protected virtual void OnColumnHeaderRightClick(CustomGridColumnEventArgs e)
        {
            ColumnHeaderRightClick?.Invoke(this, e);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            if (e.Buttons == MouseButtons.Alternate && e.Modifiers == Keys.None && e.Location.Y <= this.RowHeight)
            {
                GridCell cell = GetCellAt(e.Location);
                GridColumn column = cell.Column;

                var args = new CustomGridColumnEventArgs(column, e);
                OnColumnHeaderRightClick(args);

                base.OnColumnHeaderClick(new GridColumnEventArgs(column));
            }
        }

    }

}
