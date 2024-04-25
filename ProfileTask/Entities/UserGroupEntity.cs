using Microsoft.EntityFrameworkCore;
using VKProfileTask.Interfaces;

namespace VKProfileTask.Entities;

[Index(nameof(Id), IsUnique = true)]
public class UserGroupEntity : IEntity
{
    public static long Count { get; set; } = 1;
    public long Id { get; set; }
    public GroupCode Code { get; set; }
    public string Description { get; set; } = string.Empty;
    
    public UserGroupEntity() { }

    public UserGroupEntity(GroupCode code, string description)
        => (Id, Code, Description) =
            (Count++, code, description);
    
    public UserGroupEntity(long id, GroupCode code, string description)
        => (Id, Code, Description) =
            (id, code, description);
    
    public void Copy(UserGroupEntity otherGroup) => (Code, Description) = (otherGroup.Code, otherGroup.Description);
}