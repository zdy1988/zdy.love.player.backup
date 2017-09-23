using Infrastructure.EntityFramework;


namespace Zhu.Services
{
    public class PlayerDataContext : EntityFrameworkRepositoryContext
    {
        public PlayerDataContext()
            : base(new Models.WantChaContext())
        {

        }
    }
}
