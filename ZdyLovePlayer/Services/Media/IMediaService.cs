using Infrastructure;
using Infrastructure.SearchModel.Model;
using ZdyLovePlayer.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ZdyLovePlayer.ViewModels.Pages;

namespace ZdyLovePlayer.Services
{
    public interface IMediaService : IBaseService<Media>
    {
        Task<Tuple<List<Media>, int>> GetMediasForPagingAsync(int pageIndex, int pageSize, string orderField, bool isAsc, QueryModel model);
    }
}
