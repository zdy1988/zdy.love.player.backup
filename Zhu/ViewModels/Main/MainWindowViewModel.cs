using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using MaterialDesignThemes.Wpf;
using NLog;
using Zhu.Messaging;
using Zhu.Models;
using Zhu.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Zhu.ViewModels.Pages;
using Zhu.UserControls.Reused;
using GalaSoft.MvvmLight.Threading;
using Zhu.ViewModels.Dialogs;
using Zhu.UserControls.Home.Dialogs;

namespace Zhu.ViewModels.Main
{
    public class MainWindowViewModel : ViewModelBase
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private IApplicationState _applicationState;
        public IApplicationState ApplicationState
        {
            get { return _applicationState; }
            set { Set(() => ApplicationState, ref _applicationState, value); }
        }

        private IMediaService _mediaService;

        public MainWindowViewModel(IApplicationState applicationState,
            IMediaService mediaService)
        {
            _applicationState = applicationState;
            _mediaService = mediaService;

            RegisterMessages();
            RegisterCommands();
        }

        private bool _isMovieFlyoutOpen;
        public bool IsMovieFlyoutOpen
        {
            get { return _isMovieFlyoutOpen; }
            set { Set(() => IsMovieFlyoutOpen, ref _isMovieFlyoutOpen, value); }
        }

        public event EventHandler<string> MessageNotice;

        private void RegisterMessages()
        {
            Messenger.Default.Register<MediaFlyoutOpenMessage>(this, e =>
            {
                IsMovieFlyoutOpen = true;
            });
            Messenger.Default.Register<MediaFlyoutCloseMessage>(this, e =>
            {
                IsMovieFlyoutOpen = false;
            });
            Messenger.Default.Register<ManageExceptionMessage>(this, e =>
            {
                MessageNotice?.Invoke(this, e.UnHandledException.Message);
            });
            //Dialog Message
            Messenger.Default.Register<MediaSourcePlayDialogOpenMessage>(this, (e) =>
            {
                var dialog = new MediaSourcePlayDialog
                {
                    DataContext = new MediaSourcePlayDialogViewModel()
                };

                _applicationState.ShowDialog(dialog);
            });
            Messenger.Default.Register<ImportNetworkMediaDialogOpenMessage>(this, (e) =>
            {
                var dialog = new ImportNetworkMediaDialog
                {
                    DataContext = new ImportNetworkMediaDialogViewModel(_applicationState, _mediaService)
                };

                _applicationState.ShowDialog(dialog);
            });
            Messenger.Default.Register<ScanMediaFileDialogOpenMessage>(this, (e) =>
            {
                var dialog = new ScanMediaFileDialog
                {
                    DataContext = new ScanMediaFileDialogViewModel(_applicationState, _mediaService)
                };

                _applicationState.ShowDialog(dialog);
            });
        }

        public RelayCommand ToggleMoviePalyerCommand { get; private set; }

        public RelayCommand MediaSourcePlayDialogOpenCommand { get; private set; }

        private void RegisterCommands()
        {
            ToggleMoviePalyerCommand = new RelayCommand(() => {
                IsMovieFlyoutOpen = !IsMovieFlyoutOpen;
            });

            MediaSourcePlayDialogOpenCommand = new RelayCommand(() =>
            {
                Messenger.Default.Send(new MediaSourcePlayDialogOpenMessage());
            });
        }
    }

    public class TabItem : ViewModelBase
    {
        private string _title;
        private string _icon;

        public string Title
        {
            get { return _title; }
            set { Set(() => Title, ref _title, value); }
        }

        public string Icon
        {
            get { return _icon; }
            set { Set(() => Icon, ref _icon, value); }
        }
    }
}