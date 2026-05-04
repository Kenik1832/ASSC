using ClosedXML.Excel;
using Microsoft.EntityFrameworkCore;
using ASSC.Data;

namespace ASSC.Services
{
    public class ExcelExportService
    {
        private readonly ApplicationDbContext _context;

        public ExcelExportService(ApplicationDbContext context)
        {
            _context = context;
        }

        // 📊 Экспорт счетов
        public async Task<byte[]> ExportInvoices()
        {
            var invoices = await _context.Invoices
                .Include(i => i.Contract)
                .ToListAsync();

            using var workbook = new XLWorkbook();
            var ws = workbook.Worksheets.Add("Invoices");

            ws.Cell(1, 1).Value = "ID";
            ws.Cell(1, 2).Value = "Contract";
            ws.Cell(1, 3).Value = "Amount";
            ws.Cell(1, 4).Value = "IssueDate";
            ws.Cell(1, 5).Value = "DueDate";
            ws.Cell(1, 6).Value = "Status";

            int row = 2;

            foreach (var i in invoices)
            {
                ws.Cell(row, 1).Value = i.Id;
                ws.Cell(row, 2).Value = i.Contract?.Number;
                ws.Cell(row, 3).Value = i.Amount;
                ws.Cell(row, 4).Value = i.IssueDate;
                ws.Cell(row, 5).Value = i.DueDate;
                ws.Cell(row, 6).Value = i.Status;
                row++;
            }

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);

            return stream.ToArray();
        }

        // 💰 Экспорт платежей
        public async Task<byte[]> ExportPayments()
        {
            var payments = await _context.Payments
                .Include(p => p.Invoice)
                .ToListAsync();

            using var workbook = new XLWorkbook();
            var ws = workbook.Worksheets.Add("Payments");

            ws.Cell(1, 1).Value = "ID";
            ws.Cell(1, 2).Value = "Invoice";
            ws.Cell(1, 3).Value = "Amount";
            ws.Cell(1, 4).Value = "Date";

            int row = 2;

            foreach (var p in payments)
            {
                ws.Cell(row, 1).Value = p.Id;
                ws.Cell(row, 2).Value = p.InvoiceId;
                ws.Cell(row, 3).Value = p.Amount;
                ws.Cell(row, 4).Value = p.PaymentDate;
                row++;
            }

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);

            return stream.ToArray();
        }
    }
}