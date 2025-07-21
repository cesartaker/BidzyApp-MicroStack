
using System.Net.Mail;
using Domain.Contracts.Services;
using Microsoft.Extensions.Options;
using MimeKit;
using SmtpClient = MailKit.Net.Smtp.SmtpClient;

namespace Infrastructure.Services.EmailService;

public class EmailService: IEmailNotification
{
    private readonly string _smtpServer;
    private readonly int _smtpPort;
    private readonly string _senderEmail;
    private readonly string _appPassword;

    public EmailService(IOptions<EmailServiceOptions> options)
    {
        _smtpServer = options.Value.SmtpServer ?? throw new ArgumentNullException(nameof(options.Value.SmtpServer));
        _smtpPort = options.Value.SmtpPort > 0 ? options.Value.SmtpPort : throw new ArgumentOutOfRangeException(nameof(options.Value.SmtpPort));
        _senderEmail = options.Value.SenderEmail ?? throw new ArgumentNullException(nameof(options.Value.SenderEmail));
        _appPassword = options.Value.AppEmailPassword ?? throw new ArgumentNullException(nameof(options.Value.AppEmailPassword));
    }
    /// <summary>
    /// Envía una notificación por correo electrónico 
    /// </summary>
    /// <param name="email">Direccion electrónica del destinatario.</param>
    /// <param name="body">Mensaje del correo.</param>
    /// <param name="subject">Título del correo.</param>
    public async Task Send(string email, string body, string subject)
    {
        var message = new MimeMessage();
        message.From.Add(new MailboxAddress("Bidzy App", _senderEmail));
        message.To.Add(new MailboxAddress("Destinatario", email));
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
    /// <summary>
    /// Intenta enviar notificación por correo varias veces en caso de fallo. 
    /// </summary>
    /// <param name="email">Direccion electrónica del destinatario.</param>
    /// <param name="body">Mensaje del correo.</param>
    /// <param name="subject">Título del correo.</param>
    /// <param name="maxRetries">Cantidad de veces que intenta enviar el correo en caso de fallo.</param>
    /// <param name="delayMs">Retraso antes de cada intento.</param>
    
    public async Task<bool> TrySendEmail(string email, string body, string subject, int maxRetries = 3, int delayMs = 2000)
    {
        for (int attempt = 1; attempt <= maxRetries; attempt++)
        {
            try
            {
                await Send(email, body, subject);
                return true; 
            }
            catch (Exception ex)
            {
                
                Console.WriteLine($"Intento {attempt} fallido: {ex.Message}");

                if (attempt == maxRetries)
                    return false;

                await Task.Delay(delayMs); 
            }
        }

        return false; 
    }
}
