namespace Bekam.Application.Abstraction.Contracts.Services;
public interface IEmailService
{
    Task SendEmailAsync(string email, string subject, string htmlMessage);
} 
 