namespace DarkKitchen.Domain;

public enum OrderStatus
{
    /// <summary>Order is pending and waiting to be prepared.</summary>
    Pending,

    /// <summary>Order has been prepared and is ready for delivery.</summary>
    Prepared,

    /// <summary>Order has been cancelled.</summary>
    Cancelled,

    /// <summary>Order is on the way to the customer.</summary>
    OnTheWay,

    /// <summary>Order has been successfully delivered.</summary>
    Delivered,

    /// <summary>Order could not be delivered.</summary>
    NotDelivered,
}
