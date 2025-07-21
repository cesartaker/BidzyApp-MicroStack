using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Contracts.Services;
using Domain.Entities;
using MediatR;

namespace Application.Commands.Handlers;

public class GetBidsCommandHandler : IRequestHandler<GetBidsCommand, List<Bid>>
{
    private readonly IBidsService _bidsService;

    public GetBidsCommandHandler(IBidsService bidsService)
    {
        _bidsService = bidsService;
    }
    /// <summary>
    /// Maneja el comando <see cref="GetBidsCommand"/> para recuperar las pujas asociadas a una subasta específica.
    /// Invoca el servicio de pujas <see cref="_bidsService"/> utilizando el identificador de subasta provisto en el comando.
    /// </summary>
    /// <param name="request">
    /// Comando que contiene el identificador único (<see cref="Guid"/>) de la subasta cuyos datos de pujas se desean consultar.
    /// </param>
    /// <param name="cancellationToken">
    /// Token utilizado para observar solicitudes de cancelación de la operación asincrónica.
    /// </param>
    /// <returns>
    /// Una <see cref="Task{List{Bid}}"/> que representa la operación asincrónica:
    /// devuelve una lista de objetos <see cref="Bid"/> relacionados con la subasta especificada en el comando.
    /// </returns>
    public Task<List<Bid>> Handle(GetBidsCommand request, CancellationToken cancellationToken)
    {
        var bids = _bidsService.GetBidsByAuctionIdAsync(request.auctionId);
        return bids;
    }
}
