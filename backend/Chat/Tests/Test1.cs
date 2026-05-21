using Application.Auth.Service;
using Chat.Controllers;
using Chat.Models.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Chat.Tests.Controllers;

[TestClass]
public class AuthControllerTests
{
    private Mock<AccountService> _accountServiceMock = null!;
    private AuthController _controller = null!;

    [TestInitialize]
    public void Setup()
    {
        _accountServiceMock = new Mock<AccountService>();
        _controller = new AuthController(_accountServiceMock.Object);
    }

    [TestMethod]
    public async Task Register_ValidRequest_ReturnsNoContent()
    {
        // Створення рекорду через круглі дужки (конструктор)
        var request = new RegisterUserRequest("testuser", "test@example.com", "Password123!");

        _accountServiceMock
            .Setup(s => s.RegisterAsync(request.UserName, request.Email, request.Password))
            .Returns(Task.CompletedTask);

        var result = await _controller.Register(request);

        Assert.IsInstanceOfType(result, typeof(NoContentResult));
        _accountServiceMock.Verify(
            s => s.RegisterAsync(request.UserName, request.Email, request.Password),
            Times.Once);
    }

    [TestMethod]
    public async Task Register_ServiceThrows_PropagatesException()
    {
        var request = new RegisterUserRequest("existing", "existing@example.com", "pass");

        _accountServiceMock
            .Setup(s => s.RegisterAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .ThrowsAsync(new InvalidOperationException("User already exists"));

        // Альтернативна перевірка асинхронного ексепшену без застарілого ThrowsExceptionAsync
        try
        {
            await _controller.Register(request);
            Assert.Fail("Expected exception was not thrown.");
        }
        catch (InvalidOperationException ex)
        {
            Assert.AreEqual("User already exists", ex.Message);
        }
    }

    [TestMethod]
    public async Task Login_ValidCredentials_ReturnsOkWithToken()
    {
        var request = new LoginRequest("test@example.com", "Password123!");
        const string expectedToken = "jwt-token-abc123";

        _accountServiceMock
            .Setup(s => s.LoginAsync(request.Email, request.Password))
            .ReturnsAsync(expectedToken);

        var result = await _controller.Login(request);

        var okResult = result as OkObjectResult;
        Assert.IsNotNull(okResult);
        Assert.AreEqual(200, okResult.StatusCode);
        Assert.AreEqual(expectedToken, okResult.Value?.ToString());
        _accountServiceMock.Verify(
            s => s.LoginAsync(request.Email, request.Password),
            Times.Once);
    }

    [TestMethod]
    public async Task Login_InvalidCredentials_PropagatesException()
    {
        var request = new LoginRequest("wrong@example.com", "wrong");

        _accountServiceMock
            .Setup(s => s.LoginAsync(request.Email, request.Password))
            .ThrowsAsync(new UnauthorizedAccessException("Invalid credentials"));

        try
        {
            await _controller.Login(request);
            Assert.Fail("Expected exception was not thrown.");
        }
        catch (UnauthorizedAccessException ex)
        {
            Assert.AreEqual("Invalid credentials", ex.Message);
        }
    }
}