using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthenticationService.Core.Domain.Enums
{
    public enum EOrderStatus
    {
        Pending,
        Processing,
        Completed,
        Canceled,
        Failed
    }
}
