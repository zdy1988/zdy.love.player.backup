using Infrastructure.EntityFramework;
using ZdyLovePlayer.Models;

namespace ZdyLovePlayer.Services
{
    public class PlayerDataContext : EntityFrameworkRepositoryContext
    {
        public PlayerDataContext()
            : base(new ZdyLovePlayerContext())
        {

        }
    }
}
