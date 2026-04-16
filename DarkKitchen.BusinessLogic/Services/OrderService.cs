using DarkKitchen.Domain;
using DarkKitchen.IBusinessLogic;
using DarkKitchen.IDataAccess;

namespace DarkKitchen.BusinessLogic.Services;

public class OrderService(
    IRepository<Order> orderRepository,
    IRepository<Product> productRepository,
    IRepository<User> userRepository,
    IShippingCostCalculatorFactory shippingFactory) : IOrderService
{
    public OrderResultDTO CreateOrder(
        int clientId,
        string deliveryType,
        string street,
        string doorNumber,
        string apartment,
        List<string> items)
    {
        try
        {
            var user = userRepository.GetAll(user => user.Id == clientId);
            if(user == null)
            {
                throw new ArgumentException("User not found");
            }

            var products = productRepository
                .GetAll(p => items.Contains(p.Code))
                .ToList();

            var deliveryTypeEnum = Enum.Parse<DeliveryType>(deliveryType);

            var calculator = shippingFactory.GetCalculator(deliveryTypeEnum);
            var shippingCost = calculator.GetCost();

            var address = Address.Create(street, doorNumber, apartment);

            var subtotal = products.Sum(p => (double)p.Price);

            var total = subtotal + shippingCost;

            var order = Order.Create(
                0,
                deliveryTypeEnum,
                address,
                products,
                clientId,
                0,
                subtotal,
                shippingCost,
                total);

            orderRepository.Add(order);

            return new OrderResultDTO
            {
                ClientId = order.ClientId,
                OrderNumber = order.OrderNumber,
                Subtotal = (decimal)order.Subtotal,
                ShippingCost = (decimal)order.ShippingCost,
                Total = (decimal)order.TotalCost
            };
        }
        catch(Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public UpdateStatusExitDTO UpdateStatus(int orderId, string status)
    {
        try
        {
            var order = orderRepository.GetAll(o => o.OrderId == orderId).FirstOrDefault();
            if(order == null)
            {
                throw new ArgumentException("Order not found");
            }

            order.UpdateStatus(Enum.Parse<OrderStatus>(status));

            orderRepository.Update(order);

            return new UpdateStatusExitDTO(
                order.OrderStatus.ToString(),
                DateTime.Now);
        }
        catch(Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}
