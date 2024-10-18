using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS_BFF.Core.Domain.Requests
{
    public class CashierDto
    {
        public Guid Id { get; set; }
        public decimal InitialBalance { get; set; }
        public decimal TotalBalance { get; set; }
        public decimal TotalPix { get; set; }
        public decimal TotalCreditCard { get; set; }
        public decimal TotalDebitCard { get; set; }
        public decimal TotalCash { get; set; }
        public bool IsOpen { get; set; }
        public DateTime DateOpened { get; set; }
        public DateTime DateClosed { get; set; }
        public Guid EmployeerId { get; set; }
    }
}
