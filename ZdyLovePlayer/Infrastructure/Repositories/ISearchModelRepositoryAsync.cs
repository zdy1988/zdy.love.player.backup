using Infrastructure.EntityFramework.Order;
using Infrastructure.SearchModel.Model;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    //扩展查询方法支持SearchModel
    public interface ISearchModelRepositoryAsync<T> where T : class
    {
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
    }
}
