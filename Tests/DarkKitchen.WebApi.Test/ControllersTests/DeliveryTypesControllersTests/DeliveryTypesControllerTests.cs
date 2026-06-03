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

    [TestMethod]
    public void Update_ValidData_Returns200()
    {
        _serviceMock
            .Setup(s => s.Update(It.IsAny<int>(), It.IsAny<IBusinessLogic.DTOs.Entry.DeliveryTypeDTOs.DeliveryTypeEntryDto>()))
            .Returns(new DeliveryTypeExitDto(1, "NextDay", 180m));

        var result = _controller.Update(1, new DeliveryTypeRequestModel { Name = "NextDay", ShippingCost = 180m })
            as OkObjectResult;

        Assert.IsNotNull(result);
        Assert.AreEqual(200, result.StatusCode);

        var response = result.Value as DeliveryTypeResponseModel;
        Assert.IsNotNull(response);
        Assert.AreEqual("NextDay", response.Name);
        Assert.AreEqual(180m, response.ShippingCost);
    }

    [TestMethod]
    public void GetAll_Returns200WithList()
    {
        _serviceMock
            .Setup(s => s.GetAll())
            .Returns([
                new DeliveryTypeExitDto(1, "Express", 250m),
                new DeliveryTypeExitDto(2, "SameDay", 200m),
            ]);

        var result = _controller.GetAll() as OkObjectResult;

        Assert.IsNotNull(result);
        Assert.AreEqual(200, result.StatusCode);

        var response = result.Value as List<DeliveryTypeResponseModel>;
        Assert.IsNotNull(response);
        Assert.AreEqual(2, response.Count);
        Assert.AreEqual("Express", response[0].Name);
        Assert.AreEqual("SameDay", response[1].Name);
    }

    [TestMethod]
    public void Delete_ExistingId_Returns204NoContent()
    {
        _serviceMock.Setup(s => s.Delete(1));

        var result = _controller.Delete(1);

        Assert.IsInstanceOfType(result, typeof(NoContentResult));
    }
}
