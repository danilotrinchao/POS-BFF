﻿using POS_BFF.Application.Contracts;
using POS_BFF.Application.DTOs;
using POS_BFF.Core.Domain.Repositories;
using POS_BFF.Domain.Entities;
using POS_BFF.Domain.Repositories;

namespace POS_BFF.Application.Services
{
    public class UserService: IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IAddressRepository _addressRepository;
        private readonly ICryptography _criptography;
        private readonly IUserRoleRepository _userRoleRepository;
        private readonly IRoleRepository _roleRepository;
        public UserService(IUserRepository userRepository, IAddressRepository addressRepository, ICryptography criptography, IUserRoleRepository userRoleRepository, IRoleRepository roleRepository)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _addressRepository = addressRepository;
            _criptography = criptography;
            _userRoleRepository = userRoleRepository;
            _roleRepository = roleRepository;
        }

        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            return await _userRepository.GetAllAsync();
        }

        public async Task<User> GetUserByIdAsync(Guid id)
        {
            if (Equals(id))
                throw new ArgumentException("Invalid user id", nameof(id));

            return await _userRepository.GetByIdAsync(id);
        }
        public async Task<User> GetUserByCPF(string cpf)
        {
            if (cpf.Length <= 0)
                throw new ArgumentException("Invalid user id", nameof(cpf));

            return await _userRepository.GetByCPF(cpf);
        }

        public async Task<Guid> CreateUserAsync(UserDto userDto)
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
                Inative = false,
                UserType = userDto.UserType,
                PasswordHash = _criptography.CryptographyPassword(userDto.Password),
                RoleIds = new List<Guid>(),
            };
            user.RoleIds = userDto.RoleIds;
            // Primeiro, insira o endereço no banco de dados
            var addressId = await _addressRepository.InsertAsync(user.Address);

            // Em seguida, associe o ID do endereço ao usuário
            user.Address.Id = addressId;

            // Finalmente, insira o usuário no banco de dados
            var result = await _userRepository.InsertAsync(user);

            if (Equals(result))
            {
                // Cria um usuário na tabela UserClient somente se o usuário foi criado com sucesso
                await _userRepository.CreateUserClientAsync(userDto.CPF, userDto.Password, 0, result);
            }
            var role = await _roleRepository.GetByGroupAsync((int)user.UserType);
            user.RoleIds.Add(role.Id);
            await _userRoleRepository.InsertAsync(result, role.Id);


            return result;
        }
        public async Task<bool> UpdateUserAsync(UserDto userDto)
        {
            if (userDto == null)
                throw new ArgumentNullException(nameof(userDto));

            var user = await _userRepository.GetByIdAsync(userDto.Id);
            user = new User
            {
                Id = user.Id,
                Nome = userDto.Nome,
                Sobrenome = userDto.Sobrenome,
                Email = userDto.Email,
                Phone = userDto.Phone,
                CPF = userDto.CPF,
                Address = userDto.Address,
                DtNascimento = userDto.DtNascimento,
                Inative = false,
                UserType = userDto.UserType,
                PasswordHash = _criptography.CryptographyPassword(userDto.Password),
                RoleIds = userDto.RoleIds,
            };

            var result =  await _userRepository.UpdateAsync(user);
            if (result)
                await _userRepository.UpdateUserClientCredentialsAsync(user.Id, user.CPF, user.PasswordHash);
            return result;
        }

        public async Task<bool> DeleteUserAsync(Guid id)
        {
            if (Equals(id))
                throw new ArgumentException("Invalid user id", nameof(id));

            return await _userRepository.DeleteAsync(id);
        }

    }
}
