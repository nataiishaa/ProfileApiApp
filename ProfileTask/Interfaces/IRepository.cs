using VKProfileTask.Entities;

namespace VKProfileTask.Interfaces;

public interface IRepository<T, in TKey> where T : IEntity
{
    Task<List<T>> GetAllAsync();
    ValueTask<T?> GetByKeyAsync(TKey id); 
    Task<bool> AddAsync(T item); 
    Task<bool> UpdateAsync(T item);
    Task SaveAsync();  
}