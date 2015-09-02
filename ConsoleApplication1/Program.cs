using System;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Linq;

namespace ConsoleApplication1
{
    class Program
    {
        static void Main(string[] args)
        {
            InitializeDatabase();

            //Country country;
            //using (var context = new ManagementContext())
            //{
            //    country = context.Countries.First();
            //}
            //country.State = State.Unchanged;
            //Customer customer = new Customer()
            //{
            //    Name = "Nuevo cliente",
            //    State = State.Added,
            //    Addresses = new Collection<Address>()
            //    {
            //        new Address()
            //        {
            //            Region = "Nueva región",
            //            CountryId = country.CountryId,
            //            Country = country, // si se pone algo, CountryId tiene que ir a nivel, pero puede no ponerse Country, dejarse a null
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
            //customer.Name = "Cliente modificado";
            //customer.State = State.Modified;
            //var country = customer.Addresses.First().Country;
            //customer.Addresses.Add(new Address()
            //{
            //    Region = "Nueva región",
            //    CountryId = country.CountryId,
            //    Country = country,                
            //    State = State.Added
            //});
            //using (var context = new ManagementContext())
            //{
            //    context.Customers.Add(customer);
            //    context.ApplyStateChanges();
            //    context.SaveChanges();
            //}



            using (var context = new ManagementContext())
            {
                var customer = context.Customers
                    .Include(p => p.Addresses)
                    .Include(p => p.Addresses.Select(t => t.Country)).First();
                customer.Addresses.First().Country = context.Countries.Find(2);

                context.ChangeTracker.DetectChanges();

                context.SaveChanges();
            }

            Console.ReadKey();
        }

        static void InitializeDatabase()
        {
            Database.SetInitializer(new ManagementInitializer());
            using (var context = new ManagementContext())
            {
                context.Database.Initialize(true);
            }
        }
    }
}
