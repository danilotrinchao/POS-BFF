using POS_BFF.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS_BFF.Domain.Repositories
{
    public interface ISessionRepository
    {
        Task<Session> CreateSessionAsync(User user);
        Task<Session> GetSessionAsync(string sessionId);
        Task<bool> EndSessionAsync(string sessionId);
        Task<bool> EndAllSessionsAsync(int userId);
    }
}
