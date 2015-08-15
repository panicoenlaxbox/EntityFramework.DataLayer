using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;

namespace ConsoleApplication1
{
    class Program
    {
        static void Main(string[] args)
        {
            Initialize();

            using (var context = new ManagementContext())
            {
                var customer = context.Customers.Find(1);
            }

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

    public class ManagementInitializer : DropCreateDatabaseAlways<ManagementContext>
    {
        protected override void Seed(ManagementContext context)
        {
            var country = new Country() { Name = "España" };
            context.Countries.Add(country);

            var customer = new Customer()
            {
                Name = "Customer 1",
                Addresses = new Collection<Address>()
                {
                    new Address() { Region = "Madrid", Country = country },
                    new Address() { Region = "Marbella", Country = country}
                }
            };
            context.Customers.Add(customer);
        }
    }

    public class ManagementContext : DbContext
    {
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<Country> Countries { get; set; }
    }

    public class Address
    {
        public int AddressId { get; set; }
        public string Region { get; set; }
        public int CountryId { get; set; }
        public virtual Country Country { get; set; }
        public int CustomerId { get; set; }
        public virtual Customer Customer { get; set; }
    }

    public class Customer
    {
        public int CustomerId { get; set; }
        public string Name { get; set; }
        public ICollection<Address> Addresses { get; set; }
    }

    public class Country
    {
        public int CountryId { get; set; }
        public string Name { get; set; }
        public ICollection<Address> Addresses { get; set; }
    }
}
