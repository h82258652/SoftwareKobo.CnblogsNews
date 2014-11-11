using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// “内容对话框”项模板在 http://go.microsoft.com/fwlink/?LinkID=390556 上有介绍

namespace SoftwareKobo.CnblogsNews.Dialog
{
    public sealed partial class JumpPageDialog : ContentDialog
    {
        public JumpPageDialog()
        {
            this.InitializeComponent();
        }

        public int? Page
        {
            get;
            private set;
        }

        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            int page;
            int.TryParse(TxtPage.Text, out page);
            if (page <= 0 || page > 100)
            {
                Page = -1;
            }
            else
            {
                Page = page;
            }
        }

        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
        }

        private void JumpPageDialog_OnOpened(ContentDialog sender, ContentDialogOpenedEventArgs args)
        {
            TxtPage.Focus(FocusState.Programmatic);
        }
    }
}