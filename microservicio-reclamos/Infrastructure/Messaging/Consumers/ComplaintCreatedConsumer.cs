using Application.Contracts.Respositories;
using Domain.Entities;
using MassTransit;

namespace Infrastructure.Messaging.Consumers;

public class ComplaintCreatedConsumer : IConsumer<Complaint>
{
    private readonly IMongoComplaintReadRespository _complaintReadRepository;

    public ComplaintCreatedConsumer(IMongoComplaintReadRespository mongoComplaintReadRespository)
    {
        _complaintReadRepository = mongoComplaintReadRespository;
    }
    /// <summary>
    /// Consume un mensaje de tipo <see cref="Complaint"/> recibido desde una cola de mensajería,
    /// y agrega el reclamo al repositorio de lectura para su posterior consulta.
    /// </summary>
    /// <param name="context">Contexto que contiene el mensaje <see cref="Complaint"/> y metadatos de la operación.</param>
    public async Task Consume(ConsumeContext<Complaint> context)
    {
        var complaint = context.Message;
        await _complaintReadRepository.AddComplaint(complaint);
    }
}
