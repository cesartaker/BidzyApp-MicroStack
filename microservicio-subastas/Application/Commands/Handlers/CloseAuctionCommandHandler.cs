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

public class CloseAuctionCommandHandler : IRequestHandler<CloseAuctionCommand>
{
    private readonly IAuctionService _auctionService;
    private readonly IMediator _mediator;
    private readonly IBidsService _bidsService;
    private readonly IProductService _productService;

    public CloseAuctionCommandHandler(IAuctionService auctionService, IMediator mediator, IBidsService bidsService, IProductService productService)
    {
        _auctionService = auctionService;
        _mediator = mediator;
        _bidsService = bidsService;
        _productService = productService;
    }
    /// <summary>
    /// Maneja el comando <see cref="CloseAuctionCommand"/> que cierra una subasta específica.
    /// Publica una notificación de cierre, espera brevemente, obtiene las pujas realizadas y determina el estado final de la subasta.
    /// Si la subasta tiene ofertas que cumplen con el precio de reserva, se marca como finalizada y se asigna un ganador.
    /// Actualiza el estado del producto asociado y la subasta, publicando las notificaciones correspondientes.
    /// </summary>
    /// <param name="request">
    /// Comando <see cref="CloseAuctionCommand"/> que contiene el identificador de la subasta a cerrar.
    /// </param>
    /// <param name="cancellationToken">
    /// Token de cancelación que permite cancelar la operación asincrónica de forma cooperativa.
    /// </param>
    /// <returns>
    /// Una <see cref="Task"/> que representa la ejecución asincrónica del proceso de cierre.
    /// </returns>
    public async Task Handle(CloseAuctionCommand request, CancellationToken cancellationToken)
    {
        await _mediator.Publish(new AuctionClosedEventNotification(request.auctionId));

        await Task.Delay(TimeSpan.FromSeconds(5), cancellationToken);

        var bids = await _bidsService.GetBidsByAuctionIdAsync(request.auctionId);
        var auction = await _auctionService.GetAuctionByIdAsync(request.auctionId);
        AuctionStatus auctionStatus;
        
        if (bids.Count == 0)
        {
            auctionStatus = AuctionStatus.Deserted;
        }
        else
        {
            var highestBid = bids.Max(b => b.Amount);
            auctionStatus = _auctionService.DetermineAuctionStatus(auction.ReservePrice, highestBid);
        }

        auction.SetState(auctionStatus);

        if (auctionStatus == AuctionStatus.Ended)
        {
            var winningBid = bids.OrderByDescending(b => b.Amount).FirstOrDefault();
            var bidderId = winningBid?.BidderId ?? Guid.Empty;
            auction.SetWinner(bidderId);
            await _productService.UpdateProductStatus(auction.ProductId, "Unavailable");
        }
        else
        {
            await _productService.UpdateProductStatus(auction.ProductId, "Available");
        }

        await _auctionService.UpdateAuctionAsync(auction);
        await _mediator.Publish(new AuctionUpdatedEventNotification(auction), cancellationToken);
    }
}
