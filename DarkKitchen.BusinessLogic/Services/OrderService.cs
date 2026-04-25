using DarkKitchen.Domain.Entities;
using DarkKitchen.Domain.Enums;
using DarkKitchen.IBusinessLogic.DTOs.Entry.OrderDTOs;
using DarkKitchen.IBusinessLogic.DTOs.Exit.OrderDTOs;
using DarkKitchen.IBusinessLogic.IDiscounts;
using DarkKitchen.IBusinessLogic.IServices;
using DarkKitchen.IBusinessLogic.IShippingCost;
using DarkKitchen.IDataAccess.RepositoriesInterfaces;

namespace DarkKitchen.BusinessLogic.Services;

public sealed class OrderService(
    IOrderRepository orderRepository,
    IProductRepository productRepository,
    IUserRepository userRepository,
    IShippingCostCalculatorFactory shippingFactory,
    IPromotionRepository promotionRepository,
    IDiscountCalculator discountCalculator) : IOrderService
{
    private const decimal VatRate = 1.22m;

    public CreateOrderResultExitDto CreateOrder(CreateOrderEntryDto dto)
    {
        var client = userRepository.GetAll(u => u.Id == dto.ClientId).FirstOrDefault();
        if(client == null)
        {
            throw new ArgumentException("Client not found");
        }

        var products = productRepository.GetAll(p => dto.Products.Contains(p.Code)).ToList();

        var inactiveProduct = products.FirstOrDefault(p => !p.Active);
        if(inactiveProduct != null)
        {
            throw new ArgumentException($"Product '{inactiveProduct.Code}' is not available");
        }

        var deliveryTypeEnum = Enum.Parse<DeliveryType>(dto.DeliveryType);
        var shippingCost = shippingFactory.GetCalculator(deliveryTypeEnum).GetCost();
        var address = Address.Create(dto.Street, dto.DoorNumber, dto.Apartment);
        var activePromotions = GetActivePromotions();

        var subtotal = products.Sum(p => discountCalculator.CalculatePrice(p, activePromotions));
        var total = (subtotal + shippingCost) * VatRate;

        var order = Order.Create(0, deliveryTypeEnum, address, products, dto.ClientId, 0, subtotal, shippingCost,
            total);
        orderRepository.Add(order);

        return ToCreateOrderResultExitDto(order);
    }

    public UpdateStatusExitDTO UpdateStatus(int orderId, UpdateOrderStatusEntryDTO dto)
    {
        var order = orderRepository.GetAll(o => o.OrderId == orderId).FirstOrDefault()
                    ?? throw new KeyNotFoundException("Order not found");

        order.UpdateStatus(Enum.Parse<OrderStatus>(dto.Action));
        orderRepository.Update(order);

        return new UpdateStatusExitDTO(order.OrderStatus.ToString(), DateTime.Now);
    }

    public OrderDetailExitDTO GetOrderById(int orderId)
    {
        var order = orderRepository.GetOrderById(orderId)
                    ?? throw new KeyNotFoundException($"Order {orderId} not found.");

        var clientName = GetClientName(order.ClientId);
        var activePromotions = GetActivePromotions();

        var productDetails = order.Products
            .Select(p => ToOrderProductDetail(p, activePromotions))
            .ToList();

        return new OrderDetailExitDTO
        {
            OrderNumber = order.OrderNumber,
            ClientId = order.ClientId,
            ClientFullName = clientName,
            OrderDate = order.OrderDate,
            Status = order.OrderStatus.ToString(),
            TotalCost = order.TotalCost,
            Products = productDetails
        };
    }

    public List<OrderSummaryExitDTO> GetClientOrders(int clientId, DateTime? from, DateTime? to, string? status)
    {
        OrderStatus? statusEnum = null;
        if(status != null)
        {
            statusEnum = Enum.Parse<OrderStatus>(status, true);
        }

        var orders = orderRepository.GetClientOrders(clientId, from, to, statusEnum);

        var clientName = GetClientName(clientId);
        return orders.Select(o => ToOrderSummary(o, clientName)).ToList();
    }

    public List<OrderSummaryExitDTO> GetDispatcherOrders(DateTime from, DateTime to, string? street, string? status)
    {
        OrderStatus? statusEnum = null;
        if(status != null)
        {
            statusEnum = Enum.Parse<OrderStatus>(status, true);
        }

        var orders = orderRepository.GetOrdersByDateRange(from, to, street, statusEnum);

        var clientIds = orders.Select(o => o.ClientId).Distinct().ToList();
        var clients = userRepository.GetAll(u => clientIds.Contains(u.Id)).ToList();

        var result = new List<OrderSummaryExitDTO>();
        foreach(var order in orders)
        {
            var client = clients.FirstOrDefault(c => c.Id == order.ClientId);
            result.Add(ToOrderSummary(order, client?.FullName ?? "Unknown client"));
        }

        return result;
    }

    private List<Promotion> GetActivePromotions()
    {
        var today = DateOnly.FromDateTime(DateTime.Today);
        return promotionRepository.GetAll(p => p.DateFrom <= today && p.DateTo >= today).ToList();
    }

    private string GetClientName(int clientId)
    {
        var client = userRepository.GetAll(u => u.Id == clientId).FirstOrDefault();
        return client?.FullName ?? "Unknown client";
    }

    private static Promotion? FindBestPromotion(Product product, List<Promotion> activePromotions)
    {
        return activePromotions
            .Where(promo => promo.Products.Any(prod => prod.Code == product.Code))
            .OrderByDescending(promo => promo.DiscountPercentage)
            .ThenBy(promo => promo.Name)
            .FirstOrDefault();
    }

    private static CreateOrderResultExitDto ToCreateOrderResultExitDto(Order order)
    {
        return new CreateOrderResultExitDto
        {
            ClientId = order.ClientId,
            OrderNumber = order.OrderNumber,
            Subtotal = order.Subtotal,
            ShippingCost = order.ShippingCost,
            Total = order.TotalCost
        };
    }

    private static OrderProductDetailExitDTO ToOrderProductDetail(Product product, List<Promotion> activePromotions)
    {
        var bestPromotion = FindBestPromotion(product, activePromotions);

        return new OrderProductDetailExitDTO
        {
            Code = product.Code,
            Name = product.Name,
            Price = product.Price,
            Category = product.Category,
            PromotionName = bestPromotion?.Name,
            DiscountPercentage = bestPromotion?.DiscountPercentage
        };
    }

    private static OrderSummaryExitDTO ToOrderSummary(Order order, string clientFullName)
    {
        return new OrderSummaryExitDTO
        {
            OrderNumber = order.OrderNumber,
            ClientId = order.ClientId,
            ClientFullName = clientFullName,
            OrderDate = order.OrderDate,
            Status = order.OrderStatus.ToString(),
            TotalCost = order.TotalCost,
            ProductCount = order.Products.Count
        };
    }
}
