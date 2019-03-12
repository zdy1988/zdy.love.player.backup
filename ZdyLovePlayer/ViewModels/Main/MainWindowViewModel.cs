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
using ZdyLovePlayer.Messaging;
using ZdyLovePlayer.Models;
using ZdyLovePlayer.Services;
using ZdyLovePlayer.ViewModels.Dialogs;
using ZdyLovePlayer.UserControls.Dialogs;
using ZdyLovePlayer.Untils;

namespace ZdyLovePlayer.ViewModels.Main
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

        #region  Media Groups

        private List<Tuple<Group, int>> _mediaGroups = new List<Tuple<Group, int>>();
        public List<Tuple<Group, int>> MediaGroups
        {
            get { return _mediaGroups; }
            set { Set(() => MediaGroups, ref _mediaGroups, value); }
        }

        private bool _isCreateMeidaGroup;
        public bool IsCreateMeidaGroup
        {
            get { return _isCreateMeidaGroup; }
            set { Set(() => IsCreateMeidaGroup, ref _isCreateMeidaGroup, value); }
        }

        private string _tempMeidaGroupName;
        public string TempMeidaGroupName
        {
            get { return _tempMeidaGroupName; }
            set { Set(() => TempMeidaGroupName, ref _tempMeidaGroupName, value); }
        }

        #endregion

        #endregion

        #region Methods

        public async Task LoadMediaGroup()
        {
            MediaGroups.Clear();

            var watch = Stopwatch.StartNew();

            Logger.Info($"Loading media group ...");

            try
            {
                await Task.Run(async () =>
                {
                    var groups = await _groupService.GetEntitiesAsync(t => true, "ID", false);
                    var mediaGroups = new List<Tuple<Group, int>>();
                    foreach (var group in groups)
                    {
                        var memberCount = await _groupMemberService.GetEntitiesCountAsync(t => t.GroupID == group.ID);
                        mediaGroups.Add(new Tuple<Group, int>(group, memberCount));
                    }

                    DispatcherHelper.CheckBeginInvokeOnUI(() => MediaGroups = mediaGroups);
                }).ConfigureAwait(false);
            }
            catch (Exception exception)
            {
                Logger.Error($"Error loading group : {exception.Message}");
                Messenger.Default.Send(new ManageExceptionMessage(exception));
            }
            finally
            {
                watch.Stop();
                var elapsedMs = watch.ElapsedMilliseconds;
                Logger.Info($"Loaded group in {elapsedMs} milliseconds.");
            }
        }

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

            Messenger.Default.Register<RefreshMediaGroupListMessage>(this, async (e) =>
            {
                await LoadMediaGroup();
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

        public RelayCommand PreCreateMediaGroupCommand => new Lazy<RelayCommand>(() => new RelayCommand(() =>
        {
            IsCreateMeidaGroup = true;
            TempMeidaGroupName = "";
        })).Value;

        public RelayCommand CreateOrCancelMediaGroupCommand => new Lazy<RelayCommand>(() => new RelayCommand(async () =>
        {
            IsCreateMeidaGroup = false;
            if (!string.IsNullOrEmpty(TempMeidaGroupName.Trim()))
            {
                await _groupService.InsertAsync(new Group
                {
                    Name = TempMeidaGroupName.Trim()
                });

                await LoadMediaGroup();
            }
        })).Value;

        public RelayCommand<IMedia> JoinTheMediaGroupDialogOpenCommand => new Lazy<RelayCommand<IMedia>>(() => new RelayCommand<IMedia>((media) =>
        {
            JoinTheMediaGroupDialog dialog = new JoinTheMediaGroupDialog
            {
                DataContext = new JoinTheMediaGroupDialogViewModel(_applicationState, MediaGroups, media)
            };

            _applicationState.ShowDialog(dialog);
        })).Value;

        public RelayCommand<Group> MediaGroupViewOpenCommand => new Lazy<RelayCommand<Group>>(() => new RelayCommand<Group>((group) =>
        {
            Messenger.Default.Send<SwitchPageMessage>(new SwitchPageMessage
            {
                Content = AssemblyHelper.CreateInternalInstance($"UserControls.Pages.MediaGroup")
            });
            Messenger.Default.Send(new RefreshMediaGroupMembersMessage(group));
        })).Value;

        #endregion
    }
}