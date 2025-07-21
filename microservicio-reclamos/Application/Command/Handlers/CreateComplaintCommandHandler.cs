using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Contracts.Services;
using Application.Dtos;
using Application.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Command.Handlers;

public class CreateComplaintCommandHandler : IRequestHandler<CreateComplaintCommand,ComplaintCreatedDto>
{
    private readonly ILogger<CreateComplaintCommandHandler> _logger;
    private readonly IComplaintService _complaintService;
    private readonly IMediator _mediator;

    public CreateComplaintCommandHandler(ILogger<CreateComplaintCommandHandler> logger,
        IComplaintService complaintService, IMediator mediator)
    {
        _complaintService = complaintService;
        _logger = logger;
        _mediator = mediator;
    }
    /// <summary>
    /// Procesa el comando para registrar un nuevo reclamo de usuario, 
    /// publica el evento correspondiente una vez creado el reclamo 
    /// y devuelve un DTO con la información del reclamo registrado.
    /// </summary>
    /// <param name="request">Comando con los datos necesarios para registrar el reclamo.</param>
    /// <param name="cancellationToken">Token que puede usarse para cancelar la operación asincrónica.</param>
    /// <returns>Un objeto <see cref="ComplaintCreatedDto"/> con los detalles del reclamo registrado.</returns>
    public async Task<ComplaintCreatedDto> Handle(CreateComplaintCommand request, CancellationToken cancellationToken)
    {
        var complaint = await _complaintService.RegisterComplaintAsync(request.userId, request.reason, request.description, request.evidence);
        await _mediator.Publish(new ComplaintCreatedEvent(complaint),cancellationToken);
        return new ComplaintCreatedDto
        {
            Id = complaint.Id,
            UserId = complaint.UserId,
            Reason = complaint.Reason,
            Description = complaint.Description,
            EvidenceUrl = complaint.EvidenceUrl,
            CreatedAt = complaint.CreatedAt,
            Status = complaint.Status
        };
    }
}
