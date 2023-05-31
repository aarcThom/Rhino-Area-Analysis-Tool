using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Eto.Forms;

namespace AreaAnalysis.Views
{
    public class WarningMessageModal : BaseModal
    {

        public WarningMessageModal(string message, string title)
        {
            Title = title;

            Label warning = new Label { Text = message };

            // adding the dropdown to the layout
            ModalLayout.Items.Insert(0, warning);
        }
    }
}
