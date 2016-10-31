namespace Bitretsmah.Core.Models
{
    public class Quota
    {
        public Quota()
        {
        }

        public Quota(decimal total, decimal used)
        {
            Total = total;
            Used = used;
        }

        public decimal Total { get; set; }
        public decimal Used { get; set; }
    }
}