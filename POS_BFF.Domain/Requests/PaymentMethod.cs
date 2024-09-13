using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS_BFF.Core.Domain.Requests
{
    public class PaymentMethod
    {
        public int Id { get; set; }
        public string Nome { get; set; }
    }
}
