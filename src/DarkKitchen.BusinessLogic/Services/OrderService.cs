using DarkKitchen.Domain.Deliveries;
using DarkKitchen.Domain.Entities;
using DarkKitchen.IBusinessLogic.DTOs.Entry.OrderDTOs;
using DarkKitchen.IBusinessLogic.DTOs.Exit.OrderDTOs;
using DarkKitchen.IBusinessLogic.IDiscounts;
using DarkKitchen.IBusinessLogic.IServices;
using DarkKitchen.IDataAccess.RepositoriesInterfaces;

namespace DarkKitchen.BusinessLogic.Services;

public sealed class OrderService(
    IOrderRepository orderRepository,
    IProductRepository productRepository,
    IRepository<User> userRepository,
    IPromotionRepository promotionRepository,
    IDiscountCalculator discountCalculator) : IOrderService
{
    private const decimal Iva = 1.22m;

    public CreateOrderResultExitDto CreateOrder(CreateOrderEntryDto dto)
    {
        ValidateClientExists(dto.ClientId);

        var fetchedProducts = FetchAndValidateProducts(dto.Products);
        var orderProducts = BuildOrderProducts(dto.Products, fetchedProducts);

        var delivery = Delivery.FromName(dto.DeliveryType);
        var shippingCost = delivery.ShippingCost;
        var address = Address.Create(dto.Street, dto.DoorNumber, dto.Apartment);
        var activePromotions = GetActivePromotions();

        var subtotal = CalculateSubtotal(orderProducts, activePromotions);
        var total = (subtotal + shippingCost) * Iva;
        var orderCode = GenerateUniqueNumber(c => orderRepository.Exists(o => o.OrderNumber == c));

        var order = Order.Create(delivery, address, orderProducts, dto.ClientId, orderCode, subtotal,
            shippingCost, total);
        orderRepository.Add(order);

        return ToCreateOrderResultExitDto(order);
    }

    public UpdateStatusExitDto UpdateStatus(int orderId, UpdateOrderStatusEntryDto dto)
    {
        var order = orderRepository.Get(o => o.OrderId == orderId)
                    ?? throw new KeyNotFoundException("Order not found");

        order.UpdateStatus(dto.Action);
        orderRepository.Update(order);

        return new UpdateStatusExitDto(order.State.Name, DateTime.Now);
    }

    public List<OrderSummaryExitDto> GetClientOrders(int clientId, DateTime? from, DateTime? to, string? status)
    {
        if(status != null)
        {
            Order.StateFromName(status);
        }

        var orders = orderRepository.GetClientOrders(clientId, from, to, status);

        var clientName = GetClientName(clientId);
        return orders.Select(o => ToOrderSummary(o, clientName)).ToList();
    }

    public List<OrderSummaryExitDto> GetDispatcherOrders(DateTime from, DateTime to, string? street, string? status)
    {
        if(status != null)
        {
            Order.StateFromName(status);
        }

        var orders = orderRepository.GetOrdersByDateRange(from, to, street, status);

        var clientIds = orders.Select(o => o.ClientId).Distinct().ToList();
        var clients = userRepository.GetAll(u => clientIds.Contains(u.Id)).ToList();

        var result = new List<OrderSummaryExitDto>();
        foreach(var order in orders)
        {
            var client = clients.FirstOrDefault(c => c.Id == order.ClientId);
            result.Add(ToOrderSummary(order, client?.FullName ?? "Unknown client"));
        }

        return result;
    }

    public OrderDetailExitDto GetOrderById(int orderId)
    {
        var order = orderRepository.GetOrderById(orderId)
                    ?? throw new KeyNotFoundException($"Order {orderId} not found.");

        var clientName = GetClientName(order.ClientId);
        var activePromotions = GetActivePromotions();

        var productDetails = order.Products
            .Select(op => ToOrderProductDetail(op, activePromotions))
            .ToList();

        return new OrderDetailExitDto
        {
            OrderNumber = order.OrderNumber,
            ClientId = order.ClientId,
            ClientFullName = clientName,
            OrderDate = order.OrderDate,
            Status = order.State.Name,
            TotalCost = order.TotalCost,
            Products = productDetails
        };
    }

    private void ValidateClientExists(int clientId)
    {
        if(!userRepository.Exists(u => u.Id == clientId))
        {
            throw new ArgumentException("Client not found");
        }
    }

    private List<Product> FetchAndValidateProducts(List<OrderProductEntryDto> items)
    {
        var productCodes = new List<string>();
        foreach(var item in items)
        {
            productCodes.Add(item.Code);
        }

        var fetchedProducts = productRepository.GetAll(p => productCodes.Contains(p.Code)).ToList();

        foreach(var product in fetchedProducts)
        {
            if(!product.Active)
            {
                throw new ArgumentException($"Product '{product.Code}' is not available");
            }
        }

        return fetchedProducts;
    }

    private static List<OrderProduct> BuildOrderProducts(
        List<OrderProductEntryDto> items,
        List<Product> fetchedProducts)
    {
        var orderProducts = new List<OrderProduct>();
        foreach(var item in items)
        {
            if(item.Quantity <= 0)
            {
                throw new ArgumentException("Product quantity must be at least 1.");
            }

            var product = fetchedProducts.First(p => p.Code == item.Code);
            orderProducts.Add(new OrderProduct
            {
                ProductId = product.Id,
                Product = product,
                Quantity = item.Quantity
            });
        }

        return orderProducts;
    }

    private decimal CalculateSubtotal(
        List<OrderProduct> orderProducts,
        List<Promotion> activePromotions)
    {
        var subtotal = 0m;
        foreach(var op in orderProducts)
        {
            var unitPrice = discountCalculator.CalculatePrice(op.Product, activePromotions);
            subtotal += unitPrice * op.Quantity;
        }

        return subtotal;
    }

    private List<Promotion> GetActivePromotions()
    {
        var today = DateOnly.FromDateTime(DateTime.Today);
        return promotionRepository.GetAll(p => p.DateFrom <= today && p.DateTo >= today).ToList();
    }

    private string GetClientName(int clientId)
    {
        var client = userRepository.Get(u => u.Id == clientId);
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

    private static int GenerateUniqueNumber(Func<int, bool> exists)
    {
        int code;
        do
        {
            code = Random.Shared.Next(100000, 999999);
        }
        while(exists(code));

        return code;
    }

    private static CreateOrderResultExitDto ToCreateOrderResultExitDto(Order order)
    {
        return new CreateOrderResultExitDto(
            order.ClientId,
            order.OrderNumber,
            order.Subtotal,
            order.ShippingCost,
            order.TotalCost);
    }

    private static OrderProductDetailExitDto ToOrderProductDetail(
        OrderProduct orderProduct,
        List<Promotion> activePromotions)
    {
        var product = orderProduct.Product;
        var bestPromotion = FindBestPromotion(product, activePromotions);

        return new OrderProductDetailExitDto
        {
            Code = product.Code,
            Name = product.Name,
            Price = product.Price,
            Category = product.Category,
            Quantity = orderProduct.Quantity,
            PromotionName = bestPromotion?.Name,
            DiscountPercentage = bestPromotion?.DiscountPercentage
        };
    }

    private static OrderSummaryExitDto ToOrderSummary(Order order, string clientFullName)
    {
        return new OrderSummaryExitDto
        {
            OrderNumber = order.OrderNumber,
            ClientId = order.ClientId,
            ClientFullName = clientFullName,
            OrderDate = order.OrderDate,
            Status = order.State.Name,
            TotalCost = order.TotalCost,
            ProductCount = order.Products.Sum(op => op.Quantity)
        };
    }
}
