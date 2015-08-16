using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;

namespace ConsoleApplication1
{
    class Program
    {
        static void Main(string[] args)
        {
            Initialize();

            //Country country;
            //using (var context = new ManagementContext())
            //{
            //    country = context.Countries.First();
            //}
            ////country.State = State.Unchanged;
            //Customer customer = new Customer()
            //{
            //    Name = "Customer 2",
            //    State = State.Added,
            //    Addresses = new Collection<Address>()
            //    {
            //        new Address()
            //        {
            //            Region = "Carabanchel",
            //            CountryId = country.CountryId,
            //            Country = country,
            //            State = State.Added
            //        }
            //    }
            //};
            //using (var context = new ManagementContext())
            //{
            //    context.Customers.Add(customer);
            //    context.ApplyStateChanges();
            //    context.SaveChanges();
            //}

            //Customer customer;
            //using (var context = new ManagementContext())
            //{
            //    customer = context.Customers
            //        .Include(p => p.Addresses)
            //        .Include(p => p.Addresses.Select(t => t.Country)).First();
            //}
            //customer.Name = "Customer 2";
            //customer.State = State.Modified;
            //var country = customer.Addresses.First().Country;
            //customer.Addresses.Add(new Address()
            //{
            //    Region = "Carabanchel",
            //    Country = country,
            //    CountryId = country.CountryId,
            //    State = State.Added
            //});
            //using (var context = new ManagementContext())
            //{
            //    context.Customers.Add(customer);
            //    context.ApplyStateChanges();
            //    context.SaveChanges();
            //}

            //Customer customer;
            //using (var context = new ManagementContext())
            //{
            //    customer = context.Customers
            //        .Include(p => p.Addresses)
            //        .Include(p => p.Addresses.Select(t => t.Country)).First();
            //    customer.Addresses.First().Country = context.Countries.Find(2);
            //    context.ChangeTracker.DetectChanges();
            //    context.SaveChanges();
            //}

            Console.ReadKey();
        }

        static void Initialize()
        {
            Database.SetInitializer(new ManagementInitializer());
            using (var context = new ManagementContext())
            {
                context.Database.Initialize(true);
            }
        }
    }

    public abstract class EntityWithState
    {
        [NotMapped]
        public State State { get; set; }
    }

    public interface ICustomerRepository
    {
        void InsertOrUpdate(Customer customer);
        void Remove(Customer customer);
        IQueryable<Customer> GetAll();
        Customer Find(int customerId);
        void SaveChanges();
    }

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

    public class ManagementInitializer : DropCreateDatabaseAlways<ManagementContext>
    {
        protected override void Seed(ManagementContext context)
        {
            var country = new Country() { Name = "España" };
            context.Countries.Add(country);

            context.Countries.Add(new Country() { Name = "USA" });

            var customer = new Customer()
            {
                Name = "Customer 1",
                Code = "C1",
                Addresses = new Collection<Address>()
                {
                    new Address() { Region = "Madrid", Country = country },
                    new Address() { Region = "Marbella", Country = country}
                }
            };
            context.Customers.Add(customer);
        }
    }

    public static class DbContextExtensions
    {
        public static void ApplyStateChanges(this DbContext context)
        {
            foreach (var entry in context.ChangeTracker.Entries<EntityWithState>())
            {
                entry.State = entry.Entity.State.ConvertToEntityState();
            }
        }
    }

    public class ManagementContext : DbContext
    {
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<Country> Countries { get; set; }
    }

    public class Address : EntityWithState
    {
        public int AddressId { get; set; }
        public string Region { get; set; }
        public int CountryId { get; set; }
        public virtual Country Country { get; set; }
        public int CustomerId { get; set; }
        public virtual Customer Customer { get; set; }
    }

    public class Customer : EntityWithState
    {
        public int CustomerId { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public virtual ICollection<Address> Addresses { get; set; }
    }

    public class Country : EntityWithState
    {
        public int CountryId { get; set; }
        public string Name { get; set; }
        public virtual ICollection<Address> Addresses { get; set; }
    }

    public enum State
    {
        Unchanged,
        Added,
        Deleted,
        Modified
    }

    public static class StateExtensions
    {
        public static EntityState ConvertToEntityState(this State state)
        {
            switch (state)
            {
                case State.Added:
                    return EntityState.Added;
                case State.Deleted:
                    return EntityState.Deleted;
                case State.Modified:
                    return EntityState.Modified;
                default:
                    return EntityState.Detached;
            }
        }
    }
}
