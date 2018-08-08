using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.ComponentModel;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Threading;
using Zhu.Untils;
using Zhu.ViewModels.Main;
using Zhu.Messaging;
using Zhu.Models;

namespace Zhu.Windows
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindowViewModel ViewModel => DataContext as MainWindowViewModel;

        public MainWindow()
        {
            InitializeComponent();


            Loaded += MainWindow_Loaded;
            KeyDown += MainWindow_KeyDown;
            SizeChanged += MainWindow_SizeChanged;
        }



        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            InitializeTaskBarNotify();
            InitializeMessageNotice();

            Task.Factory
                .StartNew(async () => await SQLiteDatabase.Initialize())
                .ContinueWith(async (t) =>
                {
                    if (ViewModel != null)
                    {
                        await ViewModel.LoadMediaGroup();
                    }

                }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        private void MainWindow_KeyDown(object sender, KeyEventArgs e)
        {
            if ((e.KeyboardDevice.Modifiers & ModifierKeys.Control) == ModifierKeys.Control && e.Key == Key.E)
            {
                this.Close();
            }
        }

        private void MainWindow_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            this.Player.Width = this.ActualWidth;
        }

        #region MessageNotice

        private void InitializeMessageNotice()
        {
            ViewModel.MessageNotice += Vm_MessageNotice;
        }

        private void Vm_MessageNotice(object sender, string message)
        {
            DispatcherHelper.CheckBeginInvokeOnUI(() =>
            {
                MainSnackbar.MessageQueue.Enqueue(message);
            });
        }

        #endregion

        #region DragWindow

        private void TopBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }

        #endregion

        #region DragFile

        private void PlayerMain_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effects = DragDropEffects.Link;
            else e.Effects = DragDropEffects.None;
        }

        private void PlayerMain_Drop(object sender, DragEventArgs e)
        {
            string fileName = ((System.Array)e.Data.GetData(DataFormats.FileDrop)).GetValue(0).ToString();

            Messenger.Default.Send(new OpenMediaMessage(new Media { MediaSource = fileName }));
        }

        #endregion

        #region WindowClose

        private void WindowClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            notifyIcon.Dispose();
            base.OnClosing(e);
        }

        #endregion

        #region WindowMinimized

        private void WindowMinimized_Click(object sender, RoutedEventArgs e)
        {
            this.Visibility = Visibility.Hidden;
        }

        System.Windows.Forms.NotifyIcon notifyIcon;
        System.Windows.Controls.ContextMenu notifyIconMenu;

        private void InitializeTaskBarNotify()
        {
            string iconPath = System.IO.Path.GetFullPath(@"favicon.ico");
            if (File.Exists(iconPath))
            {
                notifyIcon = new System.Windows.Forms.NotifyIcon();
                notifyIcon.Text = Untils.Constants.ProductName;
                notifyIcon.Icon = new System.Drawing.Icon(iconPath);
                notifyIcon.Visible = true;
                notifyIcon.MouseDoubleClick += NotifyIcon_MouseDoubleClick;
                notifyIcon.MouseClick += NotifyIcon_MouseClick;

                notifyIconMenu = (System.Windows.Controls.ContextMenu)this.FindResource("NotifyIconMenu");
                notifyIconMenu.LostFocus += NotifyIconMenu_LostFocus;
            }
        }

        private void NotifyIconMenu_LostFocus(object sender, RoutedEventArgs e)
        {
            notifyIconMenu.IsOpen = false;
        }

        private void NotifyIcon_MouseClick(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                notifyIconMenu.IsOpen = true;
                notifyIconMenu.Focus();
            }
        }

        private void NotifyIcon_MouseDoubleClick(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (this.Visibility == Visibility.Hidden)
            {
                this.Visibility = Visibility.Visible;
                this.Activate();
            }
            else
            {
                this.Visibility = Visibility.Hidden;
            }
        }

        #endregion

        #region WindowFullScreen

        private void TopBar_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            SetWindowFullScreen();
            e.Handled = true;
        }

        private void WindowFullScreen_Click(object sender, RoutedEventArgs e)
        {
            SetWindowFullScreen();
            e.Handled = true;
        }

        private void SetWindowFullScreen()
        {
            if (ViewModel != null)
            {
                ViewModel.ApplicationState.IsFullScreen = !ViewModel.ApplicationState.IsFullScreen;
            }
        }

        #endregion

        #region MediaGroup Create 

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

        #endregion
    }
}
