using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace TMS.Controllers
{
    [Authorize]
    public class BaseController : Controller
    {
        public int CurrentUserID()
        {
            int id = 0;
            if (User?.Identity?.IsAuthenticated == true)
            {
                var idStr = User?.Claims?.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value ?? "0";
                int.TryParse(idStr, out id);
            }
            return id;
        }
    }
}
