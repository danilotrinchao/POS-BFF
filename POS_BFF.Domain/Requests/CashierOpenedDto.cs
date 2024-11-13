using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS_BFF.Core.Domain.Requests
{
    public class CashierOpenedDto
    {
        public Guid EmployeerId { get; set; }
        public decimal InitialBalance { get; set; }
    }
}
