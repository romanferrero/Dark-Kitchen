using System.Security.Claims;
using DarkKitchen.Domain.Enums;
using DarkKitchen.IBusinessLogic.DTOs.Entry.OrderDTOs;
using DarkKitchen.IBusinessLogic.DTOs.Exit.OrderDTOs;
using DarkKitchen.IBusinessLogic.IServices;
using DarkKitchen.WebApi.Controllers.OrdersControllers;
using DarkKitchen.WebApi.Models.Request.OrdersModels;
using DarkKitchen.WebApi.Models.Response.OrdersModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace DarkKitchen.WebApi.Test.ControllersTests.OrdersControllersTests;

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
            Products = ["BURG01", "BURG01"],
        };
    }

    private void SetupUserRole(UserRole role)
    {
        var claims = new List<Claim> { new(ClaimTypes.Role, role.ToString()) };
        var identity = new ClaimsIdentity(claims);
        var principal = new ClaimsPrincipal(identity);

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = principal }
        };
    }

    [TestMethod]
    public void CreateOrder_Valid_Returns201()
    {
        var dto = new CreateOrderResultExitDto
        {
            ClientId = 1,
            OrderNumber = 100,
            Subtotal = 400,
            ShippingCost = 100,
            Total = 610
        };

        _orderServiceMock
            .Setup(s => s.CreateOrder(It.IsAny<CreateOrderEntryDto>()))
            .Returns(dto);

        var result = _controller.CreateOrder(BuildValidRequest());

        Assert.IsInstanceOfType(result, typeof(CreatedResult));

        _orderServiceMock.VerifyAll();
    }

    [TestMethod]
    public void CreateOrder_ReturnsCorrectBody()
    {
        var dto = new CreateOrderResultExitDto
        {
            ClientId = 1,
            OrderNumber = 100,
            Subtotal = 400,
            ShippingCost = 100,
            Total = 610
        };

        _orderServiceMock
            .Setup(s => s.CreateOrder(It.IsAny<CreateOrderEntryDto>()))
            .Returns(dto);

        var result = _controller.CreateOrder(BuildValidRequest()) as CreatedResult;

        Assert.IsNotNull(result);

        var response = result.Value as CreateOrderResponseModel;

        Assert.IsNotNull(response);
        Assert.AreEqual(1, response.ClientId);
        Assert.AreEqual(100, response.OrderNumber);
        Assert.AreEqual(400, response.Subtotal);
        Assert.AreEqual(100, response.ShippingCost);
        Assert.AreEqual(610, response.Total);

        _orderServiceMock.VerifyAll();
    }

    [TestMethod]
    public void CreateOrder_VerifyMappingToDTO()
    {
        _orderServiceMock
            .Setup(s => s.CreateOrder(It.Is<CreateOrderEntryDto>(dto =>
                dto.ClientId == 1 &&
                dto.DeliveryType == "express" &&
                dto.Street == "18 de Julio" &&
                dto.DoorNumber == "1234" &&
                dto.Apartment == "Apto 101" &&
                dto.Products.Count == 2)))
            .Returns(new CreateOrderResultExitDto());

        _controller.CreateOrder(BuildValidRequest());

        _orderServiceMock.VerifyAll();
    }

    [TestMethod]
    public void UpdateStatus_Valid_Prepared_Returns200()
    {
        SetupUserRole(UserRole.Dispatcher);

        _orderServiceMock
            .Setup(s => s.UpdateStatus(1,
                It.Is<UpdateOrderStatusEntryDto>(d => d.Action == "Prepared")))
            .Returns(new UpdateStatusExitDto("Prepared", DateTime.Now));

        var result = _controller.UpdateStatus(1,
            new UpdateOrderStatusRequestModel { Action = "Prepared" });

        Assert.IsInstanceOfType(result, typeof(OkObjectResult));

        _orderServiceMock.VerifyAll();
    }

    [TestMethod]
    public void UpdateStatus_ReturnsCorrectBody()
    {
        SetupUserRole(UserRole.Dispatcher);

        _orderServiceMock
            .Setup(s => s.UpdateStatus(1,
                It.Is<UpdateOrderStatusEntryDto>(d => d.Action == "Prepared")))
            .Returns(new UpdateStatusExitDto("Prepared", DateTime.Now));

        var result = _controller.UpdateStatus(1,
            new UpdateOrderStatusRequestModel { Action = "Prepared" }) as OkObjectResult;

        Assert.IsNotNull(result);

        var response = result.Value as UpdateOrderStatusResponseModel;

        Assert.IsNotNull(response);
        Assert.AreEqual("Prepared", response.Status);

        _orderServiceMock.VerifyAll();
    }

    [TestMethod]
    [ExpectedException(typeof(KeyNotFoundException))]
    public void UpdateStatus_OrderNotFound_Throws()
    {
        _orderServiceMock
            .Setup(s => s.UpdateStatus(1, It.IsAny<UpdateOrderStatusEntryDto>()))
            .Throws(new KeyNotFoundException("Order not found"));

        _controller.UpdateStatus(1, new UpdateOrderStatusRequestModel { Action = "Prepared" });

        _orderServiceMock.VerifyAll();
    }

    [TestMethod]
    public void UpdateStatus_Admin_CanCancel()
    {
        SetupUserRole(UserRole.Admin);

        _orderServiceMock
            .Setup(s => s.UpdateStatus(1,
                It.Is<UpdateOrderStatusEntryDto>(d => d.Action == "Cancel")))
            .Returns(new UpdateStatusExitDto("Cancel", DateTime.Now));

        var result = _controller.UpdateStatus(1,
            new UpdateOrderStatusRequestModel { Action = "Cancel" });

        Assert.IsInstanceOfType(result, typeof(OkObjectResult));

        _orderServiceMock.VerifyAll();
    }
}
