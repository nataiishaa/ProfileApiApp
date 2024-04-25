using System.ComponentModel.DataAnnotations;

namespace VKProfileTask.Models;

public class UserForAdding
{
    [Required]
    public string Login { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public int GroupCode { get; set; }
    public string GroupDescription { get; set; } = string.Empty;
    public string StateDescription { get; set; } = string.Empty;
}