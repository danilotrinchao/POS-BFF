﻿using POS_BFF.Core.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS_BFF.Core.Domain.Gateways.Authentication
{
    public interface IAuthenticationTenantGateway
    {
        Task<Tenant> GetConnectionStringByTenantIdAsync(Guid TenantId);
    }
}
