using System.ComponentModel.DataAnnotations;
using System.Resources;

namespace TMS.Dtos
{
    public class UserDto
    {
        public int Id { get; set; }

        public int Index { get; set; }

        [Required(ErrorMessage = "Provide email address!")]
        [EmailAddress(ErrorMessage = "Invalid email address!")]
        [MaxLength(255)]
        public string Email { get; set; }

        [Required(ErrorMessage = "Provide password for login!")]
        [MaxLength(16, ErrorMessage = "Maximum 16 characters allowed!")]
        [MinLength(8, ErrorMessage = "Minimum 8 characters required!")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[#$^+=!*()@%&]).{8,}$", ErrorMessage = "Password must have at least 1 lower case, 1 upper case,  1 number and 1 special character!")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Provide confirmation of password!")]
        [Compare("Password", ErrorMessage = "Password and confirm password missmatched!")]
        public string ConfirmPassword { get; set; }

        [Required(ErrorMessage = "Provide username!")]
        [MaxLength(25, ErrorMessage = "Provide maximum 25 characters for username!")]
        [MinLength(5, ErrorMessage = "Provide minimum 5 characters for username!")]
        public string Username { get; set; }

        [MaxLength(11, ErrorMessage = "Provide maximum 11 characters for first name!")]
        [RegularExpression("^03[0-9]{9}$", ErrorMessage = "Phone number must be formated as 03xxxxxxxxx!")]
        public string? Phone { get; set; }

        [Required(ErrorMessage = "Provide first name!")]
        [MaxLength(25, ErrorMessage = "Provide maximum 25 characters for first name!")]
        [MinLength(3, ErrorMessage = "Provide minimum 3 characters for first name!")]
        //[Required(ErrorMessageResourceType = typeof(Resources.Messages), ErrorMessageResourceName = "RequiredFieldError")]

        public string FirstName { get; set; }

        [Required(ErrorMessage = "Provide last name!")]
        [MaxLength(25, ErrorMessage = "Provide maximum 25 characters for last name!")]
        [MinLength(3, ErrorMessage = "Provide minimum 3 characters for last name!")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Select staff role!")]
        public string Role { get; set; }
        public bool IsDeleted { get; set; }
    }
    public class PasswordDto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Provide old password for verification!")]
        [MaxLength(16, ErrorMessage = "Maximum 16 characters allowed!")]
        [MinLength(8, ErrorMessage = "Minimum 8 characters required!")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[#$^+=!*()@%&]).{8,}$", ErrorMessage = "Password must have at least 1 lower case, 1 upper case,  1 number and 1 special character!")]
        public string OldPassword { get; set; }

        [Required(ErrorMessage = "Provide new password for verification!")]
        [MaxLength(16, ErrorMessage = "Maximum 16 characters allowed!")]
        [MinLength(8, ErrorMessage = "Minimum 8 characters required!")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[#$^+=!*()@%&]).{8,}$", ErrorMessage = "Password must have at least 1 lower case, 1 upper case,  1 number and 1 special character!")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Provide confirmation password for verification!")]
        [Compare("Password", ErrorMessage = "Password and confirm password missmatched!")]
        public string ConfirmPassword { get; set; }


    }

}
