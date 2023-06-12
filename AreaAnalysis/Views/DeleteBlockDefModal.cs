using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Eto.Forms;
using Rhino.UI;
using Button = Eto.Forms.Button;
using Label = Eto.Forms.Label;

namespace AreaAnalysis.Views
{
    public class DeleteBlockDefModal : BaseModal
    {
        private bool _deleteRequest = false;

        public DeleteBlockDefModal()
        {
            Title = "Remove block definition";

            Label label = new Label
                { Text = "Are you sure you want to remove the block definition from the document?" };
            

            ModalLayout.Items.Insert(0, label);
        }

        protected override void OnOKButtonClicked()
        {
            _deleteRequest = true;
            base.OnOKButtonClicked();
        }

        protected override void OnCancelButtonClicked()
        {
            _deleteRequest = false;
            base.OnCancelButtonClicked();
        }

        public bool getResult()
        {
            return _deleteRequest;
        }
    }
}
