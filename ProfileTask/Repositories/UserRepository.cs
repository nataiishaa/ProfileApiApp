using Microsoft.EntityFrameworkCore;
using VKProfileTask.Entities;
using VKProfileTask.Interfaces;

namespace VKProfileTask.Repositories;

public class UserRepository : IRepository<UserEntity, string>
{
    private readonly DataContext _dataContext;
    
    public UserRepository(DataContext dataContext) => _dataContext = dataContext;

    public Task<List<UserEntity>> GetAllAsync() => _dataContext.Users.ToListAsync();

    public Task<List<UserEntity>> GetPageAsync(string firstUserLogin, int pageCapacity) 
        => _dataContext.Users.OrderBy(user => user.Login)
            // ReSharper disable once StringCompareToIsCultureSpecific
            .Where(user => user.Login.CompareTo(firstUserLogin) >= 0)
        .Take(pageCapacity)
        .ToListAsync();

    public Task<UserEntity?> GetFirstUserInPageAsync(int pageNumber, int pageCapacity)
        => _dataContext.Users.OrderBy(user => user.Login)
            .Take((pageNumber - 1) * pageCapacity + 1)
            .LastOrDefaultAsync();
    
    public async Task<bool> AddAsync(UserEntity item)
    {
        if (await GetByKeyAsync(item.Login) != null || await _dataContext.Users.AnyAsync(user => user.Login == item.Login))
        {
            return false;
        }

        await _dataContext.Users.AddAsync(item);
        await SaveAsync();
        return true;
    }

    public async Task<bool> UpdateAsync(UserEntity item)
    {
        var user = await GetByKeyAsync(item.Login);
        if (user == null)
        {
            return false;
        }
        
        user.Copy(item);
        await SaveAsync();
        return true;
    }

    public ValueTask<UserEntity?> GetByKeyAsync(string login) =>
        _dataContext.Users.FindAsync(login);
    
    public Task SaveAsync() => _dataContext.SaveChangesAsync();
    
}