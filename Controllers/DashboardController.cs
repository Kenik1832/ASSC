using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using ASSC.Data;
using ASSC.Services;
using ASSC.ViewModels;

namespace ASSC.Controllers
{
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly FinanceService _finance;

        public DashboardController(
            ApplicationDbContext context,
            FinanceService finance)
        {
            _context = context;
            _finance = finance;
        }

        public async Task<IActionResult> Index()
        {
            decimal totalDebt = 0;
            int overdueCount = 0;

            var invoices = await _context.Invoices
                .ToListAsync();

            foreach(var invoice in invoices)
            {
                totalDebt +=
                    await _finance
                        .GetDebtAsync(invoice.Id);

                var status =
                    await _finance
                        .GetStatusAsync(invoice.Id);

                if(status=="Overdue")
                    overdueCount++;
            }

            var vm = new DashboardViewModel
            {
                TotalDebt = totalDebt,

                OverdueInvoicesCount =
                    overdueCount,

                ContractsCount =
                    await _context.Contracts
                        .CountAsync(),

                SuppliersCount =
                    await _context.Suppliers
                        .CountAsync(),

                RecentPayments =
                    await _context.Payments
                       .OrderByDescending(
                           p=>p.PaymentDate)
                       .Take(5)
                       .ToListAsync()
            };

            return View(vm);
        }
    }
}