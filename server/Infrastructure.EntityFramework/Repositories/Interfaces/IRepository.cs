namespace Infrastructure.EntityFramework.Repositories;

public interface IRepository<T>
{
    public Task<T?> GetAsync(int id);
    public Task<IEnumerable<T>> GetAllAsync();
    public Task<T> AddAsync(T entity);
    public Task<bool> DeleteAsync(int id);
    public Task<T?> UpdateAsync(T entity);
}