using Mapster;
using Bekam.Application.Abstraction.Contracts.Persistence;
using Bekam.Application.Abstraction.Contracts.Services;
using Bekam.Application.Abstraction.Contracts.Services.Cart;
using Bekam.Application.Abstraction.Contracts.Services.Helpers;
using Bekam.Application.Abstraction.Errors;
using Bekam.Application.Abstraction.Results;
using Bekam.Application.DTOs.Cart;
using Bekam.Domain.Entities.Cart;
using Bekam.Domain.Entities.Product;
using Bekam.Domain.Specifications.Products;

namespace Bekam.Application.Services.Cart;
internal class CartService(ICartRepository cartRepository, IUnitOfWork unitOfWork, ILoggedInUserService loggedInUserService, IUrlBuilder urlBuilder) : ICartService
{
    private readonly ICartRepository _cartRepository = cartRepository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IUrlBuilder _urlBuilder = urlBuilder;
    private readonly string? _loggedInUserId = loggedInUserService.UserId;

    public async Task<Result<CartDto>> GetCartAsync(string cartId)
    {
        if (await _cartRepository.GetAsync(cartId) is not { } cart)
            return Result.Failure<CartDto>(CartErrors.CartNotFound);

        return Result.Success(cart.Adapt<CartDto>());
    }

    // the idea : if(authenticated || cartId is wrong) then never return cartId.
    public async Task<Result<CartDto>> GetCartWithMergeAsync(string? cartId)
    {
        var empty = new CartDto() { };
        var isLogged = !string.IsNullOrEmpty(_loggedInUserId);
        var anonCart = !string.IsNullOrEmpty(cartId)
            ? await _cartRepository.GetAsync(cartId!)
            : null;

        if (!isLogged)
            return Result.Success(anonCart?.Adapt<CartDto>() ?? empty);

        var userCart = await _cartRepository.GetAsync(_loggedInUserId!);

        if (anonCart is null)
        {
            if (userCart is not null)
                userCart.Id = null; // to prevent client from using userId as anonymous user cartId

            return Result.Success(userCart?.Adapt<CartDto>() ?? empty);
        }

        var mergedItems = (userCart?.Items ?? Enumerable.Empty<CartItem>())
            .Where(u => !anonCart.Items.Any(a => a.ProductId == u.ProductId))
            .Concat(anonCart.Items)
            .ToList();

        var mergedCart = new Domain.Entities.Cart.Cart { Id = _loggedInUserId };
        mergedCart.Items = mergedItems;

        await _cartRepository.SaveAsync(mergedCart);
        await _cartRepository.DeleteAsync(cartId!);

        if (mergedCart.Id == _loggedInUserId) // to prevent client from using userId as anonymous user cartId
            mergedCart.Id = null;

        return Result.Success(mergedCart.Adapt<CartDto>());
    }

    public async Task<Result<CartDto>> AddProductAsync(string? cartId, int productId)
    {
        var cart = new Domain.Entities.Cart.Cart();
        IEnumerable<CartItem> items = Enumerable.Empty<CartItem>();

        if (!string.IsNullOrEmpty(_loggedInUserId))
        {
            cart.Id = _loggedInUserId;

            var userCart = await _cartRepository.GetAsync(_loggedInUserId!);
            var anonCart = !string.IsNullOrEmpty(cartId)
                ? await _cartRepository.GetAsync(cartId!)
                : null;

            if (anonCart is not null)
            {
                items = (userCart?.Items ?? Enumerable.Empty<CartItem>())
                    .Where(u => !anonCart.Items.Any(a => a.ProductId == u.ProductId))
                    .Concat(anonCart.Items);

                await _cartRepository.DeleteAsync(cartId!);
            }
            else
            {
                items = userCart?.Items ?? Enumerable.Empty<CartItem>();
            }
        }
        else if (!string.IsNullOrEmpty(cartId))
        {
            cart.Id = cartId;
            var anonCart = await _cartRepository.GetAsync(cartId);
            items = anonCart?.Items ?? Enumerable.Empty<CartItem>();
        }
        else
        {
            cart.Id = Guid.NewGuid().ToString();
            items = Enumerable.Empty<CartItem>();
        }

        cart.Items = items.ToList();

        var item = cart.Items.FirstOrDefault(i => i.ProductId == productId);

        if (item is null)
        {
            var productRepo = _unitOfWork.GetRepository<Product, int>();
            var spec = new ProductWithDetailsSpecification(productId);

            if (await productRepo.GetAsync(spec) is not { } product)
                return Result.Failure<CartDto>(ProductErrors.ProductNotFound);

            var result = product.Adapt<CartItem>();
            result.PictureUrl = _urlBuilder.BuildPictureUrl(product.PictureUrl);

            cart.Items.Add(result);
        }
        else
        {
            item.Quantity++;
        }

        await _cartRepository.SaveAsync(cart);

        if (cart.Id == _loggedInUserId) // to prevent client from using userId as anonymous user cartId
            cart.Id = null;

        return Result.Success(cart.Adapt<CartDto>());
    }


    public async Task<Result> DeleteCartAsync(string? cartId)
    {
        if(!string.IsNullOrEmpty(cartId))
            await _cartRepository.DeleteAsync(cartId);

        if(!string.IsNullOrEmpty(_loggedInUserId))
            await _cartRepository.DeleteAsync(_loggedInUserId);

        return Result.Success();
    }

    public async Task<Result<CartDto>> RemoveProductAsync(string? cartId, int productId)
    {
        string? resolvedCartId = null;

        if (!string.IsNullOrEmpty(_loggedInUserId))
            resolvedCartId = _loggedInUserId;
        else if (!string.IsNullOrEmpty(cartId))
            resolvedCartId = cartId;

        if (string.IsNullOrEmpty(resolvedCartId))
            return Result.Success(new CartDto());

        var cart = await _cartRepository.GetAsync(resolvedCartId);

        if (cart is null)
            return Result.Success(new CartDto());

        var item = cart.Items.FirstOrDefault(i => i.ProductId == productId);

        if (item is null)
        {
            if (cart.Id == _loggedInUserId)
                cart.Id = null;

            return Result.Success(cart.Adapt<CartDto>());
        }

        cart.Items.Remove(item);

        if (!cart.Items.Any())
        {
            await _cartRepository.DeleteAsync(resolvedCartId);
            return Result.Success(new CartDto());
        }

        await _cartRepository.SaveAsync(cart);

        if (cart.Id == _loggedInUserId)
            cart.Id = null;

        return Result.Success(cart.Adapt<CartDto>());
    }


    public async Task<Result<CartDto>> UpdateProductQuantityAsync(string? cartId, int productId, int quantity)
    {
        if (quantity <= 0)
            return await RemoveProductAsync(cartId, productId);

        string? resolvedCartId = null;

        if (!string.IsNullOrEmpty(_loggedInUserId))
            resolvedCartId = _loggedInUserId;
        else if (!string.IsNullOrEmpty(cartId))
            resolvedCartId = cartId;

        if (string.IsNullOrEmpty(resolvedCartId))
            return Result.Success(new CartDto());

        var cart = await _cartRepository.GetAsync(resolvedCartId);

        if (cart is null)
            return Result.Success(new CartDto());

        var item = cart.Items.FirstOrDefault(i => i.ProductId == productId);

        if (item is null)
            return Result.Failure<CartDto>(CartErrors.ProductNotInCart);

        item.Quantity = quantity;

        await _cartRepository.SaveAsync(cart);

        if (cart.Id == _loggedInUserId)
            cart.Id = null;

        return Result.Success(cart.Adapt<CartDto>());
    }

    
}




