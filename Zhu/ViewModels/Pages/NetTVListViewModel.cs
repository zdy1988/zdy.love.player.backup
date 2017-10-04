using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using NLog;
using Zhu.Messaging;
using Zhu.Services;
using Zhu.Models;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Command;
using Zhu.Untils;
using System.Collections.Generic;
using Zhu.Controls;
using MaterialDesignThemes.Wpf;
using System.Threading;
using GalaSoft.MvvmLight.Ioc;
using Infrastructure.SearchModel.Model;
using Zhu.UserControls.Reused;

namespace Zhu.ViewModels.Pages
{
    public class NetTVListViewModel : MediaListViewModel
    {
        public NetTVListViewModel(IApplicationState applicationState, IMediaService mediaService)
            : base(applicationState, mediaService)
        {
            RegisterCommands();
            RegisterMessages();
        }

        public override Task LoadMediasAsync(bool isRefresh = false)
        {
            if (!this.SearchQueryModel.Items.Any(t => t.Field == "MediaType"))
            {
                this.SearchQueryModel.Items.Add(new ConditionItem("MediaType", QueryMethod.Equal, (int)PubilcEnum.MediaType.NetTV));
            }

            return base.LoadMediasAsync(isRefresh);
        }

        private void RegisterCommands()
        {
         
        }

        private void RegisterMessages()
        {
            Messenger.Default.Register<RefreshNetTVListMessage>(this, async (e) =>
            {
                await LoadMediasAsync(e.IsRefresh).ConfigureAwait(false);
            });
        }
    }
}
