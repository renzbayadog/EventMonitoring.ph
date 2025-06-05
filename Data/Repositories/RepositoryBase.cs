using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

using EventMonitoring.ph.ViewModels; 
using EventMonitoring.ph.Data.Entities; 


namespace EventMonitoring.ph.Data.Repositories
{
    public interface IRepositoryBase<T>
    {
        void Add(T newObject);
        void AddRange(IEnumerable<T> newObject);
        IEnumerable<T> FindAll();
        IEnumerable<T> FindByCondition(Expression<Func<T, bool>> expression);
        Task<T> FindFirstAsync(Expression<Func<T, bool>> expression);
        T FindFirst(Expression<Func<T, bool>> expression);
        void Update(T updatedObject);
        Task SaveChangesAsync();
        void Delete(T deleteObject);
        void DeleteRange(IEnumerable<T> deleteObject);
    }
    public abstract class RepositoryBase<T> : IRepositoryBase<T> where T : class
    {
        private db_ab9d6a_dbrenzContext _context;
        
        public RepositoryBase(db_ab9d6a_dbrenzContext context)
        {
            _context = context;
        }

        public IEnumerable<T> FindAll()
        {
            return _context.Set<T>();
        }

        public IEnumerable<T> FindByCondition(Expression<Func<T, bool>> expression)
        {
            return _context.Set<T>().AsNoTracking().Where(expression);
        }

        public async Task<T> FindFirstAsync(Expression<Func<T, bool>> expression)
        {
            return await _context.Set<T>().AsNoTracking().FirstOrDefaultAsync(expression);
        }

        public T FindFirst(Expression<Func<T, bool>> expression)
        {
            return _context.Set<T>().AsNoTracking().First(expression);
        }

        public void Add(T newObject)
        {
            _context.Set<T>().Add(newObject);
        }

        public void AddRange(IEnumerable<T> newObject)
        {
            _context.Set<T>().AddRange(newObject);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public void Update(T updatedObject)
        {
            _context.Attach(updatedObject).State = EntityState.Modified;
        }

        public void Delete(T deleteObject)
        {
            _context.Set<T>().Attach(deleteObject).State = EntityState.Deleted;
        }

        public void DeleteRange(IEnumerable<T> deleteObject)
        {
            _context.Set<T>().RemoveRange(deleteObject);
        }       
    }
}
