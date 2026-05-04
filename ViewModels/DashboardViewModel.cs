using ASSC.Models;

namespace ASSC.ViewModels
{
    public class DashboardViewModel
    {
        public decimal TotalDebt { get; set; }

        public int OverdueInvoicesCount { get; set; }

        public int ContractsCount { get; set; }

        public int SuppliersCount { get; set; }

        public List<Payment> RecentPayments { get; set; }
            = new();
        
        public List<string> Months { get; set; } = new();

        public List<decimal> PaymentsData { get; set; } = new();
        
        public List<decimal> DebtData { get; set; } = new();
    }
}