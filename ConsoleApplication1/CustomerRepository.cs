using System;
using System.Data.Entity;
using System.Linq;

namespace ConsoleApplication1
{
    class CustomerRepository : ICustomerRepository, IDisposable
    {
        private readonly ManagementContext _context;

        public CustomerRepository()
        {
            _context = new ManagementContext();
        }

        public void InsertOrUpdateGraph(Customer customer)
        {

        }

        public void InsertOrUpdate(Customer customer)
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
            _context.Customers.Remove(customer);
        }

        public IQueryable<Customer> GetAll()
        {
            return _context.Customers;
        }

        public Customer Find(int customerId)
        {
            return _context.Customers.Find(customerId);
        }

        public void SaveChanges()
        {
            _context.SaveChanges();
        }

        #region IDisposable
        bool _disposed;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~CustomerRepository()
        {
            Dispose(false);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                // free other managed objects that implement
                // IDisposable only
                _context.Dispose();
            }

            // release any unmanaged objects
            // set the object references to null

            _disposed = true;
        }
        #endregion
    }
}