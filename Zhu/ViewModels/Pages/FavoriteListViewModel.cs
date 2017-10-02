using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Infrastructure.SearchModel.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zhu.Messaging;
using Zhu.Models;
using Zhu.Services;

namespace Zhu.ViewModels.Pages
{
    public class FavoriteListViewModel : MediaListViewModel
    {
        public FavoriteListViewModel(IApplicationState applicationState, IMediaService mediaService)
            : base(applicationState, mediaService)
        {
            RegisterCommands();
        }

        public override Task LoadMediasAsync(bool isRefresh = false)
        {
            if (!this.SearchQueryModel.Items.Any(t => t.Field == "IsFavorite"))
            {
                this.SearchQueryModel.Items.Add(new ConditionItem("IsFavorite", QueryMethod.Equal, 1));
            }

            return base.LoadMediasAsync(isRefresh);
        }

        private void RegisterCommands()
        {

        }
    }
}
