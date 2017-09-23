using Zhu.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using System.Threading.Tasks;
using Infrastructure.SearchModel.Model;
using Infrastructure.EntityFramework;
using Infrastructure.SearchModel;
using Zhu.Untils.SearchMethod;

namespace Zhu.Services
{
    public class NetTVService : BaseService<NetTV>, INetTVService
    {
        public async Task<Tuple<List<MediaDTO>, int>> GetMediasForPagingAsync(int pageIndex, int pageSize, string orderField, bool isAsc, QueryModel model)
        {
            if (pageIndex <= 0)
                throw new ArgumentOutOfRangeException("pageIndex", pageIndex, "页码必须大于或等于1");
            if (pageSize <= 0)
                throw new ArgumentOutOfRangeException("pageSize", pageSize, "每页大小必须大于或等于1");

            using (PlayerDataContext ctx = new PlayerDataContext())
            {
                var query = $@"select a.* from (
                                    select n.*,i.FileName as Cover,i.Path as CoverPath from NetTV n
                                    left join Image i on n.ID = i.TypeSourceID and i.Type = 'NetTV'
                                ) a inner join (
                                    select n.ID from NetTV n
                                    left join Tag t on n.ID = t.TypeSourceID and t.Type = 'NetTV'
                                    group by n.ID
                                ) b on a.ID = b.ID";
                var queryWhere = SqlToolkit.GetQueryWhere(model);
                if (queryWhere.Length > 0)
                {
                    query += " where " + queryWhere;
                }
                string sqlCountQuery = SqlToolkit.GetRowCountSql(query);
                int count = await ctx.Context.Database.SqlQuery<int>(sqlCountQuery).FirstOrDefaultAsync();

                string sqlPagedQuery = SqlToolkit.GetDataPagedSql(query, orderField, isAsc, pageIndex, pageSize);
                var data = await ctx.Context.Database.SqlQuery<MediaDTO>(sqlPagedQuery).ToListAsync();

                return new Tuple<List<MediaDTO>, int>(data, count);
            }
        }
    }
}
