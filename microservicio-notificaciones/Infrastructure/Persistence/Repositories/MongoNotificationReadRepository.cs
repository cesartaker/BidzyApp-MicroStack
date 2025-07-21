using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Domain.Contracts.Repositories;
using Domain.Entities;
using Infrastructure.Persistence.Contexts;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Persistence.Repositories;

public class MongoNotificationReadRepository : IMongoNotificationReadRepository
{
    private readonly MongoDbReadContext _context;
    private readonly ILogger<MongoNotificationReadRepository> _logger;
    public MongoNotificationReadRepository(MongoDbReadContext context, ILogger<MongoNotificationReadRepository> logger)
    {
        _context = context;
        _logger = logger;
    }
    /// <summary>
    /// Registra la notificación en la base de datos.
    /// </summary>
    /// <param name="notification">Instancia de una notificación.</param>
    /// <returns>Retorna un objeto Notificación.</returns>
    public async Task<Notification?> AddNotification(Notification notification)
    {
        _logger.LogInformation("Iniciando la inserción de una nueva notificación.");

        try
        {
            if (notification == null)
            {
                _logger.LogWarning("Se intentó insertar una notificación nula.");
                return null;
            }

            await _context.Notifications.InsertOneAsync(notification);
            _logger.LogDebug("Notificación insertada correctamente: {@Notification}", notification);
            return notification;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al insertar la notificación: {@Notification}", notification);
            return notification;
        }
    }
}
