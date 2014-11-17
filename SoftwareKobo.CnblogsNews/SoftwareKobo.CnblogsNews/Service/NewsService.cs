using System.Net;
using System.Text.RegularExpressions;
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

        private static readonly Regex NewsIdRegex = new Regex(@"/m/n/(\d+)");

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
                var commentNode = item.ChildNodes[2] as IElement;
                if (actionLinkNode == null
                    || publishTimeNode == null
                    || commentNode == null)
                {
                    Debugger.Break();
                    continue;
                }

                var href = actionLinkNode.GetAttribute("href");
                var match = NewsIdRegex.Match(href);
                if (match.Success == false)
                {
                    Debugger.Break();
                    continue;
                }
                var groups = match.Groups;
                if (groups == null || groups.Count < 1)
                {
                    Debugger.Break();
                    continue;
                }
                var newsIdGroup = groups[1];
                if (newsIdGroup.Success == false)
                {
                    Debugger.Break();
                    continue;
                }

                int commentCount;
                if (int.TryParse(commentNode.InnerHtml, out commentCount) == false)
                {
                    Debugger.Break();
                    continue;
                }

                var newsId = newsIdGroup.Value;
                var title = WebUtility.HtmlDecode(actionLinkNode.InnerHtml);
                var publishTime = publishTimeNode.Text.TrimStart('(').TrimEnd(',');

                list.Add(new News()
                {
                    NewsId = newsId,
                    Title = title,
                    PublishTime = publishTime,
                    CommentCount = commentCount
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