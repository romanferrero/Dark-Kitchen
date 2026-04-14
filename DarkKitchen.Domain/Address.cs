namespace DarkKitchen.Domain;

public class Address
{
    private string? _street;
    private string? _doorNumber;
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
            if(value == string.Empty)
            {
                throw new ArgumentException("Street cannot be empty");
            }

            _street = value;
        }
    }

    public string? DoorNumber
    {
        get => _doorNumber;
        set => _doorNumber = value;
    }

    public string? Apartment
    {
        get => _apartment;
        set => _apartment = value;
    }
}
