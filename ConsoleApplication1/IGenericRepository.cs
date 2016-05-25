using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace ConsoleApplication1
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;


    public interface IGenericRepository<T> where T : Entity
    {
        T Get(int id);

        IQueryable<T> GetAll();

        IQueryable<T> GetAll(
            IEnumerable<Expression<Func<T, bool>>> predicates,
            IEnumerable<Expression<Func<T, object>>> includes);

        IQueryable<T> GetAll<TKey>(
            IEnumerable<Expression<Func<T, bool>>> predicates,
            IEnumerable<Expression<Func<T, object>>> includes,
            Expression<Func<T, TKey>> sortExpression);

        IQueryable<TResult> GetAll<TResult, TKey>(
            Expression<Func<T, TResult>> select,
            IEnumerable<Expression<Func<T, bool>>> predicates,
            IEnumerable<Expression<Func<T, object>>> includes,
            Expression<Func<T, TKey>> sortExpression);

        PaginatedResult<T> GetPaginated<TKey>(
            IEnumerable<Expression<Func<T, bool>>> predicates,
            IEnumerable<Expression<Func<T, object>>> includes,
            Expression<Func<T, TKey>> sortExpression,
            PaginatedConfiguration pagination);

        PaginatedResult<T> GetPaginated(
            IEnumerable<Expression<Func<T, bool>>> predicates,
            IEnumerable<Expression<Func<T, object>>> includes,
            string sortExpression,
            PaginatedConfiguration pagination);

        void Remove(T entity);
        void Save(T entity);
        void SaveGraph(T entity);
    }
}