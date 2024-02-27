using Microsoft.AspNetCore.Identity;

namespace TMS.Auth.Dtos
{
    public class ApplicationRole : IdentityRole<int>
    {
        public ApplicationRole() : base()
        {

        }

        public ApplicationRole(string name) : base(name)
        {

        }
    }
}
