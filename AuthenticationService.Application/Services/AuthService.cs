using AuthenticationService.Application.Contracts;
using AuthenticationService.Domain.Repositories;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
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
        public AuthService(IUserRepository userRepository, ITokenService tokenService, IHttpContextAccessor httpContextAccessor, ICryptography criptography)
        {
            _userRepository = userRepository;
            _tokenService = tokenService;
            _httpContextAccessor = httpContextAccessor;
            _criptography = criptography;
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



        

        private bool VerifyPassword(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA256(passwordSalt))
            {
                var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
                return computedHash.SequenceEqual(passwordHash);
            }
        }
    }

}
