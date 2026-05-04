using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

using ASSC.Data;
using ASSC.Models;

namespace ASSC.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ContractorsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ContractorsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Список
        public async Task<IActionResult> Index()
        {
            return View(
                await _context.Contractors
                    .OrderBy(x => x.Name)
                    .ToListAsync()
            );
        }

        // Форма создания
        public IActionResult Create()
        {
            return View();
        }

        // Создание
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Contractor contractor)
        {
            if (!ModelState.IsValid)
                return View(contractor);

            _context.Contractors.Add(contractor);

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // Форма редактирования
        public async Task<IActionResult> Edit(int id)
        {
            var contractor = await _context.Contractors.FindAsync(id);

            if (contractor == null)
                return NotFound();

            return View(contractor);
        }

        // Обновление
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Contractor contractor)
        {
            if (!ModelState.IsValid)
                return View(contractor);

            _context.Contractors.Update(contractor);

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // Подтверждение удаления
        public async Task<IActionResult> Delete(int id)
        {
            var contractor = await _context.Contractors.FindAsync(id);

            if (contractor == null)
                return NotFound();

            return View(contractor);
        }

        // Удаление
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var contractor = await _context.Contractors.FindAsync(id);

            if (contractor != null)
            {
                _context.Contractors.Remove(contractor);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}