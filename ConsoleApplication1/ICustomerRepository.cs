using System.Linq;

namespace ConsoleApplication1
{
    public interface ICustomerRepository
    {
        void InsertOrUpdate(Customer customer);
        void Remove(Customer customer);
        IQueryable<Customer> GetAll();
        Customer Find(int customerId);
        void SaveChanges();
    }
}