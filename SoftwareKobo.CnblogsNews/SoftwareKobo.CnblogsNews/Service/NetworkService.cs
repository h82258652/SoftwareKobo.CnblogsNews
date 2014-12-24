using System.Net.NetworkInformation;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Views;

namespace SoftwareKobo.CnblogsNews.Service
{
    public class NetworkService
    {
        public static bool IsNetworkAvailable()
        {
            return NetworkInterface.GetIsNetworkAvailable();
        }

        public static async Task ShowCheckNetwork()
        {
            await new DialogService().ShowError("请检查网络连接。", "错误", "关闭", null);
        }
    }
}