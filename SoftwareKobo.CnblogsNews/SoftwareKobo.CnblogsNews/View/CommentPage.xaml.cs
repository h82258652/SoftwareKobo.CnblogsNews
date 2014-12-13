// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkID=390556 上有介绍

using System;
using System.Linq;
using Windows.UI.Xaml;
using GalaSoft.MvvmLight.Views;
using SoftwareKobo.CnblogsAPI.Extension;
using SoftwareKobo.CnblogsAPI.Model;
using Windows.Phone.UI.Input;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using SoftwareKobo.CnblogsNews.Service;

namespace SoftwareKobo.CnblogsNews.View
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class CommentPage : Page
    {
        public CommentPage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            HardwareButtons.BackPressed -= HardwareButtons_BackPressed;

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

            LoadComments(e.Parameter as News);

            base.OnNavigatedTo(e);
        }

        private void HardwareButtons_BackPressed(object sender, BackPressedEventArgs e)
        {
            if (Frame.CanGoBack)
            {
                Frame.GoBack();
                e.Handled = true;
            }
        }

        public async void LoadComments(News news)
        {
            Pb.Visibility = Visibility.Visible;

            if (news != null)
            {
                if (NetworkService.IsNetworkAvailable() == false)
                {
                    await new DialogService().ShowError("请检查网络连接。", "错误", "关闭", null);
                }
                else
                {
                    Exception exception = null;
                    try
                    {
                        var comments = await news.CommentAsync(1, int.MaxValue);
                        var index = 0;
                        var commentsWithIndex = from temp in comments
                                                select new
                                                {
                                                    Index = ++index,
                                                    Comment = temp
                                                };
                        Lv.ItemsSource = commentsWithIndex;
                    }
                    catch (Exception ex)
                    {
                        exception = ex;
                    }
                    if (exception != null)
                    {
                        await new DialogService().ShowError(exception, "错误", "关闭", null);
                    }
                }
            }

            Pb.Visibility = Visibility.Collapsed;
        }
    }
}