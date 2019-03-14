using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Infrastructure.SearchModel.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZdyLovePlayer.Messaging;
using ZdyLovePlayer.Services;

namespace ZdyLovePlayer.ViewModels.Pages
{
    public class VideoListViewModel : MediaListViewModel
    {
        #region Constructor

        public VideoListViewModel(IApplicationState applicationState, IMediaService mediaService)
            : base(applicationState, mediaService)
        {
            RegisterMessages();
        }

        #endregion

        #region Methods

        public override void ExecuteLoadMedias(bool isRefresh = false)
        {
            if (!this.SearchQueryModel.Items.Any(t => t.Field == "MediaType"))
            {
                this.SearchQueryModel.Items.Add(new ConditionItem("MediaType", QueryMethod.Equal, (int)PubilcEnum.MediaType.Video));
            }

            base.ExecuteLoadMedias(isRefresh);
        }

        #endregion

        #region Messgaes

        private void RegisterMessages()
        {
            Messenger.Default.Register<RefreshVideoListMessage>(this, e => ExecuteLoadMedias(e.IsRefresh));
        }

        #endregion
    }
}
