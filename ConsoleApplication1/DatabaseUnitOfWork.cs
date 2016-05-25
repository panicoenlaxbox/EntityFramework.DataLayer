using System.Data.Entity;

namespace ConsoleApplication1
{
    public class DatabaseUnitOfWork : IUnitOfWork
    {
        public DatabaseUnitOfWork(DbContext context)
        {
            Context = context;
        }

        public DbContext Context { get; private set; }

        public void Save()
        {
            Context.SaveChanges();
        }
    }
}