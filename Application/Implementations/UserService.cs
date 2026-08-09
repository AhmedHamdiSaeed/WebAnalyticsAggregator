
using Application.Interfaces;
using DTOs.Auth;
using DTOs;
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

        public async Task<Result<AuthResponseDto>> RegisterAsync(string name, string email, string password)
        {
            try
            {
                // Check if the email already exists
                if (await _userRepository.ExistsByEmailAsync(email))
                {
                    return Result<AuthResponseDto>.Failure(
                        errorMessage: "Email is already registered.",
                        code: "EMAIL_EXISTS"
                    );
                }

                // Create new user
                var user = new User
                {
                    Name = name,
                    Email = email,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
                    CreatedAt = DateTime.UtcNow
                };

                await _userRepository.AddAsync(user);

                // Generate JWT
                var token = _jwtProvider.GenerateToken(user);

                // Create response DTO
                var response = new AuthResponseDto
                {
                    Name = user.Name,
                    Email = user.Email,
                    Token = token
                };

                // Return success result
                return Result<AuthResponseDto>.Success(
                    data: response,
                    code: "USER_REGISTERED"
                );
            }
            catch (Exception) {
                // Return a failure result
                return Result<AuthResponseDto>.Failure(
                    errorMessage: "An error occurred while registering the user.",
                    code: "USER_REGISTRATION_FAILED"
                );
            } 
        }

        public async Task<Result<AuthResponseDto>> LoginAsync(string email, string password)
        {
            try
            {
                // Get the user by email
                var user = await _userRepository.GetByEmailAsync(email);

                // Check if user exists and password is correct
                if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
                {
                    return Result<AuthResponseDto>.Failure(
                        errorMessage: "Invalid email or password.",
                        code: "INVALID_CREDENTIALS"
                    );
                }

                // Generate JWT token
                var token = _jwtProvider.GenerateToken(user);

                // Create response DTO
                var response = new AuthResponseDto
                {
                    Name = user.Name,
                    Email = user.Email,
                    Token = token
                };

                // Return success result
                return Result<AuthResponseDto>.Success(
                    data: response,
                    code: "LOGIN_SUCCESS"
                );
            }
            catch (Exception) {
                // Return a standardized failure response
                return Result<AuthResponseDto>.Failure(
                    errorMessage: "Invalid credentials or an error occurred during login.",
                    code: "USER_LOGIN_FAILED"
                );
            }
        }
    }
}

