using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Contracts.Respositories;
using Domain.Entities;
using MassTransit;

namespace Infrastructure.Messaging.Consumers;

public class ComplaintUpdatedConsumer : IConsumer<Complaint>
{
    private readonly IMongoComplaintReadRespository _complaintReadRepository;
    public ComplaintUpdatedConsumer(IMongoComplaintReadRespository readRespository)
    {
        _complaintReadRepository = readRespository;
    }
    /// <summary>
    /// Consume un mensaje de tipo <see cref="Complaint"/> recibido desde la cola de mensajería,
    /// y actualiza el reclamo en el repositorio de lectura con los datos contenidos en el mensaje.
    /// </summary>
    /// <param name="context">
    /// Contexto del consumidor que contiene el mensaje <see cref="Complaint"/> y metadatos de entrega.
    /// </param>
    public async Task Consume(ConsumeContext<Complaint> context)
    {
        var complaint = context.Message;
        await _complaintReadRepository.UpdateComplaint(complaint);
    }
}
