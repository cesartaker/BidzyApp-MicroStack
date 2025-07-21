using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Contracts.Services;
using Application.Dtos;
using Application.Event;
using MediatR;

namespace Application.Commands.Handlers;

public class CreatePaymentCommandHandler:IRequestHandler<CreatePaymentCommand, PaymentCreatedDto>
{
    private readonly IMediator _mediator;
    private readonly IPaymentService _paymentService;

    public CreatePaymentCommandHandler(IMediator mediator, IPaymentService paymentService)
    {
        _mediator = mediator;
        _paymentService = paymentService;
    }
    /// <summary>
    /// Maneja el comando <see cref="CreatePaymentCommand"/> para crear un nuevo registro de pago asociado a una subasta.
    /// Invoca el servicio de pagos para realizar la operación y publica un evento <see cref="PaymentCreatedEvent"/> mediante <c>Mediator</c>.
    /// Finalmente, construye y retorna un <see cref="PaymentCreatedDto"/> con los datos resultantes.
    /// </summary>
    /// <param name="request">
    /// Comando <see cref="CreatePaymentCommand"/> que contiene el identificador del usuario, el de la subasta y el monto del pago.
    /// </param>
    /// <param name="cancellationToken">
    /// Token de cancelación utilizado para abortar la operación si es necesario.
    /// </param>
    /// <returns>
    /// Un objeto <see cref="PaymentCreatedDto"/> que encapsula los datos del pago recién creado.
    /// </returns>
    public async Task<PaymentCreatedDto> Handle(CreatePaymentCommand request, CancellationToken cancellationToken)
    {
        var payment = await _paymentService.CreatePayment(request.userId, request.auctionId, request.amount);
        await _mediator.Publish(new PaymentCreatedEvent(payment), cancellationToken);
        return new PaymentCreatedDto(payment.Id,payment.UserId,payment.AuctionId,
            payment.Amount,payment.Status,payment.CreatedAt);
    }
}

