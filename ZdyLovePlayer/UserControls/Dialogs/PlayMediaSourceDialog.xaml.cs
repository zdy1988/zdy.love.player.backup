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
using ZdyLovePlayer.Untils;

namespace ZdyLovePlayer.UserControls.Dialogs
{
    /// <summary>
    /// MovieSelectDialog.xaml 的交互逻辑
    /// </summary>
    public partial class PlayMediaSourceDialog : UserControl
    {
        public PlayMediaSourceDialog()
        {
            InitializeComponent();
        }

        private void FilePathTextBox_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dialog = new Microsoft.Win32.OpenFileDialog();
            dialog.Filter = Constants.VedioFilters;
            if (dialog.ShowDialog() == true)
            {
                TextBox_FilePath.Text = dialog.FileName;
            }
        }
    }
}
