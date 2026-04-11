using DarkKitchen.Domain;

public class Product
{
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

    public static Product Create(string code, string name, string description,
                             string line, string category, string images, bool active)
    {
        return new Product
        {
            Code = code,
            Name = name,
            Description = description,
            Line = line,
            Category = category,
            Images = ParseImages(images),
            Active = active
        };
    }

    public void Update(string name, string description, string line,
                       string category, string images, bool active)
    {
        Name = name;
        Description = description;
        Line = line;
        Category = category;
        Images = ParseImages(images);
        Active = active;
    }

    private static List<ProductImage> ParseImages(string images)
    {
        return images
            .Split(',', StringSplitOptions.RemoveEmptyEntries)
            .Select(url => new ProductImage { Url = url.Trim() })
            .ToList();
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
            if(value.Length < 5 || value.Length > 20)
            {
                throw new ArgumentException("Product code must be between 5 and 20 characters.");
            }

            _code = value;
        }
    }

    public string Name
    {
        get => _name;
        set
        {
            if(value.Length < 10 || value.Length > 50)
            {
                throw new ArgumentException("Product name must be between 10 and 50 characters.");
            }

            _name = value;
        }
    }

    public string Description
    {
        get => _description;
        set
        {
            if(value.Length < 20 || value.Length > 500)
            {
                throw new ArgumentException("Product description must be between 20 and 500 characters.");
            }

            _description = value;
        }
    }

    public string Line
    {
        get => _line;
        set { _line = value; }
    }

    public string Category
    {
        get => _category;
        set { _category = value; }
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
            if(value.Count == 0 || value.Count > 3)
            {
                throw new ArgumentException("Product must have between 1 and 3 images.");
            }

            _images = value;
        }
    }

    public bool Active
    {
        get => _active;
        set { _active = value; }
    }
}
