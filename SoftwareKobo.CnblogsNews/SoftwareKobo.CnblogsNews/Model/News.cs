using GalaSoft.MvvmLight;
using System;
using SoftwareKobo.CnblogsNews.Service;

namespace SoftwareKobo.CnblogsNews.Model
{
    public class News : ObservableObject
    {
        private int _commentCount;

        private string _publishTime;

        private string _title;

        public int CommentCount
        {
            get
            {
                return _commentCount;
            }
            set
            {
                _commentCount = value;
                RaisePropertyChanged(() => CommentCount);
            }
        }

        public string NewsId
        {
            get;
            set;
        }

        public string PublishTime
        {
            get
            {
                return _publishTime;
            }
            set
            {
                _publishTime = value;
                RaisePropertyChanged(() => PublishTime);
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
    }
}