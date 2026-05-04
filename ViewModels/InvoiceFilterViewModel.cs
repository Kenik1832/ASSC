namespace ASSC.ViewModels
{
    public class InvoiceFilterViewModel
    {
        public string Search { get; set; } = null!;

        public DateTime? IssueDateFrom { get; set; }
        public DateTime? IssueDateTo { get; set; }

        public DateTime? DueDateFrom { get; set; }
        public DateTime? DueDateTo { get; set; }

        public decimal? MinAmount { get; set; }
        public decimal? MaxAmount { get; set; }

        public string Status { get; set; } = null!;
    }
}