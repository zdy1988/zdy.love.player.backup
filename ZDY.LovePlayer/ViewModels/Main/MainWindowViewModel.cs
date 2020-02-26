using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Diagnostics;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Threading;
using NLog;
using ZDY.LovePlayer.Messaging;
using ZDY.LovePlayer.Models;
using ZDY.LovePlayer.Services;
using ZDY.LovePlayer.ViewModels.Dialogs;
using ZDY.LovePlayer.UserControls.Dialogs;
using ZDY.LovePlayer.Untils;

namespace ZDY.LovePlayer.ViewModels.Main
{
    public class MainWindowViewModel : ViewModelBase
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        #region Constructor

        private IMediaService _mediaService;

        private IGroupService _groupService;

        private IGroupMemberService _groupMemberService;

        public MainWindowViewModel(IApplicationState applicationState, IMediaService mediaService, IGroupService groupService, IGroupMemberService groupMemberService)
        {
            _applicationState = applicationState;
            _mediaService = mediaService;
            _groupService = groupService;
            _groupMemberService = groupMemberService;

            RegisterMessages();
        }

        #endregion

        #region Properties

        private IApplicationState _applicationState;
        public IApplicationState ApplicationState
        {
            get { return _applicationState; }
            set { Set(() => ApplicationState, ref _applicationState, value); }
        }

        private bool _isMediaPlayerFlyoutOpen = true;
        public bool IsMediaPlayerFlyoutOpen
        {
            get { return _isMediaPlayerFlyoutOpen; }
            set { Set(() => IsMediaPlayerFlyoutOpen, ref _isMediaPlayerFlyoutOpen, value); }
        }

        private object _mainContent;

        public object MainContent
        {
            get { return _mainContent; }
            set { Set(() => MainContent, ref _mainContent, value); }
        }

        private bool _isPageLoading;

        public bool IsPageLoading
        {
            get { return _isPageLoading; }
            set { Set(() => IsPageLoading, ref _isPageLoading, value); }
        }

        #endregion

        #region Methods
        #endregion

        #region Events

        public event EventHandler<string> OnMessageNotice;

        public event EventHandler OnWindowMinimized;

        public event EventHandler OnWindowClosed;

        public event EventHandler<bool> OnWindowFullScreen;

        #endregion

        #region  Messages

        private void RegisterMessages()
        {
            Messenger.Default.Register<SwitchPageMessage>(this, async (e) =>
            {
                IsPageLoading = true;

                if (MainContent is IDisposable disposable)
                {
                    disposable.Dispose();
                }

                await Task.Delay(1000);

                IsPageLoading = false;

                MainContent = e.Content;
            });

            Messenger.Default.Register<ManageExceptionMessage>(this, e =>
            {
                OnMessageNotice?.Invoke(this, e.UnHandledException.Message);
            });

            Messenger.Default.Register<MediaFlyoutOpenMessage>(this, e =>
            {
                if (IsMediaPlayerFlyoutOpen == false)
                {
                    IsMediaPlayerFlyoutOpen = true;
                }
            });

            Messenger.Default.Register<MediaFlyoutCloseMessage>(this, e =>
            {
                if (IsMediaPlayerFlyoutOpen == true)
                {
                    IsMediaPlayerFlyoutOpen = false;
                }
            });

            Messenger.Default.Register<PlayMediaSourceDialogOpenMessage>(this, (e) =>
            {
                PlayMediaSourceDialogOpenCommand.Execute(null);
            });
        }

        #endregion

        #region Commands

        public RelayCommand<NavItem> SwitchPageCommand => new Lazy<RelayCommand<NavItem>>(() => new RelayCommand<NavItem>((item) =>
        {
            if (!string.IsNullOrEmpty(item.Tag))
            {
                Messenger.Default.Send<SwitchPageMessage>(new SwitchPageMessage
                {
                    Content = AssemblyHelper.CreateInternalInstance($"UserControls.Pages.{item.Tag}")
                });
            }
            else
            {
                OnMessageNotice?.Invoke(this, "error");
            }
        })).Value;

        public RelayCommand WindowMinimizedCommand => new Lazy<RelayCommand>(() => new RelayCommand(() =>
        {
            OnWindowMinimized?.Invoke(this, null);
        })).Value;

        public RelayCommand WindowClosedCommand => new Lazy<RelayCommand>(() => new RelayCommand(() =>
        {
            OnWindowClosed?.Invoke(this, null);
        })).Value;

        public RelayCommand WindowFullScreenCommand => new Lazy<RelayCommand>(() => new RelayCommand(() =>
        {
            ApplicationState.IsFullScreen = !ApplicationState.IsFullScreen;
            OnWindowFullScreen?.Invoke(this, ApplicationState.IsFullScreen);
        })).Value;

        public RelayCommand ToggleMediaPalyerCommand => new Lazy<RelayCommand>(() => new RelayCommand(() =>
        {
            IsMediaPlayerFlyoutOpen = !IsMediaPlayerFlyoutOpen;
        })).Value;

        public RelayCommand PlayMediaSourceDialogOpenCommand => new Lazy<RelayCommand>(() => new RelayCommand(() =>
        {
            var dialog = new PlayMediaSourceDialog
            {
                DataContext = new PlayMediaSourceDialogViewModel()
            };

            _applicationState.ShowDialog(dialog);
        })).Value;

        public RelayCommand ScanMediaFileDialogOpenCommand => new Lazy<RelayCommand>(() => new RelayCommand(() =>
        {
            var dialog = new ScanMediaFileDialog
            {
                DataContext = new ScanMediaFileDialogViewModel(_applicationState, _mediaService)
            };

            _applicationState.ShowDialog(dialog);
        })).Value;

        public RelayCommand ImportNetworkStreamDialogOpenCommand => new Lazy<RelayCommand>(() => new RelayCommand(() =>
        {
            var dialog = new ImportNetworkStreamDialog
            {
                DataContext = new ImportNetworkStreamDialogViewModel(_applicationState, _mediaService)
            };

            _applicationState.ShowDialog(dialog);
        })).Value;

        #endregion
    }
}