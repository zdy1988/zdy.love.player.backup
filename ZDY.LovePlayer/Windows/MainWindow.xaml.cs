using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.ComponentModel;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Threading;
using ZDY.LovePlayer.Untils;
using ZDY.LovePlayer.ViewModels.Main;
using ZDY.LovePlayer.Messaging;
using ZDY.LovePlayer.Models;

namespace ZDY.LovePlayer.Windows
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
            SizeChanged += MainWindow_SizeChanged;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            InitializeNotifyIcon();
            InitializeViewModelEvents();

            Task.Factory
                .StartNew(async () => await SQLiteDatabase.Initialize())
                .ContinueWith((t) =>
                {

                }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        private void MainWindow_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            this.Player.Width = this.ActualWidth;
        }

        #region ViewModel Events

        private void InitializeViewModelEvents()
        {
            ViewModel.OnMessageNotice += ViewModel_OnMessageNotice;
            ViewModel.OnWindowClosed += ViewModel_OnWindowClosed;
            ViewModel.OnWindowMinimized += ViewModel_OnWindowMinimized;
        }

        private void ViewModel_OnWindowMinimized(object sender, EventArgs e)
        {
            this.Visibility = Visibility.Hidden;
        }

        private void ViewModel_OnWindowClosed(object sender, EventArgs e)
        {
            this.Close();
        }

        private void ViewModel_OnMessageNotice(object sender, string message)
        {
            DispatcherHelper.CheckBeginInvokeOnUI(() => MainSnackbar.MessageQueue.Enqueue(message));
        }

        #endregion

        #region TopBar

        private void TopBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }

        private void TopBar_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            ViewModel.WindowFullScreenCommand.Execute(null);
            e.Handled = true;
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

        #region NotifyIcon

        System.Windows.Forms.NotifyIcon notifyIcon;
        System.Windows.Controls.ContextMenu notifyIconMenu;

        private void InitializeNotifyIcon()
        {
            string iconPath = System.IO.Path.GetFullPath(@"favicon.ico");
            if (File.Exists(iconPath))
            {
                notifyIcon = new System.Windows.Forms.NotifyIcon();
                notifyIcon.Text = Constants.ProductName;
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

        #region WindowClose

        protected override void OnClosing(CancelEventArgs e)
        {
            notifyIcon.Dispose();
            base.OnClosing(e);
        }

        #endregion
    }
}
