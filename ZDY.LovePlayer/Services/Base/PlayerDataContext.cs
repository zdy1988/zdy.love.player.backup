using Infrastructure.EntityFramework;
using ZDY.LovePlayer.Models;

namespace ZDY.LovePlayer.Services
{
    public class PlayerDataContext : EntityFrameworkRepositoryContext
    {
        public PlayerDataContext()
            : base(new LovePlayerContext())
        {

        }
    }
}
