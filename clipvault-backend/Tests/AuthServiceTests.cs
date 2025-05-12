using Microsoft.AspNetCore.Identity;
using Moq;
using Xunit;
using ClipVault.Services;
using ClipVault.Interfaces;
using ClipVault.Models;
using Microsoft.EntityFrameworkCore;

namespace ClipVault.Tests
{
    public class AuthServiceTests
    {
        private readonly Mock<IAppDbContext> _mockDbContext;
        private readonly Mock<IConfiguration> _mockConfiguration;
        private readonly Mock<IPasswordHasher<User>> _mockPasswordHasher;
        private readonly AuthService _authService;

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
            var mockUserSet = new Mock<DbSet<User>>();
            mockUserSet.As<IQueryable<User>>().Setup(m => m.Provider).Returns(new TestAsyncQueryProvider<User>(new List<User>
        {
            new User
            {
                UserName = "testuser",
                Email = "test@example.com",
                PasswordHash = "hashedpassword123" // Set the required property
            }
        }.AsQueryable().Provider));
                mockUserSet.As<IQueryable<User>>().Setup(m => m.Expression).Returns(new List<User>
        {
            new User
            {
                UserName = "testuser",
                Email = "test@example.com",
                PasswordHash = "hashedpassword123" // Set the required property
            }
        }.AsQueryable().Expression);
                mockUserSet.As<IQueryable<User>>().Setup(m => m.ElementType).Returns(new List<User>
        {
            new User
            {
                UserName = "testuser",
                Email = "test@example.com",
                PasswordHash = "hashedpassword123" // Set the required property
            }
        }.AsQueryable().ElementType);
                mockUserSet.As<IQueryable<User>>().Setup(m => m.GetEnumerator()).Returns(new List<User>
        {
            new User
            {
                UserName = "testuser",
                Email = "test@example.com",
                PasswordHash = "hashedpassword123" // Set the required property
            }
        }.AsQueryable().GetEnumerator());

            _mockDbContext.Setup(db => db.Users).Returns(mockUserSet.Object);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _authService.RegisterUserAsync("testuser", "test@example.com", "password123"));
        }

        [Fact]
        public async Task RegisterUserAsync_ShouldCreateUser_WhenUsernameAndEmailAreUnique()
        {
            // Arrange
            var mockUserSet = new Mock<DbSet<User>>();
            mockUserSet.As<IQueryable<User>>().Setup(m => m.Provider).Returns(new TestAsyncQueryProvider<User>(new List<User>().AsQueryable().Provider));
            mockUserSet.As<IQueryable<User>>().Setup(m => m.Expression).Returns(new List<User>().AsQueryable().Expression);
            mockUserSet.As<IQueryable<User>>().Setup(m => m.ElementType).Returns(new List<User>().AsQueryable().ElementType);
            mockUserSet.As<IQueryable<User>>().Setup(m => m.GetEnumerator()).Returns(new List<User>().AsQueryable().GetEnumerator());

            _mockDbContext.Setup(db => db.Users).Returns(mockUserSet.Object);

            _mockPasswordHasher.Setup(ph => ph.HashPassword(It.IsAny<User>(), It.IsAny<string>()))
                .Returns("hashedpassword");

            _mockDbContext.Setup(db => db.Users.Add(It.IsAny<User>()));
            _mockDbContext.Setup(db => db.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            // Act
            var result = await _authService.RegisterUserAsync("testuser", "test@example.com", "password123");

            // Assert
            Assert.NotNull(result);
            Assert.Equal("testuser", result.UserName);
            Assert.Equal("test@example.com", result.Email);
            Assert.Equal("hashedpassword", result.PasswordHash);
        }
    }
}
