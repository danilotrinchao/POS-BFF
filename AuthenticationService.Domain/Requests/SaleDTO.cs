using AuthenticationService.Core.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthenticationService.Core.Domain.Requests
{
    public class SaleDTO
    {
        public Guid Id { get; set; }
        public DateTime DtSale { get; set; }
        public List<OrderItems> Produtos { get; set; }
        public int ClientId { get; set; }
        public int EmployeerId { get; set; }
        public decimal PrecoTotal { get; set; }
        public decimal Desconto { get; set; }
        public decimal Credito { get; set; }
        public ESaleStatus SaleStatus { get; set; }
        public ICollection<Payment> Payments { get; set; }
    }
}
