namespace EShopOnContainers.Identity.Services;

using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.UI.Services;

public class EmailSender : IEmailSender
{
    private readonly string _smtpHost;
    private readonly int _smtpPort;
    private readonly string _smtpUser;
    private readonly string _smtpPass;

    public EmailSender(IConfiguration configuration)
    {
        _smtpHost = configuration.GetValue<string>(key: "Smtp:Host")!;
        _smtpPort = configuration.GetValue<int>(key: "Smtp:Port")!;
        _smtpUser = configuration.GetValue<string>(key: "Smtp:User")!;
        _smtpPass = configuration.GetValue<string>(key: "Smtp:Pass")!;
    }

    public async Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
        var smtpClient = new SmtpClient(_smtpHost)
        {
            Port = _smtpPort,
            Credentials = new NetworkCredential(_smtpUser, _smtpPass),
            EnableSsl = true,
        };

        var mailMessage = new MailMessage
        {
            From = new MailAddress(_smtpUser),
            Subject = subject,
            Body = htmlMessage,
            IsBodyHtml = true,
        };
        mailMessage.To.Add(email);

        await smtpClient.SendMailAsync(mailMessage);
    }
}
