using AuthenticationService.Application.Contracts;
using AuthenticationService.Application.DTOs;
using AuthenticationService.Domain.Entities;
using AuthenticationService.Domain.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace AuthenticationService.Presentation.Api.Controllers
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

        [HttpPost]
        public async Task<int> CreateUserAsync(UserDto user)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            return await _userService.CreateUserAsync(user);
        }

        [HttpPut]
        public async Task<bool> UpdateUserAsync(User user)
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
