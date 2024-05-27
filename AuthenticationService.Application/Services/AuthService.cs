using AuthenticationService.Application.Contracts;
using AuthenticationService.Core.Domain.Configurations;
using AuthenticationService.Core.Domain.Repositories;
using AuthenticationService.Domain.Entities;
using AuthenticationService.Domain.Repositories;
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

namespace AuthenticationService.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly ITokenService _tokenService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ICryptography _criptography;
        private readonly JwtSettings _jwtSettings;
        private readonly IUserRoleRepository _userRoleRepository;
        public AuthService(IUserRepository userRepository, ITokenService tokenService, IHttpContextAccessor httpContextAccessor, ICryptography criptography, JwtSettings jwtSettings, IUserRoleRepository userRoleRepository)
        {
            _userRepository = userRepository;
            _tokenService = tokenService;
            _httpContextAccessor = httpContextAccessor;
            _criptography = criptography;
            _jwtSettings = jwtSettings;
            _userRoleRepository = userRoleRepository;
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

        public async Task<string> LoginAsync(string email, string password)
        {
            // Buscar o usuário pelo e-mail
            var user = await _userRepository.GetByEmail(email);
            var roles = _userRoleRepository.GetByUserIdAsync(user.Id);
            if (roles != null)
            {
                foreach (var item in roles.Result)
                {
                    user.RoleIds.Add(item.RoleId);
                }
            }
            // Verificar se o usuário existe
            if (user == null)
            {
                throw new Exception("Usuário não encontrado");
            }

            // Descriptografar a senha armazenada no banco de dados
            var decryptedPassword = _criptography.VerificarSenha(password, user.PasswordHash);

            // Comparar a senha criptografada do usuário com a senha fornecida
            if (decryptedPassword)
            {
                throw new Exception("Senha incorreta");
            }

            // Gerar token de autenticação para o usuário
            var token = _tokenService.GenerateToken(user);

            return token;
        }
        public async Task<string> Login(string email, string password)
        {
            var token = Authenticate(email, password);
            if (string.IsNullOrEmpty(token))
            {
                // Log failed login attempt
                return null;
            }
            return token;
        }

        public string Authenticate(string email, string password)
        {
            // Validar usuário e senha
            var user = ValidateUser(email, password);
            if (user == null) return null;

            var roles = _userRoleRepository.GetByUserIdAsync(user.Id);
            if (roles != null)
            {
                user.RoleIds = new List<Guid>();
                foreach (var item in roles.Result)
                {
                    user.RoleIds.Add(item.RoleId);
                }
            }
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email)
            };

            // Adicionar reivindicações de função para cada ID de função do usuário
            foreach (var roleId in user.RoleIds)
            {
                claims.Add(new Claim(ClaimTypes.Role, roleId.ToString()));
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
