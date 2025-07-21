using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Contracts;
using Domain.Models;


using MassTransit;
using MediatR;

namespace Infrastructure.Messaging.Consumers;

public class AuctionClosedConsumer : IConsumer<AuctionIdOnly>
{
    private readonly ICacheAuctionsStates _cacheAuctionsStates;
    private readonly IBidsSocketHub _hub;

    public AuctionClosedConsumer(ICacheAuctionsStates cacheAuctionsStates, IBidsSocketHub hub)
    {
        _cacheAuctionsStates = cacheAuctionsStates;
        _hub = hub;
    }
    /// <summary>
    /// Consume un mensaje de tipo <see cref="AuctionIdOnly"/> indicando el cierre de una subasta.
    /// Cierra las conexiones en tiempo real asociadas a la subasta mediante el <see cref="_hub"/> y elimina su estado del sistema de caché.
    /// Este método forma parte de la infraestructura de mensajería reactiva que gestiona el ciclo de vida de las subastas.
    /// </summary>
    /// <param name="context">
    /// Contexto del mensaje recibido (<see cref="ConsumeContext{AuctionIdOnly}"/>), que contiene el identificador de la subasta a cerrar.
    /// </param>
    /// <returns>
    /// Una <see cref="Task"/> completada asincrónicamente tras ejecutar las operaciones de cierre y limpieza.
    /// </returns>
    public async Task Consume(ConsumeContext<AuctionIdOnly> context)
    {
        await _hub.CloseSocketsForAuctionAsync(context.Message.AuctionId.ToString());
        _cacheAuctionsStates.Remove(context.Message.AuctionId.ToString());
        await Task.CompletedTask;
    }
}
