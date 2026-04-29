using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

using ASSC.Services;
using ASSC.Data;
using ASSC.Models;

namespace ASSC.Controllers
{
    public class InvoicesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly FinanceService _financeService;
        
        public InvoicesController(
           ApplicationDbContext context,
           FinanceService financeService)
        {
           _context = context;
           _financeService = financeService;
        }
        
        // список счетов
        public async Task<IActionResult> Index()
        {
            var userId = 
                User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            var user =
                await _context.Users
                .FirstOrDefaultAsync(u => u.Id == userId);

            var query = _context.Invoices
                .Include(i => i.Contract)
                .AsQueryable();

            if (user.SupplierId != null)
            {
                query = query.Where(i =>
                    i.Contract.SupplierId == user.SupplierId);
            }

            if (user.ContractorId != null)
            {
                query = query.Where(i =>
                    i.Contract.ContractorId == user.ContractorId);
            }

            var invoices = await query.ToListAsync();

            return View(invoices);
        }

        // форма создания
        public IActionResult Create()
        {
            ViewBag.Contracts =
                new SelectList(
                    _context.Contracts,
                    "Id",
                    "Number"
                );

            return View();
        }

        // создание
        [HttpPost]
        public async Task<IActionResult> Create(
            Invoice invoice)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Contracts =
                    new SelectList(
                        _context.Contracts,
                        "Id",
                        "Number"
                    );

                return View(invoice);
            }

            // пока простая логика статуса
            invoice.Status = "Unpaid";

            _context.Invoices.Add(invoice);

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}