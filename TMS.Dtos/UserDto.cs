using System.ComponentModel.DataAnnotations;
using TMS.Dtos.Resources;

namespace TMS.Dtos
{
    public class UserDto
    {
        public int Id { get; set; }

        public int Index { get; set; }

        [Required(ErrorMessageResourceName = "Provide_email_address", ErrorMessageResourceType = typeof(ValidationResources))]
        [EmailAddress(ErrorMessageResourceName = "Invalid_email_address", ErrorMessageResourceType = typeof(ValidationResources))]
        [MaxLength(255)]
        public string Email { get; set; }

        [Required(ErrorMessageResourceName = "Provide_password_for_login", ErrorMessageResourceType = typeof(ValidationResources))]
        [MaxLength(16, ErrorMessageResourceName = "Maximum_characters_allowed", ErrorMessageResourceType = typeof(ValidationResources))]
        [MinLength(8, ErrorMessageResourceName = "Minimum_characters_required", ErrorMessageResourceType = typeof(ValidationResources))]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[#$^+=!*()@%&]).{8,}$", ErrorMessageResourceName = "Password_must_have_atleast", ErrorMessageResourceType = typeof(ValidationResources))]
        public string Password { get; set; }

        [Required(ErrorMessageResourceName = "Provide_confirmation_of_password", ErrorMessageResourceType = typeof(ValidationResources))]
        [Compare("Password", ErrorMessageResourceName = "Password_and_confirm_password_missmatched", ErrorMessageResourceType = typeof(ValidationResources))]
        public string ConfirmPassword { get; set; }

        [Required(ErrorMessageResourceName = "Provide_username", ErrorMessageResourceType = typeof(ValidationResources))]
        [MaxLength(25, ErrorMessageResourceName = "Maximum_characters_allowed", ErrorMessageResourceType = typeof(ValidationResources))]
        [MinLength(5, ErrorMessageResourceName = "Minimum_characters_required", ErrorMessageResourceType = typeof(ValidationResources))]
        public string Username { get; set; }

        [MaxLength(11, ErrorMessageResourceName = "Maximum_characters_allowed", ErrorMessageResourceType = typeof(ValidationResources))]
        [RegularExpression("^03[0-9]{9}$", ErrorMessageResourceName = "Phone_number_must_be_formated_as", ErrorMessageResourceType = typeof(ValidationResources))]
        public string? Phone { get; set; }

        [Required(ErrorMessageResourceName = "Provide_first_name", ErrorMessageResourceType = typeof(ValidationResources))]
        [MaxLength(25, ErrorMessageResourceName = "Maximum_characters_allowed", ErrorMessageResourceType = typeof(ValidationResources))]
        [MinLength(3, ErrorMessageResourceName = "Minimum_characters_required", ErrorMessageResourceType = typeof(ValidationResources))]
        public string FirstName { get; set; }

        [Required(ErrorMessageResourceName = "Provide_last_name", ErrorMessageResourceType = typeof(ValidationResources))]
        [MaxLength(25, ErrorMessageResourceName = "Maximum_characters_allowed", ErrorMessageResourceType = typeof(ValidationResources))]
        [MinLength(3, ErrorMessageResourceName = "Minimum_characters_required", ErrorMessageResourceType = typeof(ValidationResources))]
        public string LastName { get; set; }

        [Required(ErrorMessageResourceName = "Select_staff_role", ErrorMessageResourceType = typeof(ValidationResources))]
        public string Role { get; set; }
        public bool IsDeleted { get; set; }
    }
    public class PasswordDto
    {
        public int Id { get; set; }

        [Required(ErrorMessageResourceName = "Provide_old_password_for_verification", ErrorMessageResourceType = typeof(ValidationResources))]
        [MaxLength(16, ErrorMessageResourceName = "Maximum_characters_allowed", ErrorMessageResourceType = typeof(ValidationResources))]
        [MinLength(8, ErrorMessageResourceName = "Minimum_characters_required", ErrorMessageResourceType = typeof(ValidationResources))]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[#$^+=!*()@%&]).{8,}$", ErrorMessageResourceName = "Password_must_have_atleast", ErrorMessageResourceType = typeof(ValidationResources))]
        public string OldPassword { get; set; }

        [Required(ErrorMessageResourceName = "Provide_new_password")]
        [MaxLength(16, ErrorMessageResourceName = "Maximum_characters_allowed", ErrorMessageResourceType = typeof(ValidationResources))]
        [MinLength(8, ErrorMessageResourceName = "Minimum_characters_required", ErrorMessageResourceType = typeof(ValidationResources))]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[#$^+=!*()@%&]).{8,}$", ErrorMessageResourceName = "Password_must_have_atleast", ErrorMessageResourceType = typeof(ValidationResources))]
        public string Password { get; set; }

        [Required(ErrorMessageResourceName = "Provide_confirmation_password")]
        [Compare("Password", ErrorMessageResourceName = "Password_and_confirm_password_missmatched", ErrorMessageResourceType = typeof(ValidationResources))]
        public string ConfirmPassword { get; set; }


    }

}
