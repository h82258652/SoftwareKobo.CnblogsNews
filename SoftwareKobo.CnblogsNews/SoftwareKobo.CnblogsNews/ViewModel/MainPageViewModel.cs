using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System.Windows.Input;

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
    }
}