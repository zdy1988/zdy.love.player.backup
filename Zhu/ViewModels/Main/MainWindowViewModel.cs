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
using System.Diagnostics;
using System.Windows;
using Zhu.UserControls.Home.Media;

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

        private IGroupService _groupService;

        private IGroupMemberService _groupMemberService;

        private bool _isMediaPlayerFlyoutOpen = true;
        public bool IsMediaPlayerFlyoutOpen
        {
            get { return _isMediaPlayerFlyoutOpen; }
            set { Set(() => IsMediaPlayerFlyoutOpen, ref _isMediaPlayerFlyoutOpen, value); }
        }

        private int _tabSelectedIndex = 0;
        public int TabSelectedIndex
        {
            get { return _tabSelectedIndex; }
            set { Set(() => TabSelectedIndex, ref _tabSelectedIndex, value); }
        }

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

        public MainWindowViewModel(IApplicationState applicationState,
            IMediaService mediaService,
            IGroupService groupService,
            IGroupMemberService groupMemberService)
        {
            _applicationState = applicationState;
            _mediaService = mediaService;
            _groupService = groupService;
            _groupMemberService = groupMemberService;

            RegisterMessages();
            RegisterCommands();
        }

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

        public event EventHandler<string> MessageNotice;

        private void RegisterMessages()
        {
            Messenger.Default.Register<ManageExceptionMessage>(this, e => {
                MessageNotice?.Invoke(this, e.UnHandledException.Message);
            });

            Messenger.Default.Register<RefreshMediaGroupListMessage>(this, async (e) => {
                await LoadMediaGroup();
            });

            Messenger.Default.Register<MediaFlyoutOpenMessage>(this, e => {
                if (IsMediaPlayerFlyoutOpen == false)
                {
                    IsMediaPlayerFlyoutOpen = true;
                }
            });
            Messenger.Default.Register<MediaFlyoutCloseMessage>(this, e => {
                if (IsMediaPlayerFlyoutOpen == true)
                {
                    IsMediaPlayerFlyoutOpen = false;
                }
            });

            Messenger.Default.Register<PlayMediaSourceDialogOpenMessage>(this, (e) => {
                PlayMediaSourceDialogOpenCommand.Execute(null);
            });
        }

        public RelayCommand ToggleMediaPalyerCommand { get; private set; }

        public RelayCommand PlayMediaSourceDialogOpenCommand { get; private set; }

        public RelayCommand ScanMediaFileDialogOpenCommand { get; private set; }

        public RelayCommand ImportNetworkStreamDialogOpenCommand { get; private set; }

        public RelayCommand PreCreateMediaGroupCommand { get; private set; }

        public RelayCommand CreateOrCancelMediaGroupCommand { get; private set; }

        public RelayCommand<IMedia> JoinTheMediaGroupDialogOpenCommand { get; private set; }

        public RelayCommand<Group> MediaGroupViewOpenCommand { get; private set; }

        private void RegisterCommands()
        {
            ToggleMediaPalyerCommand = new RelayCommand(() => IsMediaPlayerFlyoutOpen = !IsMediaPlayerFlyoutOpen);

            // 打开媒体文件窗口
            PlayMediaSourceDialogOpenCommand = new RelayCommand(() =>
            {
                var dialog = new PlayMediaSourceDialog
                {
                    DataContext = new PlayMediaSourceDialogViewModel()
                };

                _applicationState.ShowDialog(dialog);
            });

            // 打开扫描本地文件
            ScanMediaFileDialogOpenCommand = new RelayCommand(() =>
            {
                var dialog = new ScanMediaFileDialog
                {
                    DataContext = new ScanMediaFileDialogViewModel(_applicationState, _mediaService)
                };

                _applicationState.ShowDialog(dialog);
            });

            // 打开导入网络媒体窗口
            ImportNetworkStreamDialogOpenCommand = new RelayCommand(() =>
            {
                var dialog = new ImportNetworkStreamDialog
                {
                    DataContext = new ImportNetworkStreamDialogViewModel(_applicationState, _mediaService)
                };

                _applicationState.ShowDialog(dialog);
            });

            // 创建影集前
            PreCreateMediaGroupCommand = new RelayCommand(() =>
            {
                IsCreateMeidaGroup = true;
                TempMeidaGroupName = "";
            });

            // 创建影集
            CreateOrCancelMediaGroupCommand = new RelayCommand(async () =>
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
            });

            // 打开影片加入影集窗口
            JoinTheMediaGroupDialogOpenCommand = new RelayCommand<IMedia>((media) =>
            {
                JoinTheMediaGroupDialog dialog = new JoinTheMediaGroupDialog
                {
                    DataContext = new JoinTheMediaGroupDialogViewModel(_applicationState,MediaGroups, media)
                };

                _applicationState.ShowDialog(dialog);
            });
            
            // 打开影集画面窗口
            MediaGroupViewOpenCommand = new RelayCommand<Group>((group) =>
            {
                TabSelectedIndex = 6;
                Messenger.Default.Send(new RefreshMediaGroupMembersMessage(group));
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