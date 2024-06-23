using AuthenticationService.Application.Contracts;
using AuthenticationService.Application.DTOs;
using AuthenticationService.Core.Domain.Repositories;
using AuthenticationService.Domain.Entities;
using AuthenticationService.Domain.Repositories;

namespace AuthenticationService.Application.Services
{
    public class UserService: IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IAddressRepository _addressRepository;
        private readonly ICryptography _criptography;
        private readonly IUserRoleRepository _userRoleRepository;

        public UserService(IUserRepository userRepository, IAddressRepository addressRepository, ICryptography criptography, IUserRoleRepository userRoleRepository)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _addressRepository = addressRepository;
            _criptography = criptography;
            _userRoleRepository = userRoleRepository;
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
        public async Task<User> GetUserByCPF(string cpf)
        {
            if (cpf.Length <= 0)
                throw new ArgumentException("Invalid user id", nameof(cpf));

            return await _userRepository.GetByCPF(cpf);
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
                PasswordHash = _criptography.CryptographyPassword(userDto.Password),
                RoleIds = userDto.RoleIds,
            };
            
            
            // Primeiro, insira o endereço no banco de dados
            int addressId = await _addressRepository.InsertAsync(user.Address);

            // Em seguida, associe o ID do endereço ao usuário
            user.Address.Id = addressId;

            // Finalmente, insira o usuário no banco de dados
            var result = await _userRepository.InsertAsync(user);
            foreach (var item in user.RoleIds)
            {
                await _userRoleRepository.InsertAsync(result, item);
            }
            return result;
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
