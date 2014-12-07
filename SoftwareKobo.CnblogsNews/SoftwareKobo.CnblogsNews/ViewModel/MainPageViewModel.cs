using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Views;
using SoftwareKobo.CnblogsNews.Service;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.UI.Xaml.Controls;
using News = SoftwareKobo.CnblogsAPI.Model.News;

namespace SoftwareKobo.CnblogsNews.ViewModel
{
    public class MainPageViewModel : ViewModelBase
    {
        private int _currentPage;

        private bool _isLoading;

        private ObservableCollection<CnblogsAPI.Model.News> _newsItems;

        public MainPageViewModel()
        {
            OnCreated();
        }

        public ICommand AboutCommand
        {
            get
            {
                return new RelayCommand(AboutCommandExecute);
            }
        }

        public ICommand BackCommand
        {
            get
            {
                return new RelayCommand(BackCommandExecute, BackCommandCanExecute);
            }
        }

        public int CurrentPage
        {
            get
            {
                return _currentPage;
            }
            set
            {
                _currentPage = value;
                RaisePropertyChanged(() => CurrentPage);
                RaisePropertyChanged(() => BackCommand);
            }
        }

        public ICommand ForwardCommand
        {
            get
            {
                return new RelayCommand(ForwardCommandExecute, ForwardCommandCanExecute);
            }
        }

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

        public ICommand JumpPageCommand
        {
            get
            {
                return new RelayCommand(JumpPageCommandExecute);
            }
        }

        public ICommand NewsItemClickCommand
        {
            get
            {
                return new RelayCommand<ItemClickEventArgs>(NewsItemClickCommandExecute);
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

        public ObservableCollection<CnblogsAPI.Model.News> NewsItems
        {
            get
            {
                return _newsItems;
            }
            set
            {
                _newsItems = value;
                RaisePropertyChanged(() => NewsItems);
            }
        }

        public ICommand RefreshCommand
        {
            get
            {
                return new RelayCommand(RefreshCommandExecute);
            }
        }

        public void AboutCommandExecute()
        {
            Messenger.Default.Send<string>("about");
        }

        public bool BackCommandCanExecute()
        {
            return CurrentPage > 1;
        }

        public async void BackCommandExecute()
        {
            if (IsLoading == false)
            {
                CurrentPage--;
                await GetNews();
            }
        }

        public bool ForwardCommandCanExecute()
        {
            return CurrentPage < 100;
        }

        public async void ForwardCommandExecute()
        {
            if (IsLoading == false)
            {
                CurrentPage++;
                await GetNews();
            }
        }

        public async Task GetNews()
        {
            if (NetworkService.IsNetworkAvailable() == false)
            {
                await new DialogService().ShowError("请检查网络连接。", "错误", "关闭", null);
                return;
            }
            this.IsLoading = true;
            Exception exception = null;
            try
            {
                var news = new ObservableCollection<CnblogsAPI.Model.News>(await CnblogsAPI.Service.NewsService.RecentAsync(CurrentPage, 15));
                ScrollView();
                this.NewsItems = news;
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

        public async void JumpPageCommandExecute()
        {
            var page = await JumpPageService.GetPage();
            if (page.HasValue)
            {
                if (page.Value > 0 && page.Value <= 100)
                {
                    CurrentPage = page.Value;
                    await GetNews();
                }
                else
                {
                    await new DialogService().ShowMessageBox("请输入大于 0，小于等于 100 的整数。", "页码错误");
                }
            }
        }

        public void NewsItemClickCommandExecute(ItemClickEventArgs e)
        {
            if (e == null || e.ClickedItem == null)
            {
                return;
            }
            var news = e.ClickedItem as CnblogsAPI.Model.News;
            if (news == null)
            {
                return;
            }
            Messenger.Default.Send<Tuple<string, News>>(new Tuple<string, News>("detail", news));
        }

        public async void OnCreated()
        {
            CurrentPage = 1;
            await GetNews();
        }

        public async void RefreshCommandExecute()
        {
            if (IsLoading == false)
            {
                await GetNews();
            }
        }

        private void ScrollView()
        {
            Messenger.Default.Send<string>("scrolltop");
        }
    }
}