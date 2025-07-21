using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Application.Dtos;
using Application.Events.InternalEvents;
using Application.Exceptions;
using Domain.Contracts.Services;
using Domain.Entities;
using MediatR;

namespace Application.Commands.Handlers;

public class NewBidCommandHandler : IRequestHandler<NewBidCommand,BidResponseDto?>
{
    private readonly IMediator _mediator;
    private readonly IBidsService _bidsService;
    private readonly IAuctionsService _auctionService;

    public NewBidCommandHandler(IMediator mediator, IBidsService bidsService, IAuctionsService auctionsService)
    {
        _auctionService = auctionsService;
        _bidsService = bidsService;
        _mediator = mediator;
    }
    /// <summary>
    /// Procesa el comando <see cref="NewBidCommand"/> que representa la creación de una nueva puja en una subasta activa.
    /// Verifica que la subasta esté disponible, crea el objeto <see cref="Bid"/> con la información del comando,
    /// lo registra mediante el servicio de pujas y publica la notificación <see cref="BidAddedEventNotification"/> si la operación es exitosa.
    /// </summary>
    /// <param name="request">
    /// Comando que contiene los datos necesarios para crear la puja: identificador de subasta, del postor, monto y nombre del postor.
    /// </param>
    /// <param name="cancellationToken">
    /// Token que indica si la operación asincrónica debe ser cancelada por el sistema.
    /// </param>
    /// <returns>
    /// Una <see cref="Task{BidResponseDto}"/> que representa la operación asincrónica:
    /// retorna una instancia de <see cref="BidResponseDto"/> con la información de la puja si la operación fue exitosa;
    /// en caso contrario, retorna <c>null</c>.
    /// </returns>
    public async Task<BidResponseDto?> Handle(NewBidCommand request, CancellationToken cancellationToken)
    {
        var auction = await _auctionService.GetAuction(request.AuctionId);
        if (auction == null)
        {
            throw new EndedAuctionException();
        }
        var bid = new Bid(request.AuctionId,request.BidderId,request.Amount,request.bidderName);
        var result = await _bidsService.AddBidAsync(bid,auction);
        if (result != HttpStatusCode.OK)
        {
            return null;
        }

        await _mediator.Publish(new BidAddedEventNotification(bid), cancellationToken);

        return new BidResponseDto{bidderName = request.bidderName, amount=bid.Amount};
    }
}
