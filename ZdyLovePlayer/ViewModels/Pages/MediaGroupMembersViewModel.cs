using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Threading;
using Infrastructure.SearchModel.Model;
using NLog;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZdyLovePlayer.Messaging;
using ZdyLovePlayer.Models;
using ZdyLovePlayer.Services;
using ZdyLovePlayer.ViewModels.Reused;

namespace ZdyLovePlayer.ViewModels.Pages
{
    public class MediaGroupMembersViewModel : MediaListViewModel
    {
        #region Constructor

        public IGroupService _groupService;

        public IGroupMemberService _groupMemberService;

        public MediaGroupMembersViewModel(IApplicationState applicationState,
            IMediaService mediaService,
            IGroupService groupService,
            IGroupMemberService groupMemberService)
            : base(applicationState, mediaService)
        {
            _groupService = groupService;
            _groupMemberService = groupMemberService;

            RegisterMessages();
        }

        #endregion

        #region Properties

        private Group _currentGroup;
        public Group CurrentGroup
        {
            get { return _currentGroup; }
            set { Set(() => CurrentGroup, ref _currentGroup, value); }
        }

        #endregion

        #region Methods

        public override void ExecuteLoadMedias(bool isRefresh = false)
        {
            if (!this.SearchQueryModel.Items.Any(t => t.Field == "GroupID"))
            {
                this.SearchQueryModel.Items.Add(new ConditionItem("GroupID", QueryMethod.Equal, CurrentGroup.ID));
            }

            ExecuteLoadMedias(() => _groupService.GetMediasForPagingAsync(PageIndex, PageSize, OrderField, false, SearchQueryModel).Result, isRefresh);
        }

        public async void RemoveMediaFromGroup(IMedia media)
        {
            var member = await _groupMemberService.GetEntityAsync(t => t.GroupID == CurrentGroup.ID && t.MediaID == media.ID);
            if (member != null)
            {
                await _groupMemberService.DeleteAsync(member);

                var item = Medias.First(t => t.Item1.ID == media.ID);
                if (item != null) Medias.Remove(item);
            }
        }

        #endregion

        #region Messages

        private void RegisterMessages()
        {
            Messenger.Default.Register<RefreshMediaGroupMembersMessage>(this, (e) =>
            {
                this.SearchQueryModel.Items.Clear();
                this.CurrentGroup = e.Group;
                ExecuteLoadMedias(e.IsRefresh);
            });
        }

        #endregion

        #region Commands

        public RelayCommand<IMedia> RemoveMediaFromGroupCommand => new Lazy<RelayCommand<IMedia>>(() => new RelayCommand<IMedia>((media) =>
        {
            RemoveMediaFromGroup(media);

            Messenger.Default.Send(new ManageExceptionMessage(new Exception($"已完成移除！")));
            Messenger.Default.Send(new RefreshMediaGroupListMessage());
        })).Value;

        #endregion
    }
}
