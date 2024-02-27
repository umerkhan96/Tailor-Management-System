using ClosedXML.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Newtonsoft.Json;
using System;
using TMS.Auth.Services.Interfaces;
using TMS.Business.Services;
using TMS.Common.Helpers;
using TMS.Data.Entities;
using TMS.Dtos;
using Cell = iText.Layout.Element.Cell;
using Table = iText.Layout.Element.Table;

namespace TMS.Controllers
{
    public class OrdersController : BaseController
    {
        private readonly IOrdersService _ordersService;
        private readonly ICustomerService _customerService;
        private readonly IUserService _userService;
        private readonly IConfiguration _configuration;
        private readonly SmsHelper _smsHelper;
        private readonly InvoiceHelper _invoiceHelper;
        private readonly ICompositeViewEngine _viewEngine;
        private readonly ITempDataProvider _tempDataProvider;

        public OrdersController(IOrdersService ordersService, ICustomerService customerService, IUserService userService,
            IConfiguration configuration, SmsHelper smsHelper, InvoiceHelper invoiceHelper, ICompositeViewEngine viewEngine,
            ITempDataProvider tempDataProvider)
        {
            _ordersService = ordersService;
            _customerService = customerService;
            _userService = userService;
            _configuration = configuration;
            _smsHelper = smsHelper;
            _invoiceHelper = invoiceHelper;
            _viewEngine = viewEngine;
            _tempDataProvider = tempDataProvider;
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

            string statusStr = Request.Form["isCollected"];
            string dtFromStr = Request.Form["dtFrom"];
            string dtToStr = Request.Form["dtTo"];

            bool isCollected = false;
            DateTime? dtFrom = null;
            DateTime? dtTo = null;

            if (!string.IsNullOrEmpty(statusStr))
            {
                isCollected = bool.Parse(statusStr);
            }
            if (!string.IsNullOrEmpty(dtFromStr))
            {
                dtFrom = DateTime.Parse(dtFromStr);
            }
            if (!string.IsNullOrEmpty(dtToStr))
            {
                dtTo = DateTime.Parse(dtToStr);
            }


            var res = await _ordersService.Paginate(start, length, searchValue, sortColumnName,
                sortDirection, isCollected, dtFrom, dtTo);
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
            var model = new OrdersDto();
            if (ID > 0)
            {
                model = await _ordersService.GetByIDAsync(ID);
            }
            model.Customers = await _customerService.GetAllAsync();
            model.Cutters = await _userService.GetUsersByRole(_configuration.GetSection("SeederData:DefaultRoleCutter").Value);
            model.Tailors = await _userService.GetUsersByRole(_configuration.GetSection("SeederData:DefaultRoleTailor").Value);
            return View("_SaveForm", model);
        }

        [HttpPost]
        public async Task<JsonResult> SaveOrder(OrdersDto order)
        {
            try
            {
                if (order.Id == 0)
                {
                    order.CreatedDate = DateTime.Now;
                    order.CreatedBy = CurrentUserID();
                    order = await _ordersService.CreateAsync(order);
                }
                else
                {
                    order.UpdatedDate = DateTime.Now;
                    order.UpdatedBy = CurrentUserID();
                    order = await _ordersService.UpdateAsync(order);
                }
                return new JsonResult(new { status = true, msg = $"Order saved successfully! Order Number is {order.Id}.", id = order.Id });
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<JsonResult> SendSms(int OrderID, bool Status)
        {
            var order = await _ordersService.GetByIDAsync(OrderID);
            if (order == null)
            {
                return new JsonResult(new { status = false, msg = $"Invalid order details!" });
            }

            var usr = await _customerService.GetByIDAsync(order.CustomerId);
            var cmpName = _configuration.GetSection("CompanyName").Value;
            var msg = $"Dear customer {usr.FirstName}, your Order#{order.Id} is taken at {cmpName}.\n\nThanks for trusting us.\n\n{cmpName}";
            if (order.IsReady)
            {
                msg = $"Dear customer {usr.FirstName}, your Order#{order.Id} is ready. Kindly pick your order with in 30 days.\n\nThanks for trusting us.\n\n{cmpName}";
            }
            var resp = _smsHelper.SendSms(usr.MobileNumber, msg);

            //if (order.IsCollected)
            //{
            //    msg = $"Dear {usr.FirstName}, your Order#{order.Id} is collected from at {cmpName} on {DateTime.Now.ToString("dd/MM/yyyy hh:mm tt")}.";
            //}
            return new JsonResult(new { status = true, msg = "Message sent succesfully!" });
        }

        public async Task<IActionResult> DownloadInvoice(int OrderID)
        {
            var order = await _ordersService.GetByIDAsync(OrderID);
            var html = await GetHtmlOfViewWithModel(order);
            var pdfBuffer = _invoiceHelper.GetPdf(html);
            order.IsForDownload = true;
            return File(pdfBuffer, "application/pdf", $"Invoice-{OrderID}-{DateTime.Now.ToString("yyyy-MM-dd-hh-mm-ss")}.pdf");
        }

        public async Task<IActionResult> GetInvoice(int ID)
        {
            var order = await _ordersService.GetByIDAsync(ID);
            if (order == null)
            {
                return View("_InvoiceTemplate", new OrdersDto());
            }
            order.IsForDownload = false;
            return View("_InvoiceTemplate", order);
        }

        public async Task<IActionResult> GetMeasurements(int ID)
        {
            var order = await _ordersService.GetByIDAsync(ID);
            return View("_Measurements", order);
        }

        public async Task<IActionResult> PrintMeasurements(int ID)
        {
            var order = await _ordersService.GetByIDAsync(ID);
            var html = await GetHtmlOfViewWithModelForMeasurements(order);
            var pdfBuffer = _invoiceHelper.GetPdf(html);
            order.IsForDownload = true;
            return File(pdfBuffer, "application/pdf", $"Measurements-{ID}-{DateTime.Now.ToString("yyyy-MM-dd-hh-mm-ss")}.pdf");
            //return View("_PrintMeasurements", order);
        }

        public async Task<string> GetHtmlOfViewWithModel(OrdersDto model)
        {
            ViewBag.RootPath = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}";
            model.IsForDownload = true;
            // Get the view result
            var viewResult = _viewEngine.FindView(ControllerContext, "_InvoiceTemplate", false);

            // Create the view context
            var viewContext = new ViewContext(
                ControllerContext,
                viewResult.View,
                new ViewDataDictionary<OrdersDto>(ViewData, model),
                new TempDataDictionary(ControllerContext.HttpContext, _tempDataProvider),
                new System.IO.StringWriter(),
                new HtmlHelperOptions()
            );

            // Render the view to a string
            await viewResult.View.RenderAsync(viewContext);
            var html = (viewContext.Writer as StringWriter)?.ToString();
            return html ?? "";
        }

        public async Task<string> GetHtmlOfViewWithModelForMeasurements(OrdersDto model)
        {
            ViewBag.RootPath = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}";
            model.IsForDownload = true;
            // Get the view result
            var viewResult = _viewEngine.FindView(ControllerContext, "_PrintMeasurements", false);

            // Create the view context
            var viewContext = new ViewContext(
                ControllerContext,
                viewResult.View,
                new ViewDataDictionary<OrdersDto>(ViewData, model),
                new TempDataDictionary(ControllerContext.HttpContext, _tempDataProvider),
                new System.IO.StringWriter(),
                new HtmlHelperOptions()
            );

            // Render the view to a string
            await viewResult.View.RenderAsync(viewContext);
            var html = (viewContext.Writer as StringWriter)?.ToString();
            return html ?? "";
        }


        public IActionResult Invoices()
        {
            return View();
        }

        public async Task<IActionResult> BalanceSheet()
        {
            ViewBag.Customers = await _customerService.GetAllAsync();
            return View();
        }

        [HttpGet]
        public IActionResult DownloadBalanceSheet(int Cid = 0, int Oid = 0, DateTime? dtFrom = null, DateTime? dtTo = null)
        {
            var data = _ordersService.GetBalanceSheetData(Cid, Oid, dtFrom, dtTo);
            var fileBytes = GenerateBalanceSheetExcelInBackground(data);
            var fileName = $"Balancesheet_{DateTime.Now.ToString("dd_MM_yyyy_hh_mm_ss")}.xlsx";
            return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }

        [HttpGet]
        public IActionResult DownloadBalancePdf(int Cid = 0, int Oid = 0, DateTime? dtFrom = null, DateTime? dtTo = null)
        {
            var fileName = $"Balancesheet_{DateTime.Now.ToString("dd_MM_yyyy_hh_mm_ss")}.pdf";
            var data = _ordersService.GetBalanceSheetData(Cid, Oid, dtFrom, dtTo);
            var fileBytes = GenerateBalanceSheetPdfInBackground(data, fileName);
            return File(fileBytes, "application/pdf", fileName);
        }

        private byte[] GenerateBalanceSheetExcelInBackground(List<OrdersDto> data)
        {
            var cids = data.Select(x => new { x.CustomerId, x.CustomerName }).DistinctBy(x => x.CustomerId).ToList();

            using (var workbook = new XLWorkbook())
            {
                foreach (var cid in cids)
                {
                    var orders = data.Where(x => x.CustomerId == cid.CustomerId).OrderByDescending(x => x.OrderDate).ToList();
                    var worksheet = workbook.Worksheets.Add($"{cid.CustomerName}");
                    worksheet.Cell(1, 1).Value = "#";
                    worksheet.Cell(1, 2).Value = "Date";
                    worksheet.Cell(1, 3).Value = "Description";
                    worksheet.Cell(1, 4).Value = "Total Amount";
                    worksheet.Cell(1, 5).Value = "Paid Amount";
                    worksheet.Cell(1, 6).Value = "Balance Amount";
                    worksheet.Cell(1, 7).Value = "Status";
                    int row = 2;
                    foreach (var order in orders)
                    {
                        worksheet.Cell(row, 1).Value = row - 1;
                        worksheet.Cell(row, 2).Value = order.OrderDate.ToString("dd MMM, yyyy");
                        worksheet.Cell(row, 3).Value = $"{order.CustomerName}";
                        worksheet.Cell(row, 4).Value = $"Rs {order.TotalAmount}";
                        worksheet.Cell(row, 5).Value = $"Rs {order.PaidAmount}";
                        worksheet.Cell(row, 6).Value = $"Rs {order.BalanceAmount}";
                        worksheet.Cell(row, 7).Value = $"{(order.IsCollected ? "Collected" : "In Store")}";
                        row++;
                    }
                }

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    return stream.ToArray();
                }
            }

        }

        private byte[] GenerateBalanceSheetPdfInBackground(List<OrdersDto> data, string fileName)
        {
            var cids = data.Select(x => new { x.CustomerId, x.CustomerName }).DistinctBy(x => x.CustomerId).ToList();
            var pdfDoc = new PdfDocument(new PdfWriter(fileName));
            var document = new Document(pdfDoc);
            int i = 0;
            foreach (var cid in cids)
            {
                if (i > 0)
                    document.Add(new AreaBreak());
                i++;
                var orders = data.Where(x => x.CustomerId == cid.CustomerId).OrderByDescending(x => x.OrderDate).ToList();

                document.Add(new Paragraph($"Balance Sheet for {cid.CustomerName}").SetTextAlignment(TextAlignment.CENTER).SetBold());
                document.Add(new Paragraph($""));
                document.Add(new Paragraph($"Customer ID: {cid.CustomerId}"));
                document.Add(new Paragraph($"Customer Name: {cid.CustomerName}"));

                var table = new Table(new float[] { 1, 2, 3, 4, 5, 6, 7 }).SetWidth(UnitValue.CreatePercentValue(100))
                  .SetHorizontalAlignment(HorizontalAlignment.CENTER);

                table.AddHeaderCell(new Cell().Add(new Paragraph("#")));
                table.AddHeaderCell(new Cell().Add(new Paragraph("Date")));
                table.AddHeaderCell(new Cell().Add(new Paragraph("Description")));
                table.AddHeaderCell(new Cell().Add(new Paragraph("Total Amount")));
                table.AddHeaderCell(new Cell().Add(new Paragraph("Paid Amount")));
                table.AddHeaderCell(new Cell().Add(new Paragraph("Balance Amount")));
                table.AddHeaderCell(new Cell().Add(new Paragraph("Status")));
                int row = 1;
                foreach (var order in orders)
                {
                    table.AddCell(new Cell().Add(new Paragraph($"{row}")));
                    table.AddCell(new Cell().Add(new Paragraph($"{order.OrderDate.ToString("dd MMM, yyyy")}")));
                    table.AddCell(new Cell().Add(new Paragraph($"{order.CustomerName}")));
                    table.AddCell(new Cell().Add(new Paragraph($"Rs {order.TotalAmount}")));
                    table.AddCell(new Cell().Add(new Paragraph($"Rs {order.PaidAmount}")));
                    table.AddCell(new Cell().Add(new Paragraph($"Rs {order.TotalAmount - order.PaidAmount}")));
                    table.AddCell(new Cell().Add(new Paragraph($"{(order.IsCollected ? "Collected" : "In Store")}")));
                    row++;
                }

                document.Add(table);
            }
            document.Close();
            byte[] fileBytes = System.IO.File.ReadAllBytes(fileName);
            return fileBytes;
        }

        [HttpPost]
        public async Task<JsonResult> MarkAsReady(int ID)
        {
            await _ordersService.MarkAsReady(ID);
            return new JsonResult(new { status = true, msg = "Order marked as ready" });
        }

        [HttpPost]
        public async Task<JsonResult> MarkAsCollected(int ID)
        {
            await _ordersService.MarkAsCollected(ID);
            return new JsonResult(new { status = true, msg = "Order marked as collected" });
        }
    }
}
