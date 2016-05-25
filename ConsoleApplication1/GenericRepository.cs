using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;

namespace ConsoleApplication1
{
    public class GenericRepository<T> : IGenericRepository<T> where T : Entity
    {
        private readonly DbContext _context;
        private readonly IDbSet<T> _set;

        public GenericRepository(IUnitOfWork unitOfWork)
        {
            _context = unitOfWork.Context;
            _set = _context.Set<T>();
        }

        public void SaveGraph(T entity)
        {
            _set.Add(entity);
            _context.ApplyStateChanges();
        }

        public void Save(T entity)
        {
            if (entity.Id == default(int))
            {
                _set.Add(entity);
            }
            else
            {
                _context.Entry(entity).State = EntityState.Modified;
            }
        }

        public void Remove(T entity)
        {
            _context.Entry(entity).State = EntityState.Deleted;
        }

        public IQueryable<T> GetAll()
        {
            return _set;
        }

        public IQueryable<T> GetAll(IEnumerable<Expression<Func<T, bool>>> predicates,
            IEnumerable<Expression<Func<T, object>>> includes)
        {
            return GetAll<int>(predicates, includes, null);
        }

        public IQueryable<T> GetAll<TKey>(IEnumerable<Expression<Func<T, bool>>> predicates,
            IEnumerable<Expression<Func<T, object>>> includes, Expression<Func<T, TKey>> sortExpression)
        {
            IQueryable<T> query = _set;
            ApplyPredicates(ref query, predicates);
            ApplyIncludes(ref query, includes);
            if (sortExpression != null)
            {
                query = query.OrderBy(sortExpression);
            }
            return query;
        }

        public IQueryable<TResult> GetAll<TResult, TKey>(Expression<Func<T, TResult>> @select,
            IEnumerable<Expression<Func<T, bool>>> predicates, IEnumerable<Expression<Func<T, object>>> includes,
            Expression<Func<T, TKey>> sortExpression)
        {
            IQueryable<T> query = _set;
            ApplyPredicates(ref query, predicates);
            ApplyIncludes(ref query, includes);
            if (sortExpression != null)
            {
                query = query.OrderBy(sortExpression);
            }
            return query.Select(select);
        }

        public T Get(int id)
        {
            return _set.Find(id);
        }

        public PaginatedResult<T> GetPaginated<TKey>(IEnumerable<Expression<Func<T, bool>>> predicates,
            IEnumerable<Expression<Func<T, object>>> includes, Expression<Func<T, TKey>> sortExpression,
            PaginatedConfiguration pagination)
        {
            IQueryable<T> query = _set;
            ApplyPredicates(ref query, predicates);
            ApplyIncludes(ref query, includes);
            var totalCount = query.Count();
            var result =
                query.OrderBy(sortExpression)
                    .Skip((pagination.PageIndex < 1 ? 0 : pagination.PageIndex - 1)*pagination.PageSize)
                    .Take(pagination.PageSize)
                    .ToList();
            return new PaginatedResult<T>(pagination.PageIndex, pagination.PageSize, result, totalCount);
        }


        public PaginatedResult<T> GetPaginated(IEnumerable<Expression<Func<T, bool>>> predicates,
            IEnumerable<Expression<Func<T, object>>> includes, string sortExpression, PaginatedConfiguration pagination)
        {
            IQueryable<T> query = _set;
            ApplyPredicates(ref query, predicates);
            ApplyIncludes(ref query, includes);
            var totalCount = query.Count();
            var result =
                query.OrderBy(sortExpression)
                    .Skip((pagination.PageIndex < 1 ? 0 : pagination.PageIndex - 1)*pagination.PageSize)
                    .Take(pagination.PageSize)
                    .ToList();
            return new PaginatedResult<T>(pagination.PageIndex, pagination.PageSize, result, totalCount);
        }

        private void ApplyIncludes(ref IQueryable<T> query, IEnumerable<Expression<Func<T, object>>> includes)
        {
            if (includes == null)
                return;
            foreach (var include in includes)
            {
                query = query.Include(include);
            }
        }

        private void ApplyPredicates(ref IQueryable<T> query, IEnumerable<Expression<Func<T, bool>>> predicates)
        {
            if (predicates == null)
                return;
            foreach (var predicate in predicates)
            {
                query = query.Where(predicate);
            }
        }
    }
}