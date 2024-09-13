using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS_BFF.Core.Domain.Entities
{
    public class ConsumerService
    {
        public Guid id { get; set; }
        public int  userId { get; set; }
        public Guid orderId { get; set; }
        public string serviceName { get; set; }
        public Guid serviceId { get; set; }
        public bool is_Active { get; set; }
        public int totalTime { get; set; }
        public DateTime StartTime { get; set; }
    }
}
