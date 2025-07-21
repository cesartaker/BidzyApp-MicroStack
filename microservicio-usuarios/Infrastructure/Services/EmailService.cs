using Application.Contracts.Services;
using MimeKit;
using SmtpClient = MailKit.Net.Smtp.SmtpClient;

namespace Infrastructure.Services;

public class EmailService : IEmailService
{
    private readonly string _smtpServer;
    private readonly int _smtpPort;
    private readonly string _senderEmail;
    private readonly string _appPassword;

    public EmailService()
    {
        _smtpServer = Environment.GetEnvironmentVariable("SMTP_SERVER")!;
        _smtpPort = int.Parse(Environment.GetEnvironmentVariable("SMTP_PORT")!);
        _senderEmail = Environment.GetEnvironmentVariable("SENDER_EMAIL")!;
        _appPassword = Environment.GetEnvironmentVariable("APP_EMAIL_PASSWORD")!;
    }

    public async Task SendEmailAsync(string recipient, string subject, string body)
    {
        var message = new MimeMessage();
            message.From.Add(new MailboxAddress("Bidzy App", _senderEmail));
            message.To.Add(new MailboxAddress("Destinatario", recipient));
            message.Subject = subject;
            message.Body = new TextPart("plain") { Text = body };

            using (var smtpClient = new SmtpClient())
            {
                await smtpClient.ConnectAsync(_smtpServer, _smtpPort, MailKit.Security.SecureSocketOptions.StartTls);
                await smtpClient.AuthenticateAsync(_senderEmail, _appPassword);
                await smtpClient.SendAsync(message);
                await smtpClient.DisconnectAsync(true);
            }
    }
    
}
    
