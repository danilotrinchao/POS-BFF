using AuthenticationService.Application.Contracts;
using AuthenticationService.Application.DTOs;
using AuthenticationService.Domain.Entities;
using AuthenticationService.Domain.Repositories;
using AuthenticationService.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace AuthenticationService.Application.Services
{
    public class UserService: IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IAddressRepository _addressRepository;
        private readonly ICryptography _criptography;

        public UserService(IUserRepository userRepository, IAddressRepository addressRepository, ICryptography criptography)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _addressRepository = addressRepository;
            _criptography = criptography;
        }

        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            return await _userRepository.GetAllAsync();
        }

        public async Task<User> GetUserByIdAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("Invalid user id", nameof(id));

            return await _userRepository.GetByIdAsync(id);
        }

        public async Task<int> CreateUserAsync(UserDto userDto)
        {
            
            if (userDto == null)
                throw new ArgumentNullException(nameof(userDto));

            var user = new User
            {
                Nome = userDto.Nome,
                Sobrenome = userDto.Sobrenome,
                Email = userDto.Email,
                Phone = userDto.Phone,
                CPF = userDto.CPF,
                Address = userDto.Address,
                DtNascimento = userDto.DtNascimento,
                Inative = true,
                UserType = userDto.UserType,
                PasswordHash = _criptography.CriptografarSenha(userDto.Password)
            };

            
            // Primeiro, insira o endereço no banco de dados
            int addressId = await _addressRepository.InsertAsync(user.Address);

            // Em seguida, associe o ID do endereço ao usuário
            user.Address.Id = addressId;

            // Finalmente, insira o usuário no banco de dados
            return await _userRepository.InsertAsync(user);
        }

        public async Task<bool> UpdateUserAsync(User user)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            return await _userRepository.UpdateAsync(user);
        }

        public async Task<bool> DeleteUserAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("Invalid user id", nameof(id));

            return await _userRepository.DeleteAsync(id);
        }

    }
}
