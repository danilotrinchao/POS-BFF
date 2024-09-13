using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS_BFF.Application.Contracts
{
    public interface IAuthService
    {
        
        Task<string> Login(string email, string password);
        Task<string> RefreshTokenAsync();
    }

}
