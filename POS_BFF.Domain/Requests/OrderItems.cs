using POS_BFF.Core.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS_BFF.Core.Domain.Requests
{
    public class OrderItems
    {
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
        public Guid OrderId { get; set; }
        public EProductType ProductType { get; set; }
        public string Name { get; set; }
    }
}
