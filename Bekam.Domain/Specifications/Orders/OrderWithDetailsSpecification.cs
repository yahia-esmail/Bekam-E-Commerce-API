using Bekam.Domain.Entities.Orders;

namespace Bekam.Domain.Specifications.Orders;
public class OrderWithDetailsSpecification
    : BaseSpecifications<Order, int>
{
    public OrderWithDetailsSpecification(int orderId)
        : base(o => o.Id == orderId)
    {
        AddInclude(o => o.DeliveryMethod!);
        AddInclude(o => o.OrderItems!);
    }

    
}
