using Microsoft.EntityFrameworkCore;
using ASSC.Data;
using ASSC.Models;

namespace ASSC.Services
{
    public class NotificationService
    {
        private readonly ApplicationDbContext _context;

        private readonly FinanceService _finance;

        public NotificationService(
            ApplicationDbContext context,
            FinanceService finance)
        {
            _context = context;
            _finance = finance;
        }

        public async Task GenerateOverdueNotifications()
        {
            var invoices =
                await _context.Invoices
                    .ToListAsync();

            foreach(var invoice in invoices)
            {
                var status =
                    await _finance
                        .GetStatusAsync(invoice.Id);

                if(status=="Overdue")
                {
                    bool exists =
                      await _context.Notifications
                       .AnyAsync(n =>
                          n.Message.Contains(
                             invoice.Id.ToString()
                          ));

                    if(!exists)
                    {
                        _context.Notifications.Add(
                          new Notification
                          {
                              Message =
                                $"Счет #{invoice.Id} просрочен",

                              IsRead=false,

                              DateCreated=
                                DateTime.Now
                          });
                    }
                }
            }

            await _context.SaveChangesAsync();
        }
    }
}