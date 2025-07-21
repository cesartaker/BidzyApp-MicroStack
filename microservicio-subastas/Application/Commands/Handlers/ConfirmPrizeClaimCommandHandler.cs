using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Contracts.Services;
using Application.Events.InternalEvents;
using MediatR;

namespace Application.Commands.Handlers;

public class ConfirmPrizeClaimCommandHandler : IRequestHandler<ConfirmPrizeClaimCommand, Unit>
{
    private readonly IAuctionService _auctionService;
    private readonly IMediator _mediator;

    public ConfirmPrizeClaimCommandHandler(IAuctionService auctionService, IMediator mediator)
    {
        _auctionService = auctionService;
        _mediator = mediator;
    }
    /// <summary>
    /// Maneja el comando <see cref="ConfirmPrizeClaimCommand"/> para confirmar que el premio de una subasta ha sido recibido.
    /// Actualiza el estado de la subasta como "receptado" y publica una notificación del evento.
    /// </summary>
    /// <param name="request">
    /// Comando <see cref="ConfirmPrizeClaimCommand"/> que contiene el identificador de la subasta cuyos premios se están confirmando.
    /// </param>
    /// <param name="cancellationToken">
    /// Token de cancelación que permite finalizar la operación de forma cooperativa si es necesario.
    /// </param>
    /// <returns>
    /// Una <see cref="Task{Unit}"/> que representa la ejecución asincrónica del proceso, sin valor de retorno.
    /// </returns>
    public async Task<Unit> Handle(ConfirmPrizeClaimCommand request, CancellationToken cancellationToken)
    {
        var auction = await _auctionService.GetAuctionByIdAsync(request.auctionId);
        auction.SetRecepted(true);
        await _auctionService.UpdateAuctionAsync(auction);
        await _mediator.Publish(new AuctionUpdatedEventNotification(auction)  ,cancellationToken);

        return Unit.Value;
    }
}
