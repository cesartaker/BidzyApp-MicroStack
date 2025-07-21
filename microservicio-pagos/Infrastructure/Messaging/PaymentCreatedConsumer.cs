using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Contracts.Repositories;
using Domain.Entities;
using MassTransit;

namespace Infrastructure.Messaging;

public class PaymentCreatedConsumer : IConsumer<Payment>
{
    private readonly IMongoPaymentReadRepository _mongoPaymentReadRepository;
    public PaymentCreatedConsumer(IMongoPaymentReadRepository mongoPaymentReadRepository)
    {
        _mongoPaymentReadRepository = mongoPaymentReadRepository;
    }
    /// <summary>
    /// Consume un mensaje de tipo <see cref="Payment"/> proveniente del bus de mensajes.
    /// Extrae el objeto <c>Payment</c> del contexto y lo almacena en el repositorio de lectura MongoDB
    /// mediante <see cref="_mongoPaymentReadRepository.AddPayment"/>.
    /// </summary>
    /// <param name="context">
    /// Contexto de consumo de MassTransit que contiene el mensaje <see cref="Payment"/> recibido.
    /// </param>
    /// <returns>
    /// Una <see cref="Task"/> que representa la operación asincrónica de persistencia del pago en MongoDB.
    /// </returns>
    public async Task Consume(ConsumeContext<Payment> context)
    {
        var payment = context.Message;
        await _mongoPaymentReadRepository.AddPayment(payment);
    }
}
