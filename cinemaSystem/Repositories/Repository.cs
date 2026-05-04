using cinemaSystem.Data;
using cinemaSystem.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace cinemaSystem.Repositories
{
    public class Repository<T> : IRepository<T> where T : class
    {
        protected readonly ApplicationDbContext _context;
        private readonly DbSet<T> _dbSet;

        public Repository(ApplicationDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }

        public async Task CreateAsync(T entity, CancellationToken ct = default)
            => await _dbSet.AddAsync(entity, ct);

        public void Update(T entity)
            => _dbSet.Update(entity);

        public void Delete(T entity)
            => _dbSet.Remove(entity);

        public async Task<int> CommitAsync(CancellationToken ct = default)
            => await _context.SaveChangesAsync(ct);

        public async Task<IEnumerable<T>> GetAsync(CancellationToken ct = default)
            => await _dbSet.ToListAsync(ct);

        public async Task<T?> GetOneAsync(int id, CancellationToken ct = default)
            => await _dbSet.FindAsync(new object[] { id }, ct);
    }
}
