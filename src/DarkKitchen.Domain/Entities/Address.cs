namespace DarkKitchen.Domain.Entities;

public class Address
{
    private string _street = null!;
    private string _doorNumber = null!;
    private string? _apartment;

    private Address()
    {
    }

    public static Address Create(string street, string doorNumber, string apartment)
    {
        return new Address
        {
            Street = street,
            DoorNumber = doorNumber,
            Apartment = apartment
        };
    }

    public string? Street
    {
        get => _street;
        set
        {
            ValidateNonEmpty(value, "Street");
            _street = value!;
        }
    }

    public string? DoorNumber
    {
        get => _doorNumber;
        set
        {
            ValidateNonEmpty(value, "DoorNumber");
            _doorNumber = value!;
        }
    }

    public string? Apartment
    {
        get => _apartment;
        set => _apartment = value;
    }

    private static void ValidateNonEmpty(string? value, string fieldName)
    {
        if(string.IsNullOrEmpty(value))
        {
            throw new ArgumentException($"{fieldName} cannot be empty");
        }
    }
}
