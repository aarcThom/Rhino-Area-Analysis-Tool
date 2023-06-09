using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AreaAnalysis.Classes;
using Eto.Forms;

namespace AreaAnalysis.Views
{ 
    public class DeleteColumnModal : BaseModal
    {
        private readonly string _colName;
        private readonly int _colIndex;

        private readonly TableController _tControl;
        public DeleteColumnModal(string colName, TableController tControl, int index)
        {
            _colName = colName;

            _tControl = tControl;

            _colIndex = index;

            Title = "Delete column?";

            Label label = new Label { Text = "Are you sure you want to delete column '" + colName + "'?" };
            ModalLayout.Items.Insert(0, label);
        }

        
        protected override void OnOKButtonClicked()
        {
            _tControl.DeleteColumn(_colName, _colIndex);
            base.OnOKButtonClicked();
        }


    } 
}
