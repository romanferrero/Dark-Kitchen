namespace DarkKitchen.Domain.Entities;

public class Product
{
    private const int MinCodeLength = 5;
    private const int MaxCodeLength = 20;
    private const int MinNameLength = 10;
    private const int MaxNameLength = 50;
    private const int MinDescriptionLength = 20;
    private const int MaxDescriptionLength = 500;
    private const int MinImageCount = 1;
    private const int MaxImageCount = 3;
    private const string RequiredImageExtension = ".jpg";

    private int _id;
    private string _code = string.Empty;
    private string _name = string.Empty;
    private string _description = string.Empty;
    private string _line = string.Empty;
    private string _category = string.Empty;
    private decimal _price;
    private List<ProductImage> _images = [];
    private bool _active = true;

    private Product()
    {
    }

    public static Product Create(CreateProductParams parameters)
    {
        return new Product
        {
            Code = parameters.Code,
            Name = parameters.Name,
            Price = parameters.Price,
            Description = parameters.Description,
            Line = parameters.Line,
            Category = parameters.Category,
            Images = ParseImages(parameters.Images),
            Active = parameters.Active
        };
    }

    public void Update(string name, decimal price, string description, string line,
                       string category, string images, bool active)
    {
        Name = name;
        Price = price;
        Description = description;
        Line = line;
        Category = category;
        Images = ParseImages(images);
        Active = active;
    }

    private static List<ProductImage> ParseImages(string images)
    {
        var imageList = images
            .Split(',', StringSplitOptions.RemoveEmptyEntries)
            .Select(entry =>
            {
                var parts = entry.Trim().Split('|');
                var url = parts[0].Trim();
                var sizeInKb = parts.Length > 1 ? decimal.Parse(parts[1].Trim()) : 0;

                return new ProductImage { Url = url, SizeInKb = sizeInKb };
            })
            .ToList();

        var hasNonJpg = imageList.Any(img => !img.Url.EndsWith(RequiredImageExtension, StringComparison.OrdinalIgnoreCase));
        if(hasNonJpg)
        {
            throw new ArgumentException($"All product images must be in {RequiredImageExtension} format.");
        }

        return imageList;
    }

    public int Id
    {
        get => _id;
        set { _id = value; }
    }

    public string Code
    {
        get => _code;
        set
        {
            ValidateCode(value);
            _code = value;
        }
    }

    public string Name
    {
        get => _name;
        set
        {
            ValidateName(value);
            _name = value;
        }
    }

    public string Description
    {
        get => _description;
        set
        {
            ValidateDescription(value);
            _description = value;
        }
    }

    public string Line
    {
        get => _line;
        set
        {
            ValidateNonEmpty(value, "Product line");
            _line = value;
        }
    }

    public string Category
    {
        get => _category;
        set
        {
            ValidateNonEmpty(value, "Product category");
            _category = value;
        }
    }

    public decimal Price
    {
        get => _price;
        set { _price = value; }
    }

    public List<ProductImage> Images
    {
        get => _images;
        set
        {
            ValidateImageCount(value);
            _images = value;
        }
    }

    public bool Active
    {
        get => _active;
        set { _active = value; }
    }

    private static void ValidateCode(string value)
    {
        var isInvalidLength = value.Length < MinCodeLength || value.Length > MaxCodeLength;
        if(isInvalidLength)
        {
            throw new ArgumentException($"Product code must be between {MinCodeLength} and {MaxCodeLength} characters.");
        }
    }

    private static void ValidateName(string value)
    {
        var isInvalidLength = value.Length < MinNameLength || value.Length > MaxNameLength;
        if(isInvalidLength)
        {
            throw new ArgumentException($"Product name must be between {MinNameLength} and {MaxNameLength} characters.");
        }
    }

    private static void ValidateDescription(string value)
    {
        var isInvalidLength = value.Length < MinDescriptionLength || value.Length > MaxDescriptionLength;
        if(isInvalidLength)
        {
            throw new ArgumentException($"Product description must be between {MinDescriptionLength} and {MaxDescriptionLength} characters.");
        }
    }

    private static void ValidateNonEmpty(string value, string fieldName)
    {
        if(string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException($"{fieldName} cannot be empty.");
        }
    }

    private static void ValidateImageCount(List<ProductImage> value)
    {
        var isInvalidCount = value.Count < MinImageCount || value.Count > MaxImageCount;
        if(isInvalidCount)
        {
            throw new ArgumentException($"Product must have between {MinImageCount} and {MaxImageCount} images.");
        }
    }
}
