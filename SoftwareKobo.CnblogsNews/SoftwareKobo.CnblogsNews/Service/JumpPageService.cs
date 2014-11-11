using SoftwareKobo.CnblogsNews.Dialog;
using System;
using System.Threading.Tasks;

namespace SoftwareKobo.CnblogsNews.Service
{
    public class JumpPageService
    {
        public async static Task<int?> GetPage()
        {
            var dialog = new JumpPageDialog();
            await dialog.ShowAsync();
            return dialog.Page;
        }
    }
}