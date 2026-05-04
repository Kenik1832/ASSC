using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;


using ASSC.Data;
using ASSC.Models;

namespace ASSC.Controllers
{
    [Authorize]
    public class PaymentsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PaymentsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // список платежей
        public async Task<IActionResult> Index()
        {
            var userId =
                User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            var user =
                await _context.Users
                .FirstOrDefaultAsync(u => u.Id == userId);
                
                if (user == null)
                    return View(new List<Payment>());

            var query = _context.Payments
                .Include(p => p.Invoice)
                    .ThenInclude(i => i.Contract)
                .AsQueryable();

            if (user.SupplierId != null)
            {
                query = query.Where(p =>
                    p.Invoice.Contract.SupplierId == user.SupplierId);
            }

            if (user.ContractorId != null)
            {
                query = query.Where(p =>
                    p.Invoice.Contract.ContractorId == user.ContractorId);
            }

            return View(await query.ToListAsync());
        }

        // форма
        public IActionResult Create()
        {
            ViewBag.Invoices =
                new SelectList(
                    _context.Invoices,
                    "Id",
                    "Id"
                );

            return View();
        }

        // создание платежа
        [HttpPost]
        public async Task<IActionResult> Create(Payment payment)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Invoices =
                    new SelectList(
                        _context.Invoices,
                        "Id",
                        "Id"
                    );

                return View(payment);
            }

            _context.Payments.Add(payment);

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}