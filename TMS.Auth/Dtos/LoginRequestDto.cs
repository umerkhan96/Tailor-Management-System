using System.ComponentModel.DataAnnotations;

namespace TMS.Auth.Dtos
{
    public class LoginRequestDto
    {
        [Required(ErrorMessage = "Enter Username")]
        [MaxLength(50, ErrorMessage = "Maximum 50 characters are allowed")]
        [MinLength(5, ErrorMessage = "Minimum 5 characters should be provided")]
        public string Username { get; set; } = null!;

        [Required(ErrorMessage = "Enter Password")]
        [MaxLength(50, ErrorMessage = "Maximum 50 characters are allowed")]
        [MinLength(8, ErrorMessage = "Minimum 8 characters should be provided")]
        public string Password { get; set; } = null!;
        public bool RememberMe { get; set; }
    }
}
