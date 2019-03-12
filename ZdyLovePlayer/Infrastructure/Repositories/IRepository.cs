using Infrastructure.EntityFramework;
using Infrastructure.EntityFramework.Order;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public interface IRepository<T>
           where T : class
    {
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
    }
}
