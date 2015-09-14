using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using AutoMapper.QueryableExtensions;

namespace ConsoleApplication1
{
    public class StubCustomerRepository : ICustomerRepository
    {
        private readonly IEnumerable<Customer> _data;

        public StubCustomerRepository(IEnumerable<Customer> data )
        {
            _data = data;
        }

        public Customer Get(int customerId)
        {
            throw new NotImplementedException();
        }

        public IQueryable<Customer> GetAll()
        {
            throw new NotImplementedException();
        }

        public IQueryable<Customer> GetAll(
            IEnumerable<Expression<Func<Customer, bool>>> predicates, 
            IEnumerable<Expression<Func<Customer, object>>> includes)
        {

            var data2 = _data.AsQueryable();
            foreach (var predicate in predicates)
            {
                data2 = data2.Where(predicate);
            }
            return data2;
        }

        public IQueryable<Customer> GetAll<TKey>(IEnumerable<Expression<Func<Customer, bool>>> predicates, IEnumerable<Expression<Func<Customer, object>>> includes, Expression<Func<Customer, TKey>> sortExpression)
        {
            throw new NotImplementedException();
        }

        public PaginatedResult<Customer> GetPaginated(int pageIndex, int pageSize, IEnumerable<Expression<Func<Customer, bool>>> predicates, IEnumerable<Expression<Func<Customer, object>>> includes, string sortExpression)
        {
            throw new NotImplementedException();
        }

        public PaginatedResult<Customer> GetPaginated<TKey>(int pageIndex, int pageSize, IEnumerable<Expression<Func<Customer, bool>>> predicates, IEnumerable<Expression<Func<Customer, object>>> includes, Expression<Func<Customer, TKey>> sortExpression)
        {
            throw new NotImplementedException();
        }

        public void Remove(Customer customer)
        {
            throw new NotImplementedException();
        }

        public void Save(Customer customer)
        {
            throw new NotImplementedException();
        }

        public void SaveGraph(Customer customer)
        {
            throw new NotImplementedException();
        }
    }
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
                predicates.Add(p => p.Name.Contains(name.Trim()));
            }
            IQueryable<Customer> customers = _customerRepository.GetAll(predicates, null);
            return customers.ProjectTo<CustomerDTO>();
        }
    }

    public class CustomerDTO
    {
        public int CustomerId { get; set; }
        public string Name { get; set; }
    }
}