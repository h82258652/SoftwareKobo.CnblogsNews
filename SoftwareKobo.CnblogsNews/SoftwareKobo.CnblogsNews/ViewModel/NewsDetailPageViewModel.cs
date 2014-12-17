using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Views;
using SoftwareKobo.CnblogsAPI.Model;
using SoftwareKobo.CnblogsAPI.Service;
using SoftwareKobo.CnblogsNews.Service;
using SoftwareKobo.HtmlRender.Core;
using System;
using System.Windows.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace SoftwareKobo.CnblogsNews.ViewModel
{
    public class NewsDetailPageViewModel : ViewModelBase
    {
        private bool _isLoading;
        private UIElement _newsDetail;
        private string _title;
        private News _news;

        public bool IsLoading
        {
            get
            {
                return _isLoading;
            }
            set
            {
                _isLoading = value;
                RaisePropertyChanged(() => IsLoading);
            }
        }

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
                return new RelayCommand<News>(ViewCommentsCommandExecute);
            }
        }

        public async void ViewCommentsCommandExecute(News news)
        {
            if (news == null)
            {
                throw new ArgumentNullException("news");
            }
            var commentCount = news.Comments;
            if (commentCount <= 0)
            {
                await new DialogService().ShowMessageBox("该新闻暂时还没有评论。", string.Empty);
            }
            else
            {
                Messenger.Default.Send<Tuple<string, News>>(new Tuple<string, News>("comment", news));
            }
        }

        public async void Render(News news)
        {
            if (NetworkService.IsNetworkAvailable() == false)
            {
                await new DialogService().ShowMessageBox("请检查网络连接。", "错误");
                return;
            }
            this.IsLoading = true;
            Exception exception = null;
            try
            {
                var newsDetail = await NewsService.DetailAsync(news.Id);
                this.Title = newsDetail.Title;
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
            catch (Exception ex)
            {
                exception = ex;
            }
            if (exception != null)
            {
                await new DialogService().ShowError(exception, "错误", "关闭", null);
            }
            this.IsLoading = false;
        }
    }
}