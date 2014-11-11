using System.Net.NetworkInformation;

namespace SoftwareKobo.CnblogsNews.Service
{
    public class NetworkService
    {
        public static bool IsNetworkAvailable()
        {
            return NetworkInterface.GetIsNetworkAvailable();
        }
    }
}