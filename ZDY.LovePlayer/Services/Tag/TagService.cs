using ZDY.LovePlayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using System.Threading.Tasks;
using Infrastructure.SearchModel.Model;
using Infrastructure.EntityFramework;
using Infrastructure.SearchModel;

namespace ZDY.LovePlayer.Services
{
    public class TagService : BaseService<Tag>, ITagService
    {
        public async Task<Tuple<List<string>, int>> GetTagsForPagingAsync(int pageIndex, int pageSize, QueryModel model)
        {
            if (pageIndex <= 0)
                throw new ArgumentOutOfRangeException("pageIndex", pageIndex, "页码必须大于或等于1");
            if (pageSize <= 0)
                throw new ArgumentOutOfRangeException("pageSize", pageSize, "每页大小必须大于或等于1");
            using (PlayerDataContext ctx = new PlayerDataContext())
            {
                var query = from t in ctx.Context.Set<Tag>()
                            select t;
                var query2 = query.Where(model).GroupBy(t => t.Keyword).Select(t => new
                {
                    KeyWord = t.Key,
                    Count = t.Count()
                }).OrderByDescending(t => t.Count).Select(t => t.KeyWord);

                int count = await query2.CountAsync();
                var data = await query2.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();
                return new Tuple<List<string>, int>(data, count);
            }
        }
    }
}
