using System.Linq;
using NUnit.Framework;
using AutoMapper;

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
            //Arrange
            using (var context = new ManagementContext())
            {
                var unitOfWork = new DatabaseUnitOfWork(context);
                var customerRepository = new CustomerRepository(unitOfWork);
                var sut = new CustomersService(customerRepository);

                Mapper.CreateMap<Customer, CustomerDTO>().ForMember(dst => dst.CustomerId, opt => opt.MapFrom(src => src.Id));

                Assert.AreEqual(0, sut.GetAll_DTO("Z").Count());
            }
        }
    }
}