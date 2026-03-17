using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using Bekam.Application.Abstraction.Contracts;
using Bekam.Application.Abstraction.Contracts.Services;

namespace Bekam.Infrastructure.Services;
public class EmailService(IOptions<MailSettings> mailSettings) : IEmailService
{
    private readonly MailSettings _mailSettings = mailSettings.Value;

    public async Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
        var message = new MimeMessage();

        message.From.Add(new MailboxAddress(
            _mailSettings.DisplayName,
            _mailSettings.Mail
        ));

        message.To.Add(MailboxAddress.Parse(email));
        message.Subject = subject;

        message.Body = new BodyBuilder
        {
            HtmlBody = htmlMessage
        }.ToMessageBody();

        using var smtp = new SmtpClient();

        await smtp.ConnectAsync(
            _mailSettings.Host,
            _mailSettings.Port,
            SecureSocketOptions.StartTls
        );

        await smtp.AuthenticateAsync(
            _mailSettings.Mail,
            _mailSettings.Password
        );

        await smtp.SendAsync(message);
        await smtp.DisconnectAsync(true);
    }
}
