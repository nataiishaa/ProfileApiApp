using System.ComponentModel.DataAnnotations;

namespace VKProfileTask.Models;

public class UserWithLogin
{
    [Required]
    public string Login { get; set; } = string.Empty;
}