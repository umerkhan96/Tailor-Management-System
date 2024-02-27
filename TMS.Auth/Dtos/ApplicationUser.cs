using Microsoft.AspNetCore.Identity;

namespace TMS.Auth.Dtos
{
    public class ApplicationUser : IdentityUser<int>
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public bool IsDeleted { get; set; }
    }

}
