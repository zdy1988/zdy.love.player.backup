using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Threading;
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
using ZdyLovePlayer.Untils.Extension;
using ZdyLovePlayer.Services;
using GalaSoft.MvvmLight.Command;
using ZdyLovePlayer.ViewModels.Reused;

namespace ZdyLovePlayer.ViewModels.Pages
{
    public class SeenListViewModel : BasePagingViewModel
    {
        #region Constructor

        public ISeenService _seenService { get; set; }

        public SeenListViewModel(IApplicationState applicationState, ISeenService seenService)
            : base(applicationState)
        {
            this._seenService = seenService;

            this.PageSize = 100;
        }

        private List<Tuple<Seen, ListItemIsSelectViewModel>> _seenMedias = new List<Tuple<Seen, ListItemIsSelectViewModel>>();
        public List<Tuple<Seen, ListItemIsSelectViewModel>> SeenMedias
        {
            get { return _seenMedias; }
            set { Set(() => SeenMedias, ref _seenMedias, value); }
        }

        #endregion

        #region Properties

        private bool? _isSelectedAll;

        public bool? IsSelectedAll
        {
            get { return _isSelectedAll; }
            set
            {
                Set(() => IsSelectedAll, ref _isSelectedAll, value);

                if (_isSelectedAll.HasValue)
                {
                    SelectAll(_isSelectedAll.Value, _seenMedias);
                }
            }
        }

        #endregion

        #region Methods

        public override void LoadMedias(bool isRefresh = false)
        {
            if (isRefresh)
            {
                PageIndex = 0;
                SeenMedias.Clear();
            }

            ExecuteActionAndWatchPageTurning(() => ExecuteActionWithProgress(async () =>
            {
                var data = await ExecuteDataDelay<Seen>(() => _seenService.GetEntitiesForPagingAsync(PageIndex, PageSize, OrderField, false, SearchQueryModel).Result);

                var list = new List<Tuple<Seen, ListItemIsSelectViewModel>>();
                for (var i = 0; i < data.Item1.Count(); i++)
                {
                    var item = data.Item1[i];
                    var isSelect = new ListItemIsSelectViewModel { IsSelected = false };

                    list.Add(new Tuple<Seen, ListItemIsSelectViewModel>(item, isSelect));
                }

                SeenMedias = list;
                IsDataFound = SeenMedias.Any();
                CurrentNumberOfData = SeenMedias.Count;
                MaxNumberOfData = data.Item2;
            }));
        }

        private static void SelectAll(bool select, IEnumerable<Tuple<Seen, ListItemIsSelectViewModel>> models)
        {
            foreach (var model in models)
            {
                model.Item2.IsSelected = select;
            }
        }

        #endregion

        #region Commands

        public RelayCommand<Seen> RepalyMediaCommand = new Lazy<RelayCommand<Seen>>(() => new RelayCommand<Seen>((seen) =>
        {
            Messenger.Default.Send(new OpenMediaMessage(new Media
            {
                ID = seen.MediaID,
                Title = seen.Title,
                MediaSource = seen.MediaSource
            }));
        })).Value;

        #endregion
    }
}
