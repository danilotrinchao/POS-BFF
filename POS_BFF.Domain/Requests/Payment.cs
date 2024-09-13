using POS_BFF.Core.Domain.Enums;

namespace POS_BFF.Core.Domain.Requests
{
    public class Payment
    {
        public int Id { get; set; }
        public decimal Value { get; set; }
        public string PaymentMethod { get; set; }
        public Guid OrderId { get; set; }
        public EPaymentType PaymentType { get; set; }
    }
}
