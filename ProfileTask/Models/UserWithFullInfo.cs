using System.ComponentModel.DataAnnotations;
using VKProfileTask.Entities;

namespace VKProfileTask.Models;

public class UserWithFullInfo
{
    public long Id { get; set; }
    [Required]
    public string Login { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public DateTime CreatedDate { get; set; }
    public string GroupCode { get; set; } = string.Empty;
    public string GroupDescription { get; set; } = string.Empty;
    public string StateCode { get; set; } = string.Empty;
    public string StateDescription { get; set; } = string.Empty;

    public UserWithFullInfo() {}
    
    public UserWithFullInfo(UserEntity user, UserGroupEntity userGroup, UserStateEntity userState)
    {
        Id = user.Id;
        Login = user.Login;
        Password = user.Password;
        CreatedDate = user.CreatedDate;
        GroupCode = userGroup.Code.ToString();
        StateCode = userState.Code.ToString();
        GroupDescription = userGroup.Description;
        StateDescription = userState.Description;
    }
}