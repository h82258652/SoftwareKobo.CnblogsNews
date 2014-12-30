using System;
using System.Threading.Tasks;
using Windows.UI.ViewManagement;

namespace SoftwareKobo.CnblogsNews.Helper
{
    public static class StatusBarHelper
    {
        public static async Task Display(bool isLoading, string text = "正在加载")
        {
            var statusBar = StatusBar.GetForCurrentView();
            if (statusBar == null)
            {
                return;
            }
            var progressIndicator = statusBar.ProgressIndicator;
            if (progressIndicator == null)
            {
                return;
            }
            if (isLoading)
            {
                progressIndicator.Text = text;
                progressIndicator.ProgressValue = null;
                await progressIndicator.ShowAsync();
            }
            else
            {
                progressIndicator.Text = string.Empty;
                await progressIndicator.HideAsync();
            }
        }
    }
}