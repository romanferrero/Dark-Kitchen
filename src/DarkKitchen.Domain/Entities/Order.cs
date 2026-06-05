using DarkKitchen.Domain.States;

namespace DarkKitchen.Domain.Entities;

public class Order
{
    private int _orderId;
    private string _deliveryName = string.Empty;
    private Address _address = null!;
    private List<OrderProduct> _products = null!;
    private IOrderState _state = new PendingOrderState();
    private int _clientId;
    private int _orderNumber;
    private decimal _subtotal;
    private decimal _shippingCost;
    private decimal _totalCost;
    private DateTime _orderDate;

    private Order()
    {
    }

    public static Order Create(
        string deliveryName,
        Address address,
        List<OrderProduct> products,
        int clientId,
        int orderNumber,
        decimal subtotal,
        decimal shippingCost,
        decimal totalCost)
    {
        return new Order
        {
            DeliveryName = deliveryName,
            Address = address,
            Products = products,
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

    public string DeliveryName
    {
        get => _deliveryName;
        private set => _deliveryName = value;
    }

    public Address Address
    {
        get => _address;
        set => _address = value;
    }

    public List<OrderProduct> Products
    {
        get => _products;
        set
        {
            ValidateProducts(value);
            _products = value;
        }
    }

    public IOrderState State => _state;

    public string OrderStatus
    {
        get => _state.Name;
        private set => _state = StateFromName(value);
    }

    public static IOrderState StateFromName(string name) => name switch
    {
        "Pending" => new PendingOrderState(),
        "Prepared" => new PreparedOrderState(),
        "Cancelled" => new CancelledOrderState(),
        "Delayed" => new DelayedOrderState(),
        "OnTheWay" => new OnTheWayOrderState(),
        "Delivered" => new DeliveredOrderState(),
        "NotDelivered" => new NotDeliveredOrderState(),
        _ => throw new ArgumentException($"Unknown order state: '{name}'")
    };

    public int ClientId
    {
        get => _clientId;
        private set
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

    public decimal Subtotal
    {
        get => _subtotal;
        private set
        {
            ArgumentOutOfRangeException.ThrowIfNegative(value);
            _subtotal = value;
        }
    }

    public decimal ShippingCost
    {
        get => _shippingCost;
        private set => _shippingCost = value;
    }

    public decimal TotalCost
    {
        get => _totalCost;
        private set
        {
            ArgumentOutOfRangeException.ThrowIfNegative(value);
            _totalCost = value;
        }
    }

    public DateTime OrderDate
    {
        get => _orderDate;
        set => _orderDate = value;
    }

    public void UpdateStatus(string targetStateName)
    {
        _state = _state.TransitionTo(targetStateName);
    }

    private static void ValidateProducts(List<OrderProduct> value)
    {
        var isEmpty = value == null || value.Count == 0;
        if(isEmpty)
        {
            throw new ArgumentException("Product list cannot be empty");
        }
    }
}
