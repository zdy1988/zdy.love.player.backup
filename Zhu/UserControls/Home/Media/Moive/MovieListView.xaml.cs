using System;
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
        public MovieListView()
        {
            InitializeComponent();
            this.Loaded += MovieLibrary_Loaded;
        }

        private bool isLoaded = false;

        private void MovieLibrary_Loaded(object sender, RoutedEventArgs e)
        {
            if (isLoaded) return; else isLoaded = true;

            this.ComboBox_MediaSort.SelectedIndex = 0;
            this.ListBox_SelectCountry.SelectedIndex = 0;
            this.ListBox_SelectImageQuality.SelectedIndex = 0;
            this.ListBox_SelectMediaLength.SelectedIndex = 0;

            var vm = DataContext as MovieListViewModel;
            vm.LoadMediaCommand.Execute(null);
        }

        private void ScrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            var totalHeight = e.VerticalOffset + e.ViewportHeight;
            if (!totalHeight.Equals(e.ExtentHeight)) return;

            var vm = DataContext as MovieListViewModel;
            if (vm != null && !vm.IsLoadingMedias)
                vm.LoadMediaCommand.Execute(null);

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

        private void RatingBar_ValueChanged(object sender, EventArgs e)
        {
            var ratingBar = (MaterialDesignThemes.Wpf.RatingBar)sender;
            var media = (Models.Media)ratingBar.Tag;
            media.Rating = ratingBar.Value;
            var vm = DataContext as MovieListViewModel;
            if (vm != null)
                vm.SetMediaRatingCommand.Execute(media);
        }
    }
}
