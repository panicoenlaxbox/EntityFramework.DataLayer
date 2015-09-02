using System.Data.Entity;

namespace ConsoleApplication1
{
    public class ManagementContext : DbContext
    {
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<Country> Countries { get; set; }
    }
}