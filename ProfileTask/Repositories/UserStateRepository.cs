using Microsoft.EntityFrameworkCore;
using VKProfileTask.Entities;
using VKProfileTask.Interfaces;

namespace VKProfileTask.Repositories;

public class UserStateRepository : IRepository<UserStateEntity, long>
{
    private readonly DataContext _dataContext;

    public UserStateRepository(DataContext dataContext) => _dataContext = dataContext;

    public Task<List<UserStateEntity>> GetAllAsync() =>  _dataContext.UserStates.ToListAsync();

    public ValueTask<UserStateEntity?> GetByKeyAsync(long id) =>  _dataContext.UserStates.FindAsync(id);

    public async Task<bool> AddAsync(UserStateEntity item)
    {
        if (await GetByKeyAsync(item.Id) != null)
        {
            return false;
        }
        
        await _dataContext.UserStates.AddAsync(item);
        await SaveAsync();
        return true;
    }

    public async Task<bool> UpdateAsync(UserStateEntity item)
    {
        var userState = await GetByKeyAsync(item.Id);
        if (userState == null)
        {
            return false;
        }
        
        userState.Copy(item);
        await SaveAsync();
        return true;
    }

    public Task SaveAsync() => _dataContext.SaveChangesAsync();
}