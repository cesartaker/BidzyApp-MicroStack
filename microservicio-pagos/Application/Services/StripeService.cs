using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Contracts.Services;
using Application.Dtos;
using Application.Services.Options;
using Microsoft.Extensions.Options;
using Stripe;

namespace Application.Services;

public class StripeService : IStripeService
{
    private readonly StripeOptions _options;

    public StripeService(IOptions<StripeOptions> options)
    {
        _options = options.Value;
        StripeConfiguration.ApiKey = _options.SecretKey;
    }

    /// <summary>
    /// Procesa un intento de pago en Stripe utilizando los datos provistos: método de pago, monto y moneda.
    /// Configura la operación como <c>Confirm = true</c> y <c>ConfirmationMethod = "manual"</c>, enviando la solicitud a la API de Stripe.
    /// Retorna un objeto <see cref="PaymentResultDto"/> que indica el éxito de la transacción, el estado devuelto y el identificador del intento.
    /// </summary>
    /// <param name="paymentMethodId">
    /// Identificador del método de pago (<see cref="string"/>) registrado en Stripe que se utilizará para realizar el cargo.
    /// </param>
    /// <param name="amount">
    /// Monto total a pagar (<see cref="decimal"/>), expresado en unidades de la moneda especificada.
    /// </param>
    /// <param name="currency">
    /// Código de moneda (<see cref="string"/>) en formato ISO, por ejemplo <c>"usd"</c> o <c>"eur"</c>.
    /// </param>
    /// <returns>
    /// Una <see cref="Task{PaymentResultDto}"/> que representa la operación asincrónica:
    /// contiene el resultado del intento de pago, incluyendo éxito, mensaje de estado e ID de la transacción.
    /// </returns>
    public async Task<PaymentResultDto> SendPaymentAsync(string paymentMethodId, decimal amount, string currency)
    {
        var options = new PaymentIntentCreateOptions
        {
            Amount = (long)(amount * 100),
            Currency = currency.ToLower(),
            PaymentMethod = paymentMethodId,
            ConfirmationMethod = "manual",
            Confirm = true,
            ReturnUrl = "http://localhost:8080/"
        };

        var service = new PaymentIntentService();
        var intent = await service.CreateAsync(options);

        return new PaymentResultDto
        {
            Success = intent.Status == "succeeded",
            Message = intent.Status,
            PaymentIntentId = intent.Id
        };
    }
    /// <summary>
    /// Obtiene los detalles de un método de pago registrado en Stripe utilizando su identificador único.
    /// Crea una instancia del servicio <see cref="PaymentMethodService"/> y ejecuta la operación <c>Get</c>
    /// para recuperar el objeto <see cref="PaymentMethod"/> correspondiente.
    /// </summary>
    /// <param name="paymentMethodId">
    /// Identificador único del método de pago (<see cref="string"/>) tal como fue registrado en Stripe.
    /// </param>
    /// <returns>
    /// Un objeto <see cref="PaymentMethod"/> que contiene los detalles del método de pago solicitado.
    /// </returns>
    public PaymentMethod GetPaymentMethod(string paymentMethodId)
    {
        var service = new PaymentMethodService();
        var paymentMethod = service.Get(paymentMethodId);
        return paymentMethod;
    }

}
