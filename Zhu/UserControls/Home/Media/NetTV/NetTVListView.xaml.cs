using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
        public NetTVListView()
        {
            InitializeComponent();

            this.Loaded += NetTVListView_Loaded;
        }

        private bool isLoaded = false;

        private void NetTVListView_Loaded(object sender, RoutedEventArgs e)
        {
            if (!isLoaded)
            {
                isLoaded = true;

                var vm = DataContext as NetTVListViewModel;
                vm.SearchOrderField = "ID";
                vm.LoadMediasAsync().ConfigureAwait(false);
            }
        }

        private void ScrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            var totalHeight = e.VerticalOffset + e.ViewportHeight;
            if (!totalHeight.Equals(e.ExtentHeight)) return;

            var vm = DataContext as NetTVListViewModel;
            if (vm != null && !vm.IsDataLoading)
                vm.LoadMediasAsync().ConfigureAwait(false);

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
