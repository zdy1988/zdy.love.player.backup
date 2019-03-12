using System;
using System.Collections.Generic;
using Infrastructure.EntityFramework.Order;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public interface IRepositoryAsync<T>
         where T : class
    {
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
    }
}
