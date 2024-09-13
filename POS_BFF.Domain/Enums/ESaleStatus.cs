using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS_BFF.Core.Domain.Enums
{
    public enum ESaleStatus
    {
        Iniciada,
        AguardandoPagamento,
        EmProcessamento,
        Concluida,
        Cancelada
    }
}
