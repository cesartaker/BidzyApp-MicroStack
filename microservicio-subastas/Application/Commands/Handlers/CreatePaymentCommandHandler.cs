using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Contracts.Services;
using Domain.Enums;
using MediatR;

namespace Application.Commands.Handlers;

public class CreatePaymentCommandHandler : IRequestHandler<CreatePaymentCommand, Unit>
{
    private readonly IAuctionService _auctionService;
    private readonly IPaymentService _paymentService;
    private readonly IBidsService _bidsService;
    public CreatePaymentCommandHandler(IAuctionService auctionService, IPaymentService paymentService, IBidsService bidsService)
    {
        _auctionService = auctionService;
        _paymentService = paymentService;
        _bidsService = bidsService;
    }
    /// <summary>
    /// Maneja el comando <see cref="CreatePaymentCommand"/> para generar el pago asociado a la subasta finalizada.
    /// Obtiene la subasta y sus pujas, determina la oferta ganadora y crea el pago correspondiente si la subasta ha finalizado exitosamente.
    /// </summary>
    /// <param name="request">
    /// Comando <see cref="CreatePaymentCommand"/> que contiene el identificador de la subasta a procesar.
    /// </param>
    /// <param name="cancellationToken">
    /// Token de cancelación que permite abortar la operación de forma cooperativa si se requiere.
    /// </param>
    /// <returns>
    /// Una <see cref="Task{Unit}"/> que representa la ejecución asincrónica del comando, sin valor de retorno.
    /// </returns>
    public async Task<Unit> Handle(CreatePaymentCommand request, CancellationToken cancellationToken)
    {
        var auction = await _auctionService.GetAuctionByIdAsync(request.auctionId);
        var bids = await _bidsService.GetBidsByAuctionIdAsync(auction.Id);
        var highestBid = bids.OrderByDescending(b => b.Amount).FirstOrDefault();
        if (highestBid != null && auction.Status == AuctionStatus.Ended)
        {
            await _paymentService.CreatePayment(auction.Id, highestBid.BidderId, highestBid.Amount);
        }

        return Unit.Value;

    }
}
