using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using TMS.Business.Services;
using TMS.Dtos;
using TMS.Models;

namespace TMS.Controllers
{
    public class HomeController : BaseController
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ICustomerService _customerService;
        private readonly IOrdersService _ordersService;

        public HomeController(ILogger<HomeController> logger, ICustomerService customerService, IOrdersService ordersService)
        {
            _logger = logger;
            _customerService = customerService;
            _ordersService = ordersService;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<int> GetCustomerCount()
        {
            var data = await _customerService.GetTotalCountAsync();
            return data;
        }

        public async Task<int> GetTotalOrdersCount()
        {
            var data = await _ordersService.GetTotalCountAsync();
            return data;
        }

        public async Task<int> GetTodayTakenOrdersCount()
        {
            var data = await _ordersService.GetTodayTakenCountAsync();
            return data;
        }

        public async Task<int> GetTodayReturnOrdersCount()
        {
            var data = await _ordersService.GetTodayReturnCountAsync();
            return data;
        }
        public async Task<List<OrdersDto>> GetCalanderOrders()
        {
            var data = await _ordersService.GetCalanderOrdersDetail();
            return data;
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}