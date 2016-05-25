using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace ConsoleApplication1
{
    public interface ICustomerRepository
    {
        Customer Get(int customerId);
        IQueryable<Customer> GetAll();
        IQueryable<Customer> GetAll(IEnumerable<Expression<Func<Customer, bool>>> predicates,
            IEnumerable<Expression<Func<Customer, object>>> includes);
        IQueryable<Customer> GetAll<TKey>(IEnumerable<Expression<Func<Customer, bool>>> predicates,
            IEnumerable<Expression<Func<Customer, object>>> includes, Expression<Func<Customer, TKey>> sortExpression);
        PaginatedResult<Customer> GetPaginated<TKey>(
            IEnumerable<Expression<Func<Customer, bool>>> predicates, IEnumerable<Expression<Func<Customer, object>>> includes,
            Expression<Func<Customer, TKey>> sortExpression, int pageIndex, int pageSize);
        void Remove(Customer customer);
        void Save(Customer customer);
        void SaveGraph(Customer customer);
    }
}