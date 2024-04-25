using Microsoft.EntityFrameworkCore;
using VKProfileTask.Interfaces;

namespace VKProfileTask.Entities;

[Index(nameof(Id), IsUnique = true)]
public class UserStateEntity : IEntity
{
    public static long Count { get; set; } = 1;
    public long Id { get; set; }
    public StateCode Code { get; set; }
    public string Description { get; set; } = string.Empty;
    
    public UserStateEntity() {}
    
    public UserStateEntity(StateCode code, string description)
        => (Id, Code, Description) =
            (Count++, code, description);
    
    public UserStateEntity(long id, StateCode code, string description)
        => (Id, Code, Description) =
            (id, code, description);
    
    public void Copy(UserStateEntity otherState)
    {
        Code = otherState.Code;
        
        if (otherState.Description != string.Empty)
        {
            Description = otherState.Description;
        }
    }
}