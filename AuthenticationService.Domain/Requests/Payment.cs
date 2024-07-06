using AuthenticationService.Core.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthenticationService.Core.Domain.Requests
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
