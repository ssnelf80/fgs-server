using System.ComponentModel.DataAnnotations;

namespace FGS.Auth.Models;

public class CreateUserModel
{
    [Required]
    public string Login { get; init; } = string.Empty;
    [Required]
    public string Password { get; init; } = string.Empty;
    [Required]
    public string DisplayName { get; init; } = string.Empty;
}