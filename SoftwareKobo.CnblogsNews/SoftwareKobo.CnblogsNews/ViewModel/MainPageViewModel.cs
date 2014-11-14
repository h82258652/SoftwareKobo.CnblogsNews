using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Views;
using SoftwareKobo.CnblogsNews.Model;
using SoftwareKobo.CnblogsNews.Service;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.UI.Popups;
using Windows.UI.Xaml.Controls;

namespace SoftwareKobo.CnblogsNews.ViewModel
{
    public class MainPageViewModel : ViewModelBase
    {
        private int _currentPage;

        private bool _isLoading;

        private ObservableCollection<News> _newsItems;

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
                var news = await NewsService.DownloadNews(CurrentPage);
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
            var news = e.ClickedItem as News;
            if (news == null)
            {
                return;
            }
            Messenger.Default.Send<News>(news);
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