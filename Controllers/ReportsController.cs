using Microsoft.AspNetCore.Mvc;
using ASSC.Services;

namespace ASSC.Controllers
{
    public class ReportsController : Controller
    {
        private readonly ExcelExportService _excel;

        public ReportsController(ExcelExportService excel)
        {
            _excel = excel;
        }

        // 📄 Экспорт счетов
        public async Task<IActionResult> Invoices()
        {
            var file = await _excel.ExportInvoices();

            return File(
                file,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "invoices.xlsx"
            );
        }

        // 💰 Экспорт платежей
        public async Task<IActionResult> Payments()
        {
            var file = await _excel.ExportPayments();

            return File(
                file,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "payments.xlsx"
            );
        }
    }
}