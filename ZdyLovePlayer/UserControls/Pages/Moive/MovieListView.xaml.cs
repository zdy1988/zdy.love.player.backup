using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ZdyLovePlayer.ViewModels.Pages;

namespace ZdyLovePlayer.UserControls.Pages.Moive
{
    /// <summary>
    /// MovieListView.xaml 的交互逻辑
    /// </summary>
    public partial class MovieListView : UserControl
    {
        public MovieListViewModel ViewModel => DataContext as MovieListViewModel;

        public MovieListView()
        {
            InitializeComponent();
            this.Loaded += MovieLibrary_Loaded;
        }

        private void MovieLibrary_Loaded(object sender, RoutedEventArgs e)
        {
            if (ViewModel != null && !ViewModel.IsDataFound)
            {
                ViewModel.OrderField = "ID";
                ViewModel.ExecuteLoadMedias();
            }
        }

        private void ReachingScrollViewerBottomBehavior_ReachingBottomEvent()
        {
            if (ViewModel != null && !ViewModel.IsDataLoading)
            {
                ViewModel.ExecuteLoadMedias();
            }
        }

        private void SearchTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                ViewModel.SearchQuery = ((TextBox)sender).Text.Trim();
                ViewModel.SearchMediaCommand.Execute(null);
            }
        }
    }
}
