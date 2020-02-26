using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Threading;
using ZDY.LovePlayer.Messaging;
using ZDY.LovePlayer.Models;
using ZDY.LovePlayer.Services;
using ZDY.LovePlayer.Untils;
using ZDY.LovePlayer.UserControls.Dialogs;
using ZDY.LovePlayer.ViewModels.Dialogs;

namespace ZDY.LovePlayer.ViewModels.Pages
{
    public class MediaGroupViewModel : ViewModelBasic
    {
        #region Constructor

        public IGroupService _groupService;

        public IGroupMemberService _groupMemberService;

        public MediaGroupViewModel(IApplicationState applicationState,
            IGroupService groupService,
            IGroupMemberService groupMemberService)
            : base(applicationState)
        {
            _groupService = groupService;
            _groupMemberService = groupMemberService;

            RegisterMessages();
        }

        #endregion

        #region Properties

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

        #region Methods

        public void LoadMediaGroup()
        {
            MediaGroups.Clear();

            ExecuteActionWithWatching("Load Media Group",
                async () =>
                {
                    var groups = await _groupService.GetEntitiesAsync(t => true, "ID", false);
                    var mediaGroups = new List<Tuple<Group, int>>();
                    foreach (var group in groups)
                    {
                        var memberCount = await _groupMemberService.GetEntitiesCountAsync(t => t.GroupID == group.ID);
                        mediaGroups.Add(new Tuple<Group, int>(group, memberCount));
                    }
                    DispatcherHelper.CheckBeginInvokeOnUI(() => MediaGroups = mediaGroups);
                },
                () => { },
                () => { });
        }

        #endregion

        #region Messages

        public void RegisterMessages()
        {
            Messenger.Default.Register<RefreshMediaGroupListMessage>(this, e => LoadMediaGroup());
        }

        #endregion

        #region Commands

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

                LoadMediaGroup();
            }
        })).Value;

        public RelayCommand<Group> MediaGroupViewOpenCommand => new Lazy<RelayCommand<Group>>(() => new RelayCommand<Group>((group) =>
        {
            Messenger.Default.Send<SwitchPageMessage>(new SwitchPageMessage
            {
                Content = AssemblyHelper.CreateInternalInstance($"UserControls.Pages.MediaGroup")
            });

            Messenger.Default.Send(new RefreshMediaGroupMembersMessage(group));
        })).Value;

        public RelayCommand<IMedia> JoinTheMediaGroupDialogOpenCommand => new Lazy<RelayCommand<IMedia>>(() => new RelayCommand<IMedia>((media) =>
        {
            JoinTheMediaGroupDialog dialog = new JoinTheMediaGroupDialog
            {
                DataContext = new JoinTheMediaGroupDialogViewModel(_applicationState, MediaGroups, media)
            };

            _applicationState.ShowDialog(dialog);
        })).Value;

        #endregion
    }
}
