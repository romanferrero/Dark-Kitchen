using DarkKitchen.BusinessLogic.Interfaces;
using DarkKitchen.WebApi.Controllers;
using DarkKitchen.WebApi.Models;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace DarkKitchen.WebApi.Test;

[TestClass]
public class ClientsControllerTests
{
    private Mock<IClientService> _clientServiceMock = null!;
    private ClientsController _controller = null!;

    [TestInitialize]
    public void Initialize()
    {
        _clientServiceMock = new Mock<IClientService>();
        _controller = new ClientsController(_clientServiceMock.Object);
    }

    [TestMethod]
    public void RegisterClient_InvalidData_Returns400()
    {
        _clientServiceMock
            .Setup(s => s.RegisterClient(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .Throws(new ArgumentException("El nombre no puede estar vacío."));

        var request = new RegisterClientRequestModel
        {
            Nombre = string.Empty,
            Apellido = "Garcia",
            Email = "juan@test.com",
            Telefono = "099123456",
            Password = "ValidPass@1Ab!xyz",
        };

        var result = _controller.RegisterClient(request);

        Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
    }

    [TestMethod]
    public void RegisterClient_ValidData_Returns201()
    {
        var request = new RegisterClientRequestModel
        {
            Nombre = "Juan",
            Apellido = "Garcia",
            Email = "juan@test.com",
            Telefono = "099123456",
            Password = "ValidPass@1Ab!xyz",
        };

        var result = _controller.RegisterClient(request);

        Assert.IsInstanceOfType(result, typeof(CreatedResult));
    }
}
