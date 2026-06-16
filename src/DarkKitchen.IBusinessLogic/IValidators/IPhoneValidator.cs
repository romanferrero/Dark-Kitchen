namespace DarkKitchen.IBusinessLogic.IValidators;

public interface IPhoneValidator
{
    bool IsValid(string phone);
    string ErrorMessage { get; }
}
