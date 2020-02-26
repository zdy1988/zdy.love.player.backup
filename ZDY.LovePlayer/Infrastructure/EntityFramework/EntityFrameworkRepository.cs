using Infrastructure.Repositories;
using Infrastructure.SearchModel.Model;
using Infrastructure.SearchModel;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Infrastructure.EntityFramework.Order;
using Infrastructure.Exceptions;

namespace Infrastructure.EntityFramework
{
    public class EntityFrameworkRepository<T> :
        ICompleteRepository<T> where T : class, IEntity
    {

        #region 属性
        /// <summary>
        /// 数据上下文
        /// </summary>
        protected EntityFrameworkRepositoryContext dbContext { get; private set; }
        #endregion

        #region 构造函数
        public EntityFrameworkRepository(IEntityFrameworkRepositoryContext db)
        {
            dbContext = (EntityFrameworkRepositoryContext)db;
        }
        #endregion

        #region IRepository<T>成员

        #region 查询普通实现方案(基于Lambda表达式的Where查询)
        /// <summary>
        /// 获取所有Entity
        /// </summary>
        /// <param name="exp">Lambda条件的where</param>
        /// <returns>返回IEnumerable类型</returns>
        public virtual IEnumerable<T> GetEntities(Expression<Func<T, bool>> exp)
        {
            return dbContext.Context.Set<T>().Where<T>(exp).ToList();
        }

        /// <summary>
        /// 获取所有Entity
        /// </summary>
        /// <param name="exp">Lambda条件的where</param>
        /// <param name="orderField">需要排序的字段</param>
        /// <param name="isAsc">是否升序</param>
        /// <returns></returns>
        public virtual IEnumerable<T> GetEntities(Expression<Func<T, bool>> exp, string orderField, bool isAsc)
        {
            return dbContext.Context.Set<T>().Where<T>(exp).Sort<T>(orderField, isAsc).ToList();
        }

        /// <summary>
        /// 获取所有Entity
        /// </summary>
        /// <param name="exp">Lambda条件的where</param>
        /// <param name="orderBy">lambda排序委托</param>
        /// <returns></returns>
        public virtual IEnumerable<T> GetEntities(Expression<Func<T, bool>> exp, Action<IOrderable<T>> orderBy)
        {
            if (orderBy == null)
                throw new InvalidOperationException("基于委托的排序必须指定排序字段和排序顺序");

            return dbContext.Context.Set<T>().Where<T>(exp).Order<T>(orderBy).ToList();
        }

        /// <summary>
        /// 计算总个数(分页)
        /// </summary>
        /// <param name="exp">Lambda条件的where</param>
        /// <returns></returns>
        public virtual int GetEntitiesCount(Expression<Func<T, bool>> exp)
        {
            return dbContext.Context.Set<T>().Where<T>(exp).Count();
        }

        /// <summary>
        /// 分页查询(Linq分页方式)
        /// </summary>
        /// <param name="pageIndex">当前页</param>
        /// <param name="pageSize">页码</param>
        /// <param name="exp">lambda查询条件where</param>
        /// <returns></returns>
        public virtual IEnumerable<T> GetEntitiesForPaging(int pageIndex, int pageSize, Expression<Func<T, bool>> exp, out int count)
        {
            if (pageIndex <= 0)
                throw new ArgumentOutOfRangeException("pageIndex", pageIndex, "页码必须大于或等于1");
            if (pageSize <= 0)
                throw new ArgumentOutOfRangeException("pageSize", pageSize, "每页大小必须大于或等于1");

            var query = dbContext.Context.Set<T>().Where<T>(exp);
            count = query.Count();
            return query.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
        }

        /// <summary>
        /// 分页查询(Linq分页方式)
        /// </summary>
        /// <param name="pageIndex">当前页</param>
        /// <param name="pageSize">页码</param>
        /// <param name="orderField">需要排序的字段</param>
        /// <param name="isAsc">是否升序</param>
        /// <returns></returns>
        public virtual IEnumerable<T> GetEntitiesForPaging(int pageIndex, int pageSize, string orderField, bool isAsc, Expression<Func<T, bool>> exp, out int count)
        {
            if (pageIndex <= 0)
                throw new ArgumentOutOfRangeException("pageIndex", pageIndex, "页码必须大于或等于1");
            if (pageSize <= 0)
                throw new ArgumentOutOfRangeException("pageSize", pageSize, "每页大小必须大于或等于1");

            var query = dbContext.Context.Set<T>().Where<T>(exp);
            count = query.Count();
            return query.Sort<T>(orderField, isAsc).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
        }

        /// <summary>
        /// 分页查询(Linq分页方式)
        /// </summary>
        /// <param name="pageIndex">当前页</param>
        /// <param name="pageSize">页码</param>
        /// <param name="orderBy">lambda排序委托</param>
        /// <param name="exp">lambda查询条件where</param>
        /// <returns></returns>
        public virtual IEnumerable<T> GetEntitiesForPaging(int pageIndex, int pageSize, Action<IOrderable<T>> orderBy, Expression<Func<T, bool>> exp, out int count)
        {
            if (pageIndex <= 0)
                throw new ArgumentOutOfRangeException("pageIndex", pageIndex, "页码必须大于或等于1");
            if (pageSize <= 0)
                throw new ArgumentOutOfRangeException("pageSize", pageSize, "每页大小必须大于或等于1");
            if (orderBy == null)
                throw new InvalidOperationException("基于委托的排序必须指定排序字段和排序顺序");

            var query = dbContext.Context.Set<T>().Where<T>(exp);
            count = query.Count();
            return query.Order<T>(orderBy).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
        }

        /// <summary>
        /// 根据条件查找满足条件的一个entites
        /// </summary>
        /// <param name="exp">lambda查询条件where</param>
        /// <returns></returns>
        public virtual T GetEntity(Expression<Func<T, bool>> exp)
        {
            return dbContext.Context.Set<T>().Where<T>(exp).SingleOrDefault();
        }

        #endregion

        #region 增改删实现
        /// <summary>
        /// 插入Entity
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public virtual void Insert(T entity)
        {
            dbContext.Context.Entry<T>(entity);
            dbContext.Context.Set<T>().Add(entity);
            this.dbContext.Committed = false;
        }

        /// <summary>
        /// 更新Entity
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public virtual void Update(T entity)
        {
            dbContext.Context.Set<T>().Attach(entity);
            dbContext.Context.Entry(entity).State = EntityState.Modified;
            this.dbContext.Committed = false;
        }

        /// <summary>
        /// 删除Entity
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual void Delete(T entity)
        {
            dbContext.Context.Set<T>().Attach(entity);
            dbContext.Context.Set<T>().Remove(entity);
            this.dbContext.Committed = false;
        }
        #endregion

        #endregion

        #region IRepositoryAsync<T>成员

        #region 异步查询普通实现方案(基于Lambda表达式的Where查询)
        /// <summary>
        /// 获取所有Entity
        /// </summary>
        /// <param name="exp">Lambda条件的where</param>
        /// <returns>返回IEnumerable类型</returns>
        public virtual Task<List<T>> GetEntitiesAsync(Expression<Func<T, bool>> exp)
        {
            return dbContext.Context.Set<T>().Where<T>(exp).ToListAsync();
        }

        /// <summary>
        /// 获取所有Entity
        /// </summary>
        /// <param name="exp">Lambda条件的where</param>
        /// <param name="orderField">需要排序的字段</param>
        /// <param name="isAsc">是否升序</param>
        /// <returns></returns>
        public virtual Task<List<T>> GetEntitiesAsync(Expression<Func<T, bool>> exp, string orderField, bool isAsc)
        {
            return dbContext.Context.Set<T>().Where<T>(exp).Sort<T>(orderField, isAsc).ToListAsync();
        }

        /// <summary>
        /// 获取所有Entity
        /// </summary>
        /// <param name="exp">Lambda条件的where</param>
        /// <param name="orderBy">lambda排序委托</param>
        /// <returns></returns>
        public virtual Task<List<T>> GetEntitiesAsync(Expression<Func<T, bool>> exp, Action<IOrderable<T>> orderBy)
        {
            if (orderBy == null)
                throw new InvalidOperationException("基于委托的排序必须指定排序字段和排序顺序");

            return dbContext.Context.Set<T>().Where<T>(exp).Order<T>(orderBy).ToListAsync();
        }

        /// <summary>
        /// 计算总个数(分页)
        /// </summary>
        /// <param name="exp">Lambda条件的where</param>
        /// <returns></returns>
        public virtual Task<int> GetEntitiesCountAsync(Expression<Func<T, bool>> exp)
        {
            return dbContext.Context.Set<T>().Where<T>(exp).CountAsync();
        }

        /// <summary>
        /// 分页查询(Linq分页方式)
        /// </summary>
        /// <param name="pageIndex">当前页</param>
        /// <param name="pageSize">页码</param>
        /// <param name="exp">lambda查询条件where</param>
        /// <returns></returns>
        public virtual async Task<Tuple<List<T>, int>> GetEntitiesForPagingAsync(int pageIndex, int pageSize, Expression<Func<T, bool>> exp)
        {
            if (pageIndex <= 0)
                throw new ArgumentOutOfRangeException("pageIndex", pageIndex, "页码必须大于或等于1");
            if (pageSize <= 0)
                throw new ArgumentOutOfRangeException("pageSize", pageSize, "每页大小必须大于或等于1");

            var query = dbContext.Context.Set<T>().Where<T>(exp);
            int count = await query.CountAsync();
            var data = await query.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();
            return new Tuple<List<T>, int>(data, count);
        }

        /// <summary>
        /// 分页查询(Linq分页方式)
        /// </summary>
        /// <param name="pageIndex">当前页</param>
        /// <param name="pageSize">页码</param>
        /// <param name="orderField">需要排序的字段</param>
        /// <param name="isAsc">是否升序</param>
        /// <returns></returns>
        public virtual async Task<Tuple<List<T>, int>> GetEntitiesForPagingAsync(int pageIndex, int pageSize, string orderField, bool isAsc, Expression<Func<T, bool>> exp)
        {
            if (pageIndex <= 0)
                throw new ArgumentOutOfRangeException("pageIndex", pageIndex, "页码必须大于或等于1");
            if (pageSize <= 0)
                throw new ArgumentOutOfRangeException("pageSize", pageSize, "每页大小必须大于或等于1");

            var query = dbContext.Context.Set<T>().Where<T>(exp);
            int count = await query.CountAsync();
            var data = await query.Sort<T>(orderField, isAsc).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();
            return new Tuple<List<T>, int>(data, count);
        }

        /// <summary>
        /// 分页查询(Linq分页方式)
        /// </summary>
        /// <param name="pageIndex">当前页</param>
        /// <param name="pageSize">页码</param>
        /// <param name="orderBy">lambda排序委托</param>
        /// <param name="exp">lambda查询条件where</param>
        /// <returns></returns>
        public virtual async Task<Tuple<List<T>, int>> GetEntitiesForPagingAsync(int pageIndex, int pageSize, Action<IOrderable<T>> orderBy, Expression<Func<T, bool>> exp)
        {
            if (pageIndex <= 0)
                throw new ArgumentOutOfRangeException("pageIndex", pageIndex, "页码必须大于或等于1");
            if (pageSize <= 0)
                throw new ArgumentOutOfRangeException("pageSize", pageSize, "每页大小必须大于或等于1");
            if (orderBy == null)
                throw new InvalidOperationException("基于委托的排序必须指定排序字段和排序顺序");

            var query = dbContext.Context.Set<T>().Where<T>(exp);
            int count = await query.CountAsync();
            var data = await query.Order<T>(orderBy).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();
            return new Tuple<List<T>, int>(data, count);
        }

        /// <summary>
        /// 根据条件查找满足条件的一个entites
        /// </summary>
        /// <param name="exp">lambda查询条件where</param>
        /// <returns></returns>
        public virtual Task<T> GetEntityAsync(Expression<Func<T, bool>> exp)
        {
            return dbContext.Context.Set<T>().Where<T>(exp).SingleOrDefaultAsync();
        }

        #endregion

        #endregion

        #region IExtensionRepository<T> 成员

        public virtual void Insert(IEnumerable<T> entitys)
        {
            entitys.ToList().ForEach(i =>
            {
                dbContext.Context.Entry<T>(i);
                dbContext.Context.Set<T>().Add(i);
            });
            this.dbContext.Committed = false;
        }

        #endregion

        #region ISearchModelRepository<T>成员

        #region 查询普通实现方案(支持SearchModel的Where查询)
        /// <summary>
        /// 获取所有Entity
        /// </summary>
        /// <param name="model">用于动态生成lambda表达式的对象</param>
        /// <returns>返回IEnumerable类型</returns>
        public virtual IEnumerable<T> GetEntities(QueryModel model)
        {
            return dbContext.Context.Set<T>().Where<T>(model).ToList();
        }

        /// <summary>
        /// 获取所有Entity
        /// </summary>
        /// <param name="model">动态生成lambda查询对象</param>
        /// <param name="orderField">需要排序的字段</param>
        /// <param name="isAsc">是否升序</param>
        /// <returns></returns>
        public virtual IEnumerable<T> GetEntities(QueryModel model, string orderField, bool isAsc)
        {
            return dbContext.Context.Set<T>().Where<T>(model).Sort<T>(orderField, isAsc).ToList();
        }

        /// <summary>
        /// 获取所有Entity
        /// </summary>
        /// <param name="model">动态生成lambda查询对象</param>
        /// <param name="orderBy">lambda排序委托</param>
        /// <returns></returns>
        public virtual IEnumerable<T> GetEntities(QueryModel model, Action<IOrderable<T>> orderBy)
        {
            if (orderBy == null)
                throw new InvalidOperationException("基于委托的排序必须指定排序字段和排序顺序");

            return dbContext.Context.Set<T>().Where<T>(model).Order<T>(orderBy).ToList();
        }

        /// <summary>
        /// 计算总个数(分页)
        /// </summary>
        /// <param name="model">用于动态生成lambda表达式的对象</param>
        /// <returns></returns>
        public virtual int GetEntitiesCount(QueryModel model)
        {
            return dbContext.Context.Set<T>().Where<T>(model).Count();
        }

        /// <summary>
        /// 分页查询(Linq分页方式)
        /// </summary>
        /// <param name="pageIndex">当前页</param>
        /// <param name="pageSize">页码</param>
        /// <param name="model">动态生成lambda查询对象</param>
        /// <returns></returns>
        public virtual IEnumerable<T> GetEntitiesForPaging(int pageIndex, int pageSize, QueryModel model, out int count)
        {
            if (pageIndex <= 0)
                throw new ArgumentOutOfRangeException("pageIndex", pageIndex, "页码必须大于或等于1");
            if (pageSize <= 0)
                throw new ArgumentOutOfRangeException("pageSize", pageSize, "每页大小必须大于或等于1");

            var query = dbContext.Context.Set<T>().Where<T>(model);
            count = query.Count();
            return query.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
        }

        /// <summary>
        /// 分页查询(Linq分页方式)
        /// </summary>
        /// <param name="pageIndex">当前页</param>
        /// <param name="pageSize">页码</param>
        /// <param name="orderField">需要排序的字段</param>
        /// <param name="isAsc">是否升序</param>
        /// <param name="model">动态生成lambda查询对象</param>
        /// <returns></returns>
        public virtual IEnumerable<T> GetEntitiesForPaging(int pageIndex, int pageSize, string orderField, bool isAsc, QueryModel model, out int count)
        {
            if (pageIndex <= 0)
                throw new ArgumentOutOfRangeException("pageIndex", pageIndex, "页码必须大于或等于1");
            if (pageSize <= 0)
                throw new ArgumentOutOfRangeException("pageSize", pageSize, "每页大小必须大于或等于1");

            var query = dbContext.Context.Set<T>().Where<T>(model);
            count = query.Count();
            return query.Sort<T>(orderField, isAsc).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
        }

        /// <summary>
        /// 分页查询(Linq分页方式)
        /// </summary>
        /// <param name="pageIndex">当前页</param>
        /// <param name="pageSize">页码</param>
        /// <param name="orderBy">lambda排序委托</param>
        /// <param name="model">用于动态生成lambda表达式的对象</param>
        /// <returns></returns>
        public virtual IEnumerable<T> GetEntitiesForPaging(int pageIndex, int pageSize, Action<IOrderable<T>> orderBy, QueryModel model, out int count)
        {
            if (pageIndex <= 0)
                throw new ArgumentOutOfRangeException("pageIndex", pageIndex, "页码必须大于或等于1");
            if (pageSize <= 0)
                throw new ArgumentOutOfRangeException("pageSize", pageSize, "每页大小必须大于或等于1");
            if (orderBy == null)
                throw new InvalidOperationException("基于委托的排序必须指定排序字段和排序顺序");

            var query = dbContext.Context.Set<T>().Where<T>(model);
            count = query.Count();
            return query.Order(orderBy).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
        }

        /// <summary>
        /// 根据条件查找满足条件的一个entites
        /// </summary>
        /// <param name="model">用于动态生成lambda表达式的对象</param>
        /// <returns></returns>
        public virtual T GetEntity(QueryModel model)
        {
            return dbContext.Context.Set<T>().Where<T>(model).SingleOrDefault();
        }

        #endregion

        #endregion

        #region ISearchModelRepositoryAsync<T>成员

        #region 查询普通实现方案(支持SearchModel的Where查询)
        /// <summary>
        /// 获取所有Entity
        /// </summary>
        /// <param name="model">用于动态生成lambda表达式的对象</param>
        /// <returns>返回IEnumerable类型</returns>
        public virtual Task<List<T>> GetEntitiesAsync(QueryModel model)
        {
            return dbContext.Context.Set<T>().Where<T>(model).ToListAsync();
        }

        /// <summary>
        /// 获取所有Entity
        /// </summary>
        /// <param name="model">动态生成lambda查询对象</param>
        /// <param name="orderField">需要排序的字段</param>
        /// <param name="isAsc">是否升序</param>
        /// <returns></returns>
        public virtual Task<List<T>> GetEntitiesAsync(QueryModel model, string orderField, bool isAsc)
        {
            return dbContext.Context.Set<T>().Where<T>(model).Sort<T>(orderField, isAsc).ToListAsync();
        }

        /// <summary>
        /// 获取所有Entity
        /// </summary>
        /// <param name="model">动态生成lambda查询对象</param>
        /// <param name="orderBy">lambda排序委托</param>
        /// <returns></returns>
        public virtual Task<List<T>> GetEntitiesAsync(QueryModel model, Action<IOrderable<T>> orderBy)
        {
            if (orderBy == null)
                throw new InvalidOperationException("基于委托的排序必须指定排序字段和排序顺序");

            return dbContext.Context.Set<T>().Where<T>(model).Order<T>(orderBy).ToListAsync();
        }

        /// <summary>
        /// 计算总个数(分页)
        /// </summary>
        /// <param name="model">用于动态生成lambda表达式的对象</param>
        /// <returns></returns>
        public virtual Task<int> GetEntitiesCountAsync(QueryModel model)
        {
            return dbContext.Context.Set<T>().Where<T>(model).CountAsync();
        }

        /// <summary>
        /// 分页查询(Linq分页方式)
        /// </summary>
        /// <param name="pageIndex">当前页</param>
        /// <param name="pageSize">页码</param>
        /// <param name="model">动态生成lambda查询对象</param>
        /// <returns></returns>
        public virtual async Task<Tuple<List<T>, int>> GetEntitiesForPagingAsync(int pageIndex, int pageSize, QueryModel model)
        {
            if (pageIndex <= 0)
                throw new ArgumentOutOfRangeException("pageIndex", pageIndex, "页码必须大于或等于1");
            if (pageSize <= 0)
                throw new ArgumentOutOfRangeException("pageSize", pageSize, "每页大小必须大于或等于1");

            var query = dbContext.Context.Set<T>().Where<T>(model);
            int count = await query.CountAsync();
            var data = await query.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();
            return new Tuple<List<T>, int>(data, count);
        }

        /// <summary>
        /// 分页查询(Linq分页方式)
        /// </summary>
        /// <param name="pageIndex">当前页</param>
        /// <param name="pageSize">页码</param>
        /// <param name="orderField">需要排序的字段</param>
        /// <param name="isAsc">是否升序</param>
        /// <param name="model">动态生成lambda查询对象</param>
        /// <returns></returns>
        public virtual async Task<Tuple<List<T>, int>> GetEntitiesForPagingAsync(int pageIndex, int pageSize, string orderField, bool isAsc, QueryModel model)
        {
            if (pageIndex <= 0)
                throw new ArgumentOutOfRangeException("pageIndex", pageIndex, "页码必须大于或等于1");
            if (pageSize <= 0)
                throw new ArgumentOutOfRangeException("pageSize", pageSize, "每页大小必须大于或等于1");

            var query = dbContext.Context.Set<T>().Where<T>(model);
            int count = await query.CountAsync();
            var data = await query.Sort<T>(orderField, isAsc).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();
            return new Tuple<List<T>, int>(data, count);
        }

        /// <summary>
        /// 分页查询(Linq分页方式)
        /// </summary>
        /// <param name="pageIndex">当前页</param>
        /// <param name="pageSize">页码</param>
        /// <param name="orderBy">lambda排序委托</param>
        /// <param name="model">用于动态生成lambda表达式的对象</param>
        /// <returns></returns>
        public virtual async Task<Tuple<List<T>, int>> GetEntitiesForPagingAsync(int pageIndex, int pageSize, Action<IOrderable<T>> orderBy, QueryModel model)
        {
            if (pageIndex <= 0)
                throw new ArgumentOutOfRangeException("pageIndex", pageIndex, "页码必须大于或等于1");
            if (pageSize <= 0)
                throw new ArgumentOutOfRangeException("pageSize", pageSize, "每页大小必须大于或等于1");
            if (orderBy == null)
                throw new InvalidOperationException("基于委托的排序必须指定排序字段和排序顺序");

            var query = dbContext.Context.Set<T>().Where<T>(model);
            int count = await query.CountAsync();
            var data = await query.Order(orderBy).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();
            return new Tuple<List<T>, int>(data, count);
        }

        /// <summary>
        /// 根据条件查找满足条件的一个entites
        /// </summary>
        /// <param name="model">用于动态生成lambda表达式的对象</param>
        /// <returns></returns>
        public virtual Task<T> GetEntityAsync(QueryModel model)
        {
            return dbContext.Context.Set<T>().Where<T>(model).SingleOrDefaultAsync();
        }

        #endregion

        #endregion
    }
}
