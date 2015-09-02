using System.Collections.ObjectModel;
using System.Data.Entity;

namespace ConsoleApplication1
{
    public class ManagementInitializer : DropCreateDatabaseAlways<ManagementContext>
    {
        protected override void Seed(ManagementContext context)
        {
            var country = CreateCountry("España");
            context.Countries.Add(country);

            context.Countries.Add(CreateCountry("Portugal"));

            var customer = new Customer
            {
                Name = "Cliente 1",
                Code = "C1",
                Addresses = new Collection<Address>
                {
                    new Address { Region = "Madrid", Country = country },
                    new Address { Region = "Barcelona", Country = country}
                }
            };
            context.Customers.Add(customer);
        }

        private static Country CreateCountry(string name)
        {
            return new Country() { Name = name };
        }
    }
}