using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Views;
using SoftwareKobo.CnblogsAPI.Model;
using SoftwareKobo.CnblogsAPI.Service;
using SoftwareKobo.CnblogsNews.Data;
using SoftwareKobo.CnblogsNews.Helper;
using SoftwareKobo.CnblogsNews.Service;
using SoftwareKobo.HtmlRender.Core;
using System;
using System.Globalization;
using System.Text;
using System.Windows.Input;
using Windows.System;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace SoftwareKobo.CnblogsNews.ViewModel
{
    public class NewsDetailPageViewModel : ViewModelBase
    {
        private UIElement _newsDetail;
        private string _title;
        private News _news;

        public UIElement NewsDetail
        {
            get
            {
                return _newsDetail;
            }
            set
            {
                _newsDetail = value;
                RaisePropertyChanged(() => NewsDetail);
            }
        }

        public string Title
        {
            get
            {
                return _title;
            }
            set
            {
                _title = value;
                RaisePropertyChanged(() => Title);
            }
        }

        public News News
        {
            get
            {
                return _news;
            }
            set
            {
                _news = value;
                RaisePropertyChanged(() => News);
            }
        }

        public ICommand ViewCommentsCommand
        {
            get
            {
                return new RelayCommand<News>((News news) =>
                {
                    if (news == null)
                    {
                        throw new ArgumentNullException("news");
                    }
                    Messenger.Default.Send<Tuple<string, News>>(new Tuple<string, News>("comment", news));
                });
            }
        }

        public ICommand ViewInBrowerCommand
        {
            get
            {
                return new RelayCommand<News>(async (News news) =>
                {
                    if (news == null)
                    {
                        throw new ArgumentNullException("news");
                    }
                    await Launcher.LaunchUriAsync(new Uri(string.Format(CultureInfo.InvariantCulture, "http://news.cnblogs.com/n/{0}", news.Id)));
                });
            }
        }

        public async void Render(News news)
        {
            if (NetworkService.IsNetworkAvailable() == false)
            {
                await NetworkService.ShowCheckNetwork();
                return;
            }
            await StatusBarHelper.Display(true);
            Exception exception = null;
            try
            {
                var newsDetail = await NewsService.DetailAsync(news.Id);
                this.Title = newsDetail.Title;
                switch (LocalSettings.RenderingEngine)
                {
                    case RenderingEngine.Inter:
                        RenderByInterEngine(newsDetail);
                        break;

                    case RenderingEngine.Browser:
                        RenderByBrowserEngine(newsDetail);
                        break;
                }
            }
            catch (Exception ex)
            {
                exception = ex;
            }
            if (exception != null)
            {
                await new DialogService().ShowError(exception, "错误", "关闭", null);
            }
            await StatusBarHelper.Display(false);
        }

        public void RenderByInterEngine(NewsDetail newsDetail)
        {
            var richTextBlock = new RichTextBlock()
            {
                FontSize = 16,
                Margin = new Thickness(20, 0, 20, 10),
                IsTextSelectionEnabled = false
            };
            var context = new RenderContextBase(newsDetail.Content);
            context.Render(richTextBlock);
            this.NewsDetail = richTextBlock;
        }

        private void RenderByBrowserEngine(NewsDetail newsDetail)
        {
            var webView = new WebView();
            var content = new StringBuilder();
            switch (Application.Current.RequestedTheme)
            {
                case ApplicationTheme.Dark:
                    webView.DefaultBackgroundColor = Colors.Black;
                    content.Append("<html><head><style type=\"text/css\">* {color: white;}</style></head><body>");
                    break;

                case ApplicationTheme.Light:
                    webView.DefaultBackgroundColor = Colors.White;
                    content.Append("<html><head></head><body>");
                    break;

                default:
                    throw new InvalidOperationException();
            }
            content.Append(newsDetail.Content);
            content.Append("</body></html>");
            webView.NavigateToString(content.ToString());
            this.NewsDetail = webView;
        }
    }
}