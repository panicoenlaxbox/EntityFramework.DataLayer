using System;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Linq;
using NUnit.Framework;

namespace ConsoleApplication1.Tests
{
    class DisconnectedGraphsPitfallsTests : ManagementContextTestsBase
    {
        [Test]
        public void DbSet_Add_Set_All_Graph_Added()
        {
            //Assert
            Country country;
            using (var context = new ManagementContext())
            {
                country = context.Countries.First();
            }
            var customer = new Customer
            {
                Name = "Nuevo cliente",
                Addresses = new Collection<Address>()
                {
                    new Address() { Region = "Nueva región", Country = country }
                }
            };

            //Act
            using (var context = new ManagementContext())
            {
                context.Customers.Add(customer);

                //Assert
                Assert.AreEqual(EntityState.Added, context.Entry(country).State, "country State no es Added");

                context.SaveChanges();

                Assert.AreEqual(3, context.Countries.Count(), "Countries no son 3");
            }
        }

        [Test]
        public void DbEntityEntry_State_Modified_Only_Set_Current_Entity()
        {
            //Assert
            var customer = GetFirstDisconnectedCustomerFullyLoaded();
            customer.Name = "Cliente modificado";

            var address = customer.Addresses.First();
            const string newRegion = "Región modificada";
            address.Region = newRegion;

            using (var context = new ManagementContext())
            {
                //Act
                context.Entry(customer).State = EntityState.Modified;

                //Assert
                Assert.AreEqual(EntityState.Unchanged, context.Entry(address).State, "address State no es Unchanged");

                //Act
                context.SaveChanges();

                context.Entry(address).Reload();

                //Assert
                Assert.AreNotEqual(address.Region, newRegion, $"Region no es {newRegion}");
            }
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void DbSet_Attach_If_Foreign_Keys_Are_Invalid_Throw_Exception()
        {
            //Assert
            var customer = GetFirstDisconnectedCustomerFullyLoaded();

            var country = new Country() { Name = "Nuevo país" };
            customer.Addresses.First().Country = country;

            using (var context = new ManagementContext())
            {
                //Assert
                Assert.AreNotEqual(country.Id, customer.Addresses.First().CountryId);

                //Act
                context.Customers.Attach(customer);
            }
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void DbEntityEntry_State_Modified_If_Foreign_Keys_Are_Invalid_Throw_Exception()
        {
            //Assert
            var customer = GetFirstDisconnectedCustomerFullyLoaded();

            var country = new Country() { Name = "Nuevo país" };
            customer.Addresses.First().Country = country;

            using (var context = new ManagementContext())
            {
                //Assert
                Assert.AreNotEqual(country.Id, customer.Addresses.First().CountryId);

                //Act
                context.Entry(customer).State = EntityState.Modified;
            }
        }
    }
}
