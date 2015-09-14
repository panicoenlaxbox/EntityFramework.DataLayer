using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace ConsoleApplication1
{
    public interface IGenericRepository<T> where T : Entity
    {
        T Get(int id);

        IQueryable<T> GetAll();

        IQueryable<T> GetAll(IEnumerable<Expression<Func<T, bool>>> predicates,
            IEnumerable<Expression<Func<T, object>>> includes);

        IQueryable<T> GetAll<TKey>(IEnumerable<Expression<Func<T, bool>>> predicates,
            IEnumerable<Expression<Func<T, object>>> includes, Expression<Func<T, TKey>> sortExpression);

        PaginatedResult<T> GetPaginated<TKey>(int pageIndex, int pageSize,
            IEnumerable<Expression<Func<T, bool>>> predicates, IEnumerable<Expression<Func<T, object>>> includes,
            Expression<Func<T, TKey>> sortExpression);

        PaginatedResult<T> GetPaginated(int pageIndex, int pageSize,
            IEnumerable<Expression<Func<T, bool>>> predicates, IEnumerable<Expression<Func<T, object>>> includes,
            string sortExpression);

        void Remove(T entity);
        void Save(T entity);
        void SaveGraph(T entity);
    }

    public class PaginatedResult<T>
    {
        public PaginatedResult(int pageIndex, int pageSize, IEnumerable<T> result, int totalCount)
        {
            Result = result;
            PageIndex = pageIndex;
            PageSize = pageSize;
            TotalCount = totalCount;
        }

        public IEnumerable<T> Result { get; private set; }
        public int PageIndex { get; private set; }
        public int PageSize { get; private set; }
        public int TotalCount { get; private set; }

        public int TotalPageCount
        {
            get
            {
                return (int)Math.Ceiling(TotalCount / (double)PageSize);
            }
        }

        public bool HasPreviousPage
        {
            get
            {
                return (PageIndex > 1);
            }
        }

        public bool HasNextPage
        {
            get
            {
                return (PageIndex < TotalPageCount);
            }
        }
    }

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

        public IQueryable<T> GetAll(IEnumerable<Expression<Func<T, bool>>> predicates, IEnumerable<Expression<Func<T, object>>> includes)
        {
            return GetAll<int>(predicates, includes, null);
        }

        public IQueryable<T> GetAll<TKey>(IEnumerable<Expression<Func<T, bool>>> predicates, IEnumerable<Expression<Func<T, object>>> includes, Expression<Func<T, TKey>> sortExpression)
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

        public T Get(int id)
        {
            return _set.Find(id);
        }

        public PaginatedResult<T> GetPaginated<TKey>(int pageIndex, int pageSize,
            IEnumerable<Expression<Func<T, bool>>> predicates, IEnumerable<Expression<Func<T, object>>> includes, Expression<Func<T, TKey>> sortExpression)
        {
            IQueryable<T> query = _set;
            ApplyPredicates(ref query, predicates);
            ApplyIncludes(ref query, includes);
            var totalCount = query.Count();
            var result = query.OrderBy(sortExpression).Skip((pageIndex < 1 ? 0 : pageIndex - 1) * pageSize).Take(pageSize).ToList();
            return new PaginatedResult<T>(pageIndex, pageSize, result, totalCount);
        }


        public PaginatedResult<T> GetPaginated(int pageIndex, int pageSize,
            IEnumerable<Expression<Func<T, bool>>> predicates, IEnumerable<Expression<Func<T, object>>> includes, string sortExpression)
        {
            IQueryable<T> query = _set;
            ApplyPredicates(ref query, predicates);
            ApplyIncludes(ref query, includes);
            var totalCount = query.Count();
            var result = query.OrderBy(sortExpression).Skip((pageIndex < 1 ? 0 : pageIndex - 1) * pageSize).Take(pageSize).ToList();
            return new PaginatedResult<T>(pageIndex, pageSize, result, totalCount);
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

    /// <summary>
    /// Dynamic SQL-like Linq OrderBy Extension
    /// </summary>
    /// <remarks>http://aonnull.blogspot.com.es/2010/08/dynamic-sql-like-linq-orderby-extension.html</remarks>
    public static class OrderByExtension
    {
        private static IQueryable<T> ApplyOrderBy<T>(IQueryable<T> collection, OrderByInfo orderByInfo)
        {
            string[] props = orderByInfo.PropertyName.Split('.');
            Type type = typeof(T);

            ParameterExpression arg = Expression.Parameter(type, "x");
            Expression expr = arg;
            foreach (string prop in props)
            {
                // use reflection (not ComponentModel) to mirror LINQ
                PropertyInfo pi = type.GetProperty(prop);
                expr = Expression.Property(expr, pi);
                type = pi.PropertyType;
            }
            Type delegateType = typeof(Func<,>).MakeGenericType(typeof(T), type);
            LambdaExpression lambda = Expression.Lambda(delegateType, expr, arg);
            string methodName = String.Empty;

            if (!orderByInfo.Initial && collection is IOrderedQueryable<T>)
            {
                if (orderByInfo.Direction == SortDirection.Ascending)
                    methodName = "ThenBy";
                else
                    methodName = "ThenByDescending";
            }
            else
            {
                if (orderByInfo.Direction == SortDirection.Ascending)
                    methodName = "OrderBy";
                else
                    methodName = "OrderByDescending";
            }

            //TODO: apply caching to the generic methodsinfos?
            return (IOrderedQueryable<T>)typeof(Queryable).GetMethods().Single(
                method => method.Name == methodName
                          && method.IsGenericMethodDefinition
                          && method.GetGenericArguments().Length == 2
                          && method.GetParameters().Length == 2)
                .MakeGenericMethod(typeof(T), type)
                .Invoke(null, new object[] { collection, lambda });
        }

        public static IEnumerable<T> OrderBy<T>(this IEnumerable<T> enumerable, string orderBy)
        {
            return enumerable.AsQueryable().OrderBy(orderBy).AsEnumerable();
        }

        public static IQueryable<T> OrderBy<T>(this IQueryable<T> collection, string orderBy)
        {
            foreach (OrderByInfo orderByInfo in ParseOrderBy(orderBy))
                collection = ApplyOrderBy<T>(collection, orderByInfo);

            return collection;
        }

        private static IEnumerable<OrderByInfo> ParseOrderBy(string orderBy)
        {
            if (String.IsNullOrEmpty(orderBy))
                yield break;

            string[] items = orderBy.Split(',');
            bool initial = true;
            foreach (string item in items)
            {
                string[] pair = item.Trim().Split(' ');

                if (pair.Length > 2)
                    throw new ArgumentException(
                        String.Format(
                            "Invalid OrderBy string '{0}'. Order By Format: Property, Property2 ASC, Property2 DESC",
                            item));

                string prop = pair[0].Trim();

                if (String.IsNullOrEmpty(prop))
                    throw new ArgumentException(
                        "Invalid Property. Order By Format: Property, Property2 ASC, Property2 DESC");

                SortDirection dir = SortDirection.Ascending;

                if (pair.Length == 2)
                    dir = ("desc".Equals(pair[1].Trim(), StringComparison.OrdinalIgnoreCase)
                        ? SortDirection.Descending
                        : SortDirection.Ascending);

                yield return new OrderByInfo() { PropertyName = prop, Direction = dir, Initial = initial };

                initial = false;
            }
        }

        private class OrderByInfo
        {
            public string PropertyName { get; set; }
            public SortDirection Direction { get; set; }
            public bool Initial { get; set; }
        }

        private enum SortDirection
        {
            Ascending = 0,
            Descending = 1
        }
    }

    public interface ICustomerRepository
    {
        Customer Get(int customerId);
        IQueryable<Customer> GetAll();

        IQueryable<Customer> GetAll(IEnumerable<Expression<Func<Customer, bool>>> predicates,
            IEnumerable<Expression<Func<Customer, object>>> includes);

        IQueryable<Customer> GetAll<TKey>(IEnumerable<Expression<Func<Customer, bool>>> predicates,
            IEnumerable<Expression<Func<Customer, object>>> includes, Expression<Func<Customer, TKey>> sortExpression);

        PaginatedResult<Customer> GetPaginated<TKey>(int pageIndex, int pageSize,
            IEnumerable<Expression<Func<Customer, bool>>> predicates, IEnumerable<Expression<Func<Customer, object>>> includes,
            Expression<Func<Customer, TKey>> sortExpression);

        PaginatedResult<Customer> GetPaginated(int pageIndex, int pageSize,
            IEnumerable<Expression<Func<Customer, bool>>> predicates, IEnumerable<Expression<Func<Customer, object>>> includes,
            string sortExpression);

        void Remove(Customer customer);
        void Save(Customer customer);
        void SaveGraph(Customer customer);
    }

    public static class RepositoryUtilities
    {
        public static IEnumerable<Expression<Func<T, bool>>> Predicates<T>(params Expression<Func<T, bool>>[] predicates)
        {
            return new List<Expression<Func<T, bool>>>(predicates);
        }

        public static IEnumerable<Expression<Func<T, object>>> Includes<T>(params Expression<Func<T, object>>[] includes)
        {
            return new List<Expression<Func<T, object>>>(includes);
        }
    }

    public class CustomerRepository : ICustomerRepository
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

        public PaginatedResult<Customer> GetPaginated<TKey>(int pageIndex, int pageSize, IEnumerable<Expression<Func<Customer, bool>>> predicates, IEnumerable<Expression<Func<Customer, object>>> includes,
            Expression<Func<Customer, TKey>> sortExpression)
        {
            return _genericRepository.GetPaginated(pageIndex, pageSize, predicates, includes, sortExpression);
        }

        public PaginatedResult<Customer> GetPaginated(int pageIndex, int pageSize, IEnumerable<Expression<Func<Customer, bool>>> predicates, IEnumerable<Expression<Func<Customer, object>>> includes,
            string sortExpression)
        {
            return _genericRepository.GetPaginated(pageIndex, pageSize, predicates, includes, sortExpression);
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