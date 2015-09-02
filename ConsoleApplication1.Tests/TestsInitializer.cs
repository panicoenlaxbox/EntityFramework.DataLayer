using System.Data.Entity;
using System.Linq;
using NUnit.Framework;

namespace ConsoleApplication1.Tests
{
    internal abstract class TestsInitializer
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

        protected static Customer GetFirstCustomerFullLoaded()
        {
            Customer customer;
            using (var context = new ManagementContext())
            {
                customer = GetFirstCustomerFullLoaded(context);
            }
            return customer;
        }

        protected static Customer GetFirstCustomerFullLoaded(ManagementContext context)
        {
            return context.Customers
                .Include(p => p.Addresses)
                .Include(p => p.Addresses.Select(t => t.Country)).First();
        }
    }
}