namespace ConsoleApplication1
{
    public class DatabaseUnitOfWork : IUnitOfWork
    {
        public DatabaseUnitOfWork(ManagementContext context)
        {
            Context = context;
        }

        public ManagementContext Context { get; private set; }

        public void Save()
        {
            Context.SaveChanges();
        }
    }
}