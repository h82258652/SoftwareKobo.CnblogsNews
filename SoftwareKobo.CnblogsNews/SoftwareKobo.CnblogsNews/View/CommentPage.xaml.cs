// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkID=390556 上有介绍

using System;
using System.Linq;
using Windows.UI.Popups;
using Windows.UI.ViewManagement;
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
using SoftwareKobo.CnblogsNews.Helper;
using SoftwareKobo.CnblogsNews.Model;
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

            this.DataContext = new CommentPageViewModel();
        }

        public CommentPageViewModel ViewModel
        {
            get
            {
                return (CommentPageViewModel)this.DataContext;
            }
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
            if (news != null)
            {
                ViewModel.News = news;
                ViewModel.LoadComments();
            }

            if (LocalSettings.LoginCookie == null)
            {
                btnNewComment.Visibility = Visibility.Collapsed;
                CmdBar.ClosedDisplayMode = AppBarClosedDisplayMode.Minimal;
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

        private void BtnNewComment_OnClick(object sender, RoutedEventArgs e)
        {
            var frameworkElement = sender as FrameworkElement;
            if (frameworkElement != null)
            {
                tbComment.Text = string.Empty;
                btnSendComment.Content = "发送";
                btnNewComment.Tag = null;
                FlyoutBase.ShowAttachedFlyout(frameworkElement);
            }
        }

        private async void BtnSendComment_Click(object sender, RoutedEventArgs e)
        {
            await StatusBarHelper.Display(true);

            var comment = tbComment.Text;
            comment = comment + Environment.NewLine + LocalSettings.LittleTail;
            if (comment.Length < 3)
            {
                comment = comment.PadRight(3);
            }

            Exception exception = null;
            string result = null;
            try
            {
                int replyId = 0;
                if (btnNewComment.Tag is int)
                {
                    replyId = (int)btnNewComment.Tag;
                }
                result = await UserService.SendNewsCommentAsync(LocalSettings.LoginCookie, ViewModel.News.Id, comment, replyId);
            }
            catch (Exception ex)
            {
                exception = ex;
            }
            if (exception != null)
            {
                await new DialogService().ShowError(exception, "错误", "关闭", null);
            }
            else
            {
                bool isSuccess = UserService.IsSendNewsCommentSuccess(result);
                if (isSuccess)
                {
                    if (btnNewComment.Tag is int)
                    {
                        await new DialogService().ShowMessage("回复成功", "成功");
                    }
                    else
                    {
                        await new DialogService().ShowMessage("发送成功", "成功");
                    }
                    ViewModel.LoadComments();// 刷新评论。
                    tbComment.Text = string.Empty;// 清空已发送的内容。
                    btnNewComment.Tag = null;// 清空回复 Id。
                }
                else
                {
                    await new DialogService().ShowMessage(result, "错误");
                }
            }

            await StatusBarHelper.Display(false);
        }

        private void MenuFlyoutItem_OnClick(object sender, RoutedEventArgs e)
        {
            var menuFlyoutItem = sender as MenuFlyoutItem;
            if (menuFlyoutItem == null)
            {
                return;
            }
            var commentWithIndex = menuFlyoutItem.DataContext as CommentWithIndex;
            if (commentWithIndex == null)
            {
                return;
            }
            var comment = commentWithIndex.Comment;
            if (comment == null)
            {
                return;
            }

            tbComment.Text = "@" + comment.Author.Name + Environment.NewLine;
            btnSendComment.Content = "回复";
            btnNewComment.Tag = comment.Id;
            FlyoutBase.ShowAttachedFlyout(btnNewComment);
        }

        private void tbComment_OnGotFocus(object sender, RoutedEventArgs e)
        {
            tbComment.SelectionStart = tbComment.Text.Length;
        }

        private void Flyout_OnOpened(object sender, object e)
        {
            CmdBar.Visibility = Visibility.Collapsed;
        }

        private void Flyout_OnClosed(object sender, object e)
        {
            CmdBar.Visibility = Visibility.Visible;
        }

        private void Comment_OnTapped(object sender, TappedRoutedEventArgs e)
        {
            var frameworkElement = sender as FrameworkElement;
            if (frameworkElement != null && LocalSettings.LoginCookie != null)
            {
                FlyoutBase.ShowAttachedFlyout(frameworkElement);
            }
        }
    }
}