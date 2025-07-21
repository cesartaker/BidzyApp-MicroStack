using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Contracts.Services;
using Application.Dtos;
using Domain.Enums;
using MediatR;

namespace Application.Command.Handlers;

public class GetPendingComplaintsCommandHandler : IRequestHandler<GetPendingComplaintsCommand, List<ComplaintCreatedDto>>
{
    private readonly IComplaintService _complaintService;

    public GetPendingComplaintsCommandHandler(IComplaintService complaintService)
    {
        _complaintService = complaintService;
    }
    /// <summary>
    /// Procesa el comando para obtener todos los reclamos pendientes 
    /// y devuelve una lista de DTOs que representan dichos reclamos.
    /// </summary>
    /// <param name="request">Comando que no requiere parámetros adicionales para la consulta.</param>
    /// <param name="cancellationToken">Token para cancelar la operación de forma controlada si es necesario.</param>
    /// <returns>Una lista de objetos <see cref="ComplaintCreatedDto"/> con los reclamos en estado pendiente.</returns>
    public Task<List<ComplaintCreatedDto>> Handle(GetPendingComplaintsCommand request, CancellationToken cancellationToken)
    {
        return _complaintService.GetComplaintsAsync(ComplaintStatus.Pending);
    }
}
