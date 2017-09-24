using Infrastructure.EntityFramework;
using Zhu.Models;

namespace Zhu.Services
{
    public class PlayerDataContext : EntityFrameworkRepositoryContext
    {
        public PlayerDataContext()
            : base(new ZhuContext())
        {

        }
    }
}
