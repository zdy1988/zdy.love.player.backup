﻿using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using MaterialDesignThemes.Wpf;
using System;
using System.IO;
using Zhu.Untils;
using Zhu.ViewModels.Main;
using GalaSoft.MvvmLight.Messaging;
using Zhu.Messaging;
using Meta.Vlc.Wpf;
using System.ComponentModel;
using Zhu.Controls;
using Zhu.Models;
using Zhu.UserControls.Reused;
using GalaSoft.MvvmLight.Ioc;
using Zhu.Services;
using GalaSoft.MvvmLight.Threading;

namespace Zhu.Windows
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            InitializeTaskBarNotify();
            InitializeMessageNotice();

            Task.Factory.StartNew(() =>
            {
                Thread.Sleep(2500);
            })
            .ContinueWith(t => SQLiteDatabase.Initialize())
            .ContinueWith(t =>
            {
                DispatcherHelper.CheckBeginInvokeOnUI(() => {
                    MainSnackbar.MessageQueue.Enqueue("Welcome to WantChaPlayer！");
                });
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        #region MessageNotice

        public void InitializeMessageNotice()
        {
            var vm = DataContext as MainWindowViewModel;
            if (vm == null) return;
            vm.MessageNotice += Vm_MessageNotice;
        }

        private void Vm_MessageNotice(object sender, string message)
        {
            DispatcherHelper.CheckBeginInvokeOnUI(() => {
                MainSnackbar.MessageQueue.Enqueue(message);
            });
        }

        #endregion

        #region Keyboard Shortcuts

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if ((e.KeyboardDevice.Modifiers & ModifierKeys.Control) == ModifierKeys.Control && e.Key == Key.E)
            {
                this.Close();
            }
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

            SimpleIoc.Default.GetInstance<IApplicationState>().ShowLoadingDialog();

            Messenger.Default.Send(new LoadMediaMessage(new Media { MediaSource = fileName }));
        }

        #endregion

        #region WindowClose

        private void WindowClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
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
                notifyIcon.Text = Untils.Constants.ApplicationName;
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
            MainWindowViewModel vm = this.DataContext as MainWindowViewModel;
            if (vm != null)
            {
                vm.ApplicationState.IsFullScreen = !vm.ApplicationState.IsFullScreen;
            }
        }

        #endregion

        protected override void OnClosing(CancelEventArgs e)
        {
            notifyIcon.Dispose();
            ApiManager.ReleaseAll();
            base.OnClosing(e);
        }
    }
}
