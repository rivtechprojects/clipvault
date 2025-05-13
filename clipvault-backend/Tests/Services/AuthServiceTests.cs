using Microsoft.AspNetCore.Identity;
using Moq;
using Xunit;
using ClipVault.Services;
using ClipVault.Interfaces;
using ClipVault.Models;
using ClipVault.Exceptions;
using ClipVault.Tests.Mocks;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using ClipVault.Utils;

namespace ClipVault.Tests
{
    public class AuthServiceTests
    {
        private readonly Mock<IAppDbContext> _mockDbContext;
        private readonly Mock<IConfiguration> _mockConfiguration;
        private readonly Mock<IPasswordHasher<User>> _mockPasswordHasher;
        private readonly Mock<IHashingService> _mockHashingService;
        private readonly IAuthService _authService;

        public AuthServiceTests()
        {
            _mockDbContext = new Mock<IAppDbContext>();
            _mockConfiguration = new Mock<IConfiguration>();
            _mockPasswordHasher = new Mock<IPasswordHasher<User>>();
            _mockHashingService = new Mock<IHashingService>();

            _authService = new AuthService(
                _mockDbContext.Object,
                _mockConfiguration.Object,
                _mockPasswordHasher.Object,
                _mockHashingService.Object
            );
        }

        [Fact]
        public async Task RegisterUserAsync_ShouldThrowException_WhenUsernameOrEmailExists()
        {
            // Arrange
            var existingUser = TestDataHelper.CreateUser();
            var mockUserSet = DbSetMockHelper.CreateMockDbSet(new List<User> { existingUser }.AsQueryable());
            _mockDbContext.Setup(db => db.Users).Returns(mockUserSet.Object);

            // Act & Assert
            await Assert.ThrowsAsync<UserAlreadyExistsException>(() => _authService.RegisterUserAsync(existingUser.UserName, existingUser.Email, "password123"));
        }

        [Fact]
        public async Task RegisterUserAsync_ShouldCreateUser_WhenUsernameAndEmailAreUnique()
        {
            // Arrange
            var mockUserSet = DbSetMockHelper.CreateMockDbSet(new List<User>().AsQueryable());
            _mockDbContext.Setup(db => db.Users).Returns(mockUserSet.Object);

            var newUser = TestDataHelper.CreateUser();
            _mockPasswordHasher.Setup(ph => ph.HashPassword(It.IsAny<User>(), It.IsAny<string>())).Returns("hashedpassword");

            _mockDbContext.Setup(db => db.Users.Add(It.IsAny<User>()));
            _mockDbContext.Setup(db => db.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            // Act
            var result = await _authService.RegisterUserAsync(newUser.UserName, newUser.Email, "password123");

            // Assert
            AssertionHelper.AssertUser(result, newUser);
        }

        [Fact]
        public async Task LogoutUserAsync_ShouldRevokeRefreshToken()
        {
            // Arrange
            var user = TestDataHelper.CreateUser();
            _mockDbContext.Setup(db => db.Users.Update(user));
            _mockDbContext.Setup(db => db.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            // Act
            await _authService.LogoutUserAsync(user);

            // Assert
            Assert.Null(user.RefreshToken);
            Assert.Null(user.RefreshTokenExpiry);
        }

        [Fact]
        public async Task RevokeRefreshTokenAsync_ShouldClearRefreshTokenFields()
        {
            // Arrange
            var user = TestDataHelper.CreateUser();
            _mockDbContext.Setup(db => db.Users.Update(user));
            _mockDbContext.Setup(db => db.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            // Act
            await _authService.RevokeRefreshTokenAsync(user);

            // Assert
            Assert.Null(user.RefreshToken);
            Assert.Null(user.RefreshTokenExpiry);
        }

        [Fact]
        public async Task ChangePasswordAsync_ShouldUpdatePasswordAndRevokeRefreshToken()
        {
            // Arrange
            var user = TestDataHelper.CreateUser();
            var newPassword = "newpassword123";
            _mockPasswordHasher.Setup(ph => ph.HashPassword(user, newPassword)).Returns("newhashedpassword");
            _mockDbContext.Setup(db => db.Users.Update(user));
            _mockDbContext.Setup(db => db.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            // Act
            await _authService.ChangePasswordAsync(user, newPassword);

            // Assert
            Assert.Equal("newhashedpassword", user.PasswordHash);
            Assert.Null(user.RefreshToken);
            Assert.Null(user.RefreshTokenExpiry);
        }

        [Fact]
        public async Task GetUserByRefreshTokenAsync_ShouldReturnUser_WhenTokenIsValid()
        {
            // Arrange
            var refreshToken = "validRefreshToken";
            var hashedToken = "hashedRefreshToken";
            var user = TestDataHelper.CreateUser();

            _mockDbContext.Setup(db => db.Users.FirstOrDefaultAsync(It.IsAny<Expression<Func<User, bool>>>()!, default))
                .ReturnsAsync(user);

            var mockHashingService = new Mock<IHashingService>();
            mockHashingService.Setup(h => h.Hash(refreshToken)).Returns(hashedToken);

            // Act
            var result = await _authService.GetUserByRefreshTokenAsync(refreshToken);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(user, result);
        }

        [Fact]
        public async Task UpdateRefreshTokenAsync_ShouldUpdateUserWithNewToken()
        {
            // Arrange
            var user = TestDataHelper.CreateUser();
            var refreshToken = "newRefreshToken";
            var hashedToken = "hashedRefreshToken";

            var mockHashingService = new Mock<IHashingService>();
            mockHashingService.Setup(h => h.Hash(refreshToken)).Returns(hashedToken);

            _mockDbContext.Setup(db => db.Users.Update(user));
            _mockDbContext.Setup(db => db.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            // Act
            await _authService.UpdateRefreshTokenAsync(user, refreshToken);

            // Assert
            Assert.Equal(hashedToken, user.RefreshToken);
            Assert.NotNull(user.RefreshTokenExpiry);
        }
    }
}
