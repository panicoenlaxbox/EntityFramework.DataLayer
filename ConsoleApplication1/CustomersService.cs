using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
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
            var predicates = new List<Expression<Func<Customer, bool>>>();
            if (!string.IsNullOrWhiteSpace(name))
            {
                //predicates.Add(p => p.Name.Contains(name.Trim()));
            }
            return _customerRepository.GetAll(predicates, null).ProjectTo<CustomerDTO>();
        }
    }

    public class CustomerDTO
    {
        public int CustomerId { get; set; }
        public string Name { get; set; }
    }
}