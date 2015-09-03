using System;
using System.Data.Entity;
using System.Linq;

namespace ConsoleApplication1
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly ManagementContext _context;

        public CustomerRepository(IUnitOfWork context)
        {
            _context = context.Context;
        }

        public void SaveGraph(Customer customer)
        {
            _context.Customers.Add(customer);
            _context.ApplyStateChanges();
        }

        public void Save(Customer customer)
        {
            if (customer.CustomerId == default(int))
            {
                _context.Customers.Add(customer);
            }
            else
            {
                _context.Entry(customer).State = EntityState.Modified;
            }
        }

        public void Remove(Customer customer)
        {
            _context.Entry(customer).State = EntityState.Deleted;
        }

        public void Remove(int customerId)
        {
            var customer = Get(customerId);
            Remove(customer);
        }

        public IQueryable<Customer> GetAll()
        {
            return _context.Customers;
        }

        public Customer Get(int customerId)
        {
            return _context.Customers.Find(customerId);
        }
    }
}