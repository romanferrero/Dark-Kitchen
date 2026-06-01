using System.Linq.Expressions;
using DarkKitchen.BusinessLogic.Services;
using DarkKitchen.Domain.Entities;
using DarkKitchen.IBusinessLogic.DTOs.Entry.DeliveryTypeDTOs;
using DarkKitchen.IDataAccess.RepositoriesInterfaces;
using Moq;

namespace DarkKitchen.BusinessLogic.Test.ServicesTests;

[TestClass]
public class DeliveryTypeServiceTests
{
    private Mock<IRepository<DeliveryType>> _repoMock = null!;
    private DeliveryTypeService _service = null!;

    [TestInitialize]
    public void Initialize()
    {
        _repoMock = new Mock<IRepository<DeliveryType>>(MockBehavior.Strict);
        _service = new DeliveryTypeService(_repoMock.Object);
    }

    [TestMethod]
    public void Create_ValidData_CallsAddAndReturnsDto()
    {
        _repoMock
            .Setup(r => r.Exists(It.IsAny<Expression<Func<DeliveryType, bool>>>()))
            .Returns(false);

        _repoMock
            .Setup(r => r.Add(It.IsAny<DeliveryType>()));

        var result = _service.Create(new DeliveryTypeEntryDto("Express", 250m));

        Assert.AreEqual("Express", result.Name);
        Assert.AreEqual(250m, result.ShippingCost);
        _repoMock.Verify(r => r.Add(It.IsAny<DeliveryType>()), Times.Once);
    }
}
