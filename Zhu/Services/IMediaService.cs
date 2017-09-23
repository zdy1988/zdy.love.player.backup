using Infrastructure;
using Infrastructure.SearchModel.Model;
using Zhu.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Zhu.Services
{
    public interface IMediaService<T> : IBaseService<T> where T : class, IEntity
    {
        Task<Tuple<List<MediaDTO>, int>> GetMediasForPagingAsync(int pageIndex, int pageSize, string orderField, bool isAsc, QueryModel model);
    }
}
