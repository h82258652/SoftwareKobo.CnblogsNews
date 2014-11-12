using Windows.Web.Http.Filters;
using AngleSharp;
using AngleSharp.DOM;
using SoftwareKobo.CnblogsNews.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.Web.Http;

namespace SoftwareKobo.CnblogsNews.Service
{
    public class NewsService
    {
        public static string NewsBaseUrl;

        static NewsService()
        {
#if DEBUG
            NewsBaseUrl = @"http://localhost:12345";
#else
            NewsBaseUrl = @"http://news.cnblogs.com";
#endif
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

                var link = new Uri(new Uri(NewsBaseUrl, UriKind.Absolute),
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

        public async static Task<ObservableCollection<News>> DownloadNews(int page = 1)
        {
            var document = await DownloadNewsHtml(page);
            var news = ConvertHtmlToNewsItems(document);
            return new ObservableCollection<News>(news);
        }

        public async static Task<IDocument> DownloadNewsHtml(int page = 1)
        {
            var url = string.Format(NewsBaseUrl + "/m?page={0}", page);
            var uri = new Uri(url, UriKind.Absolute);
            var filter = new HttpBaseProtocolFilter();
            filter.CacheControl.ReadBehavior = HttpCacheReadBehavior.MostRecent;
            using (var client = new HttpClient(filter))
            {
                var html = await client.GetStringAsync(uri);
                return DocumentBuilder.Html(html);
            }
        }
    }
}