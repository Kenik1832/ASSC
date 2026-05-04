using Microsoft.EntityFrameworkCore;
using ASSC.Data;

namespace ASSC.Services
{
    public class FinanceService
    {
        private readonly ApplicationDbContext _context;

        public FinanceService(
            ApplicationDbContext context)
        {
            _context = context;
        }

        // расчет долга
        public async Task<decimal> GetDebtAsync(
            int invoiceId)
        {
            var invoice = await _context.Invoices
                .Include(i => i.Payments)
                .FirstOrDefaultAsync(
                    x => x.Id == invoiceId);

            if (invoice == null)
                return 0;

            var paid =
                invoice.Payments.Sum(
                    p => p.Amount);

            return invoice.Amount - paid;
        }

        // статус счета
        public async Task<string> GetStatusAsync(
            int invoiceId)
        {
            var invoice = await _context.Invoices
                .Include(i => i.Payments)
                .FirstOrDefaultAsync(
                    x => x.Id == invoiceId);

            if (invoice == null)
                return "Unknown";

            decimal paid =
                invoice.Payments.Sum(
                    p => p.Amount);

            decimal debt =
                invoice.Amount - paid;

            if (debt <= 0)
                return "Paid";

            if (invoice.DueDate < DateTime.Now
                && debt > 0)
                return "Overdue";

            if (paid > 0)
                return "Partial";

            return "Unpaid";
        }
    }
}