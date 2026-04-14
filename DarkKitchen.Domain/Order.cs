namespace DarkKitchen.Domain;

public class Order
{
    private int _orderId;

    private DeliveryType _deliveryType;

    private Address _address = null!;

    private List<Product> _products = null!;

    private OrderStatus _orderStatus;

    private int _clientId;

    private int _orderNumber;

    private double _subtotal;

    private double _shippingCost;

    private double _totalCost;

    private DateTime _orderDate;

    private Order()
    {
    }

    public static Order Create(
        int orderId,
        DeliveryType deliveryType,
        Address address,
        List<Product> products,
        int clientId,
        int orderNumber,
        double subtotal,
        double shippingCost,
        double totalCost)
    {
        return new Order
        {
            OrderId = orderId,
            DeliveryType = deliveryType,
            Address = address,
            Products = products,
            OrderStatus = OrderStatus.Pending,
            ClientId = clientId,
            OrderNumber = orderNumber,
            Subtotal = subtotal,
            ShippingCost = shippingCost,
            TotalCost = totalCost,
            OrderDate = DateTime.Now
        };
    }

    public int OrderId
    {
        get => _orderId;
        set
        {
            ArgumentOutOfRangeException.ThrowIfNegative(value);
            _orderId = value;
        }
    }

    public DeliveryType DeliveryType
    {
        get => _deliveryType;
        set => _deliveryType = value;
    }

    public Address Address
    {
        get => _address;
        set => _address = value;
    }

    public List<Product> Products
    {
        get => _products;
        set
        {
            if(value.Count == 0)
            {
                throw new ArgumentException("Product list cannot be empty");
            }

            _products = value;
        }
    }

    public OrderStatus OrderStatus
    {
        get => _orderStatus;
        set => _orderStatus = value;
    }

    public int ClientId
    {
        get => _clientId;
        set
        {
            ArgumentOutOfRangeException.ThrowIfNegative(value);
            _clientId = value;
        }
    }

    public int OrderNumber
    {
        get => _orderNumber;
        set
        {
            ArgumentOutOfRangeException.ThrowIfNegative(value);
            _orderNumber = value;
        }
    }

    public double Subtotal
    {
        get => _subtotal;
        set => _subtotal = value;
    }

    public double ShippingCost
    {
        get => _shippingCost;
        set => _shippingCost = value;
    }

    public double TotalCost
    {
        get => _totalCost;
        set => _totalCost = value;
    }

    public DateTime OrderDate
    {
        get => _orderDate;
        set => _orderDate = value;
    }
}
