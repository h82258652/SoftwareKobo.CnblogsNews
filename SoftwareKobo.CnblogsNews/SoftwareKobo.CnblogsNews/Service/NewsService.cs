using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Windows.Web.Http;
using AngleSharp;
using AngleSharp.DOM;
using SoftwareKobo.CnblogsNews.Model;

namespace SoftwareKobo.CnblogsNews.Service
{
    public class NewsService
    {
        public async static Task<ObservableCollection<News>> DownloadNews(int page = 1)
        {
            var document = await DownloadNewsHtml(page);
            var news = ConvertHtmlToNewsItems(document);
            return new ObservableCollection<News>(news);
        }

        public static IEnumerable<News> ConvertHtmlToNewsItems(IDocument document)
        {
            var items = document.QuerySelectorAll(@"div.list_item");
            var list = new List<News>();
            foreach (var item in items)
            {
                if (item.ChildNodes.Length != 4)
                {
                    Debugger.Break();
                    continue;
                }
                var actionLinkNode = item.ChildNodes[0] as IElement;
                var publishTimeNode = item.ChildNodes[1] as IText;
                if (actionLinkNode == null || publishTimeNode == null)
                {
                    Debugger.Break();
                    continue;
                }

                var link = new Uri(new Uri("http://news.cnblogs.com", UriKind.Absolute),
                     actionLinkNode.GetAttribute("href"));
                var title = actionLinkNode.InnerHtml;
                var publishTime = publishTimeNode.Text.TrimStart('(').TrimEnd(',');
                list.Add(new News()
                {
                    DetailLink = link,
                    Title = title,
                    PublishTime = publishTime
                });
            }
            return list;
        }

        public async static Task<IDocument> DownloadNewsHtml(int page = 1)
        {
            var url = string.Format("http://news.cnblogs.com/m?page={0}", page);
            var uri = new Uri(url, UriKind.Absolute);
            return await DocumentBuilder.HtmlAsync(uri);
        }
    }
}
