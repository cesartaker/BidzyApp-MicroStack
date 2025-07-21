using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Application.Contracts.Repositories;
using Application.Contracts.Services;
using Application.Dtos;
using Domain.Entities;
using Domain.Enums;

namespace Application.Services;
public class PaymentService : IPaymentService
{
    private readonly IMongoPaymentWriteRepository _paymentWriteRepository;
    private readonly IMongoPaymentReadRepository _paymentReadRepository;
    private readonly IStripeService _stripeService;

    public PaymentService(IMongoPaymentWriteRepository writeRepository, IMongoPaymentReadRepository readRepository, IStripeService stripeService)
    {
        _paymentReadRepository = readRepository;
        _paymentWriteRepository = writeRepository;
        _stripeService = stripeService;
    }
    /// <summary>
    /// Crea un nuevo registro de pago asociado a un usuario y una subasta específica.
    /// Construye una instancia de <see cref="Payment"/> con los datos proporcionados y la persiste en el repositorio de escritura.
    /// </summary>
    /// <param name="userId">
    /// Identificador único del usuario (<see cref="Guid"/>) que realiza el pago.
    /// </param>
    /// <param name="auctionId">
    /// Identificador único de la subasta (<see cref="Guid"/>) a la que corresponde el pago.
    /// </param>
    /// <param name="amount">
    /// Monto del pago (<see cref="decimal"/>) que el usuario debe cubrir.
    /// </param>
    /// <returns>
    /// Una <see cref="Task{Payment}"/> que representa la operación asincrónica:
    /// retorna el objeto <see cref="Payment"/> creado y persistido en el sistema.
    /// </returns>
    public async Task<Payment> CreatePayment(Guid userId, Guid auctionId, decimal amount)
    {
        var payment = new Payment(userId,auctionId,amount);
        await _paymentWriteRepository.AddPayment(payment);
        return payment;
    }
    /// <summary>
    /// Recupera todos los pagos pendientes asociados a un usuario específico.
    /// Consulta el repositorio de lectura y filtra los resultados cuya propiedad <c>Status</c> sea <c>PaymentStatus.pending</c>.
    /// Luego transforma cada pago en una instancia de <see cref="PaymentCreatedDto"/> que encapsula los datos relevantes.
    /// </summary>
    /// <param name="userId">
    /// Identificador único del usuario (<see cref="Guid"/>) cuyos pagos pendientes se desean obtener.
    /// </param>
    /// <returns>
    /// Una <see cref="Task{List{PaymentCreatedDto}}"/> que representa la operación asincrónica:
    /// retorna una lista de objetos <see cref="PaymentCreatedDto"/> que representan los pagos pendientes del usuario.
    /// </returns>
    public async Task<List<PaymentCreatedDto>> GetPendingPaymentsByUserId(Guid userId)
    {
        var payments = await _paymentReadRepository.GetPendingPaymentsByUserId(userId);

        var pendingPayments = payments
            .Where(p => p.Status == PaymentStatus.pending)
            .Select(p => new PaymentCreatedDto(
                p.Id,
                p.UserId,
                p.AuctionId,
                p.Amount,
                p.Status,
                p.CreatedAt
            ))
            .ToList();

        return pendingPayments;
    }
    /// <summary>
    /// Procesa el envío de un pago identificado por <paramref name="paymentId"/> utilizando el método de pago especificado.
    /// Recupera los datos del pago desde el repositorio de lectura, ejecuta la operación en Stripe mediante <see cref="_stripeService"/>,
    /// y actualiza el objeto <see cref="Payment"/> con el identificador del intento de pago y el método utilizado.
    /// </summary>
    /// <param name="paymentId">
    /// Identificador único del pago (<see cref="Guid"/>) que será procesado.
    /// </param>
    /// <param name="paymentMethodId">
    /// Identificador del método de pago (<see cref="string"/>) que se utilizará para realizar la transacción en Stripe.
    /// </param>
    /// <returns>
    /// Una <see cref="Task{Payment}"/> que representa la operación asincrónica:
    /// retorna el objeto <see cref="Payment"/> actualizado tras el envío exitoso.
    /// </returns>
    public async Task<Payment> SendPayment(Guid paymentId, string paymentMethodId)
    {
        var payment = await _paymentReadRepository.GetPaymentById(paymentId);
        var confirmation = await _stripeService.SendPaymentAsync(paymentMethodId,payment.Amount,payment.Currency);
        payment.UpdatePayment(confirmation.PaymentIntentId, paymentMethodId);
        return payment;
    }
}
