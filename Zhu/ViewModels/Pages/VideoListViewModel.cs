using Infrastructure.SearchModel.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zhu.Services;

namespace Zhu.ViewModels.Pages
{
    public class VideoListViewModel : MediaListViewModel
    {
        public VideoListViewModel(IApplicationState applicationState, IMediaService mediaService)
            : base(applicationState, mediaService)
        {
            RegisterCommands();
        }

        public override Task LoadMediasAsync(bool isRefresh = false)
        {
            if (!this.SearchQueryModel.Items.Any(t => t.Field == "MediaType"))
            {
                this.SearchQueryModel.Items.Add(new ConditionItem("MediaType", QueryMethod.Equal, (int)PubilcEnum.MediaType.Video));
            }

            return base.LoadMediasAsync(isRefresh);
        }

        private void RegisterCommands()
        {
        }
    }
}
