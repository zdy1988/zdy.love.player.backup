using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Zhu.ViewModels.Pages;

namespace Zhu.UserControls.Home.Media.NetTV
{
    /// <summary>
    /// NetTVListView.xaml 的交互逻辑
    /// </summary>
    public partial class NetTVListView : UserControl
    {
        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);

        public NetTVListView()
        {
            InitializeComponent();

            this.Loaded += NetTVListView_Loaded;
        }

        private async void NetTVListView_Loaded(object sender, RoutedEventArgs e)
        {
            var vm = DataContext as NetTVListViewModel;
            vm.OrderField = "ID";
            await vm.LoadMediasAsync().ConfigureAwait(false);
        }

        private async void ScrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            var totalHeight = e.VerticalOffset + e.ViewportHeight;
            if (e.VerticalChange <= 0 || totalHeight < 2d / 3d * e.ExtentHeight)
            {
                return;
            }

            if (_semaphore.CurrentCount == 0)
            {
                return;
            }

            await _semaphore.WaitAsync();
            var vm = DataContext as NetTVListViewModel;
            if (vm == null)
            {
                _semaphore.Release();
                return;
            }

            if (!vm.IsDataLoading)
            {
                await vm.LoadMediasAsync().ConfigureAwait(false);
            }

            _semaphore.Release();

        }

        private void SearchTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                var vm = DataContext as NetTVListViewModel;
                vm.SearchText = ((TextBox)sender).Text.Trim();
                vm.SearchMediaCommand.Execute(null);
            }
        }

        private void RatingBar_ValueChanged(object sender, EventArgs e)
        {
            var ratingBar = (MaterialDesignThemes.Wpf.RatingBar)sender;
            var media = (Models.Media)ratingBar.Tag;
            media.Rating = ratingBar.Value;
            var vm = DataContext as NetTVListViewModel;
            if (vm != null)
                vm.SetMediaRatingCommand.Execute(media);
        }
    }
}
