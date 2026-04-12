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

    private static CreateOrderRequestModel BuildValidRequest()
    {
        return new CreateOrderRequestModel
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

        var request = BuildValidRequest();

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

        var request = BuildValidRequest();

        var result = _controller.CreateOrder(request);

        Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
    }

    [TestMethod]
    public void CreateOrder_ValidData_Returns201AndResponseBody()
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

        var request = BuildValidRequest();

        var result = _controller.CreateOrder(request) as CreatedResult;

        Assert.IsNotNull(result);
        Assert.AreEqual(201, result.StatusCode);

        var response = result.Value as CreateOrderResponseModel;
        Assert.IsNotNull(response);
        Assert.AreEqual(1, response.ClientId);
        Assert.AreEqual(100, response.OrderNumber);
        Assert.AreEqual(400m, response.Subtotal);
        Assert.AreEqual(100m, response.ShippingCost);
        Assert.AreEqual(610m, response.Total);
    }

    [TestMethod]
    public void CreateOrder_ValidData_CallsServiceWithExpectedArguments()
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
                It.Is<List<(string ProductCode, int Quantity)>>(items =>
                    items.Count == 1 &&
                    items[0].ProductCode == "BURG01" &&
                    items[0].Quantity == 2)))
            .Returns(expectedResult);

        var request = BuildValidRequest();

        _controller.CreateOrder(request);

        _orderServiceMock.Verify(s => s.CreateOrder(
            1,
            "express",
            "18 de Julio",
            "1234",
            "Apto 101",
            It.Is<List<(string ProductCode, int Quantity)>>(items =>
                items.Count == 1 &&
                items[0].ProductCode == "BURG01" &&
                items[0].Quantity == 2)),
            Times.Once);
    }

    [TestMethod]
    public void CreateOrder_InvalidData_Returns400WithMessage()
    {
        const string expectedMessage = "Order must have at least one product.";

        _orderServiceMock
            .Setup(s => s.CreateOrder(
                It.IsAny<int>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<List<(string ProductCode, int Quantity)>>()))
            .Throws(new ArgumentException(expectedMessage));

        var request = BuildValidRequest();

        var result = _controller.CreateOrder(request) as BadRequestObjectResult;

        Assert.IsNotNull(result);
        Assert.AreEqual(400, result.StatusCode);
        Assert.AreEqual(expectedMessage, result.Value);
    }
}
