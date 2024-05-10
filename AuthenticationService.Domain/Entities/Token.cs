using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthenticationService.Domain.Entities
{
    public class Token
    {
        public Token() { }
        public Token(string token) { }
        public string TokenValue { get; set; }
        public int UserId { get; set; }
        public DateTime ExpitarionDate { get; set; }

    }
}
