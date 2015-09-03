using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Linq;
using NUnit.Framework;

namespace ConsoleApplication1.Tests
{
    [TestFixture]
    class DisconnectedGraphsTests : ManagementContextTestsBase
    {
        [Test]
        public void Manage_EntityWithState()
        {
            //Arrange
            Country country;
            using (var context = new ManagementContext())
            {
                country = context.Countries.First();
            }
            var customer = new Customer()
            {
                State = State.Added,
                Name = "Nuevo cliente",                
                Addresses = new Collection<Address>()
                {
                    new Address()
                    {
                        State = State.Added,
                        Region = "Nueva región 1",
                        CountryId = country.CountryId,
                        Country = country,                        
                    }
                    ,
                    new Address()
                    {
                        State = State.Added,
                        Region = "Nueva región 2",
                        CountryId = country.CountryId,
                    }
                    ,
                    new Address()
                    {
                        State = State.Added,
                        Region = "Nueva región 3",
                        Country = country,
                    }
                }
            };

            using (var context = new ManagementContext())
            {
                context.Customers.Add(customer);

                //Act
                context.ApplyStateChanges();

                //Assert
                Assert.AreEqual(context.Entry(customer).State, EntityState.Added, "customer no es Added");

                var nuevaRegion1 = customer.Addresses.Single(p => p.Region == "Nueva región 1");
                Assert.AreEqual(context.Entry(nuevaRegion1).State, EntityState.Added, "address no es Added");
                Assert.AreEqual(context.Entry(nuevaRegion1.Country).State, EntityState.Unchanged, "country no es Unchanged");
                Assert.IsNotNull(nuevaRegion1.Country, "country es null");

                var nuevaRegion2 = customer.Addresses.Single(p => p.Region == "Nueva región 2");
                Assert.IsNotNull(nuevaRegion2.Country, "country es null");

                var nuevaRegion3 = customer.Addresses.Single(p => p.Region == "Nueva región 3");
                Assert.AreNotEqual(nuevaRegion3.CountryId, 0, "CountryId es 0");
            }
        }
    }
}