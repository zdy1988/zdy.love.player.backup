using Zhu.ViewModels.FileExplorer;
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

namespace Zhu.UserControls
{
    /// <summary>
    /// FileExplorer.xaml 的交互逻辑
    /// </summary>
    public partial class FileExplorer : UserControl
    {
        public FileExplorer()
        {
            InitializeComponent();
        }

        public FileExplorerViewModel ViewModel => DataContext as FileExplorerViewModel;

        private void TreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (ViewModel == null) return;

            ViewModel.SelectedItem = (LocalDirectoryInfo)e.NewValue;
        }

        private void DataGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            LocalFileInfo fileInfo = (LocalFileInfo)e.Row.DataContext;
            if (e.Column.DisplayIndex == 1)
            {
                string directoryName = fileInfo.File.DirectoryName + "\\";
                string oldFileName = fileInfo.FileName;
                string newFileName = (e.EditingElement as TextBox).Text;
                if (System.IO.File.Exists(directoryName + oldFileName))
                {
                    System.IO.File.Move(directoryName + oldFileName, directoryName + newFileName);
                }
            }
        }
    }
}
