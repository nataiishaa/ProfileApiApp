using Microsoft.EntityFrameworkCore;
using VKProfileTask.Entities;
using VKProfileTask.Interfaces;

namespace VKProfileTask.Repositories;

public class UserGroupRepository : IRepository<UserGroupEntity, long>
{
    private readonly DataContext _dataContext;

    public UserGroupRepository(DataContext dataContext) => _dataContext = dataContext;

    public Task<List<UserGroupEntity>> GetAllAsync() => _dataContext.UserGroups.ToListAsync();

    public ValueTask<UserGroupEntity?> GetByKeyAsync(long id) => _dataContext.UserGroups.FindAsync(id);

    public async Task<bool> AddAsync(UserGroupEntity item)
    {
        if (await GetByKeyAsync(item.Id) != null)
        {
            return false;
        }

        await _dataContext.UserGroups.AddAsync(item);
        await SaveAsync();
        return true;
    }

    public async Task<bool> UpdateAsync(UserGroupEntity item)
    {
        var userGroup = await GetByKeyAsync(item.Id);
        if (userGroup == null)
        {
            return false;
        }
        userGroup.Copy(item);
        await SaveAsync();
        return true;
    }

    public Task SaveAsync() => _dataContext.SaveChangesAsync();
}