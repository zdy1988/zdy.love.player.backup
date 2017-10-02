using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Zhu.ViewModels.Pages;

namespace Zhu.UserControls.Home.Media.Moive
{
    /// <summary>
    /// MovieListView.xaml 的交互逻辑
    /// </summary>
    public partial class MovieListView : UserControl
    {
        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);

        public MovieListView()
        {
            InitializeComponent();
            this.Loaded += MovieLibrary_Loaded;
        }

        private async void MovieLibrary_Loaded(object sender, RoutedEventArgs e)
        {
            var vm = DataContext as MovieListViewModel;
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
            var vm = DataContext as MovieListViewModel;
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
                var vm = DataContext as MovieListViewModel;
                vm.SearchText = ((TextBox)sender).Text.Trim();
                vm.SearchMediaCommand.Execute(null);
            }
        }
    }
}
