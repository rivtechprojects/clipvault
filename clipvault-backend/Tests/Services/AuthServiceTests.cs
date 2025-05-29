using Microsoft.AspNetCore.Identity;
using Moq;
using Xunit;
using ClipVault.Services;
using ClipVault.Interfaces;
using ClipVault.Models;
using ClipVault.Exceptions;
using ClipVault.Tests.Mocks;
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
            var existingUser = MockDataFactory.CreateUser();
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

            var newUser = MockDataFactory.CreateUser();
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
            var user = MockDataFactory.CreateUser();
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
            var user = MockDataFactory.CreateUser();
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
            var user = MockDataFactory.CreateUser();
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
        public async Task LoginUserAsync_ShouldReturnJwtToken_WhenCredentialsAreValid()
        {
            // Arrange
            var user = MockDataFactory.CreateUser();
            var mockUserSet = DbSetMockHelper.CreateMockDbSet(new List<User> { user }.AsQueryable());
            _mockDbContext.Setup(db => db.Users).Returns(mockUserSet.Object);

            _mockPasswordHasher.Setup(ph => ph.VerifyHashedPassword(user, user.PasswordHash, "password123"))
                .Returns(PasswordVerificationResult.Success);

            // Mock Jwt configuration
            _mockConfiguration.Setup(config => config.GetSection("Jwt")["Key"]).Returns("c3VwZXJzZWNyZXRrZXkrZXkyNTZiaXRzMTIzNDU2Nzg5MA==");
            _mockConfiguration.Setup(config => config.GetSection("Jwt")["Issuer"]).Returns("testIssuer");
            _mockConfiguration.Setup(config => config.GetSection("Jwt")["Audience"]).Returns("testAudience");
            _mockConfiguration.Setup(config => config.GetSection("Jwt")["TokenExpirationMinutes"]).Returns("60");

            // Act
            var token = await _authService.LoginUserAsync(user.UserName, "password123");

            // Assert
            Assert.NotNull(token);
        }

        [Fact]
        public async Task LoginUserWithRefreshTokenAsync_ShouldReturnTokens_WhenCredentialsAreValid()
        {
            // Arrange
            var user = MockDataFactory.CreateUser();
            var mockUserSet = DbSetMockHelper.CreateMockDbSet(new List<User> { user }.AsQueryable());
            _mockDbContext.Setup(db => db.Users).Returns(mockUserSet.Object);

            _mockPasswordHasher.Setup(ph => ph.VerifyHashedPassword(user, user.PasswordHash, "password123"))
                .Returns(PasswordVerificationResult.Success);

            // Mock Jwt configuration
            _mockConfiguration.Setup(config => config.GetSection("Jwt")["Key"]).Returns("c3VwZXJzZWNyZXRrZXkrZXkyNTZiaXRzMTIzNDU2Nzg5MA==");
            _mockConfiguration.Setup(config => config.GetSection("Jwt")["Issuer"]).Returns("testIssuer");
            _mockConfiguration.Setup(config => config.GetSection("Jwt")["Audience"]).Returns("testAudience");
            _mockConfiguration.Setup(config => config.GetSection("Jwt")["TokenExpirationMinutes"]).Returns("60");

            // Act
            var (accessToken, refreshToken) = await _authService.LoginUserWithRefreshTokenAsync(user.UserName, "password123");

            // Assert
            Assert.NotNull(accessToken);
            Assert.NotNull(refreshToken);
        }

        [Fact]
        public async Task GetUserByRefreshTokenAsync_ShouldReturnUser_WhenTokenIsValid()
        {
            // Arrange
            var user = MockDataFactory.CreateUser();
            var hashedToken = "hashedToken";
            user.RefreshToken = hashedToken;

            var mockUserSet = DbSetMockHelper.CreateMockDbSet(new List<User> { user }.AsQueryable());
            _mockDbContext.Setup(db => db.Users).Returns(mockUserSet.Object);

            _mockHashingService.Setup(hs => hs.Hash("validToken")).Returns(hashedToken);

            // Act
            var result = await _authService.GetUserByRefreshTokenAsync("validToken");

            // Assert
            Assert.NotNull(result);
            Assert.Equal(user, result);
        }

        [Fact]
        public async Task UpdateRefreshTokenAsync_ShouldUpdateUserRefreshToken()
        {
            // Arrange
            var user = MockDataFactory.CreateUser();
            var refreshToken = "newRefreshToken";

            // Mock hashing service
            _mockHashingService.Setup(hs => hs.Hash(refreshToken)).Returns("hashedRefreshToken");

            // Mock DbContext save operation
            _mockDbContext.Setup(db => db.Users.Update(user));
            _mockDbContext.Setup(db => db.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            // Act
            await _authService.UpdateRefreshTokenAsync(user, refreshToken);

            // Assert
            Assert.Equal("hashedRefreshToken", user.RefreshToken);
            Assert.NotNull(user.RefreshTokenExpiry);
        }

        [Fact]
        public void GenerateJwtToken_ShouldReturnToken_WhenUserIsValid()
        {
            // Arrange
            var user = MockDataFactory.CreateUser();
            _mockConfiguration.Setup(config => config.GetSection("Jwt")["Key"]).Returns("c3VwZXJzZWNyZXRrZXkyNTZiaXRzMTIzNDU2Nzg5MTIzNDU2Nzg5MA==");
            _mockConfiguration.Setup(config => config.GetSection("Jwt")["Issuer"]).Returns("testIssuer");
            _mockConfiguration.Setup(config => config.GetSection("Jwt")["Audience"]).Returns("testAudience");
            _mockConfiguration.Setup(config => config.GetSection("Jwt")["TokenExpirationMinutes"]).Returns("60");

            // Act
            var token = _authService.GenerateJwtToken(user);

            // Assert
            Assert.NotNull(token);
        }

        [Fact]
        public void GenerateRefreshToken_ShouldReturnNonEmptyString()
        {
            // Act
            var refreshToken = _authService.GenerateRefreshToken();

            // Assert
            Assert.False(string.IsNullOrEmpty(refreshToken));
            Assert.True(refreshToken.Length > 32, "Refresh token should be longer than 32 characters.");
            Assert.DoesNotContain(" ", refreshToken);
        }
    }
}
