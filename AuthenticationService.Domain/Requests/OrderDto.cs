using AuthenticationService.Core.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthenticationService.Core.Domain.Requests
{
    public class OrderDto
    {
        public Guid Id { get; set; }
        public Guid SaleId { get; set; }
        public int UserId { get; set; }
        public int ClientId { get; set; }
        public EOrderStatus OrderStatus { get; set; }
        public DateTime Created { get; set; } = DateTime.Now;
        public DateTime? DtEnd { get; set; } // Nullable end date
        public decimal TotalAmount { get; set; } // Total amount for the order
        public string PaymentMethod { get; set; } // Payment method used
    }
}
