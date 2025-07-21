using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Contracts.Repositories;
using Domain.Entities;
using MassTransit;

namespace Infrastructure.Messaging.Consumers;

public class WaybillAddedConsumer : IConsumer<Waybill>
{
    private readonly IMongoWaybillReadRepository _mongoWaybillReadRepository;

    public WaybillAddedConsumer(IMongoWaybillReadRepository mongoWaybillReadRepository)
    {
        _mongoWaybillReadRepository = mongoWaybillReadRepository;
    }
    /// <summary>
    /// Consume el mensaje <see cref="Waybill"/> recibido desde el bus de mensajes.
    /// Extrae la guía de entrega del contexto y la almacena en el repositorio de lectura basado en MongoDB.
    /// </summary>
    /// <param name="context">
    /// Contexto del consumidor que contiene el mensaje de tipo <see cref="Waybill"/> con los datos de la guía de entrega.
    /// </param>
    /// <returns>
    /// Una <see cref="Task"/> que representa la operación asincrónica de almacenamiento,
    /// finalizada inmediatamente al completar el proceso de persistencia.
    /// </returns>
    public Task Consume(ConsumeContext<Waybill> context)
    {
        var waybill = context.Message;
        _mongoWaybillReadRepository.AddWaybill(waybill);
        return Task.CompletedTask;
    }
}
