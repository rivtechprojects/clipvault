using Microsoft.AspNetCore.Identity;
using Moq;
using Xunit;
using ClipVault.Services;
using ClipVault.Interfaces;
using ClipVault.Models;
using ClipVault.Exceptions;
using ClipVault.Tests.Mocks;

namespace ClipVault.Tests
{
    public class AuthServiceTests
    {
        private readonly Mock<IAppDbContext> _mockDbContext;
        private readonly Mock<IConfiguration> _mockConfiguration;
        private readonly Mock<IPasswordHasher<User>> _mockPasswordHasher;
        private readonly IAuthService _authService;

        public AuthServiceTests()
        {
            _mockDbContext = new Mock<IAppDbContext>();
            _mockConfiguration = new Mock<IConfiguration>();
            _mockPasswordHasher = new Mock<IPasswordHasher<User>>();

            _authService = new AuthService(
                _mockDbContext.Object,
                _mockConfiguration.Object,
                _mockPasswordHasher.Object
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
    }
}
