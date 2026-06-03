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

    [TestMethod]
    public void Update_NotFound_ThrowsKeyNotFoundException()
    {
        _repoMock
            .Setup(r => r.Get(It.IsAny<Expression<Func<DeliveryType, bool>>>()))
            .Returns((DeliveryType?)null);

        Assert.ThrowsException<KeyNotFoundException>(() =>
            _service.Update(99, new DeliveryTypeEntryDto("Express", 250m)));
    }

    [TestMethod]
    public void GetAll_ReturnsMappedDtos()
    {
        var stored = new List<DeliveryType>
        {
            DeliveryType.Create("Express", 250m),
            DeliveryType.Create("SameDay", 200m),
        };

        _repoMock
            .Setup(r => r.GetAll(null))
            .Returns(stored);

        var result = _service.GetAll();

        Assert.AreEqual(2, result.Count);
        Assert.AreEqual("Express", result[0].Name);
        Assert.AreEqual("SameDay", result[1].Name);
    }

    [TestMethod]
    public void Delete_NonExistentId_ThrowsKeyNotFoundException()
    {
        _repoMock
            .Setup(r => r.Get(It.IsAny<Expression<Func<DeliveryType, bool>>>()))
            .Returns((DeliveryType?)null);

        Assert.ThrowsException<KeyNotFoundException>(() => _service.Delete(99));
    }

    [TestMethod]
    public void Delete_ExistingId_CallsRepositoryDelete()
    {
        var existing = DeliveryType.Create("Express", 250m);

        _repoMock
            .Setup(r => r.Get(It.IsAny<Expression<Func<DeliveryType, bool>>>()))
            .Returns(existing);

        _repoMock
            .Setup(r => r.Delete(It.IsAny<Expression<Func<DeliveryType, bool>>>()));

        _service.Delete(1);

        _repoMock.Verify(r => r.Delete(It.IsAny<Expression<Func<DeliveryType, bool>>>()), Times.Once);
    }
}
