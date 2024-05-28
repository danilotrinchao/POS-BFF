using AuthenticationService.Core.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthenticationService.Core.Domain.Requests
{
    public class ProductDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public EProductType ProductType { get; set; }

        public VirtualProductDTO? VirtualProduct { get; set; }
        public PhysiqueProductDTO? PhysiqueProduct { get; set; }
    }
}
