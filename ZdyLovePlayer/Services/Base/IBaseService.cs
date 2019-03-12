using Infrastructure;
using Infrastructure.EntityFramework;
using Infrastructure.EntityFramework.Order;
using Infrastructure.SearchModel.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ZdyLovePlayer.Services
{
    public interface IBaseService<T> where T : class, IEntity
    {

        #region 查询普通实现方案(支持SearchModel的Where查询)
        /// <summary>
        /// 获取所有Entity
        /// </summary>
        /// <param name="model">动态生成lambda查询对象</param>
        /// <returns></returns>
        IEnumerable<T> GetEntities(QueryModel model);

        /// <summary>
        /// 获取所有Entity
        /// </summary>
        /// <param name="model">动态生成lambda查询对象</param>
        /// <param name="orderField">需要排序的字段</param>
        /// <param name="isAsc">是否升序</param>
        /// <returns></returns>
        IEnumerable<T> GetEntities(QueryModel model, string orderField, bool isAsc);

        /// <summary>
        /// 获取所有Entity
        /// </summary>
        /// <param name="model">动态生成lambda查询对象</param>
        /// <param name="orderBy">lambda排序委托</param>
        /// <returns></returns>
        IEnumerable<T> GetEntities(QueryModel model, Action<IOrderable<T>> orderBy);

        /// <summary>
        /// 计算总个数(分页)
        /// </summary>
        //// <param name="model">动态生成lambda查询对象</param>
        /// <returns></returns>
        int GetEntitiesCount(QueryModel model);

        /// <summary>
        /// 分页查询(Linq分页方式)
        /// </summary>
        /// <param name="pageIndex">当前页</param>
        /// <param name="pageSize">页码</param>
        /// <param name="model">动态生成lambda查询对象</param>
        /// <returns></returns>
        IEnumerable<T> GetEntitiesForPaging(int pageIndex, int pageSize, QueryModel model, out int count);

        /// <summary>
        /// 分页查询(Linq分页方式)
        /// </summary>
        /// <param name="pageIndex">当前页</param>
        /// <param name="pageSize">页码</param>
        /// <param name="orderField">需要排序的字段</param>
        /// <param name="isAsc">是否升序</param>
        /// <param name="model">动态生成lambda查询对象</param>
        /// <returns></returns>
        IEnumerable<T> GetEntitiesForPaging(int pageIndex, int pageSize, string orderField, bool isAsc, QueryModel model, out int count);

        /// <summary>
        /// 分页查询(Linq分页方式)
        /// </summary>
        /// <param name="pageIndex">当前页</param>
        /// <param name="pageSize">页码</param>
        /// <param name="orderBy">lambda排序委托</param>
        /// <param name="model">动态生成lambda查询对象</param>
        /// <returns></returns>
        IEnumerable<T> GetEntitiesForPaging(int pageIndex, int pageSize, Action<IOrderable<T>> orderBy, QueryModel model, out int count);

        /// <summary>
        /// 根据条件查找
        /// </summary>
        /// <param name="model">动态生成lambda查询对象</param>
        /// <returns></returns>
        T GetEntity(QueryModel model);

        #endregion

        #region 异步查询普通实现方案(支持SearchModel的Where查询)
        /// <summary>
        /// 获取所有Entity
        /// </summary>
        /// <param name="model">动态生成lambda查询对象</param>
        /// <returns></returns>
        Task<List<T>> GetEntitiesAsync(QueryModel model);

        /// <summary>
        /// 获取所有Entity
        /// </summary>
        /// <param name="model">动态生成lambda查询对象</param>
        /// <param name="orderField">需要排序的字段</param>
        /// <param name="isAsc">是否升序</param>
        /// <returns></returns>
        Task<List<T>> GetEntitiesAsync(QueryModel model, string orderField, bool isAsc);

        /// <summary>
        /// 获取所有Entity
        /// </summary>
        /// <param name="model">动态生成lambda查询对象</param>
        /// <param name="orderBy">lambda排序委托</param>
        /// <returns></returns>
        Task<List<T>> GetEntitiesAsync(QueryModel model, Action<IOrderable<T>> orderBy);

        /// <summary>
        /// 计算总个数(分页)
        /// </summary>
        //// <param name="model">动态生成lambda查询对象</param>
        /// <returns></returns>
        Task<int> GetEntitiesCountAsync(QueryModel model);

        /// <summary>
        /// 分页查询(Linq分页方式)
        /// </summary>
        /// <param name="pageIndex">当前页</param>
        /// <param name="pageSize">页码</param>
        /// <param name="model">动态生成lambda查询对象</param>
        /// <returns></returns>
        Task<Tuple<List<T>, int>> GetEntitiesForPagingAsync(int pageIndex, int pageSize, QueryModel model);

        /// <summary>
        /// 分页查询(Linq分页方式)
        /// </summary>
        /// <param name="pageIndex">当前页</param>
        /// <param name="pageSize">页码</param>
        /// <param name="orderField">需要排序的字段</param>
        /// <param name="isAsc">是否升序</param>
        /// <param name="model">动态生成lambda查询对象</param>
        /// <returns></returns>
        Task<Tuple<List<T>, int>> GetEntitiesForPagingAsync(int pageIndex, int pageSize, string orderField, bool isAsc, QueryModel model);

        /// <summary>
        /// 分页查询(Linq分页方式)
        /// </summary>
        /// <param name="pageIndex">当前页</param>
        /// <param name="pageSize">页码</param>
        /// <param name="orderBy">lambda排序委托</param>
        /// <param name="model">动态生成lambda查询对象</param>
        /// <returns></returns>
        Task<Tuple<List<T>, int>> GetEntitiesForPagingAsync(int pageIndex, int pageSize, Action<IOrderable<T>> orderBy, QueryModel model);

        /// <summary>
        /// 根据条件查找
        /// </summary>
        /// <param name="model">动态生成lambda查询对象</param>
        /// <returns></returns>
        Task<T> GetEntityAsync(QueryModel model);

        #endregion

        #region 查询普通实现方案(基于Lambda表达式的Where查询)
        /// <summary>
        /// 获取所有Entity
        /// </summary>
        /// <param name="exp">Lambda条件的where</param>
        /// <returns></returns>
        IEnumerable<T> GetEntities(Expression<Func<T, bool>> exp);

        /// <summary>
        /// 获取所有Entity
        /// </summary>
        /// <param name="exp">Lambda条件的where</param>
        /// <param name="orderField">需要排序的字段</param>
        /// <param name="isAsc">是否升序</param>
        /// <returns></returns>
        IEnumerable<T> GetEntities(Expression<Func<T, bool>> exp, string orderField, bool isAsc);

        /// <summary>
        /// 获取所有Entity
        /// </summary>
        /// <param name="exp">Lambda条件的where</param>
        /// <param name="orderBy">lambda排序委托</param>
        /// <returns></returns>
        IEnumerable<T> GetEntities(Expression<Func<T, bool>> exp, Action<IOrderable<T>> orderBy);

        /// <summary>
        /// 计算总个数(分页)
        /// </summary>
        /// <param name="exp">Lambda条件的where</param>
        /// <returns></returns>
        int GetEntitiesCount(Expression<Func<T, bool>> exp);

        /// <summary>
        /// 分页查询(Linq分页方式)
        /// </summary>
        /// <param name="pageIndex">当前页</param>
        /// <param name="pageSize">页码</param>
        /// <param name="exp">lambda查询条件where</param>
        /// <returns></returns>
        IEnumerable<T> GetEntitiesForPaging(int pageIndex, int pageSize, Expression<Func<T, bool>> exp, out int count);

        /// <summary>
        /// 分页查询(Linq分页方式)
        /// </summary>
        /// <param name="pageIndex">当前页</param>
        /// <param name="pageSize">页码</param>
        /// <param name="orderField">需要排序的字段</param>
        /// <param name="isAsc">是否升序</param>
        /// <returns></returns>
        IEnumerable<T> GetEntitiesForPaging(int pageIndex, int pageSize, string orderField, bool isAsc, Expression<Func<T, bool>> exp, out int count);

        /// <summary>
        /// 分页查询(Linq分页方式)
        /// </summary>
        /// <param name="pageIndex">当前页</param>
        /// <param name="pageSize">页码</param>
        /// <param name="orderBy">lambda排序委托</param>
        /// <param name="exp">lambda查询条件where</param>
        /// <returns></returns>
        IEnumerable<T> GetEntitiesForPaging(int pageIndex, int pageSize, Action<IOrderable<T>> orderBy, Expression<Func<T, bool>> exp, out int count);

        /// <summary>
        /// 根据条件查找
        /// </summary>
        /// <param name="exp">lambda查询条件where</param>
        /// <returns></returns>
        T GetEntity(Expression<Func<T, bool>> exp);

        #endregion

        #region 异步查询普通实现方案(基于Lambda表达式的Where查询)
        /// <summary>
        /// 获取所有Entity
        /// </summary>
        /// <param name="exp">Lambda条件的where</param>
        /// <returns></returns>
        Task<List<T>> GetEntitiesAsync(Expression<Func<T, bool>> exp);

        /// <summary>
        /// 获取所有Entity
        /// </summary>
        /// <param name="exp">Lambda条件的where</param>
        /// <param name="orderField">需要排序的字段</param>
        /// <param name="isAsc">是否升序</param>
        /// <returns></returns>
        Task<List<T>> GetEntitiesAsync(Expression<Func<T, bool>> exp, string orderField, bool isAsc);

        /// <summary>
        /// 获取所有Entity
        /// </summary>
        /// <param name="exp">Lambda条件的where</param>
        /// <param name="orderBy">lambda排序委托</param>
        /// <returns></returns>
        Task<List<T>> GetEntitiesAsync(Expression<Func<T, bool>> exp, Action<IOrderable<T>> orderBy);

        /// <summary>
        /// 计算总个数(分页)
        /// </summary>
        /// <param name="exp">Lambda条件的where</param>
        /// <returns></returns>
        Task<int> GetEntitiesCountAsync(Expression<Func<T, bool>> exp);

        /// <summary>
        /// 分页查询(Linq分页方式)
        /// </summary>
        /// <param name="pageIndex">当前页</param>
        /// <param name="pageSize">页码</param>
        /// <param name="exp">lambda查询条件where</param>
        /// <returns></returns>
        Task<Tuple<List<T>, int>> GetEntitiesForPagingAsync(int pageIndex, int pageSize, Expression<Func<T, bool>> exp);

        /// <summary>
        /// 分页查询(Linq分页方式)
        /// </summary>
        /// <param name="pageIndex">当前页</param>
        /// <param name="pageSize">页码</param>
        /// <param name="orderField">需要排序的字段</param>
        /// <param name="isAsc">是否升序</param>
        /// <returns></returns>
        Task<Tuple<List<T>, int>> GetEntitiesForPagingAsync(int pageIndex, int pageSize, string orderField, bool isAsc, Expression<Func<T, bool>> exp);

        /// <summary>
        /// 分页查询(Linq分页方式)
        /// </summary>
        /// <param name="pageIndex">当前页</param>
        /// <param name="pageSize">页码</param>
        /// <param name="orderBy">lambda排序委托</param>
        /// <param name="exp">lambda查询条件where</param>
        /// <returns></returns>
        Task<Tuple<List<T>, int>> GetEntitiesForPagingAsync(int pageIndex, int pageSize, Action<IOrderable<T>> orderBy, Expression<Func<T, bool>> exp);

        /// <summary>
        /// 根据条件查找
        /// </summary>
        /// <param name="exp">lambda查询条件where</param>
        /// <returns></returns>
        Task<T> GetEntityAsync(Expression<Func<T, bool>> exp);

        #endregion

        /// <summary>
        /// 插入Entity
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        void Insert(T entity);
        /// <summary>
        /// 更新Entity
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        void Update(T entity);
        /// <summary>
        /// 删除Entity
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        void Delete(T entity);
        /// <summary>
        /// 异步插入Entity
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task InsertAsync(T entity);
        /// <summary>
        /// 异步更新Entity
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task UpdateAsync(T entity);
        /// <summary>
        /// 异步删除Entity
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task DeleteAsync(T entity);
        /// <summary>
        /// 添加集合
        /// </summary>
        /// <param name="item"></param>
        void Insert(IEnumerable<T> entitys);
        /// <summary>
        /// 异步添加集合
        /// </summary>
        /// <param name="item"></param>
        Task InsertAsync(IEnumerable<T> entitys);
    }
}
