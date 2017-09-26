using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using MaterialDesignThemes.Wpf;
using NLog;
using Zhu.Messaging;
using Zhu.Models;
using Zhu.Services;
using Zhu.UserControls.Main;
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

namespace Zhu.ViewModels.Main
{
    public class MainViewModel : ViewModelBase
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private IApplicationState _applicationState;
        public IApplicationState ApplicationState
        {
            get { return _applicationState; }
            set { Set(() => ApplicationState, ref _applicationState, value); }
        }

        private IMediaService _mediaService;

        public MainViewModel(IApplicationState applicationState,
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

        private TabItem _selectTabItem;
        public TabItem SelectTabItem
        {
            get { return _selectTabItem; }
            set { Set(() => SelectTabItem, ref _selectTabItem, value); }
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
        }

        public RelayCommand ToggleMoviePalyerCommand { get; private set; }

        public RelayCommand MediaSourcePlayDialogOpenCommand { get; private set; }

        public RelayCommand ImportNetworkMediaDialogOpenCommand { get; private set; }

        private void RegisterCommands()
        {
            ToggleMoviePalyerCommand = new RelayCommand(() => {
                IsMovieFlyoutOpen = !IsMovieFlyoutOpen;
            });

            MediaSourcePlayDialogOpenCommand = new RelayCommand(() =>
            {
                Messenger.Default.Send(new MediaSourcePlayDialogOpenMessage());
            });

            ImportNetworkMediaDialogOpenCommand = new RelayCommand(() =>
            {
                Messenger.Default.Send(new ImportNetworkMediaDialogOpenMessage());
            });
        }
    }
}