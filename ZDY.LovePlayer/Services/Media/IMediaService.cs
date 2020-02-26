using Infrastructure;
using Infrastructure.SearchModel.Model;
using ZDY.LovePlayer.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ZDY.LovePlayer.ViewModels.Pages;

namespace ZDY.LovePlayer.Services
{
    public interface IMediaService : IBaseService<Media>
    {
        Task<Tuple<List<Media>, int>> GetMediasForPagingAsync(int pageIndex, int pageSize, string orderField, bool isAsc, QueryModel model);
    }
}
