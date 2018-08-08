using System;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using Zhu.Messaging;
using Zhu.UserControls.Reused;
using Zhu.ViewModels.Reused;

namespace Zhu.Services
{
    public class ApplicationState : ObservableObject, IApplicationState
    {
        public ApplicationState()
        {
            
        }

        private bool _isFullScreen;
        public bool IsFullScreen
        {
            get { return _isFullScreen; }
            set
            {
                Set(() => IsFullScreen, ref _isFullScreen, value);
                Messenger.Default.Send(new WindowStateChangeMessage(IsMediaPlaying));
            }
        }

        private bool _isConnectionInError;
        public bool IsConnectionInError
        {
            get { return _isConnectionInError; }
            set { Set(() => IsConnectionInError, ref _isConnectionInError, value); }
        }

        private bool _isMediaPlaying;
        public bool IsMediaPlaying
        {
            get { return _isMediaPlaying; }
            set
            {
                Set(() => IsMediaPlaying, ref _isMediaPlaying, value);
                Messenger.Default.Send(new WindowStateChangeMessage(value));
            }
        }

        private object _rootDialogContent;
        public object RootDialogContent
        {
            get { return _rootDialogContent; }
            set { Set(() => RootDialogContent, ref _rootDialogContent, value); }
        }

        private bool _isRootDialogOpen;
        public bool IsRootDialogOpen
        {
            get { return _isRootDialogOpen; }
            set { Set(() => IsRootDialogOpen, ref _isRootDialogOpen, value); }
        }

        public void ShowDialog(object content)
        {
            RootDialogContent = content;
            IsRootDialogOpen = true;
        }

        public void HideDialog()
        {
            RootDialogContent = null;
            IsRootDialogOpen = false;
        }

        public void ShowLoadingDialog(string message = "加载中...")
        {
            SampleLoading loading = new SampleLoading
            {
                DataContext = new SampleLoadingViewModel
                {
                    LoadingMessage = message
                }
            };
            RootDialogContent = loading;
            IsRootDialogOpen = true;
        }

        public void HideLoadingDialog()
        {
            HideDialog();
        }
    }
}