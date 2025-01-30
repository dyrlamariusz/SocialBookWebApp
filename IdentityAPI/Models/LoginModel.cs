using System.ComponentModel.DataAnnotations;

namespace IdentityAPI.Models
{
    public class LoginModel
    {
        [Required(ErrorMessage = "Username is required.")]
        public string? Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required.")]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters long.")]
        public string? Password { get; set; } = string.Empty;

        public LoginModel()
        {
            Username = string.Empty;
            Password = string.Empty;
        }
    }
}
