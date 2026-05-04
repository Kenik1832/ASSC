using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

using ASSC.Services;
using ASSC.Data;
using ASSC.Models;
using ASSC.ViewModels;

namespace ASSC.Controllers
{
    [Authorize]
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
        public async Task<IActionResult> Index(
            InvoiceFilterViewModel filter)
        {
            var userId = 
                User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            var user =
                await _context.Users
                .FirstOrDefaultAsync(u => u.Id == userId);
                
                if (user == null)
                    return View(new List<Invoice>());

            var query = _context.Invoices
                .Include(i => i.Contract)
                .AsQueryable();

            // 🔐 фильтрация по пользователю
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

            // 🔎 ПОИСК
            if (!string.IsNullOrEmpty(filter.Search))
            {
                query = query.Where(i =>
                    i.Contract.Number.Contains(filter.Search));
            }
        
            // 📅 ДАТЫ
            if (filter.IssueDateFrom.HasValue)
            {
                query = query.Where(i =>
                    i.IssueDate >= filter.IssueDateFrom);
            }
        
            if (filter.IssueDateTo.HasValue)
            {
                query = query.Where(i =>
                    i.IssueDate <= filter.IssueDateTo);
            }
        
            // 💰 СУММА
            if (filter.MinAmount.HasValue)
            {
                query = query.Where(i =>
                    i.Amount >= filter.MinAmount);
            }
        
            if (filter.MaxAmount.HasValue)
            {
                query = query.Where(i =>
                    i.Amount <= filter.MaxAmount);
            }
        
            var invoices = await query.ToListAsync();
        
            // статус считаем через сервис
            foreach (var i in invoices)
            {
                i.Status =
                    await _financeService
                        .GetStatusAsync(i.Id);
            }
        
            // 📊 фильтр по статусу
            if (!string.IsNullOrEmpty(filter.Status))
            {
                invoices = invoices
                    .Where(i => i.Status == filter.Status)
                    .ToList();
            }

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