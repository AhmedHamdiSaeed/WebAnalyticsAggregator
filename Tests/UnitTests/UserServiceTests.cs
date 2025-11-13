using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Implementations;
using Infrastructure.Entities;
using Infrastructure.interfaces;
using Moq;

namespace Tests.UnitTests
{
    public class UserServiceTests
    {
        private readonly Mock<IUserRepository> _userRepoMock;
        private readonly Mock<IJwtProvider> _jwtProviderMock;
        private readonly UserService _userService;

        public UserServiceTests()
        {
            _userRepoMock = new Mock<IUserRepository>();
            _jwtProviderMock = new Mock<IJwtProvider>();
            _userService = new UserService(_userRepoMock.Object, _jwtProviderMock.Object);
        }

        [Fact]
        public async Task RegisterAsync_ShouldReturnFailure_WhenEmailExists()
        {
            _userRepoMock.Setup(x => x.ExistsByEmailAsync("test@test.com"))
                         .ReturnsAsync(true);

            var result = await _userService.RegisterAsync("John", "test@test.com", "Pass123");

            Assert.False(result.IsSuccess);
            Assert.Equal("EMAIL_EXISTS", result.Code);
        }

        [Fact]
        public async Task RegisterAsync_ShouldReturnSuccess_WhenNewUser()
        {
            _userRepoMock.Setup(x => x.ExistsByEmailAsync("new@test.com"))
                         .ReturnsAsync(false);

            _userRepoMock.Setup(x => x.AddAsync(It.IsAny<User>()))
                         .Returns(Task.CompletedTask);

            _jwtProviderMock.Setup(x => x.GenerateToken(It.IsAny<User>()))
                            .Returns("fake-jwt-token");

            var result = await _userService.RegisterAsync("John", "new@test.com", "Pass123");

            Assert.True(result.IsSuccess);
            Assert.Equal("USER_REGISTERED", result.Code);
            Assert.Equal("fake-jwt-token", result.Data.Token);
        }

        [Fact]
        public async Task LoginAsync_ShouldReturnFailure_WhenInvalidEmailOrPassword()
        {
            _userRepoMock.Setup(x => x.GetByEmailAsync("unknown@test.com"))
                         .ReturnsAsync((User)null);

            var result = await _userService.LoginAsync("unknown@test.com", "Pass123");

            Assert.False(result.IsSuccess);
            Assert.Equal("INVALID_CREDENTIALS", result.Code);
        }

        [Fact]
        public async Task LoginAsync_ShouldReturnSuccess_WhenValidCredentials()
        {
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword("Pass123");
            var user = new User { Name = "John", Email = "john@test.com", PasswordHash = hashedPassword };

            _userRepoMock.Setup(x => x.GetByEmailAsync("john@test.com"))
                         .ReturnsAsync(user);

            _jwtProviderMock.Setup(x => x.GenerateToken(user))
                            .Returns("jwt-token");

            var result = await _userService.LoginAsync("john@test.com", "Pass123");

            Assert.True(result.IsSuccess);
            Assert.Equal("LOGIN_SUCCESS", result.Code);
            Assert.Equal("jwt-token", result.Data.Token);
        }
    }
}
