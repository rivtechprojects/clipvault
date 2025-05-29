using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using ClipVault.Dtos;
using ClipVault.Interfaces;
using ClipVault.Controllers;
using ClipVault.Exceptions;
using ClipVault.Tests.Mocks;
using ClipVault.Models;
using System.Security.Claims;

namespace ClipVault.Tests
{
    public class AuthControllerTests
    {
        private readonly Mock<IAuthService> _authServiceMock;
        private readonly AuthController _authController;

        public AuthControllerTests()
        {
            _authServiceMock = new Mock<IAuthService>();
            _authController = new AuthController(_authServiceMock.Object);
        }

        [Fact]
        public async Task RegisterUser_ValidRequest_ReturnsOk()
        {
            // Arrange
            var registerDto = MockDataFactory.CreateRegisterDto();
            var user = MockDataFactory.CreateUser();
            _authServiceMock.Setup(s => s.RegisterUserAsync(registerDto.UserName, registerDto.Email, registerDto.Password)).ReturnsAsync(user);

            // Act
            var result = await _authController.Register(registerDto);

            // Assert
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task RegisterUser_InvalidRequest_ThrowsNullReferenceException()
        {
            // Arrange
            var registerDto = new RegisterDto { UserName = "", Email = "", Password = "" };
            _authController.ModelState.AddModelError("UserName", "Required");

            // Act & Assert
            await Assert.ThrowsAsync<NullReferenceException>(async () => await _authController.Register(registerDto));
        }

        [Fact]
        public async Task LoginUser_ValidCredentials_ReturnsTokens()
        {
            // Arrange
            var loginDto = MockDataFactory.CreateLoginDto();
            var accessToken = "mocked-access-token";
            var refreshToken = "mocked-refresh-token";
            _authServiceMock.Setup(s => s.LoginUserWithRefreshTokenAsync(loginDto.UserNameOrEmail, loginDto.Password))
                .ReturnsAsync((accessToken, refreshToken));

            // Act
            var result = await _authController.Login(loginDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var responseObject = okResult.Value as dynamic;
            Assert.NotNull(responseObject);
            Assert.Equal(accessToken, responseObject?.accessToken);
            Assert.Equal(refreshToken, responseObject?.refreshToken);
        }

        [Fact]
        public async Task LoginUser_InvalidCredentials_ThrowsInvalidCredentialsException()
        {
            // Arrange
            var loginDto = new LoginDto { UserNameOrEmail = "testuser", Password = "wrongpassword" };
            _authServiceMock.Setup(s => s.LoginUserWithRefreshTokenAsync(loginDto.UserNameOrEmail, loginDto.Password))
                .ThrowsAsync(new InvalidCredentialsException("Invalid credentials"));

            // Act & Assert
            await Assert.ThrowsAsync<InvalidCredentialsException>(async () => await _authController.Login(loginDto));
        }

        [Fact]
        public async Task RefreshToken_ValidToken_ReturnsNewTokens()
        {
            // Arrange
            var refreshTokenDto = new RefreshTokenDto { RefreshToken = "valid-refresh-token" };
            var user = MockDataFactory.CreateUser();
            var newAccessToken = "new-access-token";
            var newRefreshToken = "new-refresh-token";

            _authServiceMock.Setup(s => s.GetUserByRefreshTokenAsync(refreshTokenDto.RefreshToken))
                .ReturnsAsync(user);
            _authServiceMock.Setup(s => s.GenerateJwtToken(user)).Returns(newAccessToken);
            _authServiceMock.Setup(s => s.UpdateRefreshTokenAsync(user, It.IsAny<string>()))
                .Callback<User, string>((u, token) => u.RefreshToken = token)
                .Returns(Task.CompletedTask);
            _authServiceMock.Setup(s => s.GenerateRefreshToken()).Returns(newRefreshToken);

            // Act
            var result = await _authController.RefreshToken(refreshTokenDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var responseObject = okResult.Value as dynamic;
            Assert.NotNull(responseObject);
            Assert.Equal(newAccessToken, responseObject?.accessToken);
            Assert.Equal(newRefreshToken, responseObject?.refreshToken);
        }

        [Fact]
        public async Task RefreshToken_InvalidToken_ReturnsUnauthorized()
        {
            // Arrange
            var refreshTokenDto = new RefreshTokenDto { RefreshToken = "invalid-refresh-token" };
            _authServiceMock.Setup(s => s.GetUserByRefreshTokenAsync(refreshTokenDto.RefreshToken))
                .ReturnsAsync((User?)null);

            // Act
            var result = await _authController.RefreshToken(refreshTokenDto);

            // Assert
            var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
            var responseObject = unauthorizedResult.Value as dynamic;
            Assert.NotNull(responseObject);
            Assert.Equal("Invalid or expired refresh token.", responseObject?.message);
        }

        [Fact]
        public async Task Logout_AuthenticatedUser_ReturnsOk()
        {
            // Arrange
            var userId = 1;
            var user = MockDataFactory.CreateUser();
            user.UserId = userId;

            _authServiceMock.Setup(s => s.GetUserByIdAsync(userId)).ReturnsAsync(user);
            _authServiceMock.Setup(s => s.LogoutUserAsync(user)).Returns(Task.CompletedTask);

            var controllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            };
            controllerContext.HttpContext.User = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString())
            }));
            _authController.ControllerContext = controllerContext;

            // Act
            var result = await _authController.Logout();

            // Assert
            Assert.IsType<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            Assert.NotNull(okResult);
            Assert.Equal("User logged out successfully.", ((dynamic?)okResult?.Value)?.message);
        }
    }
}
