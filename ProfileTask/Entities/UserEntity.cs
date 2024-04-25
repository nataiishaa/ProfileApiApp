using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using VKProfileTask.Interfaces;

namespace VKProfileTask.Entities;

[Index(nameof(Login), IsUnique = true)]
public class UserEntity : IEntity
{
    public static long Count { get; set; } = 1;
    public long Id { get; set; }
    [Key]
    [Required]
    public string Login { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public DateTime CreatedDate { get; set; }
    public long UserGroupId { get; set; }
    public long UserStateId { get; set; }

    public UserEntity() { }
    
    public UserEntity(string login, string password, long userGroupId, long userStateId)
        => (Id, Login, Password, CreatedDate, UserGroupId, UserStateId) =
            (Count++, login, password, DateTime.Now, userGroupId, userStateId);
    
    public void Copy(UserEntity otherUser) => (Login, Password, CreatedDate, UserGroupId, UserStateId) = (
        otherUser.Login, otherUser.Password, otherUser.CreatedDate, otherUser.UserGroupId, otherUser.UserStateId);
}