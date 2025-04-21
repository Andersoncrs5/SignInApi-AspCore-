using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SignInApi.Entities;

[Table("users")]
public class UsersEntity
{
    [Key]
    public long Id { get; set; } 

    [Required(ErrorMessage = "Field is required")]
    [StringLength(100, ErrorMessage = "Max size of 100")]
    public string Name { get; set; } = string.Empty; 

    [Required(ErrorMessage = "Field is required")]
    [EmailAddress(ErrorMessage = "Invalid Email")]
    [StringLength(150, ErrorMessage = "Max size of 150")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Field is required")]
    [StringLength(50, ErrorMessage = "Max size of 50", MinimumLength = 6)] 
    public string Password { get; set; } = string.Empty;

}
