using POS_BFF.Application.Contracts;
using POS_BFF.Application.DTOs;
using POS_BFF.Domain.Entities;
using POS_BFF.Domain.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace POS_BFF.Presentation.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
        }

        [HttpGet]
        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            return await _userService.GetAllUsersAsync();
        }

        [HttpGet("{id}")]
        public async Task<User> GetUserByIdAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("Invalid user id", nameof(id));

            return await _userService.GetUserByIdAsync(id);
        }

        [HttpGet("getByCPF")]
        public async Task<User> GetUserByCPF(string cpf)
        {
            if (cpf.Length <= 0)
                throw new ArgumentException("Invalid user id", nameof(cpf));

            return await _userService.GetUserByCPF(cpf);
        }

        [HttpPost]
        public async Task<int> CreateUserAsync(UserDto user)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            return await _userService.CreateUserAsync(user);
        }

        [HttpPut]
        public async Task<bool> UpdateUserAsync(UserDto user)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            return await _userService.UpdateUserAsync(user);
        }

        [HttpDelete("{id}")]
        public async Task<bool> DeleteUserAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("Invalid user id", nameof(id));

            return await _userService.DeleteUserAsync(id);
        }
    }

}
