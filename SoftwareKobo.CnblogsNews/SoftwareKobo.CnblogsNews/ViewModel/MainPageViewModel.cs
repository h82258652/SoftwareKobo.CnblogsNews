using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using SoftwareKobo.CnblogsNews.Model;
using SoftwareKobo.CnblogsNews.Service;
using System;
using System.Windows.Input;
using Windows.UI.Popups;

namespace SoftwareKobo.CnblogsNews.ViewModel
{
    public class MainPageViewModel : ViewModelBase
    {
        private int _currentPage;

        public MainPageViewModel()
        {
            CurrentPage = 1;
            GetNews();
        }
        
        public ICommand BackCommand
        {
            get
            {
                return new RelayCommand(BackCommandExecute, BackCommandCanExecute);
            }
        }

        public ICommand RefreshCommand
        {
            get
            {
                return new RelayCommand(RefreshCommandExecute);
            }
        }

        public async void RefreshCommandExecute()
        {
            if (IsLoading == false)
            {
                await GetNews();
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

        private void ScrollView()
        {
            Messenger.Default.Send<string>("scrolltop");
        }

        public async Task GetNews()
        {
            if (NetworkService.IsNetworkAvailable() == false)
            {
                await new MessageDialog("请检查网络连接。").ShowAsync();
                return;
            }
            this.IsLoading = true;
            try
            {
                var news = await NewsService.DownloadNews(CurrentPage);
                ScrollView();
                this.NewsItems = news;
            }
            catch (Exception exception)
            {
                new MessageDialog(exception.Message, "错误").ShowAsync().GetAwaiter().GetResult();
            }
            this.IsLoading = false;
        }

        public async void ForwardCommandExecute()
        {
            if (IsLoading == false)
            {
                CurrentPage++;
                await GetNews();
            }
        }

        public ICommand JumpPageCommand
        {
            get
            {
                return new RelayCommand(JumpPageCommandExecute);
            }
        }

        private bool _isLoading;

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
                    await new MessageDialog("请输入大于 0，小于等于 100 的整数。").ShowAsync();
                }
            }
        }

        private ObservableCollection<News> _newsItems;

        public ObservableCollection<News> NewsItems
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

        public ICommand NewsItemClickCommand
        {
            get
            {
                return new RelayCommand<ItemClickEventArgs>(NewsItemClickCommandExecute);
            }
        }

        public void NewsItemClickCommandExecute(ItemClickEventArgs e)
        {
            if (e == null || e.ClickedItem == null)
            {
                return;
            }
            var news = e.ClickedItem as News;
            if (news == null)
            {
                return;
            }
            Messenger.Default.Send<News>(news);
        }

        public override void Cleanup()
        {
            NewsItems = null;

            base.Cleanup();
        }
    }
}