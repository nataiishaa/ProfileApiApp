using VKProfileTask.Entities;
using VKProfileTask.Models;
using VKProfileTask.Repositories;

namespace VKProfileTask.UnitOfWorks;

public sealed class PostgresUnitOfWork : IDisposable
{
    private static bool _firstStart = true;
    private readonly DataContext _dataContext;
    private bool _disposed;

    private UserRepository Users { get; }
    public UserGroupRepository Groups { get; }
    public UserStateRepository States { get; }

    public PostgresUnitOfWork(DataContext dataContext)
    {
        if (_firstStart)
        {
            if (dataContext.Users.Any())
            {
                UserEntity.Count = dataContext.Users.Max(user => user.Id) + 1;
            }
            if (dataContext.UserGroups.Any())
            {
                UserGroupEntity.Count = dataContext.UserGroups.Max(userGroup => userGroup.Id) + 1;
            }
            if (dataContext.UserStates.Any())
            {
                UserStateEntity.Count = dataContext.UserStates.Max(userState => userState.Id) + 1;
            }

            _firstStart = false;
        }

        (_dataContext, Users, Groups, States) =
            (dataContext, new UserRepository(dataContext), new UserGroupRepository(dataContext),
                new UserStateRepository(dataContext));
    }

    ~PostgresUnitOfWork()
    {
        Dispose();
    }

    public async Task<bool> AddUserAsync(UserEntity item, GroupCode groupCode)
    {
        if (!await CanAddAsync(item, groupCode))
        {
            return false;
        }
        
        return await Users.AddAsync(item);
    }
    
    private async Task<bool> CanAddAsync(UserEntity item, GroupCode groupCode) =>
        (groupCode != GroupCode.Admin || !await HasAdminAsync()) && await GetUserByLoginAsync(item.Login) == null;
    
    public async Task<IEnumerable<UserWithFullInfo>> GetFullInfoOfAllUsersAsync(int pageNumber, int pageCapacity)
    {
        var firstUserInPage = await Users.GetFirstUserInPageAsync(pageNumber, pageCapacity);
        Console.WriteLine(firstUserInPage?.Login);
        
        if (firstUserInPage == null)
        {
            return new List<UserWithFullInfo>();
        }
        
        return from user in await Users.GetPageAsync(firstUserInPage.Login, pageCapacity)
            join userGroup in _dataContext.UserGroups on user.UserGroupId equals userGroup.Id
            join userState in _dataContext.UserStates on user.UserStateId equals userState.Id
            select new UserWithFullInfo(user, userGroup, userState);
    }
    
    public async Task<IEnumerable<UserWithFullInfo>> GetFullInfoOfAllUsersAsync() => from user in await Users.GetAllAsync()
            join userGroup in await Groups.GetAllAsync() on user.UserGroupId equals userGroup.Id
            join userState in await States.GetAllAsync() on user.UserStateId equals userState.Id
            select new UserWithFullInfo(user, userGroup, userState);
    
    private async Task<bool> HasAdminAsync()
    { 
        foreach (var user in await Users.GetAllAsync())
        {
            if ((await Groups.GetByKeyAsync(user.UserGroupId))?.Code == GroupCode.Admin 
                && (await States.GetByKeyAsync(user.UserStateId))?.Code != StateCode.Blocked)
            {
                return true;
            }
        }

        return false;
    }

    public ValueTask<UserEntity?> GetUserByLoginAsync(string login) => Users.GetByKeyAsync(login);
    
    public async Task<bool> DeleteUserByLoginAsync(string login)
    {
        var user = await Users.GetByKeyAsync(login);
        if (user == null)
        {
            return false;
        }

        await States.UpdateAsync(new UserStateEntity(user.UserStateId, StateCode.Blocked, string.Empty));
        return true;
    }

    private async Task DisposeAsync(bool disposing)
    {
        if(!_disposed)
        {
            if(disposing)
            {
                await _dataContext.DisposeAsync();
            }
        }
        _disposed = true;
    }
 
    public async void Dispose()
    {
        await DisposeAsync(true);
        GC.SuppressFinalize(this);
    }
}