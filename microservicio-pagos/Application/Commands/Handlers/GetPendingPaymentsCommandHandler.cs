using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Contracts.Services;
using Application.Dtos;
using MediatR;

namespace Application.Commands.Handlers;

public class GetPendingPaymentsCommandHandler : IRequestHandler<GetPendingPaymentsCommand, List<PaymentCreatedDto>>
{
    private readonly IPaymentService _paymentService;

    public GetPendingPaymentsCommandHandler(IPaymentService paymentService)
    {
        _paymentService = paymentService;
    }
    /// <summary>
    /// Maneja el comando <see cref="GetPendingPaymentsCommand"/> para recuperar todos los pagos pendientes de un usuario específico.
    /// Invoca el servicio de pagos y retorna una lista de objetos <see cref="PaymentCreatedDto"/> con los datos de cada transacción en espera.
    /// </summary>
    /// <param name="request">
    /// Comando <see cref="GetPendingPaymentsCommand"/> que contiene el identificador del usuario cuya lista de pagos pendientes se desea consultar.
    /// </param>
    /// <param name="cancellationToken">
    /// Token de cancelación utilizado para abortar la operación asincrónica si es necesario.
    /// </param>
    /// <returns>
    /// Una <see cref="Task{List{PaymentCreatedDto}}"/> que representa la operación asincrónica de consulta:
    /// retorna la lista de pagos pendientes asociados al usuario.
    /// </returns>
    public Task<List<PaymentCreatedDto>> Handle(GetPendingPaymentsCommand request, CancellationToken cancellationToken)
    {
        return _paymentService.GetPendingPaymentsByUserId(request.userId);
    }
}
