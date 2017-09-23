using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Infrastructure.EntityFramework
{
    public class EntityFrameworkRepositoryContext : IEntityFrameworkRepositoryContext
    {
        private readonly ThreadLocal<DbContext> localContext;

        public EntityFrameworkRepositoryContext(string dbContextName)
        {
            localContext = new ThreadLocal<DbContext>(() => new System.Data.Entity.DbContext(dbContextName));
        }

        public EntityFrameworkRepositoryContext(DbContext context)
        {
            localContext = new ThreadLocal<DbContext>(() => context);
        }

        public System.Data.Entity.DbContext Context
        {
            get { return localContext.Value; }
        }

        public bool Committed
        {
            get;
            set;
        }

        public void Commit()
        {
            if (!Committed)
            {
                try
                {
                    localContext.Value.SaveChanges();
                }
                catch (DbEntityValidationException e)
                {
                    throw e;
                }
                catch (Exception e) {
                    throw e;
                }
                Committed = true;
            }
        }

        public async Task CommitAsync()
        {
            if (!Committed)
            {
                try
                {
                   await localContext.Value.SaveChangesAsync();
                }
                catch (DbEntityValidationException e)
                {
                    throw e;
                }
                catch (Exception e)
                {
                    throw e;
                }
                Committed = true;
            }
        }

        public void Rollback()
        {
            this.Committed = true;
        }

        public void Dispose()
        {
            if (!Committed)
                Commit();
            localContext.Value.Dispose();
            localContext.Dispose();
        }
    }
}
