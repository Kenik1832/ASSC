using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

using ASSC.Data;
using ASSC.Models;

namespace ASSC.Controllers
{
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
            var contracts = await _context.Contracts
                .Include(c => c.Supplier)
                .Include(c => c.Contractor)
                .ToListAsync();

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