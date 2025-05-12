using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using ClipVault.Dtos;
using ClipVault.Interfaces;
using ClipVault.Controllers;
using ClipVault.Exceptions;
using ClipVault.Tests.Mocks;

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
            var registerDto = TestDataHelper.CreateRegisterDto();
            var user = TestDataHelper.CreateUser(registerDto);
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
        public async Task LoginUser_ValidCredentials_ReturnsToken()
        {
            // Arrange
            var loginDto = TestDataHelper.CreateLoginDto();
            var token = "mocked-jwt-token";
            _authServiceMock.Setup(s => s.LoginUserAsync(loginDto.UserNameOrEmail, loginDto.Password)).ReturnsAsync(token);

            // Act
            var result = await _authController.Login(loginDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var responseObject = okResult.Value;
            Assert.NotNull(responseObject);
            Assert.Equal(token, responseObject.GetType().GetProperty("token")?.GetValue(responseObject, null));
        }

        [Fact]
        public async Task LoginUser_InvalidCredentials_ThrowsInvalidCredentialsException()
        {
            // Arrange
            var loginDto = new LoginDto { UserNameOrEmail = "testuser", Password = "wrongpassword" };
            _authServiceMock.Setup(s => s.LoginUserAsync(loginDto.UserNameOrEmail, loginDto.Password)).ThrowsAsync(new InvalidCredentialsException("Invalid credentials"));

            // Act & Assert
            await Assert.ThrowsAsync<InvalidCredentialsException>(async () => await _authController.Login(loginDto));
        }
    }
}
