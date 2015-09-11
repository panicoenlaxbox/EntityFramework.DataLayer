using System.Linq;
using AutoMapper.QueryableExtensions;

namespace ConsoleApplication1
{
    public interface ICustomersService
    {
        IQueryable<CustomerDTO> GetAll_DTO(string name);
    }

    public class CustomersService : ICustomersService
    {
        private readonly ICustomerRepository _customerRepository;

        public CustomersService(ICustomerRepository customerRepository)
        {
            _customerRepository = customerRepository;
        }

        public IQueryable<CustomerDTO> GetAll_DTO(string name)
        {
            return _customerRepository.GetAll(CustomerRepository.Predicates(p => p.Name.Contains(name)), null)
                .ProjectTo<CustomerDTO>();
        }
    }

    public class CustomerDTO
    {
        public int CustomerId { get; set; }
        public string Name { get; set; }
    }
}