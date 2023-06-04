using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Eto.Forms;

namespace AreaAnalysis.Views
{
    public class WarningMessageModal : Dialog
    {
        public StackLayout ModalLayout = new StackLayout()
        {
            Padding = 10,
            Spacing = 6
        };

        public WarningMessageModal(string message, string title)
        {
            Title = title;

            Content = ModalLayout;

            Label warning = new Label { Text = message };

            Button okButton = new Button { Text = "OK" };
            okButton.Click += (sender, e) => OnOKButtonClicked();

            ModalLayout.Items.Add(warning);
            ModalLayout.Items.Add(okButton);
        }

        protected virtual void OnOKButtonClicked()
        {
            Close();
        }
    }
}
