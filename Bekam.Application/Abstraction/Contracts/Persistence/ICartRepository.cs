using Bekam.Application.DTOs.Cart;
using Bekam.Domain.Entities.Cart;

namespace Bekam.Application.Abstraction.Contracts.Persistence;
public interface ICartRepository
{
    Task<Cart?> GetAsync(string cartId);
    Task<Cart?> SaveAsync(Cart cart);
    Task<bool> DeleteAsync(string cartId);
}
