using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    /// <summary>
    /// 扩展的Repository操作规范
    /// </summary>
    public interface IExtensionRepository<T> where T : class
    {
        /// <summary>
        /// 批量添加
        /// </summary>
        /// <param name="item"></param>
        void Insert(IEnumerable<T> entitys);
    }
}
