// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=391641 上有介绍

using GalaSoft.MvvmLight.Messaging;
using SoftwareKobo.CnblogsNews.Model;
using System.Linq;
using Windows.Graphics.Display;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

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

        public void NavigateToNewsDetail(News news)
        {
            if (news != null)
            {
                Frame.Navigate(typeof(NewsDetailPage), news);
            }
        }

        public void ProcessMessageFromViewModel(string message)
        {
            if (message == "about")
            {
                Frame.Navigate(typeof(AboutPage));
            }
            else if (message == "scrolltop")
            {
                LvwNews.ScrollIntoView(LvwNews.Items.FirstOrDefault());
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
                Grid.SetRow(LvwNews, 1);
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
                Grid.SetColumn(LvwNews, 1);
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
                Grid.SetColumn(LvwNews, 0);
                SpPage.Orientation = Orientation.Vertical;
            }
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            DisplayInformation.GetForCurrentView().OrientationChanged -= OrientationChanged;

            Messenger.Default.Unregister<News>(this, NavigateToNewsDetail);

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

            Messenger.Default.Register<News>(this, NavigateToNewsDetail);

            Messenger.Default.Register<string>(this, ProcessMessageFromViewModel);

            base.OnNavigatedTo(e);
        }

        private void OrientationChanged(DisplayInformation sender, object args)
        {
            RenderLayoutWithOrientation(sender.CurrentOrientation);
        }
    }
}