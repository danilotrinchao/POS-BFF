using POS_BFF.Application.Contracts;
using POS_BFF.Core.Domain.Configurations;
using POS_BFF.Core.Domain.Repositories;
using POS_BFF.Domain.Entities;
using POS_BFF.Domain.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace POS_BFF.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly ITokenService _tokenService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ICryptography _criptography;
        private readonly JwtSettings _jwtSettings;
        private readonly IUserRoleRepository _userRoleRepository;
        private readonly IRoleRepository _roleRepository;
        public AuthService(IUserRepository userRepository, 
            ITokenService tokenService, 
            IHttpContextAccessor httpContextAccessor, 
            ICryptography criptography, 
            JwtSettings jwtSettings, 
            IUserRoleRepository userRoleRepository,
            IRoleRepository roleRepository)
        {
            _userRepository = userRepository;
            _tokenService = tokenService;
            _httpContextAccessor = httpContextAccessor;
            _criptography = criptography;
            _jwtSettings = jwtSettings;
            _userRoleRepository = userRoleRepository;
            _roleRepository = roleRepository;
        }

        public async Task<string> RefreshTokenAsync()
        {
            // Verifique se o usuário atual está autenticado
            if (_httpContextAccessor.HttpContext.User.Identity.IsAuthenticated)
            {
                // Obtenha o identificador do usuário atual
                var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                // Obtenha o usuário do repositório
                var user = await _userRepository.GetByIdAsync(int.Parse(userId));

                // Verifique se o usuário foi encontrado
                if (user != null)
                {
                    // Gere um novo token com base no usuário e retorne-o
                    return _tokenService.GenerateToken(user);
                }
            }

            // Caso contrário, retorne nulo indicando que o token não pode ser atualizado
            return null;
        }

       
        public async Task<string> Login(string email, string password)
        {
            var token = Authenticate(email, password);
            if (string.IsNullOrEmpty(token))
            {
                // Log failed login attempt
                return null;
            }
            _httpContextAccessor.HttpContext.Response.Cookies.Append("accessToken", token, new CookieOptions
            {
                HttpOnly = true, // Para proteger contra ataques XSS
                Secure = true, // Para enviar o cookie apenas em conexões HTTPS
                SameSite = SameSiteMode.Strict // Para restringir o envio do cookie em solicitações de origens diferentes
            });
            return token;
        }
        public string Authenticate(string email, string password)
        {
            // Validar usuário e senha
            var user = ValidateUser(email, password);
            if (user == null) return null;

            // Buscar roles do usuário
            var roles = _userRoleRepository.GetByUserIdAsync(user.Id).Result;
            var roleNames = new List<string>();
            if (roles != null)
            {
                foreach (var item in roles)
                {
                    // Adicione o nome da role à lista de nomes de roles do usuário
                    var roleName = _roleRepository.GetByIdAsync(item.RoleId).Result.Name;
                    roleNames.Add(roleName);
                }
            }

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim("EmployeerId", user.Id.ToString()) // Adiciona o EmployeerId como uma claim
            };

            // Adicionar reivindicações de role para cada nome de role do usuário
            foreach (var roleName in roleNames)
            {
                claims.Add(new Claim("Role", roleName)); // Use "role" ou outro nome de reivindicação que preferir
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.Now.AddMinutes(_jwtSettings.DurationInMinutes);

            var token = new JwtSecurityToken(
                _jwtSettings.Issuer,
                _jwtSettings.Audience,
                claims,
                expires: expires,
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private User ValidateUser(string email, string password)
        {
            var user = _userRepository.GetByEmail(email);
            
            if (BCrypt.Net.BCrypt.Verify(password, user.Result.PasswordHash))
            {
                return user.Result;
            }
            else
            {
                return null; 
            }
        }
    }

}
