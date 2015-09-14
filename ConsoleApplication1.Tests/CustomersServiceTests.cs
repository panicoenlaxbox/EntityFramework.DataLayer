using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using NUnit.Framework;
using AutoMapper;
using Moq;

namespace ConsoleApplication1.Tests
{
    [TestFixture]
    class CustomersServiceTests : ManagementContextTestsBase
    {
        [Test]
        public void GetAll_DTO_Success()
        {
            //Arrange
            using (var context = new ManagementContext())
            {
                var unitOfWork = new DatabaseUnitOfWork(context);
                var customerRepository = new CustomerRepository(unitOfWork);
                var sut = new CustomersService(customerRepository);

                Mapper.CreateMap<Customer, CustomerDTO>().ForMember(dst => dst.CustomerId, opt => opt.MapFrom(src => src.Id));

                Assert.Greater(sut.GetAll_DTO("C").Count(), 0);
            }
        }

        [Test]
        public void GetAll_DTO_Success_With_No_Results()
        {
            var data = new List<Customer>()
            {
                new Customer() { Name = "Cliente 1"},
                new Customer() { Name = "Cliente 2"},
                new Customer() { Name = "Cliente 3"}
            };

            var customerRepository = new Mock<ICustomerRepository>();
            customerRepository.
                Setup(p => p.GetAll(It.IsAny<IEnumerable<Expression<Func<Customer, bool>>>>(), null)).
                Returns(data.AsQueryable());

            var sut = new CustomersService(customerRepository.Object);
            Mapper.CreateMap<Customer, CustomerDTO>().ForMember(dst => dst.CustomerId, opt => opt.MapFrom(src => src.Id));

            Assert.AreEqual(0, sut.GetAll_DTO("Z").Count());
        }
    }
}