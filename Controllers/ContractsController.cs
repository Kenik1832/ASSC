using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

using ASSC.Data;
using ASSC.Models;

namespace ASSC.Controllers
{
    [Authorize]
    public class ContractsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ContractsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Список
        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
                return View(new List<Contract>());

            var query = _context.Contracts
                .Include(c => c.Supplier)
                .Include(c => c.Contractor)
                .AsQueryable();

            if (user.SupplierId != null)
            {
                query = query.Where(c => c.SupplierId == user.SupplierId);
            }

            if (user.ContractorId != null)
            {
                query = query.Where(c => c.ContractorId == user.ContractorId);
            }

            var contracts = await query.ToListAsync();

            return View(contracts);
        }

        // Форма создания
        public IActionResult Create()
        {
            ViewBag.Suppliers =
                new SelectList(_context.Suppliers,
                               "Id",
                               "Name");

            ViewBag.Contractors =
                new SelectList(_context.Contractors,
                               "Id",
                               "Name");

            return View();
        }

        // Создание
        [HttpPost]
        public async Task<IActionResult> Create(Contract contract)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Suppliers =
                    new SelectList(_context.Suppliers,
                                   "Id",
                                   "Name");

                ViewBag.Contractors =
                    new SelectList(_context.Contractors,
                                   "Id",
                                   "Name");

                return View(contract);
            }

            _context.Contracts.Add(contract);

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}