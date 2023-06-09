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

        private readonly RevTableController _tControl;
        public DeleteColumnModal(string colName, RevTableController tControl)
        {
            _colName = colName;

            _tControl = tControl;

            Title = "Delete column?";

            Label label = new Label { Text = "Are you sure you want to delete column '" + colName + "'?" };
            ModalLayout.Items.Insert(0, label);
        }

        /*
        protected override void OnOKButtonClicked()
        {
            _tControl.DeleteColumn(_colName, _colIndex);
            base.OnOKButtonClicked();
        }
        */


    } 
}
