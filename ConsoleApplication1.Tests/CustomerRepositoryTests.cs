using System.Linq;
using NUnit.Framework;

namespace ConsoleApplication1.Tests
{
    [TestFixture]
    class CustomerRepositoryTests : ManagementContextTestsBase
    {
        [Test]
        public void Remove_Customer_Succeed()
        {
            using (var context = new ManagementContext())
            {
                //Arrange
                var total = context.Customers.Count();
                var customer = context.Customers.First();
                var unitOfWork = new UnitOfWork(context);
                var customerRepository = new CustomerRepository(unitOfWork);

                //Act
                customerRepository.Remove(customer);
                unitOfWork.Save();

                //Assert
                Assert.AreEqual(context.Customers.Count(), total - 1);
            }
        }

        [Test]
        public void Remove_CustomerById_Succeed()
        {
            using (var context = new ManagementContext())
            {
                //Arrange
                var total = context.Customers.Count();
                var customer = context.Customers.First();
                var unitOfWork = new UnitOfWork(context);
                var customerRepository = new CustomerRepository(unitOfWork);

                //Act
                customerRepository.Remove(customer.CustomerId);
                unitOfWork.Save();

                //Assert
                Assert.AreEqual(context.Customers.Count(), total - 1);
            }
        }

        [Test]
        public void Save_NewCustomer_Succeed()
        {
            using (var context = new ManagementContext())
            {
                //Arrange                
                var customer = new Customer()
                {
                    Name = "Nuevo cliente"
                };
                var unitOfWork = new UnitOfWork(context);
                var customerRepository = new CustomerRepository(unitOfWork);

                //Act
                customerRepository.Save(customer);
                unitOfWork.Save();

                //Assert
                Assert.Pass();
            }
        }

        [Test]
        public void Save_Customer_Succeed()
        {
            using (var context = new ManagementContext())
            {
                //Arrange
                var customer = context.Customers.First();
                customer.Name = "Cliente modificado";

                var unitOfWork = new UnitOfWork(context);
                var customerRepository = new CustomerRepository(unitOfWork);

                //Act
                customerRepository.Save(customer);
                unitOfWork.Save();

                //Assert
                Assert.Pass();
            }
        }
    }
}