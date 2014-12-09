using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Web.Http;

namespace SoftwareKobo.CnblogsNews.Service
{
    public class ExceptionSenderService
    {
        private const string Url = "http://hzfapplication.chinacloudsites.cn/home/sendex";

        public static async Task SendExceptionToMyServer(Exception exception)
        {
            if (exception == null)
            {
                return;
            }
            try
            {
                var client = new HttpClient();
                var dict = new Dictionary<string, string>
                {
                    {
                        "stacktrace", exception.StackTrace
                    }
                };
                IHttpContent content = new HttpFormUrlEncodedContent(dict);
                await client.PostAsync(new Uri(Url, UriKind.Absolute), content);
            }
            catch
            {
            }
        }
    }
}