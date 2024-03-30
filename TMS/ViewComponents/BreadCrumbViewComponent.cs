using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Localization;
using System.Globalization;

namespace TMS.ViewComponents
{
    public class BreadCrumbViewComponent : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync(LocalizedHtmlString Title, LocalizedHtmlString From, LocalizedHtmlString To)
        {
            ViewBag.Title = Title;
            ViewBag.From = From;
            ViewBag.To = To;
            return await Task.FromResult((IViewComponentResult)View("BreadCrumb"));
        }
    }
}
