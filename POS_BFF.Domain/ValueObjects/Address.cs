using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS_BFF.Domain.ValueObjects
{
    public class Address
    {
        public int Id { get; set; }
        public string ZipCode { get; set; }
        public string CityName { get; set; }
        public string State { get; set; }
        public string Road { get; set; }
        public int Number { get; set; }
    }
}
