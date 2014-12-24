// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkID=390556 上有介绍

using GalaSoft.MvvmLight.Messaging;
using SoftwareKobo.CnblogsNews.ViewModel;
using System;
using Windows.Phone.UI.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using News = SoftwareKobo.CnblogsAPI.Model.News;

namespace SoftwareKobo.CnblogsNews.View
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class NewsDetailPage : Page
    {
        public NewsDetailPage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            HardwareButtons.BackPressed -= HardwareButtons_BackPressed;

            Messenger.Default.Unregister<Tuple<string, News>>(this, ProcessMessageFromViewModel);

            base.OnNavigatedFrom(e);
        }

        /// <summary>
        /// 在此页将要在 Frame 中显示时进行调用。
        /// </summary>
        /// <param name="e">描述如何访问此页的事件数据。
        /// 此参数通常用于配置页。</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            HardwareButtons.BackPressed += HardwareButtons_BackPressed;

            var news = e.Parameter as News;
            var vm = this.DataContext as NewsDetailPageViewModel;
            if (news != null && vm != null)
            {
                vm.News = news;
                vm.Render(news);
            }

            Messenger.Default.Register<Tuple<string, News>>(this, ProcessMessageFromViewModel);

            base.OnNavigatedTo(e);
        }

        public void ProcessMessageFromViewModel(Tuple<string, News> tuple)
        {
            if (tuple == null)
            {
                throw new ArgumentNullException("tuple");
            }
            var message = tuple.Item1;
            var news = tuple.Item2;
            if (message == null || news == null)
            {
                throw new ArgumentException("tuple 元素存在空。", "tuple");
            }
            if (message == "detail")
            {
                Frame.Navigate(typeof(NewsDetailPage), news);
            }
            else if (message == "comment")
            {
                Frame.Navigate(typeof(CommentPage), news);
            }
        }

        private void HardwareButtons_BackPressed(object sender, BackPressedEventArgs e)
        {
            if (Frame.CanGoBack)
            {
                Frame.GoBack();
                e.Handled = true;
            }
        }
    }
}