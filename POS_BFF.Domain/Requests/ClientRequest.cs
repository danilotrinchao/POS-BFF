using POS_BFF.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS_BFF.Core.Domain.Requets
{
    public class ClientRequest
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Surename { get; set; }

        public DateTime DtBirth { get; set; }
        public string Email { get; set; }
        public string CPF { get; set; }
        public string Phone { get; set; }
        public Address Address { get; set; }
        public bool Inative { get; set; }
    }
}
