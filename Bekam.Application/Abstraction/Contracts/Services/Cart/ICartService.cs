using Bekam.Application.Abstraction.Results;
using Bekam.Application.DTOs.Cart;

namespace Bekam.Application.Abstraction.Contracts.Services.Cart;
public interface ICartService
{
    Task<Result<CartDto>> GetCartAsync(string cartId);
    Task<Result<CartDto>> GetCartWithMergeAsync(string? cartId);

    Task<Result<CartDto>> AddProductAsync(string cartId, int productId);

    Task<Result<CartDto>> UpdateProductQuantityAsync(
        string cartId,
        int productId,
        int quantity);

    Task<Result<CartDto>> RemoveProductAsync(string cartId, int productId);

    Task<Result> DeleteCartAsync(string cartId);
}
