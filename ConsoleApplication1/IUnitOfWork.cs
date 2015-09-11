namespace ConsoleApplication1
{
    public interface IUnitOfWork
    {
        ManagementContext Context { get; }
        void Save();
    }
}