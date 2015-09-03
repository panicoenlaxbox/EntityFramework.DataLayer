using System.Data.Entity;
using System.Linq;
using NUnit.Framework;

namespace ConsoleApplication1.Tests
{
    internal abstract class ManagementContextTestsBase
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

        protected static Customer GetFirstDisconnectedCustomerFullyLoaded()
        {
            Customer customer;
            using (var context = new ManagementContext())
            {
                customer = GetFirstCustomerFullyLoaded(context);
            }
            return customer;
        }

        protected static Customer GetFirstCustomerFullyLoaded(ManagementContext context)
        {
            return context.Customers
                .Include(p => p.Addresses)
                .Include(p => p.Addresses.Select(t => t.Country)).First();
        }
    }
}