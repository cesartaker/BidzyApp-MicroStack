using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Contracts.Repositories;
using Application.Events.InternalEvents;
using Domain.Entities;
using MassTransit;

namespace Infrastructure.Messaging.Consumers;

public class AuctionCreatedConsumer : IConsumer<AuctionAddedEventNotification>
{
    IMongoAuctionReadRepository _mongoAuctionReadRepository;
    public AuctionCreatedConsumer(IMongoAuctionReadRepository mongoAuctionReadRepository)
    {
        _mongoAuctionReadRepository = mongoAuctionReadRepository;
    }
    /// <summary>
    /// Consume el evento <see cref="AuctionAddedEventNotification"/> recibido desde el bus de mensajes.
    /// Extrae la subasta del mensaje y la agrega al repositorio de lectura basado en MongoDB.
    /// </summary>
    /// <param name="context">
    /// Contexto del consumidor que encapsula el mensaje de evento recibido, incluyendo la subasta agregada.
    /// </param>
    /// <returns>
    /// Una <see cref="Task"/> que representa la operación asincrónica de persistencia en el repositorio de lectura.
    /// </returns>
    public async Task Consume(ConsumeContext<AuctionAddedEventNotification> context)
    {
        var auction = context.Message.Auction;
        await _mongoAuctionReadRepository.AddAuction(auction);
    }
}
