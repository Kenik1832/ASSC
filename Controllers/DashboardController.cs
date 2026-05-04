using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

using ASSC.Data;
using ASSC.Services;
using ASSC.ViewModels;

namespace ASSC.Controllers
{
    [Authorize]
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
                .Include(i => i.Payments) 
                .ToListAsync();

            var debtByMonth = invoices
            .GroupBy(i => i.IssueDate.Month)
            .Select(g => new
            {
                Month = g.Key,
                Total = g.Sum(i =>
                    i.Amount - i.Payments.Sum(p => p.Amount))
            })
            .OrderBy(x => x.Month)
            .ToList();    

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

            var paymentsByMonth = await _context.Payments
            .GroupBy(p => new { p.PaymentDate.Year, p.PaymentDate.Month })
            .Select(g => new
            {
                Month = g.Key.Month,
                Total = g.Sum(x => x.Amount)
            })
            .OrderBy(x => x.Month)
            .ToListAsync();

            var vm = new DashboardViewModel
            {
                TotalDebt = totalDebt,

                OverdueInvoicesCount =
                    overdueCount,

                ContractsCount = await _context.Contracts.CountAsync(),
                SuppliersCount = await _context.Suppliers.CountAsync(),
                RecentPayments = await _context.Payments
                       .OrderByDescending(p => p.PaymentDate)
                       .Take(5)
                       .ToListAsync(),

                Months = paymentsByMonth
                    .Select(x => x.Month.ToString())
                    .ToList(),

                PaymentsData = paymentsByMonth
                    .Select(x => x.Total)
                    .ToList(),

                DebtData = debtByMonth
                    .Select(x => x.Total)
                    .ToList()
            };

            return View(vm);
        }
    }
}