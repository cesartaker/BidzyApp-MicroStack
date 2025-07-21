using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Contracts.Services;
using Domain.Entities;
using MediatR;

namespace Application.Commands.Handlers;

public class GetPaymentMethodCommandHandler: IRequestHandler<GetPaymentMethodCommand, UserPaymentMethod>
{
    private readonly IPaymentMethodService _paymentMethodService;
    public GetPaymentMethodCommandHandler(IPaymentMethodService paymentMethodService)
    {
        _paymentMethodService = paymentMethodService;
    }
    /// <summary>
    /// Maneja el comando <see cref="GetPaymentMethodCommand"/> para recuperar el método de pago asociado a un usuario.
    /// Invoca el servicio de métodos de pago y construye una instancia de <see cref="UserPaymentMethod"/> con los datos obtenidos.
    /// </summary>
    /// <param name="request">
    /// Comando <see cref="GetPaymentMethodCommand"/> que contiene el identificador del usuario cuyo método de pago se desea obtener.
    /// </param>
    /// <param name="cancellationToken">
    /// Token que permite cancelar la operación asincrónica si el sistema lo requiere.
    /// </param>
    /// <returns>
    /// Una <see cref="Task{UserPaymentMethod}"/> que representa la operación asincrónica:
    /// retorna el método de pago del usuario encapsulado en un objeto <see cref="UserPaymentMethod"/>.
    /// </returns>
    public async Task<UserPaymentMethod> Handle(GetPaymentMethodCommand request, CancellationToken cancellationToken)
    {
        var paymentMethod =  _paymentMethodService.GetPaymentMethod();
        return new UserPaymentMethod(request.userId, paymentMethod.Id, paymentMethod.Card.Brand,
            paymentMethod.Card.Last4, paymentMethod.Card.ExpMonth.ToString(), paymentMethod.Card.ExpYear.ToString());
    }
}

