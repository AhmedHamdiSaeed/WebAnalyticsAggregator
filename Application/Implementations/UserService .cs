using Application.DTOs.Auth;
using Application.Interfaces;
using Infrastructure.Entities;
using Infrastructure.interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Implementations
{
    public class UserService : IUserService
    {

        private readonly IUserRepository _userRepository;
        private readonly IJwtProvider _jwtProvider;

        public UserService(IUserRepository userRepository, IJwtProvider jwtProvider)
        {
            _userRepository = userRepository;
            _jwtProvider = jwtProvider;
        }

        public async Task<AuthResponseDto?> RegisterAsync(string name, string email, string password)
        {
            if (await _userRepository.ExistsByEmailAsync(email))
                return null;

            var user = new User
            {
                Name = name,
                Email = email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
                CreatedAt = DateTime.UtcNow
            };

            await _userRepository.AddAsync(user);

            var token = _jwtProvider.GenerateToken(user);

            return new AuthResponseDto
            {
                Name = user.Name,
                Email = user.Email,
                Token = token
            };
        }

        public async Task<AuthResponseDto?> LoginAsync(string email, string password)
        {
            var user = await _userRepository.GetByEmailAsync(email);
            if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
                return null;

            var token = _jwtProvider.GenerateToken(user);

            return new AuthResponseDto
            {
                Name = user.Name,
                Email = user.Email,
                Token = token
            };
        }
    }
}

