using Bekam.Application.Abstraction.Results;

namespace Bekam.Application.Abstraction.Contracts.Services.Payments;
public interface IPaymentService
{
    Task<Result<string>> CreateCheckoutSession(int orderId);
    Task<bool> Webhook();
}
