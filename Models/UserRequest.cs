using System.ComponentModel.DataAnnotations;

namespace YOApi.Models;

public class UserRequest
{
    [Required]
    [MinLength(6)]
    [MaxLength(100)]
#nullable disable
    public string Address { get; set; }

    [Required]
    [MinLength(6)]
    [MaxLength(100)]
    public string Password { get; set; }
#nullable enable
}