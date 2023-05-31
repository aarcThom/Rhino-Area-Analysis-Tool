using Eto.Forms;

namespace AreaAnalysis.Views
{
    public class BaseModal : Dialog
    {
        public StackLayout ModalLayout = new StackLayout() {Padding = 10, Spacing = 6};

        public BaseModal()
        {
            Title = "Custom Dialog";

            Content = ModalLayout;

            Button okButton = new Button { Text = "OK" };
            okButton.Click += (sender, e) => OnOKButtonClicked();

            Button cancelButton = new Button { Text = "Cancel" };
            cancelButton.Click += (sender, e) => OnCancelButtonClicked();

            StackLayout buttonLayout = new StackLayout()
            {
                Spacing = 6,
                Items = { okButton, cancelButton },
                Orientation = Orientation.Horizontal
            };

            DefaultButton = okButton;
            AbortButton = cancelButton;

            ModalLayout.Items.Add(buttonLayout);

        }

        protected virtual void OnOKButtonClicked()
        {
            Close();
        }

        protected virtual void OnCancelButtonClicked()
        {
            Close();
        }
    }
}
