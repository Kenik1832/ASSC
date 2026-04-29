using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using ASSC.Data;
using ASSC.Services;

namespace ASSC.Controllers
{
    public class NotificationsController
        : Controller
    {
        private readonly ApplicationDbContext
            _context;

        private readonly NotificationService
            _notificationService;

        public NotificationsController(
            ApplicationDbContext context,
            NotificationService
                notificationService)
        {
            _context=context;

            _notificationService=
                notificationService;
        }

        public async Task<IActionResult>
            Index()
        {
            await _notificationService
                .GenerateOverdueNotifications();

            var items =
                await _context.Notifications
                  .OrderByDescending(
                      x=>x.DateCreated)
                  .ToListAsync();

            return View(items);
        }
    }
}