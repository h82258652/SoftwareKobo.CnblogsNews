using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Phone.UI.Input;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkID=390556 上有介绍
using GalaSoft.MvvmLight.Views;
using SoftwareKobo.CnblogsAPI.Service;
using SoftwareKobo.CnblogsNews.Data;
using SoftwareKobo.CnblogsNews.Helper;

namespace SoftwareKobo.CnblogsNews.View
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class SettingPage : Page
    {
        public SettingPage()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// 在此页将要在 Frame 中显示时进行调用。
        /// </summary>
        /// <param name="e">描述如何访问此页的事件数据。
        /// 此参数通常用于配置页。</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            HardwareButtons.BackPressed += HardwareButtons_BackPressed;

            InitRenderingEngineSelection();

            InitLoginGrid();

            base.OnNavigatedTo(e);
        }

        void HardwareButtons_BackPressed(object sender, BackPressedEventArgs e)
        {
            if (Frame.CanGoBack)
            {
                Frame.GoBack();
                e.Handled = true;
            }
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            HardwareButtons.BackPressed -= HardwareButtons_BackPressed;

            base.OnNavigatedFrom(e);
        }

        private void Selector_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CmbInter != null && CmbInter.IsSelected)
            {
                LocalSettings.RenderingEngine = RenderingEngine.Inter;
            }
            if (CmbBrowser != null && CmbBrowser.IsSelected)
            {
                LocalSettings.RenderingEngine = RenderingEngine.Browser;
            }
        }

        private void InitRenderingEngineSelection()
        {
            switch (LocalSettings.RenderingEngine)
            {
                case RenderingEngine.Inter:
                    CmbEngine.SelectedItem = CmbInter;
                    break;
                case RenderingEngine.Browser:
                    CmbEngine.SelectedItem = CmbBrowser;
                    break;
            }
        }

        private void InitLoginGrid()
        {
            if (LocalSettings.LoginCookie == null)
            {
                GridLogin.Visibility = Visibility.Visible;
                GridLogout.Visibility = Visibility.Collapsed;
            }
            else
            {
                GridLogin.Visibility = Visibility.Collapsed;
                GridLogout.Visibility = Visibility.Visible;
            }
        }

        private async void BtnLogin_OnClick(object sender, RoutedEventArgs e)
        {
            var userName = TxtUserName.Text;
            var password = TxtPassword.Password;

            Cookie cookie = null;
            Exception exception = null;
            try
            {
                await StatusBarHelper.Display(true, "正在登录");
                cookie = await UserService.LoginAsync(userName, password);
                await StatusBarHelper.Display(false);
            }
            catch (Exception ex)
            {
                exception = ex;
            }
            if (exception != null)
            {
                await new DialogService().ShowError(exception, "登录失败", "关闭", null);
                return;
            }
            if (cookie == null)
            {
                await new DialogService().ShowMessage("用户名或密码错误", "登录失败");
            }
            else
            {
                LocalSettings.LoginCookie = cookie;
                GridLogin.Visibility = Visibility.Collapsed;
                GridLogout.Visibility = Visibility.Visible;
            }
        }

        private void BtnLogout_OnClick(object sender, RoutedEventArgs e)
        {
            LocalSettings.LoginCookie = null;
            GridLogin.Visibility = Visibility.Visible;
            GridLogout.Visibility = Visibility.Collapsed;
        }
    }
}
