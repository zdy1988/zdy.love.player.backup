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
using ZdyLovePlayer.ViewModels.Main;

namespace ZdyLovePlayer.UserControls.Main
{
    /// <summary>
    /// LeftSide.xaml 的交互逻辑
    /// </summary>
    public partial class LeftSide : UserControl
    {
        private MainWindowViewModel ViewModel => DataContext as MainWindowViewModel;

        public LeftSide()
        {
            InitializeComponent();
        }

        private void TextBox_CreateMediaGroup_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (!TextBox_CreateMediaGroup.IsFocused)
            {
                TextBox_CreateMediaGroup.Focus();
            }
        }

        private void TextBox_CreateMediaGroup_LostFocus(object sender, RoutedEventArgs e)
        {
            if (ViewModel != null)
            {
                ViewModel.CreateOrCancelMediaGroupCommand.Execute(null);
            }
        }

        private void TextBox_CreateMediaGroup_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Button_PreCreateMediaGroup.Focus();
            }
        }
    }
}
