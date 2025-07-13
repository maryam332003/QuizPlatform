using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace QuizPlatform.Core.Interfaces
{
    public interface IGenericRepository<T> where T : class
    {
        IEnumerable<T> GetAll();
        IQueryable<T> FindAll();
        IEnumerable<T> FindBy(Expression<Func<T, bool>> predicate);

        T GetById(Expression<Func<T, bool>> predicate);
        void DeleteRange(IEnumerable<T> entities);
        Task<T> GetByIdAsync(int id);
        T Delete(T entity);
        T Add(T entity);
        Task<int> SaveChangesAsync();
        Task AddRangeAsync(IEnumerable<T> entities);
        Task<bool> AnyAsync(Expression<Func<T, bool>> predicate);
    }
}
