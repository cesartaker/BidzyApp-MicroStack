
using Application.Contracts;
using Domain.Models;
using MassTransit;

namespace Infrastructure.Messaging.Consumers;

public class AuctionAddedConsumer : IConsumer<AuctionIdOnly>
{
    private readonly ICacheAuctionsStates _cacheAuctionsStates;

    public AuctionAddedConsumer(ICacheAuctionsStates cacheAuctionsStates)
    {
        _cacheAuctionsStates = cacheAuctionsStates;
    }
    /// <summary>
    /// Consume un mensaje de tipo <see cref="AuctionIdOnly"/> que indica el inicio o activación de una subasta.
    /// Actualiza el estado de la subasta en el sistema de caché, marcándola como <c>"Active"</c>.
    /// Este método forma parte de la infraestructura de mensajería basada en <c>MassTransit</c>.
    /// </summary>
    /// <param name="context">
    /// Contexto del mensaje recibido (<see cref="ConsumeContext{AuctionIdOnly}"/>), que contiene el identificador de la subasta.
    /// </param>
    /// <returns>
    /// Una <see cref="Task"/> completada de forma inmediata, ya que la operación no requiere lógica asincrónica adicional.
    /// </returns>
    public Task Consume(ConsumeContext<AuctionIdOnly> context)
    {
        _cacheAuctionsStates.Update(context.Message.AuctionId.ToString(), "Active");
        return Task.CompletedTask;
    }
}
