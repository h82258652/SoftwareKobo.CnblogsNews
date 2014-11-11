using GalaSoft.MvvmLight;
using System;

namespace SoftwareKobo.CnblogsNews.Model
{
    public class News : ObservableObject
    {
        private string _publishTime;

        private string _title;

        public Uri DetailLink
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