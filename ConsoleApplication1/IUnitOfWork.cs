using System.Data.Entity;

namespace ConsoleApplication1
{
    public interface IUnitOfWork
    {
        DbContext Context { get; }
        void Save();
    }
}