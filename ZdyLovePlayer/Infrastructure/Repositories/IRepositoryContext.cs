using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    /// <summary>
    /// 表示实现该接口的类型是仓储上下文。
    /// </summary>
    public interface IRepositoryContext : IUnitOfWork, IDisposable
    {

    }
}
