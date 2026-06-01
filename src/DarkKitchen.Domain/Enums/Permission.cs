namespace DarkKitchen.Domain.Enums;

public enum Permission
{
    /// <summary>Create, update, delete and list internal users (Admin/Dispatcher).</summary>
    ManageInternalUsers,

    /// <summary>Read the sales and top-products reports.</summary>
    ViewReports,

    /// <summary>Create and update products.</summary>
    ManageProducts,

    /// <summary>List and read products.</summary>
    ViewProducts,

    /// <summary>Create and update promotions.</summary>
    ManagePromotions,

    /// <summary>Associate and dissociate products from a promotion.</summary>
    ManagePromotionProducts,

    /// <summary>List and read promotions.</summary>
    ViewPromotions,

    /// <summary>Place a new order (Client).</summary>
    CreateOrder,

    /// <summary>Change the status of an existing order.</summary>
    UpdateOrderStatus,

    /// <summary>List orders (filtered by client or by date range depending on role).</summary>
    ListOrders,

    /// <summary>Read the full detail of a single order.</summary>
    ViewOrderDetail,

    /// <summary>Move an order from Pending to Prepared.</summary>
    PrepareOrder,

    /// <summary>Cancel a pending order.</summary>
    CancelOrder,

    /// <summary>Move an order from Prepared to OnTheWay.</summary>
    MoveOrderOnTheWay,

    /// <summary>Mark an OnTheWay order as Delivered.</summary>
    MarkOrderDelivered,

    /// <summary>Mark an OnTheWay order as NotDelivered.</summary>
    MarkOrderNotDelivered,

    /// <summary>Create and update delivery types (Admin).</summary>
    ManageDeliveryTypes,

    /// <summary>List available delivery types (Client + Admin).</summary>
    ViewDeliveryTypes,
}
