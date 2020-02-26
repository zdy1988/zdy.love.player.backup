using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Threading;
using NLog;
using ZDY.LovePlayer.Messaging;
using ZDY.LovePlayer.Models;
using ZDY.LovePlayer.Services;
using GalaSoft.MvvmLight.Command;
using ZDY.LovePlayer.ViewModels.Reused;
using ZDY.LovePlayer.Untils.Extension;

namespace ZDY.LovePlayer.ViewModels.Pages
{
    public class SeenListViewModel : BasePaginationViewModel<Seen>
    {
        #region Constructor

        public ISeenService _seenService { get; set; }

        public SeenListViewModel(IApplicationState applicationState, ISeenService seenService)
            : base(applicationState)
        {
            this._seenService = seenService;

            this.PageSize = 100;
        }

        #endregion

        #region Properties



        #endregion

        #region Methods

        public override void ExecuteLoadMedias(bool isRefresh = false)
        {
            ExecuteLoadMedias(() => _seenService.GetEntitiesForPagingAsync(PageIndex, PageSize, OrderField, false, SearchQueryModel).Result, isRefresh);
        }

        #endregion

        #region Commands

        public RelayCommand<Seen> RepalyMediaCommand => new Lazy<RelayCommand<Seen>>(() => new RelayCommand<Seen>((seen) =>
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
