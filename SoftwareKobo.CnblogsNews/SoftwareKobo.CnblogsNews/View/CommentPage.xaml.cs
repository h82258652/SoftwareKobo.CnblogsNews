// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkID=390556 上有介绍

using System;
using System.Linq;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;
using GalaSoft.MvvmLight.Views;
using SoftwareKobo.CnblogsAPI.Extension;
using SoftwareKobo.CnblogsAPI.Model;
using Windows.Phone.UI.Input;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using SoftwareKobo.CnblogsAPI.Service;
using SoftwareKobo.CnblogsNews.Data;
using SoftwareKobo.CnblogsNews.Service;
using SoftwareKobo.CnblogsNews.ViewModel;

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

            var news = e.Parameter as News;
            var vm = this.DataContext as CommentPageViewModel;
            if (news != null && vm != null)
            {
                vm.News = news;
                vm.LoadComments();
            }

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

        private async void BtnSend_OnPointerReleased(object sender, PointerRoutedEventArgs e)
        {
            var comment = TxtComment.Text;
            comment = comment + Environment.NewLine + "——由博客园新闻WP8.1客户端发送";
            var vm = this.DataContext as CommentPageViewModel;
            if (vm == null)
            {
                return;
            }

            Exception exception = null;
            string result = null;
            try
            {
                result = await UserService.SendNewsCommentAsync(LocalSettings.LoginCookie, vm.News.Id, comment, 0);
            }
            catch (Exception ex)
            {
                exception = ex;
            }
            if (exception != null)
            {
                await new DialogService().ShowError(exception, "错误", "关闭", null);
            }
            bool isSuccess = UserService.IsSendNewsCommentSuccess(result);
            if (isSuccess)
            {
                await new DialogService().ShowMessage("发送成功", "成功");
                vm.LoadComments();
                TxtComment.Text = string.Empty;
            }
            else
            {
                await new DialogService().ShowMessage(result, "错误");
            }
        }
    }
}