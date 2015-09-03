using System;
using System.Linq;

namespace ConsoleApplication1
{
    public interface ICustomerRepository
    {
        void Save(Customer customer);
        void Remove(Customer customer);
        void Remove(int customerId);
        IQueryable<Customer> GetAll();
        Customer Get(int customerId);
    }
}