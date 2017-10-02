using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Threading;
using Infrastructure.SearchModel.Model;
using NLog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zhu.Messaging;
using Zhu.Models;
using Zhu.Services;
using Zhu.ViewModels.Reused;

namespace Zhu.ViewModels.Pages
{
    public class MediaGroupMembersViewModel : MediaListViewModel
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private IGroupService _groupService;

        private IGroupMemberService _groupMemberService;

        public MediaGroupMembersViewModel(IApplicationState applicationState, 
            IMediaService mediaService, 
            IGroupService groupService,
            IGroupMemberService groupMemberService)
            : base(applicationState, mediaService)
        {
            _groupService = groupService;
            _groupMemberService = groupMemberService;

            RegisterCommands();
            RegisterMessages();
        }

        private Group _currentGroup;
        public Group CurrentGroup
        {
            get { return _currentGroup; }
            set { Set(() => CurrentGroup, ref _currentGroup, value); }
        }

        public async override Task LoadMediasAsync(bool isRefresh = false)
        {
            if (!this.SearchQueryModel.Items.Any(t => t.Field == "GroupID"))
            {
                this.SearchQueryModel.Items.Add(new ConditionItem("GroupID", QueryMethod.Equal, CurrentGroup.ID));
            }

            if (isRefresh)
            {
                PageIndex = 0;
                Medias.Clear();
            }

            var watch = Stopwatch.StartNew();

            PageIndex++;
            HasLoadingFailed = false;
            Logger.Info($"Loading page {PageIndex}...");

            try
            {
                IsDataLoading = true;
                await Task.Run(async () =>
                {
                    var loadDataWatcher = new Stopwatch();

                    loadDataWatcher.Start();
                    var medias = await _groupService.GetMediasForPagingAsync(PageIndex, PageSize, OrderField, false, SearchQueryModel);
                    loadDataWatcher.Stop();

                    var elapsedMs = loadDataWatcher.ElapsedMilliseconds;
                    if (elapsedMs < 500)
                    {
                        await Task.Delay(500 - (int)elapsedMs).ConfigureAwait(false);
                    }

                    DispatcherHelper.CheckBeginInvokeOnUI(() =>
                    {
                        for (var i = 0; i < medias.Item1.Count(); i++)
                        {
                            var item = EmitMapper.ObjectMapperManager.DefaultInstance.GetMapper<Media, MediaListItemViewModel>().Map(medias.Item1[i]);
                            var isSelect = new ListItemIsSelectViewModel { IsSelected = false };

                            Medias.Add(new Tuple<MediaListItemViewModel, ListItemIsSelectViewModel>(item, isSelect));
                        }
                        IsDataLoading = false;
                        IsDataFound = Medias.Any();
                        CurrentNumberOfData = Medias.Count;
                        MaxNumberOfData = medias.Item2;
                    });
                }).ConfigureAwait(false);
            }
            catch (Exception exception)
            {
                PageIndex--;
                HasLoadingFailed = true;
                Logger.Error($"Error while loading page {PageIndex}: {exception.Message}");
                Messenger.Default.Send(new ManageExceptionMessage(exception));
            }
            finally
            {
                watch.Stop();
                var elapsedMs = watch.ElapsedMilliseconds;
                Logger.Info($"Loaded page {PageIndex} in {elapsedMs} milliseconds.");
            }
        }

        private void RegisterMessages()
        {
            Messenger.Default.Register<RefreshMediaGroupMembersMessage>(this, async (e) =>
            {
                this.SearchQueryModel.Items.Clear();
                this.CurrentGroup = e.Group;
                await LoadMediasAsync(e.IsRefresh).ConfigureAwait(false);
            });
        }

        public RelayCommand<IMedia> RemoveMediaFromGroupCommand { get; private set; }

        private void RegisterCommands()
        {
            RemoveMediaFromGroupCommand = new RelayCommand<IMedia>(async (media) =>
            {
                var member = await _groupMemberService.GetEntityAsync(t => t.GroupID == CurrentGroup.ID && t.MediaID == media.ID);
                if (member != null)
                {
                    await _groupMemberService.DeleteAsync(member);
                }

                DispatcherHelper.CheckBeginInvokeOnUI(() => {
                    var item = this.Medias.Where(t => t.Item1 == media).FirstOrDefault();
                    this.Medias.Remove(item);
                    this.MaxNumberOfData -= 1;
                });

                Messenger.Default.Send(new ManageExceptionMessage(new Exception($"已完成移除！")));

                Messenger.Default.Send(new RefreshMediaGroupListMessage());
            });
        }
    }
}
