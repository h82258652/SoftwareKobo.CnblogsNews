using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
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

        public void RefreshCommandExecute()
        {
            // TODO
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

        public void BackCommandExecute()
        {
            // TODO
        }

        public bool ForwardCommandCanExecute()
        {
            return CurrentPage < 100;
        }

        public void ForwardCommandExecute()
        {
            // TODO
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
                    // TODO
                }
                else
                {
                    await new MessageDialog("请输入大于 0，小于等于 100 的整数。").ShowAsync();
                }
            }
        }
    }
}