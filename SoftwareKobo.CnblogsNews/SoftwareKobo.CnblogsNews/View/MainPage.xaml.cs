// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=391641 上有介绍

using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Linq;
using Windows.Graphics.Display;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using SoftwareKobo.CnblogsAPI.Model;

namespace SoftwareKobo.CnblogsNews.View
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();

            this.NavigationCacheMode = NavigationCacheMode.Required;
        }

        public void ProcessMessageFromViewModel(string message)
        {
            if (message == "about")
            {
                Frame.Navigate(typeof(AboutPage));
            }
            else if (message == "scrolltop")
            {
                var item = lvwNews.Items.FirstOrDefault();
                if (item != null)
                {
                    lvwNews.ScrollIntoView(item, ScrollIntoViewAlignment.Leading);
                }
            }
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

        public void RenderLayoutWithOrientation(DisplayOrientations orientation)
        {
            if (orientation == DisplayOrientations.Portrait)
            {
                GdLayout.Margin = new Thickness(0, -26.667, 0, 0);
                GdLayout.ColumnDefinitions.Clear();
                GdLayout.RowDefinitions.Clear();
                GdLayout.RowDefinitions.Add(new RowDefinition()
                {
                    Height = new GridLength(26.667, GridUnitType.Pixel)
                });
                GdLayout.RowDefinitions.Add(new RowDefinition()
                {
                    Height = new GridLength(1, GridUnitType.Star)
                });
                Grid.SetRow(SpPage, 0);
                Grid.SetRow(lvwNews, 1);
                SpPage.Orientation = Orientation.Horizontal;
            }
            else if (orientation == DisplayOrientations.Landscape)
            {
                GdLayout.Margin = new Thickness(-60, 0, 0, 0);
                GdLayout.ColumnDefinitions.Clear();
                GdLayout.RowDefinitions.Clear();
                GdLayout.ColumnDefinitions.Add(new ColumnDefinition()
                {
                    Width = new GridLength(26.667, GridUnitType.Pixel)
                });
                GdLayout.ColumnDefinitions.Add(new ColumnDefinition()
                {
                    Width = new GridLength(1, GridUnitType.Star)
                });
                Grid.SetColumn(SpPage, 0);
                Grid.SetColumn(lvwNews, 1);
                SpPage.Orientation = Orientation.Vertical;
            }
            else if (orientation == DisplayOrientations.LandscapeFlipped)
            {
                GdLayout.Margin = new Thickness(0, 0, -60, 0);
                GdLayout.ColumnDefinitions.Clear();
                GdLayout.RowDefinitions.Clear();
                GdLayout.ColumnDefinitions.Add(new ColumnDefinition()
                {
                    Width = new GridLength(1, GridUnitType.Star)
                });
                GdLayout.ColumnDefinitions.Add(new ColumnDefinition()
                {
                    Width = new GridLength(26.667, GridUnitType.Pixel)
                });
                Grid.SetColumn(SpPage, 1);
                Grid.SetColumn(lvwNews, 0);
                SpPage.Orientation = Orientation.Vertical;
            }
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            DisplayInformation.GetForCurrentView().OrientationChanged -= OrientationChanged;

            Messenger.Default.Unregister<Tuple<string, News>>(this, ProcessMessageFromViewModel);

            Messenger.Default.Unregister<string>(this, ProcessMessageFromViewModel);

            base.OnNavigatedFrom(e);
        }

        /// <summary>
        /// 在此页将要在 Frame 中显示时进行调用。
        /// </summary>
        /// <param name="e">描述如何访问此页的事件数据。
        /// 此参数通常用于配置页。</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            DisplayInformation.GetForCurrentView().OrientationChanged += OrientationChanged;

            RenderLayoutWithOrientation(DisplayInformation.GetForCurrentView().CurrentOrientation);

            Messenger.Default.Register<Tuple<string, News>>(this, ProcessMessageFromViewModel);

            Messenger.Default.Register<string>(this, ProcessMessageFromViewModel);

            base.OnNavigatedTo(e);
        }

        private void OrientationChanged(DisplayInformation sender, object args)
        {
            RenderLayoutWithOrientation(sender.CurrentOrientation);
        }

        private void News_OnHolding(object sender, HoldingRoutedEventArgs e)
        {
            var frameworkElement = sender as FrameworkElement;
            if (frameworkElement != null)
            {
                FlyoutBase.ShowAttachedFlyout(frameworkElement);
            }
        }

        private void BtnSetting_OnClick(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof (SettingPage));
        }

        private void BtnAbout_OnClick(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof (AboutPage));
        }
    }
}