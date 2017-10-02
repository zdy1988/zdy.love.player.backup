using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Threading;
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

namespace Zhu.ViewModels.Dialogs
{
    public class JoinTheMediaGroupDialogViewModel : ViewModelBase
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public IApplicationState _applicationState { get; set; }

        private List<IMedia> _medias { get; set; }

        private List<Tuple<Group, ListItemIsSelectViewModel>> _mediaGroups = new List<Tuple<Group, ListItemIsSelectViewModel>>();
        public List<Tuple<Group, ListItemIsSelectViewModel>> MediaGroups
        {
            get { return _mediaGroups; }
            set { Set(() => MediaGroups, ref _mediaGroups, value); }
        }

        public JoinTheMediaGroupDialogViewModel(IApplicationState applicationState, 
            List<Tuple<Group, int>> groups, List<IMedia> medias)
        {
            _applicationState = applicationState;
            _medias = medias;

            InitializeMediaGroup(groups);

            RegisterCommands();
        }

        public JoinTheMediaGroupDialogViewModel(IApplicationState applicationState,
            List<Tuple<Group, int>> groups, IMedia medias)
        {
            _applicationState = applicationState;
            _medias = new List<IMedia> ();
            _medias.Add(medias);

            InitializeMediaGroup(groups);

            RegisterCommands();
        }

        private void InitializeMediaGroup(List<Tuple<Group, int>> groups)
        {
            var mediaGroups = new List<Tuple<Group, ListItemIsSelectViewModel>>();
            foreach (var group in groups)
            {
                mediaGroups.Add(new Tuple<Group, ListItemIsSelectViewModel>(group.Item1, new ListItemIsSelectViewModel
                {
                    IsSelected = false
                }));
            }
            MediaGroups = mediaGroups;
        }

        public RelayCommand DoJoinMediaGroupCommand { get; private set; }

        private void RegisterCommands()
        {
            DoJoinMediaGroupCommand = new RelayCommand(async () =>
            {
                if (_medias.Count > 0)
                {
                    _applicationState.ShowLoadingDialog();

                    await Task.Run(async () =>
                    {
                        foreach (var media in _medias)
                        {
                            foreach (var group in _mediaGroups)
                            {
                                if (group.Item2.IsSelected)
                                {
                                    if (await SimpleIoc.Default.GetInstance<IGroupMemberService>().GetEntitiesCountAsync(t => t.GroupID == group.Item1.ID && t.MediaID == media.ID) == 0)
                                    {
                                        await SimpleIoc.Default.GetInstance<IGroupMemberService>().InsertAsync(new GroupMember
                                        {
                                            GroupID = group.Item1.ID,
                                            MediaID = media.ID,
                                            IsTop = false
                                        });
                                    }
                                }
                            }
                        }
                    });

                    _applicationState.HideLoadingDialog();

                    Messenger.Default.Send(new ManageExceptionMessage(new Exception($"已完成加入！")));

                    Messenger.Default.Send(new RefreshMediaGroupListMessage());
                }
            });
        }
    }
}
