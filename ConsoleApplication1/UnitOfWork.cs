namespace ConsoleApplication1
{
    public class UnitOfWork : IUnitOfWork
    {
        public UnitOfWork(ManagementContext context)
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