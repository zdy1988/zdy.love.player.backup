using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    /// <summary>
    /// 完整的数据操作接口
    /// </summary>
    public interface ICompleteRepository<T> :
        IRepository<T>,
        IRepositoryAsync<T>,
        IExtensionRepository<T>,
        ISearchModelRepository<T>,
        ISearchModelRepositoryAsync<T>
         where T : class ,IEntity
    {
    }
}
