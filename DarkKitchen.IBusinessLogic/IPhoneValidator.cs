namespace DarkKitchen.IBusinessLogic;

public interface IPhoneValidator
{
    bool IsValid(string phone);
    string ErrorMessage { get; }
}
