using System;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Linq;
using NUnit.Framework;

namespace ConsoleApplication1.Tests
{
    public class DisconnectedGraphsPitfalls
    {
        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            Database.SetInitializer(new ManagementInitializer());
        }

        [SetUp]
        public void SetUp()
        {
            using (var context = new ManagementContext())
            {
                context.Database.Initialize(true);
            }
        }

        [Test]
        public void DbSet_Add_Set_All_Graph_Added()
        {
            //Assert
            Country country;
            using (var context = new ManagementContext())
            {
                country = context.Countries.First();
            }
            var customer = new Customer()
            {
                Name = "Customer 2",
                Addresses = new Collection<Address>()
                {
                    new Address() { Region = "Carabanchel", Country = country }
                }
            };

            //Act
            using (var context = new ManagementContext())
            {
                context.Customers.Add(customer);

                //Assert
                Assert.AreEqual(EntityState.Added, context.Entry(country).State, "country State is not Added");
                context.SaveChanges();
                Assert.AreEqual(2, context.Countries.Count(), "Countries are not 2");
            }
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void DbSet_Attach_Throw_Exception_If_Foreign_Keys_Are_Invalid()
        {
            //Assert
            Customer customer;
            using (var context = new ManagementContext())
            {
                customer = context.Customers
                    .Include(p => p.Addresses)
                    .Include(p => p.Addresses.Select(t => t.Country)).First();
            }
            Country country = new Country() { Name = "France" };
            customer.Addresses.First().Country = country;
            using (var context = new ManagementContext())
            {
                //Assert
                Assert.AreNotEqual(country.CountryId, customer.Addresses.First().CountryId);

                //Act
                context.Customers.Attach(customer);
            }
        }

        [Test]
        public void DbEntityEntry_State_Modified_Only_Set_Current_Entity()
        {
            //Assert
            Customer customer;
            using (var context = new ManagementContext())
            {
                customer = context.Customers
                    .Include(p => p.Addresses)
                    .Include(p => p.Addresses.Select(t => t.Country)).First();
            }
            customer.Name = "Customer 2";
            var address = customer.Addresses.First();
            address.Region = "Carabanchel";
            using (var context = new ManagementContext())
            {
                //Act
                context.Entry(customer).State = EntityState.Modified;

                //Assert
                var entityState = context.Entry(address).State;
                Assert.AreEqual(EntityState.Unchanged, entityState, "Address State is not Unchanged");

                //Act
                context.SaveChanges();

                context.Entry(address).Reload();
                var region = address.Region;

                //Assert
                Assert.AreNotEqual(region, "Carabanchel", "Region is Carabanchel");
            }
        }
    }
}
