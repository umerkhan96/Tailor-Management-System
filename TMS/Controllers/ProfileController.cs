using Microsoft.AspNetCore.Mvc;
using TMS.Auth.Services.Interfaces;
using TMS.Dtos;

namespace TMS.Controllers
{
    public class ProfileController : BaseController
    {
        private readonly IUserService _userService;

        public ProfileController(IUserService userService)
        {
            _userService = userService;
        }

        public async Task<IActionResult> Profile()
        {
            var usr = await _userService.GetUserByID(CurrentUserID().ToString());
            if (usr != null)
            {
                return View(usr);
            }
            return RedirectToAction("Logout", "Account");
        }

        public IActionResult Password()
        {
            return View();
        }

        public async Task<JsonResult> UpdatePassword(PasswordDto model)
        {
            model.Id = CurrentUserID();
            var res = await _userService.UpdatePassword(model);
            return new JsonResult(new { status = res.status, msg = res.msg });
        }

        public async Task<JsonResult> UpdateProfile(UserDto model)
        {
            model.Id = CurrentUserID();
            if (await _userService.ExistsByEmail(model.Email, CurrentUserID()))
            {
                return new JsonResult(new { status = false, msg = "Email already used by another user!" });
            }
            if (await _userService.ExistsByUsername(model.Username, CurrentUserID()))
            {
                return new JsonResult(new { status = false, msg = "Username already used by another user!" });
            }
            await _userService.UpdateUser(model);
            return new JsonResult(new { status = true, msg = "Profile updated successfully!" });
        }
    }
}
