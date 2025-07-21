using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Contracts.Repositories;
using Application.Events.InternalEvents;
using Domain.Entities;
using MassTransit;

namespace Infrastructure.Messaging.Consumers;

public class UpdateProductStatusConsumer : IConsumer<Product>
{
    private readonly IMongoProductReadRepository _productReadRepository;

    public UpdateProductStatusConsumer(IMongoProductReadRepository productReadRepository)
    {
        _productReadRepository = productReadRepository;
    }
    /// <summary>
    /// Consume un mensaje de tipo <see cref="Product"/> desde una cola de mensajería y actualiza su información
    /// en el repositorio de lectura.
    /// Reconstruye el objeto del producto con los datos recibidos y ejecuta la operación de actualización correspondiente.
    /// </summary>
    /// <param name="context">
    /// Contexto del consumidor que contiene el mensaje <see cref="Product"/> con los datos actualizados del producto.
    /// </param>
    /// <returns>
    /// Una <see cref="Task"/> que representa la operación asincrónica de actualización en el repositorio de lectura.
    /// </returns>
    public async Task Consume(ConsumeContext<Product> context)
    {
        var product = new Product(
            context.Message.Id,
            context.Message.AuctioneerId,
            context.Message.Name,
            context.Message.Description,
            context.Message.BasePrice,
            context.Message.Category,
            context.Message.ImageUrl,
            context.Message.Status
        );
        await _productReadRepository.UpdateProduct(product);
    }
}
