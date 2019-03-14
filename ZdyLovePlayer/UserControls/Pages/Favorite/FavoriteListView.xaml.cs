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
using ZdyLovePlayer.ViewModels.Pages;

namespace ZdyLovePlayer.UserControls.Pages.Favorite
{
    /// <summary>
    /// FavoriteListView.xaml 的交互逻辑
    /// </summary>
    public partial class FavoriteListView : UserControl
    {
        public FavoriteListViewModel ViewModel => DataContext as FavoriteListViewModel;

        public FavoriteListView()
        {
            InitializeComponent();

            this.Loaded += FavoriteListView_Loaded; ;
        }

        private void FavoriteListView_Loaded(object sender, RoutedEventArgs e)
        {
            this.CheckBox_SelectAll.IsChecked = false;

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
