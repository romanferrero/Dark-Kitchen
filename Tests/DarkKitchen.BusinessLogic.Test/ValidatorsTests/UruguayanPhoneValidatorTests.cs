using DarkKitchen.BusinessLogic.Validators;

namespace DarkKitchen.BusinessLogic.Test.ValidatorsTests;

[TestClass]
public class UruguayanPhoneValidatorTests
{
    private UruguayanPhoneValidator _validator = null!;

    [TestInitialize]
    public void Initialize()
    {
        _validator = new UruguayanPhoneValidator();
    }

    [TestMethod]
    public void IsValid_ValidMobileNumber_ReturnsTrue()
    {
        Assert.IsTrue(_validator.IsValid("099123456"));
    }

    [TestMethod]
    public void IsValid_ValidMobileWithDashes_ReturnsTrue()
    {
        Assert.IsTrue(_validator.IsValid("099-123-456"));
    }

    [TestMethod]
    public void IsValid_ValidMobileWithSpaces_ReturnsTrue()
    {
        Assert.IsTrue(_validator.IsValid("099 123 456"));
    }

    [TestMethod]
    public void IsValid_ValidMobileStartingWith098_ReturnsTrue()
    {
        Assert.IsTrue(_validator.IsValid("098765432"));
    }

    [TestMethod]
    public void IsValid_ValidMobileStartingWith094_ReturnsTrue()
    {
        Assert.IsTrue(_validator.IsValid("094111222"));
    }

    [TestMethod]
    public void IsValid_ValidMobileStartingWith093_ReturnsTrue()
    {
        Assert.IsTrue(_validator.IsValid("093111222"));
    }

    [TestMethod]
    public void IsValid_ValidMobileStartingWith092_ReturnsTrue()
    {
        Assert.IsTrue(_validator.IsValid("092111222"));
    }

    [TestMethod]
    public void IsValid_TooFewDigits_ReturnsFalse()
    {
        Assert.IsFalse(_validator.IsValid("09912345"));
    }

    [TestMethod]
    public void IsValid_TooManyDigits_ReturnsFalse()
    {
        Assert.IsFalse(_validator.IsValid("0991234567"));
    }

    [TestMethod]
    public void IsValid_DoesNotStartWith09_ReturnsFalse()
    {
        Assert.IsFalse(_validator.IsValid("089123456"));
    }

    [TestMethod]
    public void IsValid_LandlineNumber_ReturnsFalse()
    {
        Assert.IsFalse(_validator.IsValid("029001505"));
    }

    [TestMethod]
    public void IsValid_EmptyString_ReturnsFalse()
    {
        Assert.IsFalse(_validator.IsValid(string.Empty));
    }

    [TestMethod]
    public void IsValid_OnlyLetters_ReturnsFalse()
    {
        Assert.IsFalse(_validator.IsValid("abcdefghi"));
    }

    [TestMethod]
    public void IsValid_WhitespaceOnly_ReturnsFalse()
    {
        Assert.IsFalse(_validator.IsValid("   "));
    }

    [TestMethod]
    public void IsValid_WithCountryCode598_ReturnsTrue()
    {
        Assert.IsTrue(_validator.IsValid("+598 99 123 456"));
    }

    [TestMethod]
    public void IsValid_WithCountryCode598NoPlusSign_ReturnsTrue()
    {
        Assert.IsTrue(_validator.IsValid("59899123456"));
    }

    [TestMethod]
    public void ErrorMessage_ReturnsDescriptiveMessage()
    {
        Assert.IsFalse(string.IsNullOrWhiteSpace(_validator.ErrorMessage));
    }
}
