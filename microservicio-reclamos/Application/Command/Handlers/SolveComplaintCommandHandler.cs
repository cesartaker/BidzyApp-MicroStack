using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Contracts.Services;
using Application.Dtos;
using Application.Events;
using MediatR;

namespace Application.Command.Handlers;

public class SolveComplaintCommandHandler : IRequestHandler<SolveComplaintCommand, ComplaintSolvedDto?>
{
    private readonly IComplaintService _complaintService;
    private readonly IMediator _mediator;
    public SolveComplaintCommandHandler(IComplaintService complaintService,IMediator mediator)
    {
        _complaintService = complaintService;
        _mediator = mediator;
    }
    /// <summary>
    /// Procesa el comando para resolver un reclamo existente actualizando su solución,
    /// publica el evento correspondiente, envía la notificación al usuario y devuelve 
    /// un DTO con los datos del reclamo solucionado.
    /// </summary>
    /// <param name="request">Comando que contiene el identificador del reclamo y la solución propuesta.</param>
    /// <param name="cancellationToken">Token que permite cancelar la operación de forma segura.</param>
    /// <returns>
    /// Un objeto <see cref="ComplaintSolvedDto"/> con los datos del reclamo actualizado, 
    /// o <c>null</c> si no se encontró el reclamo.
    /// </returns>
    public async Task<ComplaintSolvedDto?> Handle(SolveComplaintCommand request, CancellationToken cancellationToken)
    {
        var complaint = await _complaintService.UpdateComplaintAsync(request.complaintId, request.solution);
        if (complaint != null)
        {
            await _mediator.Publish(new ComplaintUpdatedEvent(complaint), cancellationToken);
            await _complaintService.SendSolveComplaintNotification(complaint);
            return new ComplaintSolvedDto
            {
                Id = complaint.Id,
                UserId = complaint.UserId,
                Reason = complaint.Reason,
                Description = complaint.Description,
                EvidenceUrl = complaint.EvidenceUrl,
                Status = complaint.Status,
                Solution = request.solution,
                SolvedAt = complaint.ResolveAt
            };
        }

        return null;
        
    }
}
