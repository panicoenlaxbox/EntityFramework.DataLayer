using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.InteropServices;
using NUnit.Framework;
using NUnit.Framework.Constraints;

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
                var unitOfWork = new DatabaseUnitOfWork(context);
                var customerRepository = new CustomerRepository(unitOfWork);

                //Act
                customerRepository.Remove(customer);
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
                var unitOfWork = new DatabaseUnitOfWork(context);
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

                var unitOfWork = new DatabaseUnitOfWork(context);
                var customerRepository = new CustomerRepository(unitOfWork);

                //Act
                customerRepository.Save(customer);
                unitOfWork.Save();

                //Assert
                Assert.Pass();
            }
        }
    }

    [TestFixture]
    class CustomerRepositoryGetMethodsTests
    {
        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            Database.SetInitializer(new DropCreateDatabaseAlways<ManagementContext>());
        }

        [SetUp]
        public void SetUp()
        {
            using (var context = new ManagementContext())
            {
                context.Database.Initialize(true);
            }
        }

        private void InsertCustomers(int count)
        {
            using (var context = new ManagementContext())
            {
                for (int i = 0; i < count; i++)
                {
                    context.Customers.Add(new Customer()
                    {
                        Name = "Cliente " + i,
                        Code = "C" + i
                    });
                }
                context.SaveChanges();
            }
        }

        [Test]
        public void GetAll_Customer_Succeed()
        {
            //Arrange
            InsertCustomers(20);

            using (var context = new ManagementContext())
            {
                var unitOfWork = new DatabaseUnitOfWork(context);
                var customerRepository = new CustomerRepository(unitOfWork);

                //Act
                var total = customerRepository.GetAll().Count();

                //Assert
                Assert.AreEqual(20, total);
            }
        }

        [Test]
        public void GetAllWithPredicate_Customer_Succeed()
        {
            //Arrange
            InsertCustomers(20);

            using (var context = new ManagementContext())
            {
                var unitOfWork = new DatabaseUnitOfWork(context);
                var customerRepository = new CustomerRepository(unitOfWork);

                //Act
                var total = customerRepository.GetAll(CustomerRepository.Predicates(p => p.Id > 5), null).Count();

                //Assert
                Assert.AreEqual(15, total);
            }
        }

        [Test]
        public void GetAllWithPredicates_Customer_Succeed()
        {
            //Arrange            
            using (var context = new ManagementContext())
            {
                var executor = new SqlExecutor(context.Database.Connection.ConnectionString);
                executor.ExecuteScript(Path.Combine(Environment.CurrentDirectory, "Scripts", "2 - Customers.sql"));

                var unitOfWork = new DatabaseUnitOfWork(context);
                var customerRepository = new CustomerRepository(unitOfWork);

                //Act
                var predicates = CustomerRepository.Predicates(p => p.Id > 2, p => p.Code.StartsWith("A"));
                var total = customerRepository.GetAll<Customer>(predicates, null, null).Count();

                //Assert
                Assert.AreEqual(3, total);
            }
        }

        private IEnumerable<TestCaseData> GetData()
        {
            yield return new TestCaseData(new PaginatedData()
            {
                PageIndex = 1,
                PageSize = 5,
                TotalCount = 12,
                TotalPageCount = 3,
                ResultCount = 5,
                HasPreviousPage = false,
                HasNextPage = true
            }).SetName("First page");
            yield return new TestCaseData(new PaginatedData()
            {
                PageIndex = 2,
                PageSize = 5,
                TotalCount = 12,
                TotalPageCount = 3,
                ResultCount = 5,
                HasPreviousPage = true,
                HasNextPage = true
            }).SetName("Middle page");
            yield return new TestCaseData(new PaginatedData()
            {
                PageIndex = 3,
                PageSize = 5,
                TotalCount = 12,
                TotalPageCount = 3,
                ResultCount = 2,
                HasPreviousPage = true,
                HasNextPage = false
            }).SetName("Last page");
        }

        public class PaginatedData
        {
            public int ResultCount { get; set; }
            public bool HasPreviousPage { get; set; }
            public bool HasNextPage { get; set; }
            public int PageIndex { get; set; }
            public int TotalPageCount { get; set; }
            public int PageSize { get; set; }
            public int TotalCount { get; set; }
        }

        [TestCaseSource("GetData")]
        public void GetPaginated_Customer_Succeed(PaginatedData paginatedData)
        {
            //Arrange            
            using (var context = new ManagementContext())
            {
                var executor = new SqlExecutor(context.Database.Connection.ConnectionString);
                executor.ExecuteScript(Path.Combine(Environment.CurrentDirectory, "Scripts", "2 - Customers.sql"));

                var unitOfWork = new DatabaseUnitOfWork(context);
                var customerRepository = new CustomerRepository(unitOfWork);

                //Act
                PaginatedResult<Customer> result = customerRepository.GetPaginated(null, null, p => p.Id, paginatedData.PageIndex, paginatedData.PageSize);

                //Assert
                Assert.AreEqual(paginatedData.ResultCount, result.Result.Count());
                Assert.AreEqual(paginatedData.PageIndex, result.PageIndex);
                Assert.AreEqual(paginatedData.PageSize, result.PageSize);
                Assert.AreEqual(paginatedData.TotalCount, result.TotalCount);
                Assert.AreEqual(paginatedData.TotalPageCount, result.TotalPageCount);
                Assert.AreEqual(paginatedData.HasPreviousPage, result.HasPreviousPage);
                Assert.AreEqual(paginatedData.HasNextPage, result.HasNextPage);
            }
        }
    }
}