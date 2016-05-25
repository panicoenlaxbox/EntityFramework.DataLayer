using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace ConsoleApplication1
{
    public class CustomerRepository : Repository<Customer>, ICustomerRepository
    {
        private readonly GenericRepository<Customer> _genericRepository;

        public CustomerRepository(IUnitOfWork unitOfWork)
        {
            _genericRepository = new GenericRepository<Customer>(unitOfWork);
        }

        public Customer Get(int customerId)
        {
            return _genericRepository.Get(customerId);
        }

        public IQueryable<Customer> GetAll()
        {
            return _genericRepository.GetAll();
        }

        public IQueryable<Customer> GetAll(IEnumerable<Expression<Func<Customer, bool>>> predicates, IEnumerable<Expression<Func<Customer, object>>> includes)
        {
            return _genericRepository.GetAll(predicates, includes);
        }

        public IQueryable<Customer> GetAll<TKey>(IEnumerable<Expression<Func<Customer, bool>>> predicates, IEnumerable<Expression<Func<Customer, object>>> includes, Expression<Func<Customer, TKey>> sortExpression)
        {
            return _genericRepository.GetAll(predicates, includes, sortExpression);
        }

        public PaginatedResult<Customer> GetPaginated<TKey>(IEnumerable<Expression<Func<Customer, bool>>> predicates, IEnumerable<Expression<Func<Customer, object>>> includes,
            Expression<Func<Customer, TKey>> sortExpression, int pageIndex, int pageSize)
        {
            return _genericRepository.GetPaginated(predicates, includes, sortExpression, new PaginatedConfiguration(pageIndex, pageSize));
        }

        public void Remove(Customer customer)
        {
            _genericRepository.Remove(customer);
        }

        public void Save(Customer customer)
        {
            _genericRepository.Save(customer);
        }

        public void SaveGraph(Customer customer)
        {
            _genericRepository.SaveGraph(customer);
        }
    }
}