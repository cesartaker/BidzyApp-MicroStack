using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Contracts.Repositories;
using Domain.Entities;
using MassTransit;

namespace Infrastructure.Messaging;

public class PaymentMethodAddedConsumer : IConsumer<UserPaymentMethod>
{
    private readonly IMongoPaymentMethodReadRepository _mongoPaymentReadRepository;

    public PaymentMethodAddedConsumer(IMongoPaymentMethodReadRepository mongoPaymentMethodReadRepository)
    {
        _mongoPaymentReadRepository = mongoPaymentMethodReadRepository;
    }
    /// <summary>
    /// Consume un mensaje de tipo <see cref="UserPaymentMethod"/> desde el bus de mensajes.
    /// Extrae el objeto <c>UserPaymentMethod</c> del contexto y lo persiste en el repositorio de lectura MongoDB
    /// mediante <see cref="_mongoPaymentReadRepository.AddPaymentMethod"/>.
    /// </summary>
    /// <param name="context">
    /// Contexto de consumo de MassTransit que contiene el mensaje <see cref="UserPaymentMethod"/> recibido.
    /// </param>
    /// <returns>
    /// Una <see cref="Task"/> que representa la operación asincrónica de almacenamiento del método de pago en la base de datos.
    /// </returns>
    public async Task Consume(ConsumeContext<UserPaymentMethod> context)
    {
        var paymentMethod = context.Message;
        await _mongoPaymentReadRepository.AddPaymentMethod(paymentMethod);
    }
}
