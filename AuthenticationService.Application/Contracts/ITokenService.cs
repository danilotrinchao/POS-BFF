using AuthenticationService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthenticationService.Application.Contracts
{
    public interface ITokenService
    {
        string GenerateToken(User user);
    }

}
