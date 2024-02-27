using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TMS.Auth.Dtos;

namespace TMS.Auth.Contexts
{
    public class TmsIdentityContext : IdentityDbContext<ApplicationUser, ApplicationRole, int>
    {
        public TmsIdentityContext(DbContextOptions<TmsIdentityContext> options) : base(options)
        {
        }
    }
}
