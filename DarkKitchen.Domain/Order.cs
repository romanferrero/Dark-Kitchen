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
        private set => _address = value;
    }

    public List<Product> Products
    {
        get => _products;
        private set
        {
            if(value == null || value.Count == 0)
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
        private set
        {
            ArgumentOutOfRangeException.ThrowIfNegative(value);
            _clientId = value;
        }
    }

    public int OrderNumber
    {
        get => _orderNumber;
        private set
        {
            ArgumentOutOfRangeException.ThrowIfNegative(value);
            _orderNumber = value;
        }
    }

    public double Subtotal
    {
        get => _subtotal;
        private set
        {
            ArgumentOutOfRangeException.ThrowIfNegative(value);
            _subtotal = value;
        }
    }

    public double ShippingCost
    {
        get => _shippingCost;
        private set => _shippingCost = value;
    }

    public double TotalCost
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
        private set => _orderDate = value;
    }

    public void UpdateStatus(OrderStatus newOrderStatus)
    {
        switch(newOrderStatus)
        {
            case OrderStatus.Prepared:
                if(_orderStatus != OrderStatus.Pending)
                {
                    throw new ArgumentException("Only pending orders can be prepared");
                }

                break;

            case OrderStatus.Cancelled:
                if(_orderStatus != OrderStatus.Pending)
                {
                    throw new ArgumentException("Only pending orders can be cancelled");
                }

                break;

            case OrderStatus.OnTheWay:
                if(_orderStatus != OrderStatus.Prepared)
                {
                    throw new ArgumentException("Only prepared orders can be on the way");
                }

                break;

            case OrderStatus.Delivered:
                if(_orderStatus != OrderStatus.OnTheWay)
                {
                    throw new ArgumentException("Order must be on the way");
                }

                break;
        }

        _orderStatus = newOrderStatus;
    }
}
