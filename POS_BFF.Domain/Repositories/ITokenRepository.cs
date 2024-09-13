using POS_BFF.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS_BFF.Domain.Repositories
{
    public interface ITokenRepository
    {
        Task<Token> GenerateTokenAsync(User user);
        Task<Token> GetTokenAsync(string tokenValue);
        Task<bool> RevokeTokenAsync(string tokenValue);
        Task<bool> RevokeAllTokensAsync(int userId);
    }
}
