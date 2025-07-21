using Application.Contracts.Repositories;
using Domain.Entities;
using MassTransit;

namespace Infrastructure.Messaging;

public class PaymentUpdatedConsumer : IConsumer<Payment>
{
    private readonly IMongoPaymentReadRepository _paymentReadRepository;

    public PaymentUpdatedConsumer(IMongoPaymentReadRepository mongoPaymentReadRepository)
    {
        _paymentReadRepository = mongoPaymentReadRepository;
    }
    /// <summary>
    /// Consume un mensaje de tipo <see cref="Payment"/> desde el bus de eventos de MassTransit.
    /// Extrae el objeto <c>Payment</c> del contexto y actualiza su información en el repositorio de lectura
    /// mediante <see cref="_paymentReadRepository.UpdatePayment"/>.
    /// </summary>
    /// <param name="context">
    /// Contexto de consumo de MassTransit que contiene el mensaje <see cref="Payment"/> con los datos actualizados.
    /// </param>
    /// <returns>
    /// Una <see cref="Task"/> que representa la operación asincrónica de actualización del pago en el repositorio.
    /// </returns>
    public async Task Consume(ConsumeContext<Payment> context)
    {
        var payment = context.Message;  
        await _paymentReadRepository.UpdatePayment(payment);
    }
}
