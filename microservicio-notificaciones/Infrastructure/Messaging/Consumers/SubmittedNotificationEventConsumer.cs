using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Contracts.Repositories;
using Domain.Entities;
using MassTransit;

namespace Infrastructure.Messaging.Consumers;

public class SubmittedNotificationEventConsumer : IConsumer<Notification>
{
    private readonly IMongoNotificationReadRepository _notificationReadRepository;

    public SubmittedNotificationEventConsumer(IMongoNotificationReadRepository readRepository)
    {
        _notificationReadRepository = readRepository;
    }
    /// <summary>
    /// Solicita registrar la notificación en la base de datos de lectura.
    /// </summary>
    /// <param name="notification">Instancia de una notificación.</param>
    /// <returns>Retorna un objeto Notificación.</returns>
    public async Task Consume(ConsumeContext<Notification> context)
    {
        var notification = context.Message;
        await _notificationReadRepository.AddNotification(notification);
    }
}
