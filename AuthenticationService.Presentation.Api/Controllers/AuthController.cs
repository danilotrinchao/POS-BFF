using AuthenticationService.Application.Contracts;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace AuthenticationService.Presentation.Api.Controllers
{
    public class AuthController: ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IUserService _userService;

        public AuthController(IAuthService authService, IUserService userService)
        {
            _authService = authService;
            _userService = userService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(string email, string password)
        {
            // Chama o serviço de autenticação para fazer login
            var result = await _authService.LoginAsync(email, password);

            if (result.IsNullOrEmpty())
            {
                return Ok(result); // Retorna o token JWT ou outro dado relevante
            }
            else
            {
                return Unauthorized(result); // Retorna 401 Unauthorized se a autenticação falhar
            }
        }

    }
}
