using Microsoft.AspNetCore.Mvc;
using TMS.Business.Services;
using TMS.Dtos;

namespace TMS.Controllers
{
    public class CustomerController : BaseController
    {
        private readonly ICustomerService _customerService;

        public CustomerController(ICustomerService customerService)
        {
            _customerService = customerService;
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

            string statusStr = Request.Form["status"];
            bool status = false;
            if (!string.IsNullOrEmpty(statusStr))
            {
                status = bool.Parse(statusStr);
            }

            var res = await _customerService.Paginate(start, length, searchValue, sortColumnName, sortDirection, status);
            return Json(new
            {
                draw,
                recordsTotal = res.Total,
                recordsFiltered = res.Data.Count,
                data = res.Data
            });
        }

        public async Task<IActionResult> GetSaveForm(int ID = 0)
        {
            var model = new CustomerDto();
            if (ID > 0)
            {
                model = await _customerService.GetByIDAsync(ID);
            }
            return PartialView("_SaveForm", model);
        }

        public async Task<List<CustomerDto>> GetCustomers()
        {
            var cust = await _customerService.GetAllAsync();
            return cust;
        }

        [HttpPost]
        public async Task<JsonResult> SaveCustomer(CustomerDto model)
        {
            if (model.Id == 0)
            {
                model.CreatedBy = CurrentUserID();
                model.CreatedDate = DateTime.Now;
                model = await _customerService.CreateAsync(model);
            }
            else
            {
                model.UpdatedBy = CurrentUserID();
                model.UpdatedDate = DateTime.Now;
                await _customerService.UpdateAsync(model);
            }
            return new JsonResult(new { status = true, id = model.Id, msg = $"Customer saved successfully! Customer Number is {model.Id}" });
        }

        [HttpPost]
        public async Task<JsonResult> DeleteCustomer(int ID)
        {
            await _customerService.DeleteByIDAsync(ID, CurrentUserID());
            return new JsonResult(new { status = true, msg = "Customer de-activated successfully!" });
        }

        [HttpPost]
        public async Task<JsonResult> ActivateCustomer(int ID)
        {
            await _customerService.RestoreByIDAsync(ID, CurrentUserID());
            return new JsonResult(new { status = true, msg = "Customer re-activated successfully!" });
        }
    }
}
