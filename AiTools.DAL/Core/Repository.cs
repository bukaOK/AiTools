﻿using AiTools.DAL.Infrastucture;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AiTools.DAL.Core
{
    public abstract class Repository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        protected readonly AppDbContext DbContext;
        protected readonly DbSet<TEntity> Table;
        protected string TableName;

        public Repository(AppDbContext dbContext)
        {
            DbContext = dbContext;
            Table = dbContext.Set<TEntity>();

            if (TableName == null)
                TableName = typeof(TEntity).Name + "s";
        }

        public virtual async Task<IEnumerable<TEntity>> GetAllAsync()
        {
            return await Table.AsNoTracking().ToListAsync();
        }

        public virtual Task AddAsync(TEntity entity)
        {
            DbContext.Entry(entity).State = EntityState.Added;
            return SaveAsync();
        }

        public virtual Task UpdateAsync(TEntity entity)
        {
            DbContext.Entry(entity).State = EntityState.Modified;
            return SaveAsync();
        }

        public Task<int> GetCountAsync()
        {
            return Table.CountAsync();
        }
        protected Task SaveAsync()
        {
            return DbContext.SaveChangesAsync();
        }

        public virtual Task RemoveAsync(TEntity entity)
        {
            Table.Remove(entity);
            return SaveAsync();
        }
        public async virtual Task<TEntity> GetAsync(params object[] keyValues)
        {
            var entity = await Table.FindAsync(keyValues);
            return entity;
        }

        public Task<IDbContextTransaction> BeginTransactionAsync()
        {
            return DbContext.Database.BeginTransactionAsync();
        }
    }
}
