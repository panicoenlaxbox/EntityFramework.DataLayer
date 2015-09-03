using System.Linq;
using NUnit.Framework;

namespace ConsoleApplication1.Tests
{
    [TestFixture]
    class ConnectedGraphsTests : ManagementContextTestsBase
    {
        [Test]
        public void When_Connected_Graph_Is_Modified_ChangeTracker_Is_Aware_For_All_Entities()
        {
            using (var context = new ManagementContext())
            {
                //Arrange
                var customer = GetFirstCustomerFullyLoaded(context);

                //Act
                customer.Name = "Cliente modificado";
                var address = customer.Addresses.First();
                address.Region = "Región modificada";

                //Assert
                Assert.IsTrue(context.Entry(customer).Property(p => p.Name).IsModified, "Name no está modificado");
                Assert.IsTrue(context.Entry(address).Property(p => p.Region).IsModified, "Region no está modificada");
            }
        }
    }
}
