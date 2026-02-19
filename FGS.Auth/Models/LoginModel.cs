using System.ComponentModel.DataAnnotations;

namespace FGS.Auth.Models;

public class LoginModel
{
    [Required]
    public string Login { get; init; } = string.Empty;
    [Required]
    public string Password { get; init; } = string.Empty;
}