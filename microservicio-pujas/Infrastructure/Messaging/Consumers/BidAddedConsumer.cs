using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Events.InternalEvents;
using Domain.Contracts.Repositories;
using Infrastructure.Persistence.Repositories;
using MassTransit;

namespace Infrastructure.Messaging.Consumers;

public class BidAddedConsumer : IConsumer<BidAddedEventNotification>
{
    private readonly IMongoBidsReadRepository _mongoBidsReadRepository;

    public BidAddedConsumer(IMongoBidsReadRepository mongoBidsReadRepository)
    {
        _mongoBidsReadRepository = mongoBidsReadRepository;
    }
    /// <summary>
    /// Consume un mensaje de tipo <see cref="BidAddedEventNotification"/> que representa una puja recién registrada.
    /// Extrae la puja del mensaje recibido y la persiste en el repositorio de lectura MongoDB para facilitar su consulta posterior.
    /// </summary>
    /// <param name="context">
    /// Contexto del mensaje recibido (<see cref="ConsumeContext{BidAddedEventNotification}"/>), que contiene los datos de la puja registrada.
    /// </param>
    /// <returns>
    /// Una <see cref="Task"/> que representa la operación asincrónica de persistencia de la puja en el repositorio de lectura.
    /// </returns>
    public async Task Consume(ConsumeContext<BidAddedEventNotification> context)
    {
        var bid = context.Message.Bid;
        await _mongoBidsReadRepository.AddBidAsync(bid);
    }
}
