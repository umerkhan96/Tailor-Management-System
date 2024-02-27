using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using HiQPdf;

namespace TMS.Common.Helpers
{
    public class InvoiceHelper
    {
        public byte[] GetPdf(string html)
        {
            try
            {
                HtmlToPdf htmlToPdfConverter = new HtmlToPdf();
                htmlToPdfConverter.Document.Margins = new PdfMargins(10);
                htmlToPdfConverter.Document.PageSize = PdfPageSize.A4;
                //htmlToPdfConverter.Document.FitPageHeight = true;
                //htmlToPdfConverter.Document.FitPageWidth = true;
                //htmlToPdfConverter.BrowserWidth = 1200;
                //htmlToPdfConverter.Document.ResizePageWidth = true;
                htmlToPdfConverter.Document.PageOrientation = PdfPageOrientation.Portrait;
                //htmlToPdfConverter.BrowserHeight = 842;
                byte[] pdfBuffer = htmlToPdfConverter.ConvertHtmlToMemory(html, "");
                return pdfBuffer;
                // Return the PDF as a response
                //return File(pdfBuffer, "application/pdf", "output.pdf");
            }
            catch (Exception ex)
            {
                return null;
                // Handle exceptions appropriately
                //return BadRequest($"Error: {ex.Message}");
            }
        }
    }
}
