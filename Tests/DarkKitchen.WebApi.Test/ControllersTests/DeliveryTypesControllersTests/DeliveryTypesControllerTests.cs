using DarkKitchen.IBusinessLogic.DTOs.Exit.DeliveryTypeDTOs;
using DarkKitchen.IBusinessLogic.IServices;
using DarkKitchen.WebApi.Controllers.DeliveryTypesControllers;
using DarkKitchen.WebApi.Models.Request.DeliveryTypesModels;
using DarkKitchen.WebApi.Models.Response.DeliveryTypesModels;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace DarkKitchen.WebApi.Test.ControllersTests.DeliveryTypesControllersTests;

[TestClass]
public class DeliveryTypesControllerTests
{
    private Mock<IDeliveryTypeService> _serviceMock = null!;
    private DeliveryTypesController _controller = null!;

    [TestInitialize]
    public void Initialize()
    {
        _serviceMock = new Mock<IDeliveryTypeService>(MockBehavior.Strict);
        _controller = new DeliveryTypesController(_serviceMock.Object);
    }

    [TestMethod]
    public void Create_ValidData_Returns201()
    {
        _serviceMock
            .Setup(s => s.Create(It.IsAny<IBusinessLogic.DTOs.Entry.DeliveryTypeDTOs.DeliveryTypeEntryDto>()))
            .Returns(new DeliveryTypeExitDto(1, "Express", 250m));

        var result = _controller.Create(new DeliveryTypeRequestModel { Name = "Express", ShippingCost = 250m })
            as CreatedAtActionResult;

        Assert.IsNotNull(result);
        Assert.AreEqual(201, result.StatusCode);

        var response = result.Value as DeliveryTypeResponseModel;
        Assert.IsNotNull(response);
        Assert.AreEqual("Express", response.Name);
        Assert.AreEqual(250m, response.ShippingCost);
    }
}
