using Bekam.Domain.Common;

namespace Bekam.Domain.Entities.Cart;
public class Cart : BaseEntity<string>
{
    public ICollection<CartItem> Items { get; set; } = new List<CartItem>();
}
