using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace ConsoleApplication1
{
    public abstract class Repository<T>
    {
        public static IEnumerable<Expression<Func<T, bool>>> Predicates(params Expression<Func<T, bool>>[] predicates)
        {
            return new List<Expression<Func<T, bool>>>(predicates);
        }

        public static IEnumerable<Expression<Func<T, object>>> Includes(params Expression<Func<T, object>>[] includes)
        {
            return new List<Expression<Func<T, object>>>(includes);
        }
    }
}