using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Contracts.Services;
using Domain.Entities;
using MediatR;

namespace Application.Commands.Handlers;

public class GetBidsByAuctionIdsCommandHandler : IRequestHandler<GetBidsByAuctionIdsCommand, List<Bid>>
{
    private readonly IBidsService _bidsService;

    public GetBidsByAuctionIdsCommandHandler(IBidsService bidsService)
    {
        _bidsService = bidsService;
    }
    /// <summary>
    /// Maneja el comando <see cref="GetBidsByAuctionIdsCommand"/> que solicita las pujas asociadas a múltiples subastas.
    /// Delega la lógica al servicio de pujas <see cref="_bidsService"/>, utilizando los identificadores de subasta incluidos en el comando.
    /// </summary>
    /// <param name="request">
    /// Comando que contiene la lista de identificadores únicos (<see cref="Guid"/>) de las subastas que se desean consultar.
    /// </param>
    /// <param name="cancellationToken">
    /// Token que indica si la operación asincrónica debe ser cancelada.
    /// </param>
    /// <returns>
    /// Una <see cref="Task{List{Bid}}"/> que representa la operación asincrónica:
    /// devuelve la colección de pujas (<see cref="Bid"/>) asociadas a las subastas indicadas en el comando.
    /// </returns>
    public async Task<List<Bid>> Handle(GetBidsByAuctionIdsCommand request, CancellationToken cancellationToken)
    {
        return await _bidsService.GetBidsByAuctionIdsAsync(request.auctionIds);
    }
}
