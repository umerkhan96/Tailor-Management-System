using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TMS.Auth.Dtos;
using TMS.Auth.Services.Interfaces;

namespace TMS.ViewComponents
{
    public class NavigationViewComponent : ViewComponent
    {
        private readonly IUserService _userService;

        public NavigationViewComponent(IUserService userService)
        {
            _userService = userService;
        }


        public async Task<IViewComponentResult> InvokeAsync()
        {
            var uid = UserClaimsPrincipal.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier).Value;
            var user = await _userService.GetUserByID(uid);
            return await Task.FromResult((IViewComponentResult)View("Navigation", user));
        }
    }
}
