using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Infrastructure.SearchModel.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZdyLovePlayer.Messaging;
using ZdyLovePlayer.Models;
using ZdyLovePlayer.Services;

namespace ZdyLovePlayer.ViewModels.Pages
{
    public class FavoriteListViewModel : MediaListViewModel
    {
        #region Constructor

        public FavoriteListViewModel(IApplicationState applicationState, IMediaService mediaService)
            : base(applicationState, mediaService)
        {

        }

        #endregion

        #region Methods

        public override void ExecuteLoadMedias(bool isRefresh = false)
        {
            if (!this.SearchQueryModel.Items.Any(t => t.Field == "IsFavorite"))
            {
                this.SearchQueryModel.Items.Add(new ConditionItem("IsFavorite", QueryMethod.Equal, 1));
            }

            base.ExecuteLoadMedias(isRefresh);
        }

        #endregion
    }
}
