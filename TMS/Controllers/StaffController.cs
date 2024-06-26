﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Localization;
using TMS.Auth.Services.Interfaces;
using TMS.Dtos;

namespace TMS.Controllers
{
    public class StaffController : BaseController
    {
        private readonly IUserService _userService;
        private readonly IConfiguration _configuration;
        private readonly IHtmlLocalizer<StaffController> _localizer;

        public StaffController(IUserService userService, IConfiguration configuration, IHtmlLocalizer<StaffController> localizer)
        {
            _userService = userService;
            _configuration = configuration;
            _localizer = localizer;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Paginate()
        {
            var draw = int.Parse(Request.Form["draw"]);
            var start = int.Parse(Request.Form["start"]);
            var length = int.Parse(Request.Form["length"]);
            var searchValue = Request.Form["search[value]"];
            var sortColumnIndex = int.Parse(Request.Form["order[0][column]"]);
            var sortColumnName = Request.Form[$"columns[{sortColumnIndex}][data]"];
            var sortDirection = Request.Form["order[0][dir]"];
            bool? status = null;
            var role = Request.Form["role"];
            var statusStr = Request.Form["status"];
            if (!string.IsNullOrEmpty(statusStr))
            {
                status = bool.Parse(statusStr);
            }

            var res = await _userService.GetStaffUsers(searchValue, length, start, role, sortColumnName, sortDirection, status);
            res.Data.ForEach(x =>
            {
                x.Role = _localizer[x.Role].Value;
            });
            return Json(new
            {
                draw,
                recordsTotal = res.Data.Count,
                recordsFiltered = res.Total,
                data = res.Data,
            });
        }


        public async Task<IActionResult> GetSaveForm(int ID = 0)
        {
            var model = new UserDto();
            if (ID > 0)
            {
                model = await _userService.GetUserByID(ID.ToString());
            }
            return PartialView("_SaveForm", model);
        }

        [HttpPost]
        public async Task<JsonResult> SaveStaff(UserDto model)
        {
            if (await _userService.ExistsByEmail(model.Email, model.Id))
            {
                return new JsonResult(new { status = false, msg = _localizer["Email_already_exists_in_users"] });
            }
            if (await _userService.ExistsByUsername(model.Username, model.Id))
            {
                return new JsonResult(new { status = false, msg = _localizer["Username_already_exists_in_users"] });
            }
            if (model.Id == 0)
                model = await _userService.CreateUser(model);
            else
                await _userService.UpdateUser(model);
            return new JsonResult(new { status = true, msg = _localizer["Staff saved successfully"] });
        }

        [HttpPost]
        public async Task<JsonResult> DeleteStaff(string ID)
        {
            await _userService.DeleteUserByID(ID);
            return new JsonResult(new { status = true, msg = _localizer["Staff_deactivated_successfully"] });
        }

        [HttpPost]
        public async Task<JsonResult> ActivateStaff(string ID)
        {
            await _userService.RestoreUserByID(ID);
            return new JsonResult(new { status = true, msg = _localizer["Staff_reactivated_successfully"] });
        }
    }
}
