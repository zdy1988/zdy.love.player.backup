using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infrastructure.SearchModel.Model;
using ZDY.LovePlayer.Models;
using ZDY.LovePlayer.ViewModels.Pages;
using ZDY.LovePlayer.Untils.SearchMethod;

namespace ZDY.LovePlayer.Services
{
    public class GroupService : BaseService<Group>, IGroupService
    {
        public async Task<Tuple<List<Media>, int>> GetMediasForPagingAsync(int pageIndex, int pageSize, string orderField, bool isAsc, QueryModel model)
        {
            if (pageIndex <= 0)
                throw new ArgumentOutOfRangeException("pageIndex", pageIndex, "页码必须大于或等于1");
            if (pageSize <= 0)
                throw new ArgumentOutOfRangeException("pageSize", pageSize, "每页大小必须大于或等于1");

            using (PlayerDataContext ctx = new PlayerDataContext())
            {
                var queryWhere = SqlToolkit.GetQueryWhere(model);
                if (queryWhere.Length > 0)
                {
                    queryWhere = " and " + queryWhere;
                }

                var query = $@"select m.* from [group] g 
                               inner join groupmember gm on g.ID = gm.GroupID
                               inner join media m on gm.MediaId = m.ID 
                               where 1 = 1 {queryWhere}
                               group by m.ID";

                string sqlCountQuery = SqlToolkit.GetRowCountSql(query);
                int count = await ctx.Context.Database.SqlQuery<int>(sqlCountQuery).FirstOrDefaultAsync();

                string sqlPagedQuery = SqlToolkit.GetDataPagedSql(query, orderField, isAsc, pageIndex, pageSize);
                var data = await ctx.Context.Database.SqlQuery<Media>(sqlPagedQuery).ToListAsync();

                return new Tuple<List<Media>, int>(data, count);
            }
        }
    }
}
