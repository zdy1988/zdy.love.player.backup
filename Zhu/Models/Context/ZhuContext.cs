using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace Zhu.Models
{
    public class ZhuContext : DbContext
    {
        public ZhuContext() : base("WantCha") {
            this.Configuration.LazyLoadingEnabled = false;
        }

        public DbSet<Media> Media { get; set; }

        public DbSet<Image> Image { get; set; }

        public DbSet<Seen> Seen { get; set; }

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
