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
    public partial class MovieService : BaseService<Movie>, IMovieService
    {
        public async Task<Tuple<List<MediaDTO>, int>> GetMediasForPagingAsync(int pageIndex, int pageSize, string orderField, bool isAsc, QueryModel model)
        {
            if (pageIndex <= 0)
                throw new ArgumentOutOfRangeException("pageIndex", pageIndex, "页码必须大于或等于1");
            if (pageSize <= 0)
                throw new ArgumentOutOfRangeException("pageSize", pageSize, "每页大小必须大于或等于1");

            using (PlayerDataContext ctx = new PlayerDataContext())
            {
                var query = $@"select a.*, b.Keyword from (
                                    select m.*, i.FileName as Cover,i.Path as CoverPath from Movie m
                                    left join Image i on m.ID = i.TypeSourceID and i.Type = 'Movie'
                                ) a inner join (
                                    select m.ID, t.Keyword from Movie m
                                    left join Tag t on m.ID = t.TypeSourceID and t.Type = 'Movie'
                                    group by m.ID
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
