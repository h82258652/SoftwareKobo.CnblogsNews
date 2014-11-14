using GalaSoft.MvvmLight.Views;
using SoftwareKobo.CnblogsNews.Dialog;
using System;
using System.Threading.Tasks;

namespace SoftwareKobo.CnblogsNews.Service
{
    public partial class JumpPageService:IDialogService
    {
        public Task ShowError(Exception error, string title, string buttonText, Action afterHideCallback)
        {
            throw new NotImplementedException();
        }

        public Task ShowError(string message, string title, string buttonText, Action afterHideCallback)
        {
            throw new NotImplementedException();
        }

        public Task<bool> ShowMessage(string message, string title, string buttonConfirmText, string buttonCancelText, Action<bool> afterHideCallback)
        {
            throw new NotImplementedException();
        }

        public Task ShowMessage(string message, string title, string buttonText, Action afterHideCallback)
        {
            throw new NotImplementedException();
        }

        public Task ShowMessage(string message, string title)
        {
            throw new NotImplementedException();
        }

        public Task ShowMessageBox(string message, string title)
        {
            throw new NotImplementedException();
        }
    }

    public partial class JumpPageService
    {
        public async static Task<int?> GetPage()
        {
            var dialog = new JumpPageDialog();
            await dialog.ShowAsync();
            return dialog.Page;
        }
    }
}