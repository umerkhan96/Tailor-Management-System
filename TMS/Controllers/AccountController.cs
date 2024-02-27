using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TMS.Auth.Dtos;
using TMS.Auth.Services.Interfaces;

namespace TMS.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUserService _userService;

        public AccountController(IUserService userService)
        {
            _userService = userService;
        }


        public IActionResult Login()
        {
            LoginRequestDto model = new LoginRequestDto();
            return View(model);
        }

        public async Task<IActionResult> Logout()
        {
            if (User.Identity.IsAuthenticated)
            {
                await _userService.Logout();
            }
            return RedirectToAction("Login");
        }

        [HttpPost]
        public async Task<JsonResult> Login(LoginRequestDto model)
        {
            var res = await _userService.Login(model.Username, model.Password, model.RememberMe);
            return new JsonResult(new
            {
                status = res.status,
                msg = res.msg
            });
        }

        public IActionResult ForgotPassword()
        {
            return View();
        }
    }
}
