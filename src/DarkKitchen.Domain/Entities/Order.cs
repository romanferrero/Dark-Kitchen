using DarkKitchen.Domain.Enums;

namespace DarkKitchen.Domain.Entities;

public class Order
{
    private int _orderId;
    private DeliveryType _deliveryType;
    private Address _address = null!;
    private List<OrderProduct> _products = null!;
    private OrderStatus _orderStatus;
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
        DeliveryType deliveryType,
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
        private set
        {
            ArgumentOutOfRangeException.ThrowIfNegative(value);
            _orderId = value;
        }
    }

    public DeliveryType DeliveryType
    {
        get => _deliveryType;
        private set => _deliveryType = value;
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

    public OrderStatus OrderStatus
    {
        get => _orderStatus;
        set => _orderStatus = value;
    }

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

    public void UpdateStatus(OrderStatus newOrderStatus)
    {
        ValidateTransition(_orderStatus, newOrderStatus);
        _orderStatus = newOrderStatus;
    }

    private static readonly Dictionary<OrderStatus, OrderStatus> AllowedPreviousStatus = new()
    {
        { OrderStatus.Prepared, OrderStatus.Pending },
        { OrderStatus.Cancelled, OrderStatus.Pending },
        { OrderStatus.OnTheWay, OrderStatus.Prepared },
        { OrderStatus.Delivered, OrderStatus.OnTheWay },
        { OrderStatus.NotDelivered, OrderStatus.OnTheWay },
    };

    private static readonly Dictionary<OrderStatus, string> TransitionErrorMessages = new()
    {
        { OrderStatus.Prepared, "Only pending orders can be prepared" },
        { OrderStatus.Cancelled, "Only pending orders can be cancelled" },
        { OrderStatus.OnTheWay, "Only prepared orders can be on the way" },
        { OrderStatus.Delivered, "Order must be on the way" },
        { OrderStatus.NotDelivered, "Order must be on the way" },
    };

    private static void ValidateTransition(OrderStatus current, OrderStatus next)
    {
        if(!AllowedPreviousStatus.TryGetValue(next, out var requiredPrevious))
        {
            return;
        }

        if(current != requiredPrevious)
        {
            throw new ArgumentException(TransitionErrorMessages[next]);
        }
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
