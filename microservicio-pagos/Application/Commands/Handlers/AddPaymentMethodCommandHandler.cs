using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Contracts.Services;
using Application.Event;
using MediatR;

namespace Application.Commands.Handlers;

public class AddPaymentMethodCommandHandler : IRequestHandler<AddPaymentMethodCommand, Unit>
{
    private readonly IPaymentMethodService _paymentMethodService;
    private readonly IMediator _mediator;

    public AddPaymentMethodCommandHandler(IPaymentMethodService paymentMethodService,IMediator mediator)
    {
        _paymentMethodService = paymentMethodService;
        _mediator = mediator;
    }
    /// <summary>
    /// Maneja el comando <see cref="AddPaymentMethodCommand"/> para agregar un nuevo método de pago para un usuario.
    /// Invoca el servicio de métodos de pago para registrar el método, y luego publica un evento <see cref="PaymentMethodAddedEvent"/>
    /// utilizando el patrón <c>Mediator</c> para notificar al sistema.
    /// </summary>
    /// <param name="request">
    /// Comando <see cref="AddPaymentMethodCommand"/> que contiene el identificador del usuario para quien se agregará el método de pago.
    /// </param>
    /// <param name="cancellationToken">
    /// Token para cancelar la operación asincrónica si es requerido por el sistema.
    /// </param>
    /// <returns>
    /// Un valor <see cref="Unit"/> que indica que el comando fue manejado correctamente.
    /// </returns>
    public async Task<Unit> Handle(AddPaymentMethodCommand request, CancellationToken cancellationToken)
    {
        var paymentMethod = await _paymentMethodService.AddPaymentMethod(request.userId);
        await _mediator.Publish(new PaymentMethodAddedEvent(paymentMethod), cancellationToken);
        return Unit.Value;
    }
}
