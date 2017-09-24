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

namespace Zhu.UserControls.Home.Media.Seen
{
    /// <summary>
    /// SeenMediaListView.xaml 的交互逻辑
    /// </summary>
    public partial class SeenMediaListView : UserControl
    {
        public SeenMediaListView()
        {
            InitializeComponent();

            this.Loaded += SeenMediaListView_Loaded;
        }

        private async void SeenMediaListView_Loaded(object sender, RoutedEventArgs e)
        {
            var vm = DataContext as SeenListViewModel;
            vm.SearchOrderField = "ID";
            await vm.LoadMediasAsync();
        }
    }
}
