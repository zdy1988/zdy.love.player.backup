using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using NLog;
using ZdyLovePlayer.Messaging;
using ZdyLovePlayer.Services;
using ZdyLovePlayer.Models;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Command;
using ZdyLovePlayer.Untils;
using System.Collections.Generic;
using ZdyLovePlayer.Controls;
using MaterialDesignThemes.Wpf;
using System.Threading;
using GalaSoft.MvvmLight.Ioc;
using Infrastructure.SearchModel.Model;
using ZdyLovePlayer.UserControls.Reused;

namespace ZdyLovePlayer.ViewModels.Pages
{
    public class NetTVListViewModel : MediaListViewModel
    {
        #region Constructor

        public NetTVListViewModel(IApplicationState applicationState, IMediaService mediaService)
            : base(applicationState, mediaService)
        {
            RegisterMessages();
        }

        #endregion

        #region Methods

        public override void LoadMedias(bool isRefresh = false)
        {
            if (!this.SearchQueryModel.Items.Any(t => t.Field == "MediaType"))
            {
                this.SearchQueryModel.Items.Add(new ConditionItem("MediaType", QueryMethod.Equal, (int)PubilcEnum.MediaType.NetTV));
            }

            base.LoadMedias(isRefresh);
        }

        #endregion

        #region Messages

        private void RegisterMessages()
        {
            Messenger.Default.Register<RefreshNetTVListMessage>(this, e => LoadMedias(e.IsRefresh));
        }

        #endregion
    }
}
