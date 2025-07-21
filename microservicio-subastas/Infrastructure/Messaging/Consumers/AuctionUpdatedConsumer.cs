using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Contracts.Repositories;
using Application.Events.InternalEvents;
using MassTransit;

namespace Infrastructure.Messaging.Consumers;

public class AuctionUpdatedConsumer : IConsumer<AuctionUpdatedEventNotification>
{
    private readonly IMongoAuctionReadRepository _mongoAuctionReadRepository;

    public AuctionUpdatedConsumer(IMongoAuctionReadRepository mongoAuctionReadRepository)
    {
        _mongoAuctionReadRepository = mongoAuctionReadRepository;
    }
    /// <summary>
    /// Consume el evento <see cref="AuctionUpdatedEventNotification"/> recibido a través del bus de mensajes.
    /// Extrae la subasta actualizada del mensaje y aplica los cambios en el repositorio de lectura basado en MongoDB.
    /// </summary>
    /// <param name="context">
    /// Contexto del consumidor que encapsula el mensaje de evento recibido, incluyendo la subasta con datos actualizados.
    /// </param>
    /// <returns>
    /// Una <see cref="Task"/> que representa la operación asincrónica de actualización en el repositorio de lectura.
    /// </returns>
    public async Task Consume(ConsumeContext<AuctionUpdatedEventNotification> context)
    {
        var auction = context.Message.Auction;
        await _mongoAuctionReadRepository.UpdateAuction(auction);
    }
}
