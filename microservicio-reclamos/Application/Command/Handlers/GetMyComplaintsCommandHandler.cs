using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Contracts.Services;
using Application.Dtos;
using MediatR;

namespace Application.Command.Handlers;

public class GetMyComplaintsCommandHandler : IRequestHandler<GetMyComplaintsCommand, List<ComplaintSolvedDto>>
{
    private readonly IComplaintService _complaintService;
    public GetMyComplaintsCommandHandler(IComplaintService complaintService)
    {
        _complaintService = complaintService;
    }
    /// <summary>
    /// Procesa el comando para obtener los reclamos registrados por el usuario actual,
    /// los transforma en una lista de DTOs que representan reclamos  y devuelve dicha lista.
    /// </summary>
    /// <param name="request">Comando que contiene el identificador del usuario.</param>
    /// <param name="cancellationToken">Token que permite cancelar la operación asincrónica si es necesario.</param>
    /// <returns>Una lista de objetos <see cref="ComplaintSolvedDto"/> con los datos de cada reclamo.</returns>
    public async Task<List<ComplaintSolvedDto>> Handle(GetMyComplaintsCommand request, CancellationToken cancellationToken)
    {
        var complaints = await _complaintService.GetComplaintsByUserId(request.userId);
        return complaints.Select(c => new ComplaintSolvedDto
        {
            Id = c.Id,
            UserId = c.UserId,
            Reason = c.Reason,
            Description = c.Description,
            EvidenceUrl = c.EvidenceUrl,
            Solution = c.Solution,
            SolvedAt = c.ResolveAt,
            Status = c.Status
        }).ToList();
    }
}

