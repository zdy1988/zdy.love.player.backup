using Infrastructure.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infrastructure.EntityFramework.Order;
using Infrastructure;


namespace Zhu.Services
{
    public abstract class BaseService<T> : IBaseService<T> where T : class, IEntity
    {
        #region 查询普通实现方案(支持SearchModel的Where查询)
        public IEnumerable<T> GetEntities(Infrastructure.SearchModel.Model.QueryModel model)
        {
            using (PlayerDataContext ctx = new PlayerDataContext())
            {
                EntityFrameworkRepository<T> efr = new EntityFrameworkRepository<T>(ctx);
                return efr.GetEntities(model);
            }
        }

        public IEnumerable<T> GetEntities(Infrastructure.SearchModel.Model.QueryModel model, string orderField, bool isAsc)
        {
            using (PlayerDataContext ctx = new PlayerDataContext())
            {
                EntityFrameworkRepository<T> efr = new EntityFrameworkRepository<T>(ctx);
                return efr.GetEntities(model, orderField, isAsc);
            }
        }

        public IEnumerable<T> GetEntities(Infrastructure.SearchModel.Model.QueryModel model, Action<IOrderable<T>> orderBy)
        {
            using (PlayerDataContext ctx = new PlayerDataContext())
            {
                EntityFrameworkRepository<T> efr = new EntityFrameworkRepository<T>(ctx);
                return efr.GetEntities(model, orderBy);
            }
        }

        public int GetEntitiesCount(Infrastructure.SearchModel.Model.QueryModel model)
        {
            using (PlayerDataContext ctx = new PlayerDataContext())
            {
                EntityFrameworkRepository<T> efr = new EntityFrameworkRepository<T>(ctx);
                return efr.GetEntitiesCount(model);
            }
        }

        public IEnumerable<T> GetEntitiesForPaging(int pageIndex, int pageSize, Infrastructure.SearchModel.Model.QueryModel model, out int count)
        {
            using (PlayerDataContext ctx = new PlayerDataContext())
            {
                EntityFrameworkRepository<T> efr = new EntityFrameworkRepository<T>(ctx);
                return efr.GetEntitiesForPaging(pageIndex, pageSize, model, out count);
            }
        }

        public IEnumerable<T> GetEntitiesForPaging(int pageIndex, int pageSize, string orderField, bool isAsc, Infrastructure.SearchModel.Model.QueryModel model, out int count)
        {
            using (PlayerDataContext ctx = new PlayerDataContext())
            {
                EntityFrameworkRepository<T> efr = new EntityFrameworkRepository<T>(ctx);
                return efr.GetEntitiesForPaging(pageIndex, pageSize, orderField, isAsc, model, out count);
            }
        }

        public IEnumerable<T> GetEntitiesForPaging(int pageIndex, int pageSize, Action<Infrastructure.EntityFramework.Order.IOrderable<T>> orderBy, Infrastructure.SearchModel.Model.QueryModel model, out int count)
        {
            using (PlayerDataContext ctx = new PlayerDataContext())
            {
                EntityFrameworkRepository<T> efr = new EntityFrameworkRepository<T>(ctx);
                return efr.GetEntitiesForPaging(pageIndex, pageSize, orderBy, model, out count);
            }
        }

        public T GetEntity(Infrastructure.SearchModel.Model.QueryModel model)
        {
            using (PlayerDataContext ctx = new PlayerDataContext())
            {
                EntityFrameworkRepository<T> efr = new EntityFrameworkRepository<T>(ctx);
                return efr.GetEntity(model);
            }
        }
        #endregion

        #region 异步查询普通实现方案(支持SearchModel的Where查询)
        public Task<List<T>> GetEntitiesAsync(Infrastructure.SearchModel.Model.QueryModel model)
        {
            using (PlayerDataContext ctx = new PlayerDataContext())
            {
                EntityFrameworkRepository<T> efr = new EntityFrameworkRepository<T>(ctx);
                return efr.GetEntitiesAsync(model);
            }
        }

        public Task<List<T>> GetEntitiesAsync(Infrastructure.SearchModel.Model.QueryModel model, string orderField, bool isAsc)
        {
            using (PlayerDataContext ctx = new PlayerDataContext())
            {
                EntityFrameworkRepository<T> efr = new EntityFrameworkRepository<T>(ctx);
                return efr.GetEntitiesAsync(model, orderField, isAsc);
            }
        }

        public Task<List<T>> GetEntitiesAsync(Infrastructure.SearchModel.Model.QueryModel model, Action<IOrderable<T>> orderBy)
        {
            using (PlayerDataContext ctx = new PlayerDataContext())
            {
                EntityFrameworkRepository<T> efr = new EntityFrameworkRepository<T>(ctx);
                return efr.GetEntitiesAsync(model, orderBy);
            }
        }

        public Task<int> GetEntitiesCountAsync(Infrastructure.SearchModel.Model.QueryModel model)
        {
            using (PlayerDataContext ctx = new PlayerDataContext())
            {
                EntityFrameworkRepository<T> efr = new EntityFrameworkRepository<T>(ctx);
                return efr.GetEntitiesCountAsync(model);
            }
        }

        public Task<Tuple<List<T>, int>> GetEntitiesForPagingAsync(int pageIndex, int pageSize, Infrastructure.SearchModel.Model.QueryModel model)
        {
            using (PlayerDataContext ctx = new PlayerDataContext())
            {
                EntityFrameworkRepository<T> efr = new EntityFrameworkRepository<T>(ctx);
                return efr.GetEntitiesForPagingAsync(pageIndex, pageSize, model);
            }
        }

        public Task<Tuple<List<T>, int>> GetEntitiesForPagingAsync(int pageIndex, int pageSize, string orderField, bool isAsc, Infrastructure.SearchModel.Model.QueryModel model)
        {
            using (PlayerDataContext ctx = new PlayerDataContext())
            {
                EntityFrameworkRepository<T> efr = new EntityFrameworkRepository<T>(ctx);
                return efr.GetEntitiesForPagingAsync(pageIndex, pageSize, orderField, isAsc, model);
            }
        }

        public Task<Tuple<List<T>, int>> GetEntitiesForPagingAsync(int pageIndex, int pageSize, Action<Infrastructure.EntityFramework.Order.IOrderable<T>> orderBy, Infrastructure.SearchModel.Model.QueryModel model)
        {
            using (PlayerDataContext ctx = new PlayerDataContext())
            {
                EntityFrameworkRepository<T> efr = new EntityFrameworkRepository<T>(ctx);
                return efr.GetEntitiesForPagingAsync(pageIndex, pageSize, orderBy, model);
            }
        }

        public Task<T> GetEntityAsync(Infrastructure.SearchModel.Model.QueryModel model)
        {
            using (PlayerDataContext ctx = new PlayerDataContext())
            {
                EntityFrameworkRepository<T> efr = new EntityFrameworkRepository<T>(ctx);
                return efr.GetEntityAsync(model);
            }
        }
        #endregion

        #region 查询普通实现方案(基于Lambda表达式的Where查询)
        public IEnumerable<T> GetEntities(System.Linq.Expressions.Expression<Func<T, bool>> exp)
        {
            using (PlayerDataContext ctx = new PlayerDataContext())
            {
                EntityFrameworkRepository<T> efr = new EntityFrameworkRepository<T>(ctx);
                return efr.GetEntities(exp);
            }
        }

        public IEnumerable<T> GetEntities(System.Linq.Expressions.Expression<Func<T, bool>> exp, string orderField, bool isAsc)
        {
            using (PlayerDataContext ctx = new PlayerDataContext())
            {
                EntityFrameworkRepository<T> efr = new EntityFrameworkRepository<T>(ctx);
                return efr.GetEntities(exp, orderField, isAsc);
            }
        }

        public IEnumerable<T> GetEntities(System.Linq.Expressions.Expression<Func<T, bool>> exp, Action<IOrderable<T>> orderBy)
        {
            using (PlayerDataContext ctx = new PlayerDataContext())
            {
                EntityFrameworkRepository<T> efr = new EntityFrameworkRepository<T>(ctx);
                return efr.GetEntities(exp, orderBy);
            }
        }

        public int GetEntitiesCount(System.Linq.Expressions.Expression<Func<T, bool>> exp)
        {
            using (PlayerDataContext ctx = new PlayerDataContext())
            {
                EntityFrameworkRepository<T> efr = new EntityFrameworkRepository<T>(ctx);
                return efr.GetEntitiesCount(exp);
            }
        }

        public IEnumerable<T> GetEntitiesForPaging(int pageIndex, int pageSize, System.Linq.Expressions.Expression<Func<T, bool>> exp, out int count)
        {
            using (PlayerDataContext ctx = new PlayerDataContext())
            {
                EntityFrameworkRepository<T> efr = new EntityFrameworkRepository<T>(ctx);
                return efr.GetEntitiesForPaging(pageIndex, pageSize, exp, out count);
            }
        }

        public IEnumerable<T> GetEntitiesForPaging(int pageIndex, int pageSize, string orderField, bool isAsc, System.Linq.Expressions.Expression<Func<T, bool>> exp, out int count)
        {
            using (PlayerDataContext ctx = new PlayerDataContext())
            {
                EntityFrameworkRepository<T> efr = new EntityFrameworkRepository<T>(ctx);
                return efr.GetEntitiesForPaging(pageIndex, pageSize, orderField, isAsc, exp, out count);
            }
        }

        public IEnumerable<T> GetEntitiesForPaging(int pageIndex, int pageSize, Action<Infrastructure.EntityFramework.Order.IOrderable<T>> orderBy, System.Linq.Expressions.Expression<Func<T, bool>> exp, out int count)
        {
            using (PlayerDataContext ctx = new PlayerDataContext())
            {
                EntityFrameworkRepository<T> efr = new EntityFrameworkRepository<T>(ctx);
                return efr.GetEntitiesForPaging(pageIndex, pageSize, orderBy, exp, out count);
            }
        }

        public T GetEntity(System.Linq.Expressions.Expression<Func<T, bool>> exp)
        {
            using (PlayerDataContext ctx = new PlayerDataContext())
            {
                EntityFrameworkRepository<T> efr = new EntityFrameworkRepository<T>(ctx);
                return efr.GetEntity(exp);
            }
        }
        #endregion

        #region 异步查询普通实现方案(基于Lambda表达式的Where查询)
        public Task<List<T>> GetEntitiesAsync(System.Linq.Expressions.Expression<Func<T, bool>> exp)
        {
            using (PlayerDataContext ctx = new PlayerDataContext())
            {
                EntityFrameworkRepository<T> efr = new EntityFrameworkRepository<T>(ctx);
                return efr.GetEntitiesAsync(exp);
            }
        }

        public Task<List<T>> GetEntitiesAsync(System.Linq.Expressions.Expression<Func<T, bool>> exp, string orderField, bool isAsc)
        {
            using (PlayerDataContext ctx = new PlayerDataContext())
            {
                EntityFrameworkRepository<T> efr = new EntityFrameworkRepository<T>(ctx);
                return efr.GetEntitiesAsync(exp, orderField, isAsc);
            }
        }

        public Task<List<T>> GetEntitiesAsync(System.Linq.Expressions.Expression<Func<T, bool>> exp, Action<IOrderable<T>> orderBy)
        {
            using (PlayerDataContext ctx = new PlayerDataContext())
            {
                EntityFrameworkRepository<T> efr = new EntityFrameworkRepository<T>(ctx);
                return efr.GetEntitiesAsync(exp, orderBy);
            }
        }

        public Task<int> GetEntitiesCountAsync(System.Linq.Expressions.Expression<Func<T, bool>> exp)
        {
            using (PlayerDataContext ctx = new PlayerDataContext())
            {
                EntityFrameworkRepository<T> efr = new EntityFrameworkRepository<T>(ctx);
                return efr.GetEntitiesCountAsync(exp);
            }
        }

        public Task<Tuple<List<T>, int>> GetEntitiesForPagingAsync(int pageIndex, int pageSize, System.Linq.Expressions.Expression<Func<T, bool>> exp)
        {
            using (PlayerDataContext ctx = new PlayerDataContext())
            {
                EntityFrameworkRepository<T> efr = new EntityFrameworkRepository<T>(ctx);
                return efr.GetEntitiesForPagingAsync(pageIndex, pageSize, exp);
            }
        }

        public Task<Tuple<List<T>, int>> GetEntitiesForPagingAsync(int pageIndex, int pageSize, string orderField, bool isAsc, System.Linq.Expressions.Expression<Func<T, bool>> exp)
        {
            using (PlayerDataContext ctx = new PlayerDataContext())
            {
                EntityFrameworkRepository<T> efr = new EntityFrameworkRepository<T>(ctx);
                return efr.GetEntitiesForPagingAsync(pageIndex, pageSize, orderField, isAsc, exp);
            }
        }

        public Task<Tuple<List<T>, int>> GetEntitiesForPagingAsync(int pageIndex, int pageSize, Action<Infrastructure.EntityFramework.Order.IOrderable<T>> orderBy, System.Linq.Expressions.Expression<Func<T, bool>> exp)
        {
            using (PlayerDataContext ctx = new PlayerDataContext())
            {
                EntityFrameworkRepository<T> efr = new EntityFrameworkRepository<T>(ctx);
                return efr.GetEntitiesForPagingAsync(pageIndex, pageSize, orderBy, exp);
            }
        }

        public Task<T> GetEntityAsync(System.Linq.Expressions.Expression<Func<T, bool>> exp)
        {
            using (PlayerDataContext ctx = new PlayerDataContext())
            {
                EntityFrameworkRepository<T> efr = new EntityFrameworkRepository<T>(ctx);
                return efr.GetEntityAsync(exp);
            }
        }
        #endregion

        public void Insert(T entity)
        {
            using (PlayerDataContext ctx = new PlayerDataContext())
            {
                EntityFrameworkRepository<T> efr = new EntityFrameworkRepository<T>(ctx);
                efr.Insert(entity);
            }
        }

        public void Update(T entity)
        {
            using (PlayerDataContext ctx = new PlayerDataContext())
            {
                EntityFrameworkRepository<T> efr = new EntityFrameworkRepository<T>(ctx);
                efr.Update(entity);
            }
        }

        public void Delete(T entity)
        {
            using (PlayerDataContext ctx = new PlayerDataContext())
            {
                EntityFrameworkRepository<T> efr = new EntityFrameworkRepository<T>(ctx);
                efr.Delete(entity);
            }
        }

        public async Task InsertAsync(T entity)
        {
            using (PlayerDataContext ctx = new PlayerDataContext())
            {
                EntityFrameworkRepository<T> efr = new EntityFrameworkRepository<T>(ctx);
                efr.Insert(entity);
                await ctx.CommitAsync();
            }
        }

        public async Task UpdateAsync(T entity)
        {
            using (PlayerDataContext ctx = new PlayerDataContext())
            {
                EntityFrameworkRepository<T> efr = new EntityFrameworkRepository<T>(ctx);
                efr.Update(entity);
                await ctx.CommitAsync();
            }
        }

        public async Task DeleteAsync(T entity)
        {
            using (PlayerDataContext ctx = new PlayerDataContext())
            {
                EntityFrameworkRepository<T> efr = new EntityFrameworkRepository<T>(ctx);
                efr.Delete(entity);
                await ctx.CommitAsync();
            }
        }

        public void Insert(IEnumerable<T> entitys)
        {
            using (PlayerDataContext ctx = new PlayerDataContext())
            {
                EntityFrameworkRepository<T> efr = new EntityFrameworkRepository<T>(ctx);
                efr.Insert(entitys);
            }
        }

        public async Task InsertAsync(IEnumerable<T> entitys)
        {
            using (PlayerDataContext ctx = new PlayerDataContext())
            {
                EntityFrameworkRepository<T> efr = new EntityFrameworkRepository<T>(ctx);
                efr.Insert(entitys);
                await ctx.CommitAsync();
            }
        }
    }
}
