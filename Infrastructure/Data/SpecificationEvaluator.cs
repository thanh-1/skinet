using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Entities;
using Core.Specifications;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data
{
    public class SpecificationEvaluator<TEntity> where TEntity : BaseEntity
    {
        public static IQueryable<TEntity> GetQuery(IQueryable<TEntity> inputQuery, ISpecification<TEntity> spec)
        {
            var query = inputQuery;

            // The Order in here is Important!

            if (spec.Criteria != null)
            {
                // Criteria can be like : p => p.ProductTypeId == id
                query = query.Where(spec.Criteria);
            }

            if (spec.OrderBy != null)
            {
                query = query.OrderBy(spec.OrderBy);
            }
            
            if (spec.OrderByDescending != null)
            {
                query = query.OrderByDescending(spec.OrderByDescending);
            }

            // Paging should be the last one after all those filter and sort and select
            if (spec.IsPagingEnabled)
            {
                query = query.Skip(spec.Skip).Take(spec.Take);
            }
            query = spec.Includes.Aggregate(query, (current, include) => current.Include(include));

            return query;
        }
    }
}