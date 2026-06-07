using DarkKitchen.Domain.Enums;

namespace DarkKitchen.WebApi.Filters;

public static class RolePermissions
{
    private static readonly Dictionary<UserRole, HashSet<Permission>> Map = new()
    {
        [UserRole.Admin] =
        [
            Permission.ManageInternalUsers,
            Permission.ViewReports,
            Permission.ManageProducts,
            Permission.ViewProducts,
            Permission.ManagePromotions,
            Permission.ManagePromotionProducts,
            Permission.ViewPromotions,
            Permission.UpdateOrderStatus,
            Permission.ViewOrderDetail,
            Permission.PrepareOrder,
            Permission.CancelOrder,
            Permission.ManageDeliveryTypes,
            Permission.ViewDeliveryTypes,
            Permission.ViewAuditLog,
        ],
        [UserRole.Dispatcher] =
        [
            Permission.UpdateOrderStatus,
            Permission.ViewOrderDetail,
            Permission.ListOrders,
            Permission.PrepareOrder,
            Permission.MoveOrderOnTheWay,
            Permission.MarkOrderDelivered,
            Permission.MarkOrderNotDelivered,
        ],
        [UserRole.Client] =
        [
            Permission.ViewProducts,
            Permission.ViewPromotions,
            Permission.CreateOrder,
            Permission.ListOrders,
            Permission.ViewDeliveryTypes,
        ],
    };

    public static bool RoleHas(UserRole role, Permission permission)
    {
        return Map.TryGetValue(role, out var permissions) && permissions.Contains(permission);
    }
}
