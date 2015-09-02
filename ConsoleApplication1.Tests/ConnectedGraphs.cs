using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace ConsoleApplication1.Tests
{
    [TestFixture]
    class ConnectedGraphs : Graphs
    {
        [Test]
        public void When_Connected_Graph_Is_Modified_All_Entities_ChangeTracker_Is_Aware()
        {
            using (var context = new ManagementContext())
            {
                //Arrange
                var customer = GetFirstCustomerFullLoaded(context);

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
