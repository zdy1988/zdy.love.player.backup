using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace Zhu.Models
{
    public class WantChaContext : DbContext
    {
        public WantChaContext() : base("WantCha") {
            this.Configuration.LazyLoadingEnabled = false;
        }

        public DbSet<Movie> Movie { get; set; }

        public DbSet<NetTV> NetTV { get; set; }

        public DbSet<Image> Image { get; set; }

        public DbSet<Tag> Tag { get; set; }

        public DbSet<Actor> Actor { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();
            modelBuilder.Conventions.Remove<ManyToManyCascadeDeleteConvention>();
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }
    }
}
