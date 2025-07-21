using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Contracts.Repositories;
using Domain.Contracts.Services;
using Domain.Entities;

namespace Application.Services;

public class NotificationService : INotificationService
{
    private readonly IEmailNotification _emailNotification;
    private readonly IMongoNotificationReadRepository _mongoNotificationReadRepository;
    private readonly IMongoNotificationWriteRepository _mongoNotificationWriteRepository;

    public NotificationService(IMongoNotificationReadRepository readRepository, IMongoNotificationWriteRepository writeRepository,
        IEmailNotification emailNotification)
    {
        _emailNotification = emailNotification ?? throw new ArgumentNullException(nameof(emailNotification));
        _mongoNotificationReadRepository = readRepository ?? throw new ArgumentNullException(nameof(readRepository));
        _mongoNotificationWriteRepository = writeRepository ?? throw new ArgumentNullException(nameof(writeRepository));
    }
    /// <summary>
    /// Envía una notificación por correo electrónico y solicita el registro de la notificación en la base de datos.
    /// </summary>
    /// <param name="email">Direccion electrónica del destinatario.</param>
    /// <param name="message">Mensaje del correo.</param>
    /// <param name="subject">Título del correo.</param>
    /// <returns>Retorna un objeto Notificación.</returns>
    public async Task<Notification?> SendEmailNotification(string email,string message, string subject)
    {
        var success = await _emailNotification.TrySendEmail(email, message,subject);
        if(success)
            return await SaveNotificationAsync(email,message,subject);

        return null;
    }
    /// <summary>
    /// Solicita el registro de la notificación en la base de datos.
    /// </summary>
    /// <param name="email">Direccion electrónica del destinatario.</param>
    /// <param name="message">Mensaje del correo.</param>
    /// <param name="subject">Título del correo.</param>
    /// <returns>Retorna un objeto Notificación.</returns>
    public async Task<Notification?> SaveNotificationAsync(string email, string message, string subject)
    {
        var notification = new Notification(email, message, subject);
        return await _mongoNotificationWriteRepository.AddNotification(notification);
    }
}
