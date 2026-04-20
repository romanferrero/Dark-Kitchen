using DarkKitchen.IBusinessLogic;
using DarkKitchen.IBusinessLogic.IValidators;

namespace DarkKitchen.BusinessLogic.Validators;

public class UruguayanPhoneValidator : IPhoneValidator
{
    private const string CountryCode = "598";
    private const string MobilePrefix = "09";
    private const int LocalDigitsLength = 9;

    public string ErrorMessage => "Phone must be a valid Uruguayan mobile number (09XXXXXXX or +598 9X XXX XXX).";

    public bool IsValid(string phone)
    {
        var digits = new string(phone.Where(char.IsDigit).ToArray());

        if(digits.StartsWith(CountryCode))
        {
            digits = "0" + digits[CountryCode.Length..];
        }

        return digits.Length == LocalDigitsLength && digits.StartsWith(MobilePrefix);
    }
}
