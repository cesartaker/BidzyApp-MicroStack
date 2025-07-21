using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Contracts.Services;
using Application.Events.InternalEvents;
using Domain.Enums;
using MediatR;

namespace Application.Commands.Handlers;

public class SetAuctionCompletedCommandHandler : IRequestHandler<SetAuctionCompletedCommand, Unit>
{
    private readonly IAuctionService _auctionService;
    private readonly IMediator _mediator;

    public SetAuctionCompletedCommandHandler(IAuctionService auctionService, IMediator mediator)
    {
        _auctionService = auctionService;
        _mediator = mediator;
    }
    /// <summary>
    /// Maneja el comando <see cref="SetAuctionCompletedCommand"/> para marcar una subasta como completada.
    /// Recupera la subasta especificada, actualiza su estado a <see cref="AuctionStatus.Completed"/> y
    /// publica una notificación de actualización.
    /// </summary>
    /// <param name="request">
    /// Comando <see cref="SetAuctionCompletedCommand"/> que contiene el identificador de la subasta a completar.
    /// </param>
    /// <param name="cancellationToken">
    /// Token de cancelación que permite interrumpir la operación de forma cooperativa si es necesario.
    /// </param>
    /// <returns>
    /// Una <see cref="Task{Unit}"/> que representa la ejecución asincrónica de la operación, sin valor de retorno.
    /// </returns>
    public async Task<Unit> Handle(SetAuctionCompletedCommand request, CancellationToken cancellationToken)
    {
        var auction = await _auctionService.GetAuctionByIdAsync(request.auctionId);
        auction.SetState(AuctionStatus.Completed);
        await _auctionService.UpdateAuctionAsync(auction);
        await _mediator.Publish(new AuctionUpdatedEventNotification(auction), cancellationToken);
        return Unit.Value;
    }
}

