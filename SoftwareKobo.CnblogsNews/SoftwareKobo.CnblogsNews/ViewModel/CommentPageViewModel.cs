using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.UI.Xaml;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Views;
using SoftwareKobo.CnblogsAPI.Extension;
using SoftwareKobo.CnblogsAPI.Model;
using SoftwareKobo.CnblogsNews.Helper;
using SoftwareKobo.CnblogsNews.Model;
using SoftwareKobo.CnblogsNews.Service;

namespace SoftwareKobo.CnblogsNews.ViewModel
{
    public class CommentPageViewModel : ViewModelBase
    {
        public News News
        {
            get;
            set;
        }

        private ObservableCollection<Comment> _comments;

        public ObservableCollection<Comment> Comments
        {
            get
            {
                return _comments;
            }
            set
            {
                _comments = value;
                RaisePropertyChanged(() => Comments);
                RaisePropertyChanged(() => CommentsWithIndex);
                RaisePropertyChanged(() => NoComments);
            }
        }

        public Visibility NoComments
        {
            get
            {
                if (Comments == null)
                {
                    return Visibility.Collapsed;
                }
                return Comments.Count <= 0 ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        public List<CommentWithIndex> CommentsWithIndex
        {
            get
            {
                if (_comments == null)
                {
                    return null;
                }
                var index = 0;
                return new List<CommentWithIndex>(from temp in _comments
                                                  select new CommentWithIndex()
                                                  {
                                                      Index = ++index,
                                                      Comment = temp
                                                  });
            }
        }

        public async void LoadComments()
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
                var comments = await News.CommentAsync(1, int.MaxValue);
                Comments = new ObservableCollection<Comment>(comments);
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

        public ICommand RefreshCommand
        {
            get
            {
                return new RelayCommand(LoadComments);
            }
        }
    }
}
