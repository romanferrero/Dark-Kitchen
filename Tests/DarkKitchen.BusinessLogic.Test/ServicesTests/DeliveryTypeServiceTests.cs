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

    [TestMethod]
    public void Create_DuplicateName_ThrowsArgumentException()
    {
        _repoMock
            .Setup(r => r.Exists(It.IsAny<Expression<Func<DeliveryType, bool>>>()))
            .Returns(true);

        Assert.ThrowsException<ArgumentException>(() =>
            _service.Create(new DeliveryTypeEntryDto("Express", 250m)));
    }

    [TestMethod]
    public void Update_ValidData_CallsUpdateAndReturnsDto()
    {
        var existing = DeliveryType.Create("Express", 250m);

        _repoMock
            .Setup(r => r.Get(It.IsAny<Expression<Func<DeliveryType, bool>>>()))
            .Returns(existing);

        _repoMock
            .Setup(r => r.Update(It.IsAny<DeliveryType>()));

        var result = _service.Update(1, new DeliveryTypeEntryDto("SameDay", 200m));

        Assert.AreEqual("SameDay", result.Name);
        Assert.AreEqual(200m, result.ShippingCost);
        _repoMock.Verify(r => r.Update(It.IsAny<DeliveryType>()), Times.Once);
    }
}
