using Windows.UI.Xaml.Controls;

namespace final_project.Pages
{
    public sealed partial class Registration : ContentDialog
    {
        public Registration()
        {
            this.InitializeComponent();
        }

        public string IpAddress => IPTextBox.Text;

        private void ContentDialog_PrimaryButtonClick(
            ContentDialog sender,
            ContentDialogButtonClickEventArgs args)
        {
            // Optionally validate IP here.
            // If invalid: args.Cancel = true;
        }

        private void ContentDialog_SecondaryButtonClick(
            ContentDialog sender,
            ContentDialogButtonClickEventArgs args)
        {
            IPTextBox.Text = string.Empty;
            NameTextBox.Text = string.Empty;
            args.Cancel = true;
        }
    }
}