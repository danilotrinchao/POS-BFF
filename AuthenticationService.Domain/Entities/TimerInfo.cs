using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthenticationService.Core.Domain.Entities
{
    public class TimerInfo
    {
        public Guid TimerId { get; set; }
        public int TotalTime { get; set; } // Tempo total em segundos
        public int ElapsedTime { get; set; } // Tempo já decorrido em segundos
        public int StartTime {  get; set; }
        public bool IsRunning { get; set; } 
    }

}
