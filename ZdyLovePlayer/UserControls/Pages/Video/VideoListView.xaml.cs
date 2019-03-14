using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using ZdyLovePlayer.ViewModels.Pages;

namespace ZdyLovePlayer.UserControls.Pages.Video
{
    public partial class VideoListView : UserControl
    {
        public VideoListViewModel ViewModel => DataContext as VideoListViewModel;

        public VideoListView()
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
    }
}
