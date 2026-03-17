using Bekam.Domain.Entities.Orders;

namespace Bekam.Domain.Specifications.Orders;
public class OrdersWithDetailsForUserSpecification : BaseSpecifications<Order, int>
{
    public OrdersWithDetailsForUserSpecification(string buyerId)
        : base(o => o.BuyerId == buyerId)
    {
        
        AddInclude(o => o.DeliveryMethod!);
        AddInclude(o => o.OrderItems!);
    }
}
