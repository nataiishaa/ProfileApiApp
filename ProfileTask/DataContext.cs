using Microsoft.EntityFrameworkCore;
using VKProfileTask.Entities;

namespace VKProfileTask;

public sealed class DataContext : DbContext
{
    public DbSet<UserEntity> Users { get; set; } = null!;
    public DbSet<UserGroupEntity> UserGroups { get; set; } = null!;
    public DbSet<UserStateEntity> UserStates { get; set; } = null!;

    static DataContext()
    {
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
    }

    public DataContext(DbContextOptions<DataContext> options) : base(options)
    {
        //Database.EnsureDeleted();
        Database.EnsureCreated();
    }
}