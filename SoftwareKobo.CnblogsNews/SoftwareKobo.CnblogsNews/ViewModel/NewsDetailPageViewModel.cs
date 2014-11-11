using GalaSoft.MvvmLight;
using SoftwareKobo.CnblogsNews.Model;
using SoftwareKobo.CnblogsNews.Service;
using System;
using Windows.UI.Popups;
using Windows.UI.Xaml;

namespace SoftwareKobo.CnblogsNews.ViewModel
{
    public class NewsDetailPageViewModel : ViewModelBase
    {
        private bool _isLoading;
        private UIElement _newsDetail;
        private string _title;

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

        public async void Render(News news)
        {
            if (NetworkService.IsNetworkAvailable() == false)
            {
                await new MessageDialog("请检查网络连接。").ShowAsync();
                return;
            }
            this.IsLoading = true;
            Exception exception = null;
            try
            {
                var html = await NewsDetailService.DownloadNewsDetailHtml(news.DetailLink);
                var document = NewsDetailService.ParseHtmlToDocument(html);
                this.Title = NewsDetailService.GetTitle(document);
                this.NewsDetail = NewsDetailService.RenderNewsDetail(document);
            }
            catch (Exception ex)
            {
                exception = ex;
            }
            if (exception != null)
            {
                await new MessageDialog(exception.Message, "错误").ShowAsync();
            }
            this.IsLoading = false;
        }
    }
}