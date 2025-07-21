using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Contracts.Services;
using Application.Dtos;
using Application.Event;
using Domain.Entities;
using MediatR;

namespace Application.Commands.Handlers;

public class SendPaymentCommandHandler : IRequestHandler<SendPaymentCommand, Payment>
{
    private readonly IPaymentService _paymentService;
    private readonly IMediator _mediator;
    private readonly IAuctionService _auctionService;

    public SendPaymentCommandHandler(IStripeService stripeService, IPaymentService paymentService,IMediator mediator, IAuctionService auctionService)
    {
        _paymentService = paymentService;
        _mediator = mediator;
        _auctionService = auctionService;
    }
    /// <summary>
    /// Maneja el comando <see cref="SendPaymentCommand"/> para procesar el envío de un pago asociado a una subasta.
    /// Invoca el servicio de pagos para realizar la transacción, publica un evento <see cref="PaymentUpdatedEvent"/>
    /// que notifica el cambio de estado, y actualiza el estado de la subasta como <c>Completed</c> mediante el servicio correspondiente.
    /// </summary>
    /// <param name="request">
    /// Comando <see cref="SendPaymentCommand"/> que contiene los identificadores del pago y del método de pago utilizado.
    /// </param>
    /// <param name="cancellationToken">
    /// Token de cancelación que permite abortar la operación asincrónica si el sistema lo requiere.
    /// </param>
    /// <returns>
    /// Una <see cref="Task{Payment}"/> que representa la operación asincrónica:
    /// retorna el objeto <see cref="Payment"/> que encapsula la información del pago realizado.
    /// </returns>
    public async Task<Payment> Handle(SendPaymentCommand request, CancellationToken cancellationToken)
    {
        var payment = await _paymentService.SendPayment(request.paymentId,request.PaymentMethodId);
        await _mediator.Publish(new PaymentUpdatedEvent(payment), cancellationToken);
        await _auctionService.SetAuctionStatusAsCompleted(payment.AuctionId);
        return payment;
    }
}
