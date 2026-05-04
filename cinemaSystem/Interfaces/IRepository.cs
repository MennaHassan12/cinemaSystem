namespace cinemaSystem.Interfaces
{
    public interface IRepository<T> where T : class
    {
        Task CreateAsync(T entity, CancellationToken ct = default);
        void Update(T entity);
        void Delete(T entity);

        Task<int> CommitAsync(CancellationToken ct = default);

        Task<IEnumerable<T>> GetAsync(CancellationToken ct = default);
        Task<T?> GetOneAsync(int id, CancellationToken ct = default);
    }
}
