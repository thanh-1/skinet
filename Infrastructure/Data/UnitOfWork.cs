using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Entities;
using Core.Interfaces;

namespace Infrastructure.Data
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly StoreContext context;

        // All Repositories will be stored in this one
        private Hashtable repositories;
        public UnitOfWork(StoreContext context)
        {
            this.context = context;
        }

        public async Task<int> Complete()
        {
            return await this.context.SaveChangesAsync();
        }

        public IGenericRepository<TEntity> Repository<TEntity>() where TEntity : BaseEntity
        {
            if (this.repositories == null) this.repositories = new Hashtable();

            // Get name of the entity
            var type = typeof(TEntity).Name;

            // Check if Hash Table has the one above
            if (!repositories.ContainsKey(type))
            {
                // Specify Repository Type
                var repositoryType = typeof(GenericRepository<>);

                // Create Repository Instance
                var repositoryInstance = Activator.CreateInstance(repositoryType.MakeGenericType(typeof(TEntity)), this.context);

                this.repositories.Add(type, repositoryInstance);
            }

            return (IGenericRepository<TEntity>) this.repositories[type];
        }

        public void Dispose()
        {
            this.context.Dispose();
        }
    }
}