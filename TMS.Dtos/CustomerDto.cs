using System.ComponentModel.DataAnnotations;

namespace TMS.Dtos
{
    public class CustomerDto
    {
        public int Id { get; set; }
        public int Index { get; set; }

        [Required(ErrorMessage = "Provide first name!")]
        [MaxLength(50)]
        [MinLength(3, ErrorMessage = "Minimum 3 characters are must!")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Provide last name!")]
        [MaxLength(50)]
        [MinLength(3, ErrorMessage = "Minimum 3 characters are must!")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Provide mobile number!")]
        [MaxLength(11)]
        [MinLength(11, ErrorMessage = "Minimum 11 characters are must!")]
        [RegularExpression("^03[0-9]{9}$", ErrorMessage = "Phone number must be formated as 03xxxxxxxxx!")]
        public string MobileNumber { get; set; }

        [MaxLength(255)]
        public string? Address { get; set; }

        public int CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public bool IsDelete { get; set; }
        public int? DeletedBy { get; set; }
        public DateTime? DeletedDate { get; set; }
    }
}
