using Microsoft.EntityFrameworkCore;
using QuizPlatform.Core.Interfaces;
using QuizPlatform.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace QuizPlatform.Infrastructure.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly QuizDbContext _context;
        protected readonly DbSet<T> _dbset;

        public GenericRepository(QuizDbContext context)
        {
            _context = context;
            _dbset = context.Set<T>();
        }
      
        public  IEnumerable<T> GetAll()
        {
            return _dbset.AsEnumerable<T>();
        }
        public  T GetById(Expression<Func<T, bool>> predicate)
        {
            var query = _dbset.AsQueryable();
            query = query.Where(predicate);
            return query.SingleOrDefault();
        }
        public async Task<T> GetByIdAsync(int id) => await _dbset.FindAsync(id);

        public async Task<T?> FindOneAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbset.FirstOrDefaultAsync(predicate);
        }

        public T Add(T entity)
        {
            var item = _dbset.Add(entity);
            return item.Entity;

        }
        public T Delete(T entity)
        {
            var item = _dbset.Remove(entity);
            return item.Entity;
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }
        public IEnumerable<T> FindBy(System.Linq.Expressions.Expression<Func<T, bool>> predicate)
        {
            IEnumerable<T> query = _dbset.Where(predicate).AsEnumerable();
            return query;
        }
        public void DeleteRange(IEnumerable<T> entities)
        {
            _dbset.RemoveRange(entities);
        }

        public async Task AddRangeAsync(IEnumerable<T> entities)
        {
            await _dbset.AddRangeAsync(entities);
        }

        public IQueryable<T> FindAll()
        {
            return _dbset.AsQueryable<T>();
        }

        public async Task<bool> AnyAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbset.AnyAsync(predicate);
        }

    }
}
