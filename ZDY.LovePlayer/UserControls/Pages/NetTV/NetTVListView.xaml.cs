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
using ZDY.LovePlayer.ViewModels.Pages;

namespace ZDY.LovePlayer.UserControls.Pages.NetTV
{
    /// <summary>
    /// NetTVListView.xaml 的交互逻辑
    /// </summary>
    public partial class NetTVListView : UserControl
    {
        public NetTVListViewModel ViewModel => DataContext as NetTVListViewModel;

        public NetTVListView()
        {
            InitializeComponent();

            this.Loaded += NetTVListView_Loaded;
        }

        private void NetTVListView_Loaded(object sender, RoutedEventArgs e)
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
