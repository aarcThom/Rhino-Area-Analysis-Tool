using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AreaAnalysis.Classes;
using Eto.Forms;

namespace AreaAnalysis.Views
{ /*
    public class DeleteColumnModal : BaseModal
    {
        private readonly string _colName;
        private readonly int _colIndex;

        private readonly TableController _tControl;
        public DeleteColumnModal(string colName, int colIndex, TableController tControl)
        {
            _colName = colName;
            _colIndex = colIndex;

            _tControl = tControl;

            Title = "Delete column?";

            Label label = new Label { Text = "Are you sure you want to delete column '" + colName + "'?" };
            ModalLayout.Items.Insert(0, label);
        }

        protected override void OnOKButtonClicked()
        {
            _tControl.DeleteColumn(_colName, _colIndex);
            base.OnOKButtonClicked();
        }


    } */
}
