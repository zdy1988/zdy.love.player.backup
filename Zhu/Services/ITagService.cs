using Infrastructure.SearchModel.Model;
using Zhu.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zhu.Services
{
    public interface ITagService : IBaseService<Tag>
    {
        Task<Tuple<List<string>, int>> GetTagsForPagingAsync(int pageIndex, int pageSize, QueryModel model);
    }
}
