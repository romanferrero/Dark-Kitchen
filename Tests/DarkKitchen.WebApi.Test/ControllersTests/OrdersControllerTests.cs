using DarkKitchen.BusinessLogic.DTOS;
using DarkKitchen.BusinessLogic.Interfaces;
using DarkKitchen.WebApi.Controllers;
using DarkKitchen.WebApi.Models;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace DarkKitchen.WebApi.Test.ControllersTests;

[TestClass]
public class OrdersControllerTests
{
    private Mock<IOrderService> _orderServiceMock = null!;
    private OrdersController _controller = null!;

    [TestInitialize]
    public void Initialize()
    {
        _orderServiceMock = new Mock<IOrderService>(MockBehavior.Strict);
        _controller = new OrdersController(_orderServiceMock.Object);
    }

    [TestMethod]
    public void CreateOrder_ValidData_Returns201()
    {
        var expectedResult = new OrderResultDTO
        {
            ClientId = 1,
            OrderNumber = 100,
            Subtotal = 400m,
            ShippingCost = 100m,
            Total = 610m,
        };

        _orderServiceMock
            .Setup(s => s.CreateOrder(
                1,
                "express",
                "18 de Julio",
                "1234",
                "Apto 101",
                It.IsAny<List<(string ProductCode, int Quantity)>>()))
            .Returns(expectedResult);

        var request = new CreateOrderRequestModel
        {
            ClientId = 1,
            DeliveryType = "express",
            Street = "18 de Julio",
            DoorNumber = "1234",
            Apartment = "Apto 101",
            Items =
            [
                new OrderItemRequestModel { ProductCode = "BURG01", Quantity = 2 },
            ],
        };

        var result = _controller.CreateOrder(request);

        Assert.IsInstanceOfType(result, typeof(CreatedResult));
    }

    [TestMethod]
    public void CreateOrder_InvalidData_Returns400()
    {
        _orderServiceMock
            .Setup(s => s.CreateOrder(
                It.IsAny<int>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<List<(string ProductCode, int Quantity)>>()))
            .Throws(new ArgumentException("Order must have at least one product."));

        var request = new CreateOrderRequestModel
        {
            ClientId = 1,
            DeliveryType = "express",
            Street = "18 de Julio",
            DoorNumber = "1234",
            Apartment = "Apto 101",
            Items = [],
        };

        var result = _controller.CreateOrder(request);

        Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
    }
}
