using Microsoft.EntityFrameworkCore;

namespace EmployeeManagement.Data
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly ApplicationDbContext _context;
        private readonly DbSet<T> _dbSet;

        public Repository(ApplicationDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }

        public async Task<IEnumerable<T>> GetAllAsync() => await _dbSet.ToListAsync();
        public async Task<T?> GetByIdAsync(int id) => await _dbSet.FindAsync(id);
        public async Task AddAsync(T entity) => await _dbSet.AddAsync(entity);
        public async Task UpdateAsync(T entity) => Task.Run(() => _dbSet.Update(entity));
        public async Task DeleteAsync(T entity) => Task.Run(() => _dbSet.Remove(entity));
        public async Task SaveChangesAsync() => await _context.SaveChangesAsync();
    }
}
