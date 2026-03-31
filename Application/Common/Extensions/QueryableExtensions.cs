using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Biobrain.Application.Common.Exceptions;
using Biobrain.Application.Specifications;
using Microsoft.EntityFrameworkCore;

namespace Biobrain.Application.Common.Extensions
{
    public static class QueryableExtensions
    {
        public static async Task<T> GetSingleAsync<T>(this IQueryable<T> queryable, CancellationToken cancellationToken = new())
        {
            var result = await queryable.SingleOrDefaultAsync(cancellationToken);
            if (result == null)
                throw new ObjectWasNotFoundException(typeof(T).Name, null);

            return result;
        }
        
        public static async Task<T> GetSingleAsync<T>(this IQueryable<T> queryable, Spec<T> spec, CancellationToken cancellationToken = new())
        {
            var result = await queryable.Where(spec).SingleOrDefaultAsync(cancellationToken);
            if (result == null)
                throw new ObjectWasNotFoundException(typeof(T).Name, null);

            return result;
        }

        public static T GetSingle<T>(this IQueryable<T> queryable)
        {
            var result = queryable.SingleOrDefault();
            if (result == null)
                throw new ObjectWasNotFoundException(typeof(T).Name, null);

            return result;
        }

        public static T GetSingle<T>(this IQueryable<T> queryable, Spec<T> spec)
        {
            var result = queryable.Where(spec).SingleOrDefault();
            if (result == null)
                throw new ObjectWasNotFoundException(typeof(T).Name, null);

            return result;
        }

        public static async Task<ImmutableList<T>> ToImmutableListAsync<T>(this IQueryable<T> query, CancellationToken cancellationToken)
        {
            var result = await query.ToListAsync(cancellationToken);
            return result.ToImmutableList();
        }
    }
}